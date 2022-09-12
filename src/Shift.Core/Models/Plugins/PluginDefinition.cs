// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Shift.Core.Models.Manifests.Tasks;

namespace Shift.Core.Models.Plugins
{
    /// <summary>
    /// Defines a plugin that can be implemented on a per-product basis.
    /// </summary>
    public abstract class PluginDefinition
    {
        /// <summary>
        /// Get commands that should be added to mrshift executable.
        /// </summary>
        /// <returns>A list of commands.</returns>
        public abstract List<Command> GetCommands();

        /// <summary>
        /// Get component tasks that the plugin can handle.
        /// </summary>
        /// <returns>A list of componenet tasks.</returns>
        public abstract List<IComponentTask> GetComponentTasks();

        /// <summary>
        /// Register services required for the plugin.
        /// </summary>
        /// <param name="services">The dependency injeciton service collection.</param>
        public virtual void RegisterServices(IServiceCollection services)
        {
        }
    }
}