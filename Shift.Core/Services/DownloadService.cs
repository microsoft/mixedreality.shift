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

        /// <summary>
        /// Downloads the manifest and downloads its latest component given manifest feed location
        /// </summary>
        /// <param name="components">List of components</param>
        /// <param name="versions">List of versions</param>
        /// <param name="packageName">Manifest package name</param>
        /// <param name="organization">Manifest organization name</param>
        /// <param name="project">Manifest project name</param>
        /// <param name="feed">Manifest feed name</param>
        /// <param name="stagingDirectory">Staging directory</param>
        /// <param name="adoPat">Custom ADO PAT to use for authentication</param>
        /// <returns>Shift result code</returns>
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

        /// <summary>
        /// Downloads the component specified in the given manifest
        /// </summary>
        /// <param name="components">List of components</param>
        /// <param name="versions">List of versions</param>
        /// <param name="manifestPath">Path to the manifest file</param>
        /// <param name="stagingDirectory">Staging directory</param>
        /// <param name="adoPat">Custom ADO PAT to use for authentication</param>
        /// <returns>Shift result code</returns>
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

        /// <summary>
        /// Downloads the manifest and downloads its latest bundle components given manifest feed location
        /// </summary>
        /// <param name="budle">Bundle ID</param>
        /// <param name="packageName">Manifest package name</param>
        /// <param name="organization">Manifest organization name</param>
        /// <param name="project">Manifest project name</param>
        /// <param name="feed">Manifest feed name</param>
        /// <param name="stagingDirectory">Staging directory</param>
        /// <param name="adoPat">Custom ADO PAT to use for authentication</param>
        /// <returns>Shift result code</returns>
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


        /// <summary>
        /// Downloads the bundle components specified in the given manifest
        /// </summary>
        /// <param name="bundle">Bundle ID</param>
        /// <param name="manifestPath">Path to the manifest file</param>
        /// <param name="stagingDirectory">Staging directory</param>
        /// <param name="adoPat">Custom ADO PAT to use for authentication</param>
        /// <returns>Shift result code</returns>
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
