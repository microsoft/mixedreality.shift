// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Common;
using Shift.Core.Models.Events;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Serialization;

namespace Shift.Core.Services.Manifests
{
    public partial class ComponentService : IComponentService
    {
        public async Task<ShiftResultCode> DownloadComponentAsync(
            Component component, 
            string stagingDirectory = null,
            string adoPat = null)
        {
            var telemetryEvent = new DownloadEvent();
            telemetryEvent.ComponentId = component.Id;

            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            ShiftResultCode resultCode = ShiftResultCode.Unknown;

            try
            {
                stagingDirectory ??= ProgramDataPath.GetStagingDirectory();
                string downloadDir = string.Empty;

                if (component.Location is PackageLocation packageLocation)
                {
                    telemetryEvent.ComponentVersion = packageLocation.Version;

                    downloadDir = $@"{stagingDirectory}\{component.Id}\{packageLocation.Version}";

                    if (!Directory.Exists(downloadDir))
                    {
                        _logger.LogInformation($"Downloading component [{component.Id}]");
                        await _packageFeedService.DownloadArtifactAsync(
                            downloadDir: downloadDir,
                            feed: packageLocation.Feed,
                            package: packageLocation.Name,
                            project: packageLocation.Project,
                            version: packageLocation.Version,
                            organization: packageLocation.Organization,
                            adoPat: adoPat);
                    }
                    else
                    {
                        telemetryEvent.DownloadSkipped = true;

                        _logger.LogInformation($"Skipping download of component [{component.Id}] version [{packageLocation.Version}]. " +
                            $"Already exists under {downloadDir}");
                    }
                }
                else if (component.Location is FolderLocation folderLocation)
                {
                    downloadDir = $@"{stagingDirectory}\{component.Id}";

                    if (!Directory.Exists(downloadDir))
                    {
                        CopyDirectory(folderLocation.Path, downloadDir, true);
                    }
                    else
                    {
                        telemetryEvent.DownloadSkipped = true;

                        _logger.LogInformation($"Skipping copying of component [{component.Id}]. " +
                            $"Already exists under {downloadDir}");
                    }
                }

                resultCode = ShiftResultCode.Success;

                return resultCode;
            }
            catch (Exception ex)
            {
                telemetryEvent.ExceptionOcurred = true;
                exception = ex;
                throw;
            }
            finally
            {
                telemetryEvent.DurationMS = stopwatch.ElapsedMilliseconds;
                telemetryEvent.ResultCode = resultCode.ToString();

                _logger.Log(telemetryEvent.ExceptionOcurred ?
                    LogLevel.Critical : LogLevel.Information,
                    new EventId(),
                    telemetryEvent,
                    exception,
                    LogEventSerialization.FormatState);
            }
        }

        public async Task<ShiftResultCode> DownloadComponentsAsync(
            string[] components,
            string[] versions,
            Manifest manifest,
            string stagingDirectory = null,
            string adoPat = null)
        {
            List<Component> componentsToProcess = GetComponentFromManifestByComponentIds(manifest, components, versions);

            foreach (var component in componentsToProcess)
            {
                _logger.LogTrace($"Downloading component [{component.Id}].");

                await DownloadComponentAsync(component, stagingDirectory, adoPat);

                _logger.LogTrace($"Downloaded component [{component.Id}].");
            }

            return ShiftResultCode.Success;
        }
    }
}