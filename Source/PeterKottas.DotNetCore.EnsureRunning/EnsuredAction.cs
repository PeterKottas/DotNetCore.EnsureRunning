using PeterKottas.DotNetCore.EnsureRunning.Interfaces;
using System;
using System.Threading;
using PeterKottas.DotNetCore.EnsureRunning.DTO;
using PeterKottas.DotNetCore.EnsureRunning.Enum;

namespace PeterKottas.DotNetCore.EnsureRunning
{
    /// <summary>
    /// Action that is ensured to run given amount of times across multiple services
    /// </summary>
    public class EnsuredAction : IEnsuredAction
    {
        private EnsuredActionConfig config;
        private Timer heartBeatTimer;
        private bool stopAction;

        private void LogDebug(string message)
        {
            if (config.ShowDebug)
            {
                Console.WriteLine(message);
            }
        }

        private AfterActionConfig TryAfterAction(ActionState state, Exception e = null)
        {
            try
            {
                LogDebug("Starting onException");
                return config.OnException(e, state);
            }
            catch (Exception)
            {
                LogDebug("Exception in onException");
                return config.OnExceptionExceptionBehaviour;
            }
            finally
            {
                LogDebug("Finished onException");
            }
        }

        private void TryConnectStorage()
        {
            try
            {
                LogDebug("Trying to connect");
                if (!config.Storage.IsConnected())
                {
                    config.Storage.Connect();
                    LogDebug("Connected");
                }
                else
                {
                    LogDebug("Was already connected");
                }
            }
            catch (Exception e)
            {
                LogDebug($"Exception while connecting: {e.ToString()}");
                throw new ArgumentException("Exception occured while trying to connect to storage.", e);
            }
        }

        private void StartHeartBeat(int heartBeatId)
        {
            LogDebug($"Starting heartbeat timer");
            if (heartBeatTimer != null)
            {
                heartBeatTimer.Dispose();
            }
            heartBeatTimer = new Timer(OnTimedEvent, heartBeatId, 0, config.HeartBeatIntervalMs);
            LogDebug($"Started heartbeat timer");
        }

        private void OnTimedEvent(object heartBeatId)
        {
            LogDebug($"Starting single heartbeat");
            var resp = config.Storage.HeartBeat(new HeartBeatRequestDTO()
            {
                HeartBeatId = (int)heartBeatId
            });
            if (resp.Status == HeartBeatStatusEnum.Stop)
            {
                stopAction = true;
            }
            LogDebug($"Finished single heartbeat");
        }

        private bool ActionLoop()
        {
            LogDebug($"Starting action loop");
            var actionState = new ActionState(0, ActionStateStatusEnum.FirstRun);
            stopAction = false;
            while (true)
            {
                AfterActionConfig afterActionResponse;
                try
                {
                    afterActionResponse = config.Action(actionState);
                }
                catch (Exception e)
                {
                    actionState = new ActionState(actionState.Counter, ActionStateStatusEnum.AfterException, e);
                    afterActionResponse = TryAfterAction(actionState, e);
                }
                switch (afterActionResponse.AfterActionBehaviour)
                {
                    case AfterActionBehaviourEnum.RunAgain:
                        LogDebug($"Running action again");
                        if (afterActionResponse.AfterActionDelayMs > 0)
                        {
                            Thread.Sleep(afterActionResponse.AfterActionDelayMs);
                        }
                        else if (config.BetweenActionsIntervalMs > 0)
                        {
                            Thread.Sleep(config.BetweenActionsIntervalMs);
                        }
                        break;
                    case AfterActionBehaviourEnum.StopExecuting:
                        LogDebug($"Stoppoing executing action");
                        return true;
                    case AfterActionBehaviourEnum.StopEnsuring:
                        LogDebug($"Stopping ensuring");
                        return false;
                    default:
                        break;
                }

                actionState = new ActionState(actionState.Counter + 1, ActionStateStatusEnum.AfterNormal);
                if (stopAction)
                {
                    if (afterActionResponse.AfterActionBehaviour == AfterActionBehaviourEnum.StopEnsuring)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        private void EnsuringLoop()
        {
            LogDebug("Starting to ensure");
            while (true)
            {
                var shouldRunResp = config.Storage.TryStart(new TryStartRequestDTO()
                {
                    ActionId = config.ActionId,
                    NumberOfTimes = config.NumberOfTimes,
                    HeartBeatTimeoutMs = config.HeartBeatTimeoutMs
                });
                switch (shouldRunResp.Status)
                {
                    case TryStartStatusEnum.CanStart:
                        StartHeartBeat(shouldRunResp.HeartBeatId);
                        var shouldEnsure = ActionLoop();
                        heartBeatTimer.Dispose();
                        if (!shouldEnsure)
                        {
                            return;
                        }
                        break;
                    case TryStartStatusEnum.CannotStart:
                        break;
                    case TryStartStatusEnum.StorageError:
                        LogDebug("In ensure loop");
                        break;
                    default:
                        break;
                }
                Thread.Sleep(config.StorageCheckIntervalMs);
                LogDebug("In ensure loop");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public EnsuredAction(EnsuredActionConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Runs the action
        /// </summary>
        public void Run()
        {
            LogDebug($@"Starting to run with following config:
                ActionId:{config.ActionId}
                BetweenActionsIntervalMs:{config.BetweenActionsIntervalMs}
                HeartBeatIntervalMs:{config.HeartBeatIntervalMs}
                HeartBeatTimeoutMs:{config.HeartBeatTimeoutMs}
                NumberOfTimes:{config.NumberOfTimes}
                OnExceptionExceptionBehaviour:{config.OnExceptionExceptionBehaviour.AfterActionBehaviour}
                ShowDebug:{config.ShowDebug}
                StorageCheckIntervalMs:{config.StorageCheckIntervalMs}
                "
                );
            TryConnectStorage();
            EnsuringLoop();
        }

        /// <summary>
        /// Configures the action to run once
        /// </summary>
        /// <returns>Itself</returns>
        public IEnsuredAction Once()
        {
            config.NumberOfTimes = 1;
            return this;
        }

        /// <summary>
        /// Configures the action to run given amount of times
        /// </summary>
        /// <param name="n">Number of times to run the action across multiple services</param>
        /// <returns>Itself</returns>
        public IEnsuredAction Times(int n)
        {
            if (n < 1)
            {
                throw new ArgumentException($"Provide positive number that identifies how many times the action should run across all services. Got [{n}] instead.");
            }
            config.NumberOfTimes = n;
            return this;
        }

        /// <summary>
        /// Add method that triggers when there is an exception in the action
        /// </summary>
        /// <param name="onException">On exception callback</param>
        /// <returns>Itself</returns>
        public IEnsuredAction WithOnException(Func<Exception, ActionState, AfterActionConfig> onException)
        {
            if (onException != null)
            {
                config.OnException = onException;
            }
            return this;
        }

        /// <summary>
        /// Specifies behaviour of the library when there's an exception in onException
        /// </summary>
        /// <param name="onExceptionExceptionBehaviour">Behaviour that determines what happens if there's an exception in on exception callback</param>
        /// <returns>Itself</returns>
        public IEnsuredAction WithExceptionInOnExceptionBehaviour(AfterActionConfig onExceptionExceptionBehaviour)
        {
            if (onExceptionExceptionBehaviour != null)
            {
                config.OnExceptionExceptionBehaviour = onExceptionExceptionBehaviour;
            }
            return this;
        }

        /// <summary>
        /// Specifies the delay between actions
        /// </summary>
        /// <param name="betweenActionIntervalMs">Delay between actions in ms</param>
        /// <returns>Itself</returns>
        public IEnsuredAction WithBetweenActionDelay(int betweenActionIntervalMs)
        {
            if (betweenActionIntervalMs < 0)
            {
                throw new ArgumentException("Interval between action cannot be less than zero");
            }
            config.BetweenActionsIntervalMs = betweenActionIntervalMs;
            return this;
        }

        /// <summary>
        /// This interval determines how often the runner checks the storage when waiting to start action execution
        /// </summary>
        /// <param name="storageCheckIntervalMs">Storage check interval in ms</param>
        /// <returns>Itself</returns>
        public IEnsuredAction WithStorageCheckInterval(int storageCheckIntervalMs)
        {
            if (storageCheckIntervalMs < 0)
            {
                throw new ArgumentException(@"Storage check interval cannot be less than zero. 
                    This determines how often we check storage for running action so it's considered keeping this above 1000ms to avoid
                    performance issues");
            }
            if (storageCheckIntervalMs < 1000)
            {
                Console.WriteLine(@"Storage check interval was less than 1000ms. 
                    This determines how often we check storage for running action so it's considered keeping this above 1000ms to avoid
                    performance issues");
            }
            config.StorageCheckIntervalMs = storageCheckIntervalMs;
            return this;
        }

        /// <summary>
        /// Hearbeat interval
        /// </summary>
        /// <param name="heartBeatIntervalMs">Heartbeat interval in ms</param>
        /// <returns>Itself</returns>
        public IEnsuredAction WithHeartBeatInterval(int heartBeatIntervalMs)
        {
            if (heartBeatIntervalMs < 0)
            {
                throw new ArgumentException(@"Heartbeat interval cannot be less than zero. 
                    This determines how often we update storage, consider keeping this above 1000ms to avoid
                    performance issues");
            }
            if (heartBeatIntervalMs < 1000)
            {
                Console.WriteLine(@"Heartbeat interval was less than 1000ms. 
                    This determines how often we update storage, consider keeping this above 1000ms to avoid
                    performance issues");
            }
            config.HeartBeatIntervalMs = heartBeatIntervalMs;
            config.HeartBeatTimeoutMs = heartBeatIntervalMs * 3;
            return this;
        }

        /// <summary>
        /// Time after which heatbeat is considered to be dead and is automatically replaced by different runner
        /// </summary>
        /// <param name="heartBeatTimeoutMs">Hearbeat timeout in ms</param>
        /// <returns>Itself</returns>
        public IEnsuredAction WithHeartBeatTimeout(int heartBeatTimeoutMs)
        {
            if (heartBeatTimeoutMs < 0)
            {
                throw new ArgumentException(@"Heartbeat timeout cannot be less than zero. 
                    This determines how often we update storage, consider keeping this above 1000ms to avoid
                    performance issues");
            }
            if (heartBeatTimeoutMs < config.HeartBeatIntervalMs)
            {
                Console.WriteLine(@"Heartbeat timeout was less than heartbeat interval.");
            }
            config.HeartBeatTimeoutMs = heartBeatTimeoutMs;
            return this;
        }

        /// <summary>
        /// Outputs debug info
        /// </summary>
        /// <returns>Itself</returns>
        public IEnsuredAction WithDebugInfo()
        {
            config.ShowDebug = true;
            return this;
        }
    }
}
