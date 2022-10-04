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

        public async Task<ShiftResultCode> DownloadAndProcessBundleAsync(Manifest manifest, string bundle)
        {
            await DownloadBundleAsync(manifest, bundle);
            await ProcessBundleAsync(manifest, bundle);
            return ShiftResultCode.Success;
        }

        public async Task<ShiftResultCode> DownloadAndProcessDefaultBundleAsync(Manifest manifest)
        {
            return await DownloadAndProcessBundleAsync(manifest, "default");
        }

        public async Task<ShiftResultCode> DownloadBundleAsync(Manifest manifest, string bundle)
        {
            _logger.LogInformation($"Downloading bundle:{bundle}...");

            var components = _manifestService.GetBundleComponents(manifest, bundle);
            foreach (var component in components)
            {
                await _componentProcessor.DownloadComponentAsync(component);
            }
            return ShiftResultCode.Success;
        }

        public async Task<ShiftResultCode> DownloadBundleAsync(string manifestPath, string bundle)
        {
            Manifest manifest = await _manifestService.GetManifestAsync(manifestPath);
            return await DownloadBundleAsync(manifest, bundle);
        }

        public async Task<ShiftResultCode> DownloadDefaultBundleAsync(Manifest manifest)
        {
            return await DownloadBundleAsync(manifest, "default");
        }

        public async Task<ShiftResultCode> ProcessBundleAsync(Manifest manifest, string bundle)
        {
            _logger.LogInformation($"Processing bundle: {bundle}...");

            var components = _manifestService.GetBundleComponents(manifest, bundle);
            foreach (var component in components)
            {
                await _componentProcessor.InstallComponentAsync(component);
            }
            return ShiftResultCode.Success;
        }

        public async Task<ShiftResultCode> ProcessDefaultBundleAsync(Manifest manifest)
        {
            return await ProcessBundleAsync(manifest, "default");
        }

        public async Task<ShiftResultCode> ProcessDefaultBundleFromReleaseAsync(Manifest manifest)
        {
            var components = _manifestService.GetDefaultComponents(manifest);
            var workingDir = Directory.GetCurrentDirectory();

            // For each component, perform the associated task
            foreach (var component in components)
            {
                var componentLocation = Path.Join(Directory.GetParent(workingDir).FullName, component.Id);
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