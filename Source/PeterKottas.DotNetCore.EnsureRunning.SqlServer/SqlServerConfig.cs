namespace PeterKottas.DotNetCore.EnsureRunning.SqlServer
{
    public class SqlServerConfig
    {
        public int HeartBeatIntervalMs { get; set; }

        public SqlServerConfig()
        {
            HeartBeatIntervalMs = 15000;
        }
    }
}