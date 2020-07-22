using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AngularCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*
             Host definition:
                A host is an object that encapsulates an app's resources, such as:
                    -Dependency injection (DI)
                    -Logging
                    -Configuration
                    -IHostedService implementations
                When a host starts, it calls IHostedService.StartAsync on each implementation of IHostedService that it finds in the DI container.
                In a web app, one of the IHostedService implementations is a web service that starts an HTTP server implementation.
                The main reason for including all of the app's interdependent resources in one object is lifetime management: control over app startup and graceful shutdown.

             The following Framework-provided services are registered automatically:
                -IHostApplicationLifetime
                -IHostLifetime
                -IHostEnvironment / IWebHostEnvironment
             */
            CreateHostBuilder(args).Build().Run();
        }

        /*
         Serve static files
         Static files are stored within the project's web root directory.
         The default directory is {content root}/wwwroot, but it can be changed via the UseWebRoot method. 
         See Content root and Web root for more information.
         The Host.CreateDefaultBuilder method sets the content root to the current directory
           wwwroot
               -css
               -images
               -js
         
         The URI format to access a file in the images subfolder is http://<server_address>/images/<image_file_name>. 
          For example, http://localhost:9189/images/banner3.svg.
         If targeting .NET Core, the Microsoft.AspNetCore.App metapackage includes this package.
        */
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
                                                                           .ConfigureAppConfiguration((hostingContext, config) =>
                                                                           {
                                                                               //clears all the configuration providers
                                                                               config.Sources.Clear();

                                                                               //In the following code, settings in the appsettings-angular.json file:
                                                                               // Override settings in the appsettings.json and appsettings.Environment.json files.
                                                                               // Are overridden by settings in the Environment variables configuration provider 
                                                                               // and the Command-line configuration provider.

                                                                               IHostEnvironment env = hostingContext.HostingEnvironment;

                                                                               config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                                                     .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                                                                               config.AddJsonFile(@"./appsettings-angular.json", false, true);
                                                                               config.AddEnvironmentVariables();
                                                                               if (args != null)
                                                                                   config.AddCommandLine(args);
                                                                           })
                                                                           .ConfigureWebHostDefaults(webBuilder =>
                                                                           {
                                                                               webBuilder.ConfigureKestrel(serverOptions => serverOptions.Limits.MaxRequestBodySize = 1000 * 1024);
                                                                               //When false, errors during startup result in the host exiting. When true, the host captures exceptions during startup and attempts to start the server.
                                                                               webBuilder.CaptureStartupErrors(true);
                                                                               //When enabled, or when the environment is Development, the app captures detailed errors.
                                                                               webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                                                                               //webBuilder.UseSetting("https_port", "8080");
                                                                               webBuilder.UseStartup<Startup>();
                                                                           });
    }

    /* url: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1#default-builder-settings
     The CreateDefaultBuilder method:
      -Sets the content root to the path returned by GetCurrentDirectory.
      -Loads host configuration from:
          -Environment variables prefixed with DOTNET_.
          -Command-line arguments.
      -Loads app configuration from:
          -appsettings.json.
          -appsettings.{Environment}.json.
          -Secret Manager when the app runs in the Development environment.
          -Environment variables.
          -Command-line arguments.
      -Adds the following logging providers:
          -Console
          -Debug
          -EventSource
          -EventLog (only when running on Windows)
      -Enables scope validation and dependency validation when the environment is Development.
     */

    /* url: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1#default-builder-settings
     The ConfigureWebHostDefaults method:
        Loads host configuration from environment variables prefixed with ASPNETCORE_.
        Sets Kestrel server as the web server and configures it using the app's hosting configuration providers. For the Kestrel server's default options, see Kestrel web server implementation in ASP.NET Core.
        Adds Host Filtering middleware.
        Adds Forwarded Headers middleware if ASPNETCORE_FORWARDEDHEADERS_ENABLED equals true.
        Enables IIS integration. For the IIS default options, see Host ASP.NET Core on Windows with IIS.
     */
}
