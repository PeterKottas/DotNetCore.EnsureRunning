using PeterKottas.DotNetCore.EnsureRunning.DTO;
using System;

namespace PeterKottas.DotNetCore.EnsureRunning.Interfaces
{
    /// <summary>
    /// Storage interface. Anything that implements this can be used as sotrage for EnsureRunning
    /// </summary>
    public interface IStorage : IDisposable
    {
        /// <summary>
        /// Tries to start an action
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        TryStartResponseDTO TryStart(TryStartRequestDTO request);

        /// <summary>
        /// Connects storage
        /// </summary>
        /// <returns></returns>
        ConnectResponseDTO Connect();

        /// <summary>
        /// HeartBeat
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        HeartBeatResponseDTO HeartBeat(HeartBeatRequestDTO request);

        /// <summary>
        /// It returns true if connection to the server was successfully established, otherwise, it returns false.
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
    }
}
