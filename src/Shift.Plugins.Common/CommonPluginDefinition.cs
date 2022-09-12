// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.CommandLine;
using Shift.Core.Models.Manifests.Tasks;
using Shift.Core.Models.Plugins;

namespace Shift.Plugins.Common
{
    /// <summary>
    /// Defines the plugin for the ARDK HMD that contains ardk-specific commands.
    /// </summary>
    public class CommonPluginDefinition : PluginDefinition
    {
        public override List<Command> GetCommands()
        {
            return new List<Command>
            {
            };
        }

        public override List<IComponentTask> GetComponentTasks()
        {
            return new List<IComponentTask>
            {
                // Add custom tasks here
            };
        }
    }
}