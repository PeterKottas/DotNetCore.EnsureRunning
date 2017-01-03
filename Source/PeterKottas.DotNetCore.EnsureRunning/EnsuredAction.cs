using DTOs;
using Enum;
using PeterKottas.DotNetCore.EnsureRunning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using PeterKottas.DotNetCore.EnsureRunning.DTOs;
using PeterKottas.DotNetCore.EnsureRunning.Enum;

namespace PeterKottas.DotNetCore.EnsureRunning
{
    public class EnsuredAction : IEnsuredAction
    {
        private EnsuredActionConfig config;
        private Timer heartBeatTimer;
        private bool stopAction;

        public EnsuredAction(EnsuredActionConfig config)
        {
            this.config = config;
        }

        public IEnsuredAction Once()
        {
            config.NumberOfTimes = 1;
            return this;
        }

        private AfterActionConfig TryAfterAction(ActionState state, Exception e = null)
        {
            try
            {
                return config.OnException(e, state);
            }
            catch (Exception)
            {
                return config.OnExceptionExceptionBehaviour;
            }
        }

        private void TryConnectStorage()
        {
            try
            {
                if (!config.Storage.IsConnected())
                {
                    config.Storage.Connect();
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Exception occured while trying to connect to storage.", e);
            }
        }

        private void StartHeartBeat(int heartBeatId)
        {
            if (heartBeatTimer != null)
            {
                heartBeatTimer.Dispose();
            }
            heartBeatTimer = new Timer(OnTimedEvent, heartBeatId, 0, config.HeartBeatIntervalMs);
        }

        private void OnTimedEvent(object heartBeatId)
        {
            var resp = config.Storage.HeartBeat(new HeartBeatRequestDTO()
            {
                HeartBeatId = (int)heartBeatId
            });
            if (resp.Status == HeartBeatStatusEnum.Stop)
            {
                stopAction = true;
            }
        }

        private bool ActionLoop()
        {
            var actionState = new ActionState(0, 0, ActionStateStatusEnum.FirstRun);
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
                    actionState = new ActionState(actionState.Counter, actionState.RetryCount, ActionStateStatusEnum.AfterException, e);
                    afterActionResponse = TryAfterAction(actionState, e);
                }
                switch (afterActionResponse.AfterActionBehaviour)
                {
                    case AfterActionBehaviourEnum.RunAgain:
                        if (config.BetweenActionsIntervalMs > 0)
                        {
                            Thread.Sleep(config.BetweenActionsIntervalMs);
                        }
                        break;
                    case AfterActionBehaviourEnum.StopExecuting:
                        return true;
                    case AfterActionBehaviourEnum.StopEnsuring:
                        return false;
                    default:
                        break;
                }

                actionState = new ActionState(actionState.Counter + 1, actionState.RetryCount, ActionStateStatusEnum.AfterNormal);
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
            Console.WriteLine("Starting to ensure");
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
                            break;
                        }
                        break;
                    case TryStartStatusEnum.CannotStart:
                        break;
                    case TryStartStatusEnum.StorageError:
                        break;
                    default:
                        break;
                }
                Thread.Sleep(config.StorageCheckIntervalMs);
                Console.WriteLine("In ensure loop");
            }
        }

        public void Run()
        {
            TryConnectStorage();
            EnsuringLoop();
        }

        public IEnsuredAction Times(int n)
        {
            if (n < 1)
            {
                throw new ArgumentException($"Provide positive number that identifies how many times the action should run across all services. Got [{n}] instead.");
            }
            config.NumberOfTimes = n;
            return this;
        }

        public IEnsuredAction WithOnException(Func<Exception, ActionState, AfterActionConfig> onException)
        {
            if (onException != null)
            {
                config.OnException = onException;
            }
            return this;
        }

        public IEnsuredAction WithExceptionInOnExceptionBehaviour(AfterActionConfig onExceptionExceptionBehaviour)
        {
            if (onExceptionExceptionBehaviour != null)
            {
                config.OnExceptionExceptionBehaviour = onExceptionExceptionBehaviour;
            }
            return this;
        }

        public IEnsuredAction WithBetweenActionDelay(int betweenActionIntervalMs)
        {
            if (betweenActionIntervalMs < 0)
            {
                throw new ArgumentException("Interval between action cannot be less than zero");
            }
            config.BetweenActionsIntervalMs = betweenActionIntervalMs;
            return this;
        }

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
    }
}
