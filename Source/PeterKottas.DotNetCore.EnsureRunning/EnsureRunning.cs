﻿using PeterKottas.DotNetCore.EnsureRunning.Configuration;
using PeterKottas.DotNetCore.EnsureRunning.Interfaces;
using System;

namespace PeterKottas.DotNetCore.EnsureRunning
{
    public static class EnsureRunning
    {
        private static EnsureRunningConfig config = new EnsureRunningConfig();

        public static IEnsuredAction Action(string actionId, Func<ActionState, AfterActionConfig> action)
        {
            if (!config.IsConfigured)
            {
                throw new ArgumentException("EnsureRunning requires configuring first to work");
            }
            if (!config.IsStorageConfigured)
            {
                throw new ArgumentException("EnsureRunning requires storage to be configured");
            }
            if (config.Storage == null)
            {
                throw new ArgumentException("Storage was null even after configuration. That suggest storage configuration took place but behaved unexpectidly and current action can't continue");
            }
            var ensuredActionConfig = new EnsuredActionConfig()
            {
                Action = action,
                ActionId = actionId,
                Storage = config.Storage
            };
            return new EnsuredAction(ensuredActionConfig);
        }

        public static void Configure(Action<EnsureRunningConfigurator> configAction)
        {
            var configurator = new EnsureRunningConfigurator(config);
            try
            {
                configAction(configurator);
            }
            catch (Exception e)
            {
                config.IsConfigured = false;
                throw new ArgumentException("ConfigAction threw an exception", e);
            }
            config.IsConfigured = true;
        }
    }
}
