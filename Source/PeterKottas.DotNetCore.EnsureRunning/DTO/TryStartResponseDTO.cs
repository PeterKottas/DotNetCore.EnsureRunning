using PeterKottas.DotNetCore.EnsureRunning.Enum;

namespace PeterKottas.DotNetCore.EnsureRunning.DTO
{
    /// <summary>
    /// Try start response
    /// </summary>
    public class TryStartResponseDTO
    {
        /// <summary>
        /// Status that determines if the action should start or not
        /// </summary>
        public TryStartStatusEnum Status { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// HearbeatID to be used for all hearbeats in this session
        /// </summary>
        public int HeartBeatId { get; set; }
    }
}