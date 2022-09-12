// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Models.Manifests.Tasks
{
    public class PwshTaskInfo : TaskInfo
    {
        public bool Elevate { get; set; }
        public string Script { get; set; }
    }
}