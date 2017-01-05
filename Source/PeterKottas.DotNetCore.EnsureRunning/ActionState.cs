using PeterKottas.DotNetCore.EnsureRunning.Enum;
using System;

namespace PeterKottas.DotNetCore.EnsureRunning
{
    public class ActionState
    {
        /// <summary>
        /// Action execution counter
        /// </summary>
        public readonly int Counter;

        /// <summary>
        /// Last action status
        /// </summary>
        public readonly ActionStateStatusEnum LastActionStatus;

        /// <summary>
        /// Last action exception
        /// </summary>
        public readonly Exception LastActionException;

        /// <summary>
        /// Last on exception exception
        /// </summary>
        public readonly Exception LastOnExceptionException;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="lastActionStatus"></param>
        /// <param name="lastActionException"></param>
        /// <param name="lastOnExceptionException"></param>
        public ActionState(int counter, ActionStateStatusEnum lastActionStatus, Exception lastActionException = null, Exception lastOnExceptionException = null)
        {
            Counter = counter;
            LastActionStatus = lastActionStatus;
            LastActionException = lastActionException;
            LastOnExceptionException = lastOnExceptionException;
        }
    }
}
