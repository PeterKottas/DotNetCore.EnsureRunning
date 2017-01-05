namespace PeterKottas.DotNetCore.EnsureRunning.DTO
{
    /// <summary>
    /// TryStartRequest
    /// </summary>
    public class TryStartRequestDTO
    {
        /// <summary>
        /// Id of the action
        /// </summary>
        public string ActionId { get; set; }

        /// <summary>
        /// How many times the action should run in total
        /// </summary>
        public int NumberOfTimes { get; set; }

        /// <summary>
        /// Timeout for the heartbeat
        /// </summary>
        public int HeartBeatTimeoutMs { get; set; }
    }
}