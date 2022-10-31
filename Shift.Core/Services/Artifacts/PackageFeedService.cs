// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Caching.Memory;
using Shift.Core.Brokers;
using Shift.Core.Models.Artifacts;
using Shift.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shift.Core.Services.Artifacts
{
    /// <summary>
    /// Helper class for communicating with Azure DevOps
    /// </summary>
    public class PackageFeedService : IPackageFeedService
    {
        private readonly IMemoryCache _cache;
        private readonly IAdoTokenBroker _tokenBroker;
        private readonly IPackageFeedBrokerFactory _packageFeedBrokerFactory;

        /// <summary>
        /// Initializes static members of the <see cref="PackageFeedService"/> class.
        /// </summary>
        public PackageFeedService(
            IMemoryCache cache,
            IAdoTokenBroker tokenBroker,
            IPackageFeedBrokerFactory packageFeedBrokerFactory)
        {
            _cache = cache;
            _tokenBroker = tokenBroker;
            _packageFeedBrokerFactory = packageFeedBrokerFactory;
        }

        public async Task DownloadArtifactAsync(
            string downloadDir,
            string feed,
            string package,
            string project,
            string version,
            string organization)
        {
            var token = await _tokenBroker.GetTokenCredentialAsync(organization);

            var packageFeedBroker = _packageFeedBrokerFactory.CreatePackageFeedBroker(
                organization,
                project,
                token);

            var artifactToolLocation = await packageFeedBroker.InstallArtifactToolAsync();

            await packageFeedBroker.DownloadPackageAsync(
                packageName: package,
                feedName: feed,
                packageVersion: version,
                downloadPath: downloadDir,
                artifactToolLocation: artifactToolLocation,
                projectName: project);
        }

        public async Task<string> GetLatestVersionAsStringAsync(
            string collectionUri,
            string projectName,
            string feedName,
            string packageName)
        {
            var versions = await GetPackageVersionsAsListOfStringAsync(
                collectionUri,
                projectName,
                feedName,
                packageName,
                versionCount: 1);

            return versions.FirstOrDefault();
        }

        public async Task<IEnumerable<string>> GetPackageVersionsAsListOfStringAsync(
            string collectionUri,
            string projectName,
            string feedName,
            string packageName,
            int versionCount)
        {
            var versions = await GetPackageVersionsAsync(
                collectionUri,
                projectName,
                feedName,
                packageName);

            return versions.Select(x => x.Version).Take(versionCount);
        }

        public async Task<IEnumerable<PackageVersion>> GetPackageVersionsAsync(
            string collectionUri,
            string projectName,
            string feedName,
            string packageName)
        {
            // this access pattern can fail, collectionUri is sometimes FQDN and at othe times an ADO name
            var token = await _tokenBroker.GetTokenCredentialAsync(collectionUri);

            var packageFeedBroker = _packageFeedBrokerFactory.CreatePackageFeedBroker(
                collectionUri,
                projectName,
                token);

            var artifactToolLocation = await packageFeedBroker.InstallArtifactToolAsync();

            // Check if versions exist in cache
            var key = $"{feedName}-{packageName}";
            var versions = await _cache.GetOrCreateAsync(key, async (entry) =>
            {
                var versions = await packageFeedBroker.GetPackageVersionsAsync(
                    feedName,
                    packageName);

                return versions;
            });

            return versions;
        }
    }
}