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
            string packageVersion,
            string downloadPath,
            string artifactToolLocation,
            string projectName = default);

        Task<string> GetLatestPackageVersionAsync(
            string feedName,
            string packageId);

        Task<List<PackageVersion>> GetPackageVersionsAsync(
            string feedName,
            string packageId);

        Task<string> GetPackageRequestUrlAsStringAsync(
            string feedName,
            string packageId);

        Task<string> InstallArtifactToolAsync(
            string path = default,
            bool forceInstall = false);
    }
}