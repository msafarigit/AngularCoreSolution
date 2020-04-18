using NLog;

namespace Infrastructure.Logging
{
    public interface ILoggerService
    {
        ILogger DatabaseLogger { get; }
    }
}
