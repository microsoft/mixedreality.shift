// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Brokers;
using Shift.Core.Models.Artifacts;
using Shift.Core.Models.Common;
using Shift.Core.Models.Events;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Artifacts;
using Shift.Core.Services.Serialization;

namespace Shift.Core.Services.Manifests
{
    /// <summary>
    /// Implements <see cref="IPromotionService"/> to manage release image.
    /// </summary>
    public class PromotionService : IPromotionService
    {
        private readonly ILogger<PromotionService> _logger;
        private readonly IManifestService _manifestProcessingService;
        private readonly IPackageFeedService _packageFeedService;
        private readonly ISourceCodeBroker _sourceCodeBroker;
        private readonly IAdoTokenBroker _tokenBroker;

        public PromotionService(
            IManifestService manifestProcessingService,
            IPackageFeedService packageFeedService,
            ISourceCodeBroker sourceCodeBroker,
            IAdoTokenBroker tokenBroker,
            ILogger<PromotionService> logger
            )
        {
            _manifestProcessingService = manifestProcessingService;
            _packageFeedService = packageFeedService;
            _sourceCodeBroker = sourceCodeBroker;
            _tokenBroker = tokenBroker;
            _logger = logger;
        }

        public async Task<ShiftResultCode> PromoteManifestAsync(
            string adoUri,
            string adoProject,
            string adoPat,
            string targetRepo,
            string targetBranch,
            string targetManifestPath,
            string targetPromoCriteriaPath,
            string componentId)
        {
            var telemetryEvent = new PromoteManifestEvent();
            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            ShiftResultCode resultCode = ShiftResultCode.Unknown;

            try
            {
                // Download and cast the manifest file from the target repository into the manifest object
                var manifestBytes = await _sourceCodeBroker.DownloadFileAsync(adoUri, adoProject, targetRepo, targetBranch, targetManifestPath);
                var promotionCriteriaBytes = await _sourceCodeBroker.DownloadFileAsync(adoUri, adoProject, targetRepo, targetBranch, targetPromoCriteriaPath);
                var promotionCriteria = _manifestProcessingService.ConvertBytesToManifestPromotionCriteria(promotionCriteriaBytes);
                var targetManifest = _manifestProcessingService.ConvertBytesToManifest(manifestBytes);

                // Update the current manifest if there is a version update of a component
                bool updated = false;
                foreach (var targetComponent in targetManifest.Components)
                {
                    if (targetComponent.Id == componentId || string.IsNullOrWhiteSpace(componentId))
                    {
                        if (targetComponent.Location is PackageLocation packageLocation)
                        {
                            // default behavior is to grab latest version if no promotion criteria specified
                            if (!promotionCriteria.Components.TryGetValue(targetComponent.Id, out var componentCriteria))
                            {
                                componentCriteria = new PromotionCriteria
                                {
                                    Filter = new Regex(".*"),
                                    RequiredViews = Array.Empty<string>(),
                                    Strategy = "LatestChronological"
                                };
                            }

                            if (componentCriteria.Strategy == "LatestChronological")
                            {
                                var versions = await _packageFeedService.GetPackageVersionsAsync(
                                    collectionUri: packageLocation.Organization,
                                    projectName: packageLocation.Project,
                                    feedName: packageLocation.Feed,
                                    packageName: packageLocation.Name);

                                // go to latest according to strategy & filters
                                var v = versions.Where(x => componentCriteria.Filter.IsMatch(x.Version))
                                    .Where(x => componentCriteria.RequiredViews.All(tag => x.Views.Contains(tag)))
                                    .Where(x => x.PublishDate.HasValue)
                                    .OrderByDescending(x => x.PublishDate)
                                    .FirstOrDefault();

                                // set new path
                                if (v != null && v.Version != packageLocation.Version)
                                {
                                    packageLocation.Version = v.Version;
                                    _logger.LogInformation($"Updating component {targetComponent.Id} to version {v}");
                                    updated = true;
                                }
                            }
                        }
                    }
                }

                if (updated)
                {
                    var targetManifestBytes = _manifestProcessingService.ConvertManifestToBytes(targetManifest);
                    var intermediateBranch = $"automated/promote-{targetBranch}-{Guid.NewGuid().ToString().Substring(0, 8)}";

                    // Create a new branch off our target branch with the intended change
                    await _sourceCodeBroker.CreatePushAsync(
                        organization: adoUri,
                        projectName: adoProject,
                        repositoryName: targetRepo,
                        sourceBranch: targetBranch,
                        branchName: intermediateBranch,
                        comment: "Promoting manifest versions",
                        changes: new[] { new EditItemChange { Path = targetManifestPath, Content = targetManifestBytes } }
                        );

                    // Create PR to merge back into target branch
                    await _sourceCodeBroker.CreatePullRequestAsync(
                        organization: adoUri,
                        projectName: adoProject,
                        repositoryName: targetRepo,
                        sourceBranch: intermediateBranch,
                        targetBranch: targetBranch,
                        enableAutoComplete: true);

                    _logger.LogInformation($"Created pull request to update {targetBranch} since new versions were found for components.");
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
    }
}