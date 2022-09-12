// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Shift.Core.Models.Manifests;

namespace Shift.Core.Services.Manifests.Tasks
{
    public interface IComponentTaskHandler
    {
        Task ExecuteAsync(string path, Component component);
    }
}