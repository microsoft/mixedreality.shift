// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Common;
using Shift.Core.Models.Manifests;

namespace Shift.Core.Services.Manifests
{
    /// <summary>
    /// Implements <see cref="IBundleService"/> to install manifest components.
    /// </summary>
    public class BundleService : IBundleService
    {
        private readonly IComponentService _componentProcessor;
        private readonly ILogger<BundleService> _logger;
        private readonly IManifestService _manifestService;

        public BundleService(
            IComponentService componentProcessor,
            IManifestService manifestService,
            ILogger<BundleService> logger)
        {
            _componentProcessor = componentProcessor;
            _manifestService = manifestService;
            _logger = logger;
        }

        public async Task<ShiftResultCode> DownloadAndProcessBundleAsync(
            Manifest manifest,
            string bundle,
            string stagingDirectory = null)
        {
            await DownloadBundleAsync(manifest, bundle, stagingDirectory);
            await ProcessBundleAsync(manifest, bundle, stagingDirectory);

            return ShiftResultCode.Success;
        }

        public async Task<ShiftResultCode> DownloadAndProcessDefaultBundleAsync(
            Manifest manifest,
            string stagingDirectory = null)
        {
            return await DownloadAndProcessBundleAsync(manifest, bundle: "default", stagingDirectory);
        }

        public async Task<ShiftResultCode> DownloadBundleAsync(
            Manifest manifest,
            string bundle,
            string stagingDirectory = null)
        {
            _logger.LogInformation($"Downloading bundle:{bundle}...");

            var components = _manifestService.GetBundleComponents(manifest, bundle);

            foreach (var component in components)
            {
                await _componentProcessor.DownloadComponentAsync(component, stagingDirectory);
            }

            return ShiftResultCode.Success;
        }

        public async Task<ShiftResultCode> DownloadBundleAsync(
            string manifestPath,
            string bundle,
            string stagingDirectory = null)
        {
            Manifest manifest = await _manifestService.GetManifestAsync(manifestPath);
            return await DownloadBundleAsync(manifest, bundle, stagingDirectory);
        }

        public async Task<ShiftResultCode> DownloadDefaultBundleAsync(
            Manifest manifest,
            string stagingDirectory = null)
        {
            return await DownloadBundleAsync(manifest, "default", stagingDirectory);
        }

        public async Task<ShiftResultCode> ProcessBundleAsync(
            Manifest manifest,
            string bundle,
            string stagingDirectory = null)
        {
            _logger.LogInformation($"Processing bundle: {bundle}...");

            var components = _manifestService.GetBundleComponents(manifest, bundle);

            foreach (var component in components)
            {
                await _componentProcessor.InstallComponentAsync(component, stagingDirectory: stagingDirectory);
            }

            return ShiftResultCode.Success;
        }

        public async Task<ShiftResultCode> ProcessDefaultBundleAsync(
            Manifest manifest,
            string stagingDirectory = null)
        {
            return await ProcessBundleAsync(manifest, "default", stagingDirectory);
        }

        public async Task<ShiftResultCode> ProcessDefaultBundleFromReleaseAsync(
            Manifest manifest,
            string programPath)
        {
            var components = _manifestService.GetDefaultComponents(manifest);

            // For each component, perform the associated task
            foreach (var component in components)
            {
                var componentLocation = Path.Join(programPath, component.Id);
                if (typeof(PackageLocation).IsInstanceOfType(component.Location))
                {
                    componentLocation = Path.Join(componentLocation, ((PackageLocation)component.Location).Version);
                }
                await _componentProcessor.InstallComponentAsync(component, componentLocation);
            }

            return ShiftResultCode.Success;
        }
    }
}