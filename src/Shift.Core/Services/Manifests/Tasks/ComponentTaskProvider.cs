// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Shift.Core.Models.Manifests.Tasks;
using Shift.Core.Models.Plugins;

namespace Shift.Core.Services.Manifests.Tasks
{
    public class ComponentTaskProvider : IComponentTaskProvider
    {
        private readonly IEnumerable<IComponentTask> _componentTasks;

        public ComponentTaskProvider(params PluginDefinition[] plugins)
        {
            _componentTasks = plugins.SelectMany(x => x.GetComponentTasks()).ToList();
        }

        public ComponentTaskProvider(IEnumerable<PluginDefinition> plugins)
        {
            _componentTasks = plugins.SelectMany(x => x.GetComponentTasks()).ToList();
        }

        public IComponentTask GetComponentTask(string taskType)
        {
            foreach (var p in _componentTasks)
            {
                if (string.Equals(taskType, p.TaskType, System.StringComparison.OrdinalIgnoreCase))
                {
                    return p;
                }
            }

            return null;
        }
    }
}