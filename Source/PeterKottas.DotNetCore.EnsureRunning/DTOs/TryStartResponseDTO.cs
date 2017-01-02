using Enum;

namespace DTOs
{
    public class TryStartResponseDTO
    {
        public TryStartStatusEnum Status { get; set; } 

        public string ErrorMessage { get; set; }
    }
}