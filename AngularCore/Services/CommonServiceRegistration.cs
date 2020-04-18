using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Setting;
using Infrastructure.Security;
using Infrastructure.Logging;

namespace AngularCore.Services
{
    public static class CommonServiceRegistration
    {
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { });

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
            services.AddScoped<ILoggerService>(s => new LoggerService(s.GetService<AppSettings>(), connectionString));
            #endregion

            if (appSettings.LogContext == "1")
                MyLoggerFactory.AddFile("Logs/AngularEF-{Date}.txt", appSettings.LogLevel);

            services.AddDbContext<NabIncidentAnnouncementContext>(options =>
                options.UseOracle(connectionString, oracleOptions => oracleOptions.UseOracleSQLCompatibility("12"))
                       .UseLazyLoadingProxies()
                       .ConfigureWarnings(warning => warning.Default(WarningBehavior.Ignore).Log(CoreEventId.DetachedLazyLoadingWarning))
                       .UseLoggerFactory(MyLoggerFactory));

            return services;
        }
    }
}
