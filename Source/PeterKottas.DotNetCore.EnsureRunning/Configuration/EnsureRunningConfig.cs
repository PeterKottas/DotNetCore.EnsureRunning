using PeterKottas.DotNetCore.EnsureRunning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning.Configuration
{
    public class EnsureRunningConfig
    {
        public bool IsStorageConfigured { get; set; }

        public bool IsConfigured { get; set; }

        public IStorage Storage { get; set; }

        public EnsureRunningConfig()
        {
            IsStorageConfigured = false;
            IsStorageConfigured = false;
            Storage = null;
        }
    }
}
