// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Shift.Core.Models.Common;
using System.Threading.Tasks;

namespace Shift.Core.Services.Manifests
{
    public interface IPromotionService
    {
        /// <summary>
        /// Create a standalone package that contains all necessary binaries and manifest to perform
        /// an offline installation
        /// </summary>
        /// <param name="adoUri">Azure DevOps Uri</param>
        /// <param name="adoProject">Azure DevOps Project</param>
        /// <param name="adoPat">Azure DevOps Personal Access Token</param>
        /// <param name="targetRepo">The target composition repo</param>
        /// <param name="targetBranch">The repo branch</param>
        /// <param name="targetManifestPath">Path to the manifest file on the composition repo</param>
        /// <param name="targetPromoCriteriaPath">Path to a file with promotion criteria</param>
        /// <param name="componentId">Component id to update</param>
        Task<ShiftResultCode> PromoteManifestAsync(
            string adoUri,
            string adoProject,
            string adoPat,
            string targetRepo,
            string targetBranch,
            string targetManifestPath,
            string targetPromoCriteriaPath,
            string componentId);
    }
}