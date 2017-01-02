using System;

namespace PeterKottas.DotNetCore.EnsureRunning
{
    public class ActionConfig
    {
        public Func<Exception, AfterActionConfig> OnException { get; set; }

        public AfterActionConfig OnExceptionExceptionBehvaiour { get; set; }

        public ActionConfig()
        {
            OnException = (e) => new AfterActionConfig();
            OnExceptionExceptionBehvaiour = new AfterActionConfig();
        }
    }
}