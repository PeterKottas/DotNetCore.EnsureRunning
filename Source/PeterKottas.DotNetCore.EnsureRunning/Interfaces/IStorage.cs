using DTOs;
using PeterKottas.DotNetCore.EnsureRunning.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning.Interfaces
{
    public interface IStorage : IDisposable
    {
        TryStartResponseDTO TryStart(TryStartRequestDTO request);

        ConnectResponseDTO Connect();

        HeartBeatResponseDTO HeartBeat(HeartBeatRequestDTO request);

        bool IsConnected();
    }
}
