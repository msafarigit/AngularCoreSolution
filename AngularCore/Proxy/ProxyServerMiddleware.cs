using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Infrastructure;

namespace AngularCore
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ProxyServerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IMemoryCache cache;
        private readonly IProxyConfigService proxyConfigService;
        private readonly IProxyService proxyService;
        private string proxyReferrer;

        public ProxyServerMiddleware(RequestDelegate next, IProxyConfigService proxyConfigService, IProxyService proxyService, IMemoryCache memoryCache)
        {
            this.next = next;
            cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.proxyConfigService = proxyConfigService ?? throw new ArgumentNullException(nameof(proxyConfigService));
            this.proxyService = proxyService ?? throw new ArgumentNullException(nameof(proxyService));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            bool endRequest = false;
            if (httpContext.Request.QueryString.HasValue && httpContext.Request.QueryString.ToString().ToLower() == "?ping")
            {
                await httpContext.Response.WriteAsync(CreatePingResponse());
                return;
            }

            // Note: Referrer is mis-spelled in the HTTP Spec
            proxyReferrer = httpContext?.Request?.Headers["referer"];
            if (proxyConfigService.IsAllowedReferrer(proxyReferrer) == false)
            {
                CreateErrorResponse(httpContext.Response, $"Referrer {proxyReferrer} is not allowed.", HttpStatusCode.BadRequest);
                return;
            }

            // Allows request body to be read multiple times, and buffers.
            //context.Request.EnableBuffering();

            string proxiedUrl = httpContext.Request.QueryString.ToString().TrimStart('?');

            // Check if proxy URL is in the list of configured URLs.
            ServerUrl serverUrlConfig = proxyConfigService.GetProxyServerUrlConfig(proxiedUrl);

            HttpResponseMessage response = null;

            if (serverUrlConfig != null)
            {
                bool isAppLogin = !String.IsNullOrEmpty(serverUrlConfig?.ClientId) && !String.IsNullOrEmpty(serverUrlConfig?.ClientSecret);
                var isUserLogin = !String.IsNullOrEmpty(serverUrlConfig?.Username) && !String.IsNullOrEmpty(serverUrlConfig?.Password);
                string httpClientName = serverUrlConfig?.Url;

                if (isAppLogin)
                {
                    string serverToken = await CacheTryGetServerToken(serverUrlConfig, httpClientName);
                    string delimiter = String.IsNullOrEmpty(new Uri(proxiedUrl).Query) ? "?" : "&";
                    string tokenizedUrl = $"{proxiedUrl}{delimiter}token={serverToken}";
                    response = await proxyService.ForwardRequestToServer(httpContext.Request, tokenizedUrl, httpClientName);
                }
                else if (isUserLogin)
                {
                    response = await proxyService.ForwardRequestToServer(httpContext.Request, proxiedUrl, httpClientName);
                }
            }
            else
            { // No matching url to proxy, bypass and proxy the request.
                response = await proxyService.ForwardRequestToServer(httpContext.Request, proxiedUrl, "");
            }

            await CopyProxyHttpResponse(httpContext, response);

            endRequest = true;
            if (!endRequest)
                await next(httpContext);
        }

        private string CreatePingResponse()
        {
            var pingResponse = new
            {
                message = "Pong!",
                hasConfig = proxyConfigService.Config != null,
                referringUrl = proxyReferrer
            };
            return JsonConvert.SerializeObject(pingResponse);
        }

        private HttpResponse CreateErrorResponse(HttpResponse httpResponse, string message, HttpStatusCode status)
        {
            string jsonResponseMessage = JsonConvert.SerializeObject(new { message, status });
            httpResponse.StatusCode = (int)status;
            httpResponse.WriteAsync(jsonResponseMessage);
            return httpResponse;
        }

        private async Task CopyProxyHttpResponse(HttpContext context, HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
                throw new ArgumentNullException(nameof(responseMessage));

            HttpResponse response = context.Response;

            response.StatusCode = (int)responseMessage.StatusCode;
            foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Headers)
                response.Headers[header.Key] = header.Value.ToArray();

            foreach (KeyValuePair<string, IEnumerable<string>> contentHeader in responseMessage.Content?.Headers)
                response.Headers[contentHeader.Key] = contentHeader.Value.ToArray();

            // Removes the header so it doesn't expect a chunked response.
            response.Headers.Remove("transfer-encoding");

            using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync())
                await responseStream.CopyToAsync(response.Body, context.RequestAborted);
        }

        private async Task<string> CacheTryGetServerToken(ServerUrl serverUrl, string clientName, bool killCache = false)
        {
            string tokenCacheKey = "token_for_" + serverUrl.Url;

            if (!cache.TryGetValue(tokenCacheKey, out string serverTokenJson) || killCache)
            {
                // Key not in cache, so get token.
                serverTokenJson = await GetAppLoginToken(serverUrl, clientName);
                cache.Set(tokenCacheKey, serverTokenJson, TimeSpan.FromMinutes(proxyConfigService.Config.TokenCacheMinutes));
            }

            //JObject o = JObject.Parse(serverTokenJson);
            return serverTokenJson;//(string)o["token"];
        }

        private async Task<string> GetAppLoginToken(ServerUrl serverUrl, string clientName)
        {
            if (serverUrl.Oauth2Endpoint == null)
                throw new ArgumentNullException("Oauth2Endpoint");

            //http://edsabsama:6080/arcgis/tokens/generateToken?password=brol112&f=json&username=smsec&encrypted=false
            string oAuth2Endpoint = serverUrl.Oauth2Endpoint;
            if (oAuth2Endpoint[oAuth2Endpoint.Length - 1] != '/')
                oAuth2Endpoint += "/";

            string tokenUri = $"{oAuth2Endpoint}tokens/generateToken?password={serverUrl.Password}&f=json&username={serverUrl.Username}&encrypted=false";
            List<KeyValuePair<string, string>> formData = proxyConfigService.GetOAuth2FormData(serverUrl, proxyReferrer);
            string tokenJson = await proxyService.RequestTokenJson(tokenUri, formData, clientName);

            JObject o = JObject.Parse(tokenJson);
            return (string)o["token"];
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ProxyServerMiddlewareExtensions
    {
        public static IApplicationBuilder UseProxyServerMiddleware(this IApplicationBuilder builder, IProxyConfigService proxyConfigService,
            IProxyService proxyService, IMemoryCache memoryCache)
        {
            return builder.Use(next => new ProxyServerMiddleware(next, proxyConfigService, proxyService, memoryCache).Invoke);
        }
    }
}
