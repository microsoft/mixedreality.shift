// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.Common;
using Newtonsoft.Json;
using Shift.Core.Brokers;
using Shift.Core.Contracts.Manifests;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Artifacts;
using Shift.Core.Services.Manifests.Tasks;
using Shift.Core.Services.Serialization;

namespace Shift.Core.Services.Manifests
{
    /// <summary>
    /// Implements the <see cref="IManifestService"/>.
    /// </summary>
    public partial class ManifestService : IManifestService
    {
        private readonly ILogger<ManifestService> _logger;
        private readonly IPackageFeedService _packageFeedService;
        private readonly IComponentTaskProvider _taskProvider;

        private JsonConverter[] _converters;

        public ManifestService(
            IComponentTaskProvider taskProvider,
            IPackageFeedService packageFeedService,
            ILogger<ManifestService> logger)
        {
            _packageFeedService = packageFeedService;
            _logger = logger;
            _taskProvider = taskProvider;
            _converters = new JsonConverter[]
            {
                new PolymorphicTaskInfoConverter(taskProvider),
                new PolymorphicLocationConverter(),
            };
        }

        public async Task<Manifest> DownloadManifestAndConvertAsync(
            string packageName,
            string organization,
            string project,
            string feed,
            string version = null,
            string manifestPath = null,
            string stagingDirectory = null,
            string adoPat = null)
        {
            var downloadRoot = stagingDirectory ?? ProgramDataPath.GetStagingDirectory();
            manifestPath = string.IsNullOrEmpty(manifestPath) ? Path.Join(downloadRoot, $"manifest.json") : manifestPath;

            _logger.LogInformation($"Getting the latest version of {packageName}...");

            version = string.IsNullOrEmpty(version) ? await _packageFeedService.GetLatestVersionAsStringAsync(organization, project, feed, packageName) : version;

            string curVersion = "0.0.0.0";
            if (File.Exists(manifestPath))
            {
                var curManifest = await GetManifestAsync(manifestPath);
                curVersion = curManifest.Version;
            }

            if (AdoPackageFeedBroker.IsVersionGreater(version, curVersion))
            {
                await _packageFeedService.DownloadArtifactAsync(
                    downloadDir: downloadRoot,
                    feed: feed,
                    package: packageName,
                    project: project,
                    version: version,
                    organization: organization,
                    adoPat: adoPat);
            }

            var manifest = await GetManifestAsync(manifestPath);

            return manifest;
        }

        public List<Component> GetBundleComponents(Manifest manifest, string bundle)
        {
            var components = new List<Component>();
            var componentNames = new List<string>();
            var innerBundles = new HashSet<string>();

            foreach (var b in manifest.Bundles)
            {
                if (b.Id == bundle)
                {
                    if (b.Bundles != null)
                    {
                        innerBundles.UnionWith(b.Bundles);
                    }
                    if (b.Components != null)
                    {
                        componentNames.AddRange(b.Components);
                    }
                }
            }

            while (!innerBundles.IsNullOrEmpty())
            {
                var innerBundle = innerBundles.First();
                innerBundles.Remove(innerBundle);
                foreach (var b in manifest.Bundles)
                {
                    if (b.Id == innerBundle)
                    {
                        if (b.Bundles != null)
                        {
                            innerBundles.UnionWith(b.Bundles);
                        }
                        if (b.Components != null)
                        {
                            componentNames.AddRange(b.Components);
                        }
                    }
                }
            }

            foreach (var componentName in componentNames)
            {
                foreach (var c in manifest.Components)
                {
                    if (c.Id == componentName)
                    {
                        components.Add(c);
                    }
                }
            }

            return components;
        }

        public Component GetComponentByComponentName(Manifest manifest, string name)
        {
            foreach (var component in manifest.Components)
            {
                if (component.Id == name)
                {
                    return component;
                }
            }

            return null;
        }

        public List<Component> GetDefaultComponents(Manifest manifest)
        {
            return GetBundleComponents(manifest, "default");
        }

        /// <summary>
        /// Reads the manifest json and converts to Manifest object from the specified file path
        /// </summary>
        /// <param name="manifestPath">Path to manifest.json</param>
        /// <returns>Manifest object</returns>
        public async Task<Manifest> GetManifestAsync(string manifestPath)
        {
            var content = await File.ReadAllTextAsync(manifestPath, Encoding.UTF8);
            var contract = JsonConvert.DeserializeObject<ManifestV1>(content, _converters);

            var manifest = Convert(contract);
            var manifestDirectory = Path.GetDirectoryName(manifestPath);

            foreach (var t in manifest.Components)
            {
                if (t.Location is FolderLocation pl)
                {
                    if (!Path.IsPathRooted(pl.Path))
                    {
                        pl.Path = Path.Combine(manifestDirectory, pl.Path);
                    }
                }
            }

            return manifest;
        }
    }
}