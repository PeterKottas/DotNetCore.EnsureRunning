using PeterKottas.DotNetCore.EnsureRunning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning
{
    public class EnsuredActionConfig
    {
        public EnsuredActionConfig()
        {
            NumberOfTimes = 1;
            ActionId = string.Empty;
            Action = null;
            OnException = (exception, state) => new AfterActionConfig();
            Storage = null;
            OnExceptionExceptionBehaviour = new AfterActionConfig();
            StorageCheckIntervalMs = 6000;
            BetweenActionsIntervalMs = 1000;
            HeartBeatIntervalMs = 3000;
            HeartBeatTimeoutMs = HeartBeatIntervalMs * 3;
        }

        public int NumberOfTimes { get; set; }

        public Func<ActionState, AfterActionConfig> Action { get; set; }

        public Func<Exception, ActionState, AfterActionConfig> OnException { get; set; }

        public AfterActionConfig OnExceptionExceptionBehaviour { get; set; }

        public string ActionId { get; set; }

        public IStorage Storage { get; set; }

        public int StorageCheckIntervalMs { get; set; }

        public int HeartBeatIntervalMs { get; set; }

        public int HeartBeatTimeoutMs { get; set; }

        public int BetweenActionsIntervalMs { get; set; }
    }
}
