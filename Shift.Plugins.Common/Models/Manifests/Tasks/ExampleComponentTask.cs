// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Shift.Core.Models.Manifests.Tasks;
using Shift.Plugins.Common.Contracts.Manifests.Tasks;

namespace Shift.Plugins.Common.Models.Manifest.Tasks
{
    public class ExampleComponentTask : ComponentTask<ExampleTaskInfoV1, ExampleTaskInfo, ExampleComponentTaskHandler>
    {
        public override string TaskType => "exampleTask";

        public override ExampleTaskInfo Convert(ExampleTaskInfoV1 contract)
        {
            return new ExampleTaskInfo
            {
                Action = contract.Action,
                Type = TaskType,
            };
        }

        public override ExampleTaskInfoV1 Convert(ExampleTaskInfo model)
        {
            return new ExampleTaskInfoV1
            {
                Action = model.Action,
                Type = TaskType,
            };
        }
    }
}