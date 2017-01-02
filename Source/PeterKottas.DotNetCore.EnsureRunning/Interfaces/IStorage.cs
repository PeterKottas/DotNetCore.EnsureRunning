using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning.Interfaces
{
    public interface IStorage
    {
        TryStartResponseDTO TryStart(TryStartRequestDTO request);

        ConnectResponseDTO Connect();
    }
}
