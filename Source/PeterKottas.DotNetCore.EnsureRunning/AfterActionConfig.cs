using PeterKottas.DotNetCore.EnsureRunning.Enum;

namespace PeterKottas.DotNetCore.EnsureRunning
{
    /// <summary>
    /// Configuration that determins what happens after action finishes
    /// </summary>
    public class AfterActionConfig
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AfterActionConfig()
        {
            AfterActionBehaviour = AfterActionBehaviourEnum.RunAgain;
            AfterActionDelayMs = 0;
        }

        /// <summary>
        /// Behaviour that determines what happens after action finished
        /// </summary>
        public AfterActionBehaviourEnum AfterActionBehaviour { get; set; }

        /// <summary>
        /// Custom delay after action. Default from config is used otherwise
        /// </summary>
        public int AfterActionDelayMs { get; set; }
    }
}