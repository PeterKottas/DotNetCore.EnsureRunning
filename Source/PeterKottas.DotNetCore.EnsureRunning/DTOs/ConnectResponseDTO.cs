using Enum;

namespace DTOs
{
    public class ConnectResponseDTO
    {
        ConnectStatusEnum Status { get; set; }

        public string ErrorMessage { get; set; }
    }
}