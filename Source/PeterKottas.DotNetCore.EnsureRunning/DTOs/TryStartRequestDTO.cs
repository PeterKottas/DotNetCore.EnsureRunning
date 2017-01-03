namespace DTOs
{
    public class TryStartRequestDTO
    {
        public string ActionId { get; set; }

        public int NumberOfTimes { get; set; }

        public int HeartBeatTimeoutMs { get; set; }
    }
}