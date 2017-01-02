using Enum;

namespace PeterKottas.DotNetCore.EnsureRunning
{
    public class AfterActionConfig
    {
        public AfterActionConfig()
        {
            AfterActionBehaviour = AfterActionBehaviourEnum.RunAgain;
        }

        public AfterActionBehaviourEnum AfterActionBehaviour { get; set; }
    }
}