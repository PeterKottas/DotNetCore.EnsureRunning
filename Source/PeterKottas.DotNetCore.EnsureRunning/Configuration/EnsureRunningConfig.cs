using PeterKottas.DotNetCore.EnsureRunning.Interfaces;

namespace PeterKottas.DotNetCore.EnsureRunning.Configuration
{
    /// <summary>
    /// Ensure running configuration
    /// </summary>
    public class EnsureRunningConfig
    {
        /// <summary>
        /// It returns true if storage was configured, false otherwise
        /// </summary>
        public bool IsStorageConfigured { get; set; }

        /// <summary>
        /// It returns true EnsureRunning was configured, false otherwise
        /// </summary>
        public bool IsConfigured { get; set; }

        /// <summary>
        /// Configured storage instance
        /// </summary>
        public IStorage Storage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EnsureRunningConfig()
        {
            IsStorageConfigured = false;
            IsStorageConfigured = false;
            Storage = null;
        }
    }
}
