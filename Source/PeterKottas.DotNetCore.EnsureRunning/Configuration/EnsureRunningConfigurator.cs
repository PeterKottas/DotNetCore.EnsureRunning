using PeterKottas.DotNetCore.EnsureRunning.Interfaces;
using System;

namespace PeterKottas.DotNetCore.EnsureRunning.Configuration
{
    /// <summary>
    /// EnsureRunning configurator
    /// </summary>
    public class EnsureRunningConfigurator
    {
        private EnsureRunningConfig config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public EnsureRunningConfigurator(EnsureRunningConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Use this method to assign new storage to EnsureRunning
        /// </summary>
        /// <param name="storage"></param>
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
