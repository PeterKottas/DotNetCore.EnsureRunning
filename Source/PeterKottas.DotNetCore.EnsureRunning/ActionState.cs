using Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning
{
    public class ActionState
    {
        public readonly int Counter;

        public readonly int RetryCount;

        public readonly ActionStateStatusEnum LastActionStatus;

        public readonly Exception LastActionException;

        public readonly Exception LastOnExceptionException;

        public ActionState(int counter, int retryCount, ActionStateStatusEnum lastActionStatus, Exception lastActionException = null, Exception lastOnExceptionException = null)
        {
            Counter = counter;
            RetryCount = retryCount;
            LastActionStatus = lastActionStatus;
            LastActionException = lastActionException;
            LastOnExceptionException = lastOnExceptionException;
        }
    }
}
