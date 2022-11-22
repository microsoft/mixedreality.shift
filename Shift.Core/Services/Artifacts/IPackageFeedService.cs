// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Shift.Core.Models.Artifacts;

namespace Shift.Core.Services.Artifacts
{
    /// <summary>
    /// The package feed service is responsible for interacting with package feeds.
    /// </summary>
    public interface IPackageFeedService
    {
        /// <summary>
        /// Download an artifact.
        /// </summary>
        /// <param name="downloadDir"></param>
        /// <param name="feed">The feed name.</param>
        /// <param name="package">The package name.</param>
        /// <param name="project">The project name, if feed is project scoped.</param>
        /// <param name="version">The package version.</param>
        /// <param name="organization">The collection name.</param>
        /// <returns>A task.</returns>
        Task DownloadArtifactAsync(
            string downloadDir,
            string feed,
            string package,
            string project,
            string version,
            string organization);

        /// <summary>
        /// Gets the latest package version
        /// </summary>
        /// <param name="collectionUri">Organization name</param>
        /// <param name="projectName">Project name</param>
        /// <param name="feedName">Feed name</param>
        /// <param name="packageName">Package name</param>
        /// <returns>The latest package version</returns>
        Task<string> GetLatestVersionAsStringAsync(
            string collectionUri,
            string projectName,
            string feedName,
            string packageName);

        /// <summary>
        /// Get versions as a list of string for the specified package.
        /// </summary>
        /// <param name="collectionUri">The collection uri.</param>
        /// <param name="projectName">The project name, if feed is project scoped.</param>
        /// <param name="feedName">The feed name.</param>
        /// <param name="packageName">The package name.</param>
        /// <param name="versionCount">The number of versions to retrieve.</param>
        /// <returns>A list of versions.</returns>
        Task<IEnumerable<string>> GetPackageVersionsAsListOfStringAsync(
            string collectionUri,
            string projectName,
            string feedName,
            string packageName,
            int versionCount);

        /// <summary>
        /// Get versions for the specified package.
        /// </summary>
        /// <param name="collectionUri">The collection uri.</param>
        /// <param name="projectName">The project name, if feed is project scoped.</param>
        /// <param name="feedName">The feed name.</param>
        /// <param name="packageName">The package name.</param>
        /// <param name="versionCount">The number of versions to retrieve.</param>
        /// <returns>A list of versions.</returns>
        Task<IEnumerable<PackageVersion>> GetPackageVersionsAsync(
            string collectionUri,
            string projectName,
            string feedName,
            string packageName);

        /// <summary>
        /// Gets the package feed guid of an artifact
        /// </summary>
        /// <param name="organization">The organization of artifact location</param>
        /// <param name="projectName">The project of artifact location</param>
        /// <param name="feedName">The feed name artifact location</param>
        /// <param name="packageName">The artifact package name</param>
        /// <returns></returns>
        Task<string> GetPackageFeedId(
            string organization,
            string projectName,
            string feedName,
            string packageName);

        /// <summary>
        /// Gets the package project guid of an artifact
        /// </summary>
        /// <param name="organization">The organization of artifact location</param>
        /// <param name="projectName">The project of artifact location</param>
        /// <param name="feedName">The feed name artifact location</param>
        /// <param name="packageName">The artifact package name</param>
        /// <returns></returns>
        Task<string> GetPackageProjectId(
            string organization,
            string projectName,
            string feedName,
            string packageName);
    }
}