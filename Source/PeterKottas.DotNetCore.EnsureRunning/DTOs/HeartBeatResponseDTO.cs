using PeterKottas.DotNetCore.EnsureRunning.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning.DTOs
{
    public class HeartBeatResponseDTO
    {
        public HeartBeatStatusEnum Status { get; set; }
    }
}
