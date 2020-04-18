using Microsoft.Extensions.Logging;

namespace Infrastructure.Setting
{
    public class AppSettings : IAppSetting
    {
        public string LoggerThrowException { get; set; }
        public string LoggerDebugEnabled { get; set; }

        #region logging
        public string LogContext { get; set; }
        public LogLevel LogLevel { get; set; }
        #endregion

        public string OracleConnectionStringFormat { get; set; }
        public string DataSource { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
