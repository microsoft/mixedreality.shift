// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Shift.Core.Contracts.Manifests.Tasks;
using Shift.Core.Services.Manifests.Tasks;

namespace Shift.Core.Models.Manifests.Tasks
{
    public class PwshComponentTask : ComponentTask<PwshTaskInfoV1, PwshTaskInfo, PwshComponentTaskHandler>
    {
        public override string TaskType => "pwsh";

        public override PwshTaskInfo Convert(PwshTaskInfoV1 contract)
        {
            return new PwshTaskInfo
            {
                Elevate = contract.Elevate,
                Script = contract.Script,
                Type = TaskType,
            };
        }

        public override PwshTaskInfoV1 Convert(PwshTaskInfo model)
        {
            return new PwshTaskInfoV1
            {
                Elevate = model.Elevate,
                Script = model.Script,
                Type = TaskType,
            };
        }
    }
}