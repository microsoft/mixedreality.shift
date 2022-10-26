// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Common;
using Shift.Core.Models.Events;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Manifests;
using Shift.Core.Services.Serialization;

namespace Shift.Core.Services
{
    public class InstallationService : IInstallationService
    {
        private readonly IBundleService _bundleService;
        private readonly IComponentService _componentService;
        private readonly ILogger<InstallationService> _logger;
        private readonly IManifestService _manifestService;

        public InstallationService(
            IComponentService componentService,
            IBundleService bundleService,
            IManifestService manifestService,
            ILogger<InstallationService> logger
            )
        {
            _componentService = componentService;
            _bundleService = bundleService;
            _manifestService = manifestService;
            _logger = logger;
        }

        public async Task<ShiftResultCode> RunAsync(
            string manifestPath,
            string bundle,
            bool downloadOnly,
            string stagingDirectory)
        {
            var telemetryEvent = new InitEvent()
            {
                ManifestPath = manifestPath,
                DownloadOnly = downloadOnly,
                StagingDirectory = stagingDirectory
            };

            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            ShiftResultCode resultCode = ShiftResultCode.Unknown;
            
            try
            {
                Manifest manifest = await _manifestService.GetManifestAsync(manifestPath);

                if (downloadOnly)
                {
                    if (!string.IsNullOrEmpty(bundle))
                    {
                        resultCode = await _bundleService.DownloadBundleAsync(manifest, bundle, stagingDirectory);
                    }
                    else
                    {
                        foreach (Component component in manifest.Components)
                        {
                            resultCode = await _componentService.DownloadComponentAsync(component, stagingDirectory);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(bundle))
                    {
                        resultCode = await _bundleService.DownloadAndProcessBundleAsync(manifest, bundle, stagingDirectory);
                    }
                    else
                    {
                        resultCode = await _bundleService.DownloadAndProcessDefaultBundleAsync(manifest, stagingDirectory);
                    }
                }

                return resultCode;
            }
            catch (ShiftException ex)
            {
                telemetryEvent.ExceptionOcurred = true;
                telemetryEvent.ResultCode = ex.ResultCode.ToString();
                exception = ex;
                throw;
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

        public async Task<ShiftResultCode> InitAsync(
            string flavor,
            string organization,
            string project,
            string feed,
            string version)
        {
            var telemetryEvent = new InitEvent();
            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            ShiftResultCode resultCode = ShiftResultCode.Unknown;

            try
            {
                Manifest manifest = await _manifestService.DownloadManifestAndConvertAsync(flavor, organization, project, feed, version);

                resultCode = await _bundleService.DownloadAndProcessDefaultBundleAsync(manifest);

                _logger.LogInformation("Initialization complete.");

                return resultCode;
            }
            catch (ShiftException ex)
            {
                telemetryEvent.ExceptionOcurred = true;
                telemetryEvent.ResultCode = ex.ResultCode.ToString();
                exception = ex;
                throw;
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

        public async Task<ShiftResultCode> InitLocalAsync(string manifestPath)
        {
            var telemetryEvent = new InitEvent();
            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            ShiftResultCode resultCode = ShiftResultCode.Unknown;

            try
            {
                _logger.LogInformation("Starting the set up process...");

                Manifest manifest = await _manifestService.GetManifestAsync(manifestPath);

                resultCode = await _bundleService.DownloadAndProcessDefaultBundleAsync(manifest);

                _logger.LogInformation("Initialization complete.");

                return resultCode;
            }
            catch (ShiftException ex)
            {
                telemetryEvent.ExceptionOcurred = true;
                telemetryEvent.ResultCode = ex.ResultCode.ToString();
                exception = ex;
                throw;
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

        public async Task<ShiftResultCode> InstallBundleAsync(
            string bundle,
            string packageName,
            string organization,
            string project,
            string feed,
            string manifestPath,
            string stagingDirectory = null)
        {
            Manifest manifest = await _manifestService.DownloadManifestAndConvertAsync(
                packageName,
                organization,
                project,
                feed,
                manifestPath: manifestPath,
                stagingDirectory: stagingDirectory);

            return await InstallBundleAsync(bundle, manifest, stagingDirectory);
        }

        public async Task<ShiftResultCode> InstallBundleAsync(
            string bundle,
            string manifestPath,
            string stagingDirectory = null)
        {
            Manifest manifest = await _manifestService.GetManifestAsync(manifestPath);
            return await InstallBundleAsync(bundle, manifest, stagingDirectory);
        }

        private async Task<ShiftResultCode> InstallBundleAsync(
            string bundle,
            Manifest manifest,
            string stagingDirectory = null)
        {
            var telemetryEvent = new InstallEvent();
            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            ShiftResultCode resultCode = ShiftResultCode.Unknown;

            try
            {
                var components = _manifestService.GetBundleComponents(manifest, bundle);

                foreach (var component in components)
                {
                    _logger.LogTrace($"Processing component [{component.Id}].");

                    await _componentService.DownloadComponentAsync(component, stagingDirectory);

                    resultCode = await _componentService.InstallComponentAsync(component, stagingDirectory: stagingDirectory);
                }

                return resultCode;
            }
            catch (ShiftException ex)
            {
                telemetryEvent.ExceptionOcurred = true;
                telemetryEvent.ResultCode = ex.ResultCode.ToString();
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
    }
}