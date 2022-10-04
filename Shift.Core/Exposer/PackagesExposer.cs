// -----------------------------------------------------------------------
// <copyright file="PackagesExposer.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Common;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Manifests;

namespace Shift.Core.Exposer
{
    public class PackagesExposer : IPackagesExposer
    {
        private readonly IComponentService _componentService;
        private readonly ILogger<PackagesExposer> _logger;
        private readonly IManifestService _manifestService;

        public PackagesExposer(
            IManifestService manifestService,
            IComponentService componentService,
            ILogger<PackagesExposer> logger)
        {
            this._manifestService = manifestService;
            this._componentService = componentService;
            this._logger = logger;
        }

        /// <summary>
        /// Given a list of bundles, components, and component versions,
        /// downloads the packages with the specified versions.
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        /// <param name="bundles">Bundle list to download</param>
        /// <param name="components">Component list to download</param>
        /// <param name="versions">Component versions to download</param>
        /// <param name="logEvent">Log event</param>
        /// <returns></returns>
        public async Task DownloadPackagesAndLog(
            Manifest manifest,
            IEnumerable<string> bundles,
            IEnumerable<string> components,
            IEnumerable<string> versions,
            Action<string> logEvent)
        {
            var componentToDownload = ProcessComponentByBundleAndVersionSpecification(manifest, bundles, components, versions);

            foreach (var component in componentToDownload)
            {
                logEvent.Invoke($"Downloading component [{component.Id}]");

                await _componentService.DownloadComponentAsync(component);

                logEvent.Invoke($"Successfully downloaded component [{component.Id}]");
            }
        }

        /// <summary>
        /// Given a list of bundles, components, and component versions,
        /// installs the packages with the specified versions.
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        /// <param name="bundles">Bundle list to download</param>
        /// <param name="components">Component list to download</param>
        /// <param name="versions">Component versions to download</param>
        /// <param name="logEvent">Log event</param>
        /// <returns></returns>
        public async Task InstallPackagesAndLog(
            Manifest manifest,
            IEnumerable<string> bundles,
            IEnumerable<string> components,
            IEnumerable<string> versions,
            Action<string> logEvent)
        {
            var componentToDownload = ProcessComponentByBundleAndVersionSpecification(manifest, bundles, components, versions);

            foreach (var component in componentToDownload)
            {
                logEvent.Invoke($"Installing component [{component.Id}]");

                await _componentService.InstallComponentAsync(component);

                logEvent.Invoke($"Successfully installed component [{component.Id}]");
            }
        }

        /// <summary>
        /// Given a list of bundles, components, and component versions,
        /// get the Component object from the Manifest object
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        /// <param name="bundles">Bundle list to process</param>
        /// <param name="components">Component list to process</param>
        /// <param name="versions">Component versions to process</param>
        /// <returns>List of Component objects</returns>
        /// <exception cref="ShiftException"></exception>
        private IEnumerable<Component> ProcessComponentByBundleAndVersionSpecification(
            Manifest manifest,
            IEnumerable<string> bundles,
            IEnumerable<string> components,
            IEnumerable<string> versions)
        {
            if (versions.Count() != components.Count())
            {
                throw new ShiftException(ShiftResultCode.InvalidArgument, "Components length and versions length does not match");
            }

            var componentToDownload = new HashSet<Component>();

            foreach (var bundle in bundles)
            {
                var bundleComponents = _manifestService.GetBundleComponents(manifest, bundle);
                componentToDownload.UnionWith(bundleComponents);
            }

            for (int i = 0; i < components.Count(); i++)
            {
                var componentCopy = _manifestService.GetComponentByComponentName(manifest, components.ElementAt(i)).DeepClone();
                ((PackageLocation)componentCopy.Location).Version = versions.ElementAt(i);
                componentToDownload.Add(componentCopy);
            }

            return componentToDownload;
        }
    }
}