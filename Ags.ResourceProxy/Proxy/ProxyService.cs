using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Infrastructure
{
    public class ProxyService : IProxyService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ProxyService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public virtual HttpClient HttpClient(string clientName = null)
        {
            return String.IsNullOrEmpty(clientName) ? httpClientFactory.CreateClient() : httpClientFactory.CreateClient(clientName);
        }

        /// <summary>
        /// This call converts the portal token into a local server token.
        /// </summary>
        /// <param name="tokenUri"> Endpoint url for token. </param>
        /// <param name="formData"> Post form content. </param>
        /// <param name="clientName"> Named HTTP client to be used for the request. </param>
        /// <returns>JSON representing the token and when it expires. </returns>
        public async Task<string> RequestTokenJson(string tokenUri, List<KeyValuePair<string, string>> formData, string clientName)
        {
            HttpResponseMessage response = await HttpClient(clientName).PostAsync(tokenUri, new FormUrlEncodedContent(formData));
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> ForwardRequestToServer(HttpRequest request, string url, string clientName)
        {
            HttpRequestMessage proxyMessage = CreateProxyHttpRequest(request, new Uri(url));
            return await HttpClient(clientName).SendAsync(proxyMessage, HttpCompletionOption.ResponseHeadersRead);
        }

        private static HttpRequestMessage CreateProxyHttpRequest(HttpRequest request, Uri uri)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method) &&
                !HttpMethods.IsDelete(request.Method) && !HttpMethods.IsTrace(request.Method))
            {
                requestMessage.Content = new StreamContent(request.Body);
            }

            // Copy the request headers
            foreach (KeyValuePair<string, StringValues> header in request.Headers)
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());

            requestMessage.Headers.Host = uri.Authority;
            requestMessage.RequestUri = uri;
            requestMessage.Method = new HttpMethod(request.Method);

            return requestMessage;
        }
    }
}