using PeterKottas.DotNetCore.EnsureRunning.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning.SqlServer
{
    public static class StorageExtensions
    {
        public static void UseSqlServerStorage(this EnsureRunningConfigurator configurator, string connectionString, SqlServerConfig config = null)
        {
            var storage = new SqlServerStorage(connectionString, config);
            configurator.UseStorage(storage);
        }
    }
}
