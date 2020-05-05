using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Infrastructure.Setting;
using Infrastructure.Security;
using Infrastructure.Logging;
using Access.Context;

namespace AngularCore.Services
{
    public static class CommonServiceRegistration
    {
        //https://www.entityframeworktutorial.net/efcore/logging-in-entityframework-core.aspx
        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });

        public static IServiceCollection RegisterCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Settings
            services.AddSingleton(configuration.GetSection("AppSettings").Get<AppSettings>());
            services.AddScoped<IAppSetting>(s => s.GetService<AppSettings>());

            //services.AddScoped(s => new SessionManager(s.GetRequiredService<IHttpContextAccessor>()));
            #endregion

            #region ConnectionString
            AppSettings appSettings = services.BuildServiceProvider().GetService<AppSettings>();
            string password = AesCryptography.Decrypt(appSettings.Password);

            string connectionString = string.Format(appSettings.OracleConnectionStringFormat, appSettings.DataSource, appSettings.UserName, password);
            //ASP.Net Core Logger
            services.AddScoped<ILoggerService>(s => new LoggerService(s.GetService<AppSettings>(), connectionString));
            #endregion

            if (appSettings.LogContext == "1")
                loggerFactory.AddFile("Logs/AngularEF-{Date}.txt", appSettings.LogLevel);

            services.AddDbContext<DataContext>(options =>
                options.UseOracle(connectionString, oracleOptions => oracleOptions.UseOracleSQLCompatibility("12"))
                       .UseLazyLoadingProxies()
                       .ConfigureWarnings(warning => warning.Default(WarningBehavior.Ignore).Log(CoreEventId.DetachedLazyLoadingWarning))
                       .UseLoggerFactory(loggerFactory));

            return services;
        }
    }
}
