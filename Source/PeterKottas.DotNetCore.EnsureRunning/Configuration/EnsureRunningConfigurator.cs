using PeterKottas.DotNetCore.EnsureRunning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning.Configuration
{
    public class EnsureRunningConfigurator
    {
        private EnsureRunningConfig config;

        public EnsureRunningConfigurator(EnsureRunningConfig config)
        {
            this.config = config;
        }

        public void UseStorage(IStorage storage)
        {
            if (config == null)
            {
                throw new ArgumentException("Parameter passed to UseStorage method was null");
            }
            config.Storage = storage;
            config.IsStorageConfigured = true;
        }
    }
}
