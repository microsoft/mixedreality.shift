// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Shift.Core.Models.Manifests.Tasks;

namespace Shift.Core.Services.Manifests.Tasks
{
    public interface IComponentTaskProvider
    {
        IComponentTask GetComponentTask(string taskType);
    }
}