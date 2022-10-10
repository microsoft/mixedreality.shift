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
        private readonly IManifestService _manifestProcessingService;
        private readonly IPackageFeedService _packageFeedService;

        public ReleaseService(
            IComponentService componentInstallationService,
            IManifestService manifestProcessingService,
            IPackageFeedService packageFeedService,
            IConfiguration configuration,
            ILogger<ReleaseService> logger
            )
        {
            _componentInstallationService = componentInstallationService;
            _manifestProcessingService = manifestProcessingService;
            _packageFeedService = packageFeedService;
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
                Manifest manifest = await _manifestProcessingService.GetManifestAsync(manifestPath);

                // Copy manifest to download root
                Directory.CreateDirectory(downloadRoot);
                File.Copy(manifestPath, Path.Combine(downloadRoot, "manifest.json"));

                // Copy self (shift) to download root
                string targetDir = Path.Combine(downloadRoot, "shift");
                Directory.CreateDirectory(targetDir);

                string sourceDir = AppContext.BaseDirectory;
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
    }
}