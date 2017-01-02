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
    }
}
