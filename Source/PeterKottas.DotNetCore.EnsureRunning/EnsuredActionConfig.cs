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
            OnException = (exception) => new AfterActionConfig();
            Storage = null;
            OnExceptionExceptionBehaviour = new AfterActionConfig();
        }

        public int NumberOfTimes { get; set; }

        public Func<ActionState, AfterActionConfig> Action { get; set; }

        public Func<Exception, AfterActionConfig> OnException { get; set; }

        public AfterActionConfig OnExceptionExceptionBehaviour { get; set; }

        public string ActionId { get; set; }

        public IStorage Storage { get; set; }
    }
}
