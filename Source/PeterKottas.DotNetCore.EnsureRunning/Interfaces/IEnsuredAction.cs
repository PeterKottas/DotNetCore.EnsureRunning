using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.EnsureRunning.Interfaces
{
    public interface IEnsuredAction
    {
        void Run();

        IEnsuredAction Once();

        IEnsuredAction Times(int n);

        IEnsuredAction WithOnException(Func<Exception, ActionState, AfterActionConfig> onException);

        IEnsuredAction WithExceptionInOnExceptionBehaviour(AfterActionConfig onExceptionExceptionBehaviour);

        IEnsuredAction WithBetweenActionDelay(int betweenActionIntervalMs);

        IEnsuredAction WithStorageCheckInterval(int storageCheckIntervalMs);

        IEnsuredAction WithHeartBeatInterval(int heartBeatIntervalMs);

        IEnsuredAction WithHeartBeatTimeout(int heartBeatTimeoutMs);
    }
}
