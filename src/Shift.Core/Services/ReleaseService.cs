// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
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
            string organization,
            string project,
            string feed,
            string packageName)
        {
            var telemetryEvent = new CreateReleaseEvent();
            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            ShiftResultCode resultCode = ShiftResultCode.Unknown;

            try
            {
                string version = await _packageFeedService.GetLatestVersionAsStringAsync(organization, project, feed, packageName);

                string downloadRoot = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{packageName}-release-{version}");
                await _packageFeedService.DownloadArtifactAsync(downloadRoot, feed, packageName, project, version);
                Manifest manifest = await _manifestProcessingService.GetManifestAsync(Path.Join(downloadRoot, "manifest.json"));

                // download all the components
                foreach (var component in manifest.Components)
                {
                    var downloadPath = await _componentInstallationService.DownloadComponentAsync(component, downloadRoot);
                    _logger.LogInformation($"Component {component.Id} downloaded to {downloadPath}");
                }

                // Download the latest mrshift
                string shiftFeed = _configuration.GetValue<string>("feed");
                string shiftPackageName = _configuration.GetValue<string>("packageName");
                string shiftProject = _configuration.GetValue<string>("project");
                string shiftVersion = await _packageFeedService.GetLatestVersionAsStringAsync(organization, shiftProject, shiftFeed, shiftPackageName);

                await _packageFeedService.DownloadArtifactAsync(Path.Join(downloadRoot, "mrshift"), shiftFeed, shiftPackageName, shiftProject, shiftVersion);

                _logger.LogInformation($"Release image can be found under {downloadRoot}");
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