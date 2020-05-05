using NLog;
using NLog.Config;
using NLog.Targets;
using Infrastructure.Setting;

namespace Infrastructure.Logging
{
    public class LoggerService : ILoggerService
    {
        private readonly IAppSetting _appSettings;
        private readonly string _connectionString;
        private const string DatabaseTargetName = "DatabaseTarget";

        public ILogger DatabaseLogger { get; }

        public LoggerService(IAppSetting appSettings, string connectionstring)
        {
            _appSettings = appSettings;
            _connectionString = connectionstring;

            LoggingConfiguration loggingConfiguration = new LoggingConfiguration();
            ConfigDatabaseTarget(loggingConfiguration);
            LogManager.Configuration = loggingConfiguration;
            LogManager.ThrowExceptions = bool.Parse(_appSettings.LoggerThrowException);

            DatabaseLogger = LogManager.GetLogger(DatabaseTargetName);
            DatabaseLogger.Info("Start Database Logger.......................");
        }

        private void ConfigDatabaseTarget(LoggingConfiguration loggingConfiguration)
        {
            DatabaseTarget databaseTarget = new DatabaseTarget();
            databaseTarget.DBProvider = "Oracle.ManagedDataAccess.Client.OracleConnection, Oracle.ManagedDataAccess";
            databaseTarget.ConnectionString = _connectionString;

            databaseTarget.CommandType = System.Data.CommandType.Text;
            databaseTarget.CommandText = "INSERT INTO TB_SYSTEM_LOG\n" +
                                            "  (MACHINE_NAME,\n" +
                                            "   CALL_SITE,\n" +
                                            "   USER_NAME,\n" +
                                            "   LOG_DATE,\n" +
                                            "   LOG_LEVEL,\n" +
                                            "   LOG_MESSAGE,\n" +
                                            "   LOG_EXCEPTION,\n" +
                                            "   LOG_STACK_TRACE,\n" +
                                            "   MONTH_NUMBER)\n" +
                                            "VALUES\n" +
                                            "  (:V_MACHINE_NAME,\n" +
                                            "   :V_CALL_SITE,\n" +
                                            "   :V_USER_NAME,\n" +
                                            "   systimestamp,\n" +
                                            "   :V_LOG_LEVEL,\n" +
                                            "   :V_LOG_MESSAGE,\n" +
                                            "   :V_LOG_EXCEPTION,\n" +
                                            "   :V_LOG_STACK_TRACE,\n" +
                                            "   TO_NUMBER(TO_CHAR(SYSDATE,'MM','NLS_CALENDAR = PERSIAN')))";

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = ":V_MACHINE_NAME",
                Layout = "${machinename}"
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = ":V_CALL_SITE",
                Layout = "${callSite}"
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = ":V_USER_NAME",
                Layout = "${event-properties:item=UserName}"
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = ":V_LOG_LEVEL",
                Layout = "${level}"
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = ":V_LOG_MESSAGE",
                Layout = "${message}"
            });
            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = ":V_LOG_EXCEPTION",
                Layout = "${exception}"
            });
            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = ":V_LOG_STACK_TRACE",
                Layout = "${exception:format=Message,Type,Method,StackTrace,Data:separator=\r\n\r\n:maxInnerExceptionLevel=10:innerFormat=Message,Type,Method,StackTrace,Data:innerExceptionSeparator=\r\n\r\n}"
                //Layout = "${exception:format=Type,Method,StackTrace}"
            });

            bool debugEnabled = bool.Parse(_appSettings.LoggerDebugEnabled);

            loggingConfiguration.AddTarget(DatabaseTargetName, databaseTarget);
            LoggingRule rule = new LoggingRule("*", LogLevel.Trace, databaseTarget);
            if (!debugEnabled)
                rule.DisableLoggingForLevel(LogLevel.Debug);

            loggingConfiguration.LoggingRules.Add(rule);
        }
    }
}
