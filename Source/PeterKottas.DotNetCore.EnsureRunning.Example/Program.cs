using PeterKottas.DotNetCore.EnsureRunning.Enum;
using PeterKottas.DotNetCore.EnsureRunning.SqlServer;
using System;

namespace PeterKottas.DotNetCore.EnsureRunning.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EnsureRunning.Configure(configruator =>
            {
                configruator.UseSqlServerStorage("Server=GODDTW8002\\SQLEXPRESS2014;Database=ensure-running;User Id=ensure-running;Password=ensure-running;");
            });
            EnsureRunning.Action("hello-world-action", (state) =>
            {
                Console.WriteLine("Hello world action\nCounter:{0}", state.Counter);
                return new AfterActionConfig();
            }).WithDebugInfo().Times(2).WithHeartBeatInterval(1000).WithStorageCheckInterval(2000).Run();
        }
    }
}
