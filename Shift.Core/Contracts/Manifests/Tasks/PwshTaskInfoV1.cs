// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Contracts.Manifests.Tasks
{
    public class PwshTaskInfoV1 : TaskInfoV1
    {
        public bool Elevate { get; set; }
        public string Script { get; set; }
    }
}