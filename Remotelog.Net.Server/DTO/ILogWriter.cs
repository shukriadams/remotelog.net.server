namespace Remotelog.Net.Server
{
    public interface ILogWriter
    {
        void Initialize();
        void EnforeSizeQuota(LogConfig logConfig);
        void Write(LogConfig logConfig, LogEntry entry);
    }
}
