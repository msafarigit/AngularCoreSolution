using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Routing.Constraints;
using Infrastructure;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;

namespace AngularCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.MaxRequestBodySize = 30_000_000_000_000;

            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 3000_000_000;
            });

            services.AddControllersWithViews();
            // services.AddRouting();

            KestrelServerLimits kestrelServerLimits = new KestrelServerLimits();
            kestrelServerLimits.MaxRequestBodySize = 330_000_000;

            //esri arcgis server proxy
            services.AddSingleton<IProxyConfigService, ProxyConfigService>(serviceProvider => new ProxyConfigService(serviceProvider.GetService<IWebHostEnvironment>(), "/Proxy/proxy.config.json"));
            services.AddSingleton<IProxyService, ProxyService>();

            IProxyConfigService agsProxyConfig = services.BuildServiceProvider().GetService<IProxyConfigService>();
            // Loop through the config and add Named Clients for use with IHttpClientFactory
            agsProxyConfig.Config.ServerUrls.ToList().ForEach(serverUrl =>
            {
                services.AddHttpClient(serverUrl.Url)
                        .ConfigurePrimaryHttpMessageHandler(serviceProvider => new HttpClientHandler
                        {
                            AllowAutoRedirect = false,
                            Credentials = agsProxyConfig.GetCredentials(agsProxyConfig.GetProxyServerUrlConfig((serverUrl.Url)))
                        });
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Set Date Format
            IOptions<RequestLocalizationOptions> options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            IList<IRequestCultureProvider> requestCultureProviderList = options.Value.RequestCultureProviders;
            CookieRequestCultureProvider cookieProvider = requestCultureProviderList.OfType<CookieRequestCultureProvider>().First();
            QueryStringRequestCultureProvider urlProvider = requestCultureProviderList.OfType<QueryStringRequestCultureProvider>().First();

            cookieProvider.Options.DefaultRequestCulture = new RequestCulture("en-US");
            cookieProvider.Options.DefaultRequestCulture.Culture.DateTimeFormat.ShortDatePattern = "M/d/yyyy";

            urlProvider.Options.DefaultRequestCulture = new RequestCulture("en-US");
            urlProvider.Options.DefaultRequestCulture.Culture.DateTimeFormat.ShortDatePattern = "M/d/yyyy";

            cookieProvider.CookieName = "UserCulture";

            options.Value.RequestCultureProviders.Clear();
            options.Value.RequestCultureProviders.Add(cookieProvider);
            options.Value.RequestCultureProviders.Add(urlProvider);
            app.UseRequestLocalization(options.Value);
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); //Built-in middleware
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            /*
             The primary difference between UseWhen and MapWhen is how later (i.e. registered below) middleware is executed.
             Unlike MapWhen, UseWhen continues to execute later middleware regardless of whether the UseWhen predicate was true or false.
             */
            app.UseWhen(httpContext => httpContext.Request.Path.Value.ToLower().StartsWith(@"/Proxy/proxy.ashx", StringComparison.OrdinalIgnoreCase),
                builder => builder.UseProxyServerMiddleware(app.ApplicationServices.GetService<IProxyConfigService>(),
                                                            app.ApplicationServices.GetService<IProxyService>(),
                                                            app.ApplicationServices.GetService<IMemoryCache>())
            );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("DefaultWithActionApi", "api/{controller=Authenticate}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("DefaultDeleteApi", "api/{controller}/{id}", new { action = "Delete" }, new { id = @"\d+", httpMethod = new HttpMethodRouteConstraint(HttpMethod.Delete.ToString()) });
                endpoints.MapControllerRoute("DefaultGetByIdApi", "api/{controller}/{id}", new { action = "GetByID" }, new { id = @"\d+", httpMethod = new HttpMethodRouteConstraint(HttpMethod.Get.ToString()) });
                endpoints.MapControllerRoute("DefaultGetApi", "api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodRouteConstraint(HttpMethod.Get.ToString()) });
                endpoints.MapControllerRoute("DefaultPostApi", "api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodRouteConstraint(HttpMethod.Post.ToString()) });
                endpoints.MapControllerRoute("DefaultPutApi", "api/{controller}", new { action = "Put" }, new { httpMethod = new HttpMethodRouteConstraint(HttpMethod.Put.ToString()) });
            });

            /*
             For single page applications, the SPA middleware UseSpaStaticFiles usually comes last in the middleware pipeline.
             The SPA middleware comes last:
                - To allow all other middlewares to respond to matching requests first.
                - To allow SPAs with client-side routing to run for all routes that are unrecognized by the server app.
             */
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501
                spa.Options.SourcePath = "ClientApp";
                spa.Options.StartupTimeout = TimeSpan.FromSeconds(1000);

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }

    public class ServerRouteConstraint : IRouteConstraint
    {
        private readonly Func<Uri, bool> predicate;

        public ServerRouteConstraint(Func<Uri, bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = httpContext.Request.Scheme;
            uriBuilder.Host = httpContext.Request.Host.ToString();
            uriBuilder.Path = httpContext.Request.Path.ToString();
            uriBuilder.Query = httpContext.Request.QueryString.ToString();
            return this.predicate(uriBuilder.Uri);
        }
    }
}
