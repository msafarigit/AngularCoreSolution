using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AngularCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //Serve static files
        //Static files are stored within the project's web root directory.
        //The default directory is {content root}/wwwroot, but it can be changed via the UseWebRoot method. 
        //See Content root and Web root for more information.
        //The Host.CreateDefaultBuilder method sets the content root to the current directory
        //  wwwroot
        //      -css
        //      -images
        //      -js

        //The URI format to access a file in the images subfolder is http://<server_address>/images/<image_file_name>. 
        // For example, http://localhost:9189/images/banner3.svg.
        //If targeting .NET Core, the Microsoft.AspNetCore.App metapackage includes this package.
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
                                                                           .ConfigureAppConfiguration((hostingContext, config) =>
                                                                           {
                                                                               config.AddJsonFile(@"./appsettings-angular.json", false, true);
                                                                           })
                                                                           .ConfigureWebHostDefaults(webBuilder =>
                                                                           {
                                                                               webBuilder.ConfigureKestrel(serverOptions => serverOptions.Limits.MaxRequestBodySize = 1000 * 1024);
                                                                               webBuilder.UseStartup<Startup>();
                                                                           });
    }
}
