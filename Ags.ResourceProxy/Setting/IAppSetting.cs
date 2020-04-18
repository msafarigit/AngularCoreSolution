namespace Infrastructure.Setting
{
    public interface IAppSetting
    {
        string LoggerThrowException { get; set; }
        string LoggerDebugEnabled { get; set; }
    }
}
