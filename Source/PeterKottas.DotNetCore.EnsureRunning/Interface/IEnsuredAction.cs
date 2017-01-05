using System;

namespace PeterKottas.DotNetCore.EnsureRunning.Interfaces
{
    /// <summary>
    /// Action that is ensured to run given amount of times across multiple services
    /// </summary>
    public interface IEnsuredAction
    {
        /// <summary>
        /// Runs the action
        /// </summary>
        void Run();

        /// <summary>
        /// Configures the action to run once
        /// </summary>
        /// <returns>Itself</returns>
        IEnsuredAction Once();

        /// <summary>
        /// Configures the action to run given amount of times
        /// </summary>
        /// <param name="n">Number of times to run the action across multiple services</param>
        /// <returns>Itself</returns>
        IEnsuredAction Times(int n);

        /// <summary>
        /// Add method that triggers when there is an exception in the action
        /// </summary>
        /// <param name="onException">On exception callback</param>
        /// <returns>Itself</returns>
        IEnsuredAction WithOnException(Func<Exception, ActionState, AfterActionConfig> onException);

        /// <summary>
        /// Specifies behaviour of the library when there's an exception in onException
        /// </summary>
        /// <param name="onExceptionExceptionBehaviour">Behaviour that determines what happens if there's an exception in on exception callback</param>
        /// <returns>Itself</returns>
        IEnsuredAction WithExceptionInOnExceptionBehaviour(AfterActionConfig onExceptionExceptionBehaviour);

        /// <summary>
        /// Specifies the delay between actions
        /// </summary>
        /// <param name="betweenActionIntervalMs">Delay between actions in ms</param>
        /// <returns>Itself</returns>
        IEnsuredAction WithBetweenActionDelay(int betweenActionIntervalMs);

        /// <summary>
        /// This interval determines how often the runner checks the storage when waiting to start action execution
        /// </summary>
        /// <param name="storageCheckIntervalMs">Storage check interval in ms</param>
        /// <returns>Itself</returns>
        IEnsuredAction WithStorageCheckInterval(int storageCheckIntervalMs);

        /// <summary>
        /// Hearbeat interval
        /// </summary>
        /// <param name="heartBeatIntervalMs">Heartbeat interval in ms</param>
        /// <returns>Itself</returns>
        IEnsuredAction WithHeartBeatInterval(int heartBeatIntervalMs);

        /// <summary>
        /// Time after which heatbeat is considered to be dead and is automatically replaced by different runner
        /// </summary>
        /// <param name="heartBeatTimeoutMs">Hearbeat timeout in ms</param>
        /// <returns>Itself</returns>
        IEnsuredAction WithHeartBeatTimeout(int heartBeatTimeoutMs);

        /// <summary>
        /// Outputs debug info
        /// </summary>
        /// <returns>Itself</returns>
        IEnsuredAction WithDebugInfo();
    }
}
