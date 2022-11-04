// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
                if (IsPathToArchive(manifestPath))
                {
                    manifestPath = UnzipAndFindManifestFromArchivePath(manifestPath, out stagingDirectory);
                }
                else if (!IsPathToManifest(manifestPath))
                {
                    throw new ShiftException(
                        resultCode: ShiftResultCode.ManifestNotFound, 
                        message: "Correctly specify the path to manifest file or package archive.");
                }

                ProgramDataPath.UserDefinedStagingDirectory = stagingDirectory;
                telemetryEvent.StagingDirectory = stagingDirectory;

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

        private bool IsPathToManifest(string path)
        {
            return path.EndsWith(".json") && File.Exists(path);
        }

        private bool IsPathToArchive(string path)
        {
            return path.EndsWith(".zip") && File.Exists(path);
        }

        private string UnzipAndFindManifestFromArchivePath(string path, out string stagingDirectory)
        {
            string outputPath = Path.Combine(new FileInfo(path).Directory.FullName, Path.GetFileNameWithoutExtension(path));
            ZipFile.ExtractToDirectory(path, outputPath);

            string[] manifestFiles = Directory.GetFiles(outputPath, "*manifest.json");
            
            if (manifestFiles.Length > 1)
            {
                throw new ShiftException(ShiftResultCode.ManifestNotFound,
                    message: $"More than one json files found in the archive path {outputPath}");
            }
            else if (manifestFiles.Length == 0)
            {
                throw new ShiftException(ShiftResultCode.ManifestNotFound,
                    message: $"No manifest file found in the archive path {outputPath}");
            }

            stagingDirectory = outputPath;
            return manifestFiles[0];
        }
    }
}