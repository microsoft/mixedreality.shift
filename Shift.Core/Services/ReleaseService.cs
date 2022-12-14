// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Implements <see cref="IReleaseService"/> to manage release image.
    /// </summary>
    public class ReleaseService : IReleaseService
    {
        private readonly IComponentService _componentService;
        private readonly ILogger<ReleaseService> _logger;
        private readonly IManifestService _manifestService;
        private readonly IBundleService _bundleService;

        public ReleaseService(
            IComponentService componentInstallationService,
            IManifestService manifestProcessingService,
            IBundleService bundleService,
            ILogger<ReleaseService> logger
        )
        {
            _componentService = componentInstallationService;
            _manifestService = manifestProcessingService;
            _bundleService = bundleService;
            _logger = logger;
        }

        public async Task<ShiftResultCode> CreateReleaseAsync(
            string manifestPath,
            string archivePath)
        {
            var telemetryEvent = new CreateReleaseEvent();
            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            ShiftResultCode resultCode = ShiftResultCode.Unknown;

            try
            {
                // make sure archive path doesn't exist
                archivePath = archivePath.EndsWith(".zip") ? archivePath : archivePath + ".zip";
                if (Directory.Exists(archivePath))
                {
                    throw new ShiftException(
                        ShiftResultCode.InvalidUserInput,
                        message: $"Release directory already exists: {archivePath}");
                }

                var downloadRoot = Path.Combine(Path.GetTempPath(), "shift-" + Guid.NewGuid().ToString());
                Manifest manifest = await _manifestService.GetManifestAsync(manifestPath);

                // Copy manifest to download root
                Directory.CreateDirectory(downloadRoot);
                File.Copy(manifestPath, Path.Combine(downloadRoot, "manifest.json"));

                // Copy self (shift) to download root
                string targetDir = Path.Combine(downloadRoot, "shift");
                Directory.CreateDirectory(targetDir);

                string sourceDir = ProgramDataPath.GetWorkingDirectory();
                foreach (var file in Directory.GetFiles(sourceDir))
                {
                    File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
                }

                // Download all the components into the download root
                foreach (var component in manifest.Components)
                {
                    await _componentService.DownloadComponentAsync(component, downloadRoot);
                }

                // create release artifact
                ZipFile.CreateFromDirectory(downloadRoot, archivePath);
                Directory.Delete(downloadRoot, recursive: true);

                _logger.LogInformation($"Release archive can be found at {archivePath}");
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

        public async Task<ShiftResultCode> InitReleaseAsync()
        {
            var telemetryEvent = new InitEvent();
            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            ShiftResultCode resultCode = ShiftResultCode.Unknown;

            try
            {
                _logger.LogInformation("Starting the set up process...");

                string manifestPath = FindManifestPath();
                Manifest manifest = await _manifestService.GetManifestAsync(manifestPath);

                string releaseDirectory = new DirectoryInfo(ProgramDataPath.GetWorkingDirectory()).Parent.FullName;
                resultCode = await _bundleService.ProcessDefaultBundleFromReleaseAsync(manifest, releaseDirectory);

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

        private string FindManifestPath()
        {
            string manifestFileName = "manifest.json";
            string programPath = ProgramDataPath.GetWorkingDirectory();

            string manifestPath = Path.Combine(programPath, manifestFileName);
            if (File.Exists(manifestPath))
            {
                return manifestPath;
            }

            manifestPath = Path.Combine(new DirectoryInfo(programPath).Parent.FullName, manifestFileName);
            if (File.Exists(manifestPath))
            {
                return manifestPath;
            }

            manifestPath = Path.Combine(ProgramDataPath.GetStagingDirectory(), manifestPath);
            if (File.Exists(manifestPath))
            {
                return manifestPath;
            }

            throw new ShiftException(ShiftResultCode.ManifestNotFound, 
                message: $"Cannot find manifest file under working directory {programPath}");
        }
    }
}