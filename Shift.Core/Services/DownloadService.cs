using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Common;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Manifests;

namespace Shift.Core.Services
{
    /**
     * Orchestration service for downloading bundles and components
     */
    public class DownloadService : IDownloadService
    {
        private readonly ILogger _logger;
        private readonly IManifestService _manifestService;
        private readonly IComponentService _componentService;

        public DownloadService(
            IComponentService componentService,
            IManifestService manifestService,
            ILogger<ManifestService> logger
        )
        {
            this._componentService = componentService;
            this._manifestService = manifestService;
            this._logger = logger;
        }

        public async Task<ShiftResultCode> DownloadAsync(
            string[] components,
            string[] versions,
            string packageName,
            string organization,
            string project,
            string feed,
            string stagingDirectory = null,
            string adoPat = null)
        {
            Manifest manifest = await _manifestService.DownloadManifestAndConvertAsync(
                packageName: packageName,
                organization: organization,
                project: project,
                feed: feed,
                stagingDirectory: stagingDirectory,
                adoPat: adoPat);

            return await _componentService.DownloadComponentsAsync(
                components: components,
                versions: versions,
                manifest: manifest,
                stagingDirectory: stagingDirectory,
                adoPat: adoPat);
        }

        public async Task<ShiftResultCode> DownloadAsync(
            string[] components,
            string[] versions,
            string manifestPath,
            string stagingDirectory = null,
            string adoPat = null)
        {
            Manifest manifest = await _manifestService.GetManifestAsync(manifestPath);

            return await _componentService.DownloadComponentsAsync(
                components: components,
                versions: versions,
                manifest: manifest,
                stagingDirectory: stagingDirectory,
                adoPat: adoPat);
        }

        public async Task<ShiftResultCode> DownloadAsync(
            string bundle,
            string packageName,
            string organization,
            string project,
            string feed,
            string stagingDirectory = null,
            string adoPat = null)
        {
            ShiftResultCode resultCode = ShiftResultCode.Unknown;
            try
            {
                Manifest manifest = await _manifestService.DownloadManifestAndConvertAsync(
                    packageName: packageName,
                    organization: organization,
                    project: project,
                    feed: feed,
                    stagingDirectory: stagingDirectory,
                    adoPat: adoPat);

                List<Component> bundleComponents = _manifestService.GetBundleComponents(manifest, bundle);
                foreach (var component in bundleComponents)
                {
                    resultCode = await _componentService.DownloadComponentAsync(
                        component,
                        stagingDirectory: stagingDirectory,
                        adoPat: adoPat);
                }
            }
            catch (Exception ex)
            {
                resultCode = ShiftResultCode.DownloadError;
                throw new ShiftException(
                    resultCode: resultCode,
                    message: ex.Message,
                    ex: ex);
            }

            return resultCode;
        }

        public async Task<ShiftResultCode> DownloadAsync(
            string bundle,
            string manifestPath,
            string stagingDirectory = null,
            string adoPat = null)
        {
            ShiftResultCode resultCode = ShiftResultCode.Unknown;
            try
            {
                Manifest manifest = await _manifestService.GetManifestAsync(manifestPath);

                List<Component> bundleComponents = _manifestService.GetBundleComponents(manifest, bundle);
                foreach (var component in bundleComponents)
                {
                    await _componentService.DownloadComponentAsync(
                        component,
                        stagingDirectory: stagingDirectory,
                        adoPat: adoPat);
                }
            }
            catch (Exception ex)
            {
                resultCode = ShiftResultCode.DownloadError;
                throw new ShiftException(
                    resultCode: resultCode,
                    message: ex.Message,
                    ex: ex);
            }

            return resultCode;
        }

    }
}
