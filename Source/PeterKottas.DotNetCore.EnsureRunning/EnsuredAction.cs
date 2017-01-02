using DTOs;
using Enum;
using PeterKottas.DotNetCore.EnsureRunning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning
{
    public class EnsuredAction : IEnsuredAction
    {
        private EnsuredActionConfig config;

        public EnsuredAction(EnsuredActionConfig config)
        {
            this.config = config;
        }

        public IEnsuredAction Once()
        {
            config.NumberOfTimes = 1;
            return this;
        }

        private AfterActionConfig TryAfterAction(Exception e = null)
        {
            try
            {
                return config.OnException(e);
            }
            catch (Exception)
            {
                return config.OnExceptionExceptionBehaviour;
            }
        }

        public void Run()
        {
            var actionState = new ActionState(0, 0, ActionStateStatusEnum.FirstRun);
            while (true)
            {
                var shouldRunResp = config.Storage.TryStart(new TryStartRequestDTO()
                {
                    ActionId = config.ActionId,
                    NumberOfTimes = config.NumberOfTimes
                });
                switch (shouldRunResp.Status)
                {
                    case TryStartStatusEnum.CanStart:
                        AfterActionConfig afterActionResponse;
                        try
                        {
                            afterActionResponse = config.Action(actionState);
                        }
                        catch (Exception e)
                        {
                            afterActionResponse = TryAfterAction(e);
                        }
                        break;
                    case TryStartStatusEnum.CannotStart:
                        break;
                    case TryStartStatusEnum.StorageError:
                        break;
                    default:
                        break;
                }
            }
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
    }
}
