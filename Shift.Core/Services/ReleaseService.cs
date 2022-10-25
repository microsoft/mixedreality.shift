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
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Common;
using Shift.Core.Models.Events;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Artifacts;
using Shift.Core.Services.Manifests;
using Shift.Core.Services.Serialization;

namespace Shift.Core.Services
{
    /// <summary>
    /// Implements <see cref="IReleaseService"/> to manage release image.
    /// </summary>
    public class ReleaseService : IReleaseService
    {
        private readonly IComponentService _componentInstallationService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReleaseService> _logger;
        private readonly IManifestService _manifestService;
        private readonly IPackageFeedService _packageFeedService;
        private readonly IBundleService _bundleService;

        public ReleaseService(
            IComponentService componentInstallationService,
            IManifestService manifestProcessingService,
            IPackageFeedService packageFeedService,
            IBundleService bundleService,
            IConfiguration configuration,
            ILogger<ReleaseService> logger
            )
        {
            _componentInstallationService = componentInstallationService;
            _manifestService = manifestProcessingService;
            _packageFeedService = packageFeedService;
            _bundleService = bundleService;
            _configuration = configuration;
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
                var downloadTasks = new List<Task>();
                foreach (var component in manifest.Components)
                {
                    downloadTasks.Add(_componentInstallationService.DownloadComponentAsync(component, downloadRoot));
                }

                await Task.WhenAll(downloadTasks);

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