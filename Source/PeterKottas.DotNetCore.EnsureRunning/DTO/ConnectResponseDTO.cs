using PeterKottas.DotNetCore.EnsureRunning.Enum;

namespace PeterKottas.DotNetCore.EnsureRunning.DTO
{
    /// <summary>
    /// Response for connect method
    /// </summary>
    public class ConnectResponseDTO
    {
        /// <summary>
        /// Status
        /// </summary>
        ConnectStatusEnum Status { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}