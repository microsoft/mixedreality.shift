// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Shift.Core.Models.Artifacts;

namespace Shift.Core.Brokers
{
    public interface IPackageFeedBroker
    {
        Task DownloadPackageAsync(
            string packageName,
            string feedName,
            string organization,
            string packageVersion,
            string downloadPath,
            string projectName = default);

        Task<string> GetLatestPackageVersionAsync(
            string feedName,
            string packageId,
            string organization,
            string project);

        Task<List<PackageVersion>> GetPackageVersionsAsync(
            string feedName,
            string packageId,
            string organization,
            string project);
    }
}