// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Shift.Core.Contracts.Manifests.Tasks;

namespace Shift.Plugins.Common.Contracts.Manifests.Tasks
{
    public class ExampleTaskInfoV1 : TaskInfoV1
    {
        public string Action { get; set; }
    }
}