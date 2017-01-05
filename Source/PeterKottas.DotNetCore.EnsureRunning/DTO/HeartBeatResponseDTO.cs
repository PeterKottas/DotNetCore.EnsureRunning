using PeterKottas.DotNetCore.EnsureRunning.Enum;

namespace PeterKottas.DotNetCore.EnsureRunning.DTO
{
    /// <summary>
    /// HeartBeat response
    /// </summary>
    public class HeartBeatResponseDTO
    {
        /// <summary>
        /// Status of the heartbeat
        /// </summary>
        public HeartBeatStatusEnum Status { get; set; }
    }
}
