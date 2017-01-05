namespace PeterKottas.DotNetCore.EnsureRunning.DTO
{
    /// <summary>
    /// Hearbeat request
    /// </summary>
    public class HeartBeatRequestDTO
    {
        /// <summary>
        /// Id of the hearbeat session. This value comes from TryStartResponseDTO
        /// </summary>
        public int HeartBeatId { get; set; }
    }
}
