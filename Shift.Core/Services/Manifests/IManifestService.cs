// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Shift.Core.Models.Manifests;

namespace Shift.Core.Services.Manifests
{
    /// <summary>
    /// Defines a manifest service which operates on the <see cref="Manifest"/> domain object. The
    /// service is responsible for retrieving, deserializing and storing of manifests.
    /// </summary>
    public partial interface IManifestService
    {
        /// <summary>
        /// Downloads the latest manifest file from the specified composition repo and converts it
        /// into the Manifest object
        /// </summary>
        /// <param name="packageName">The manifest package name, the product build flavor</param>
        /// <param name="organization">The organization for the manifest</param>
        /// <param name="project">The project for the manifest</param>
        /// <param name="feed">The feed for the manifest</param>
        /// <param name="version">Optional parameter to specify version</param>
        /// <param name="manifestPath">Optional parameter to specify local manifest</param>
        /// <returns></returns>
        Task<Manifest> DownloadManifestAndConvertAsync(
            string packageName,
            string organization,
            string project,
            string feed,
            string version = null,
            string manifestPath = null,
            string stagingDirectory = null,
            string adoPat = null);

        /// <summary>
        /// Gets the components in the specified bundle
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        /// <param name="bundle">Bundle name</param>
        /// <returns>List of components in the specified bundle</returns>
        List<Component> GetBundleComponents(Manifest manifest, string bundle);

        /// <summary>
        /// Gets the Component object from the Manifest
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        /// <param name="name">The component to get</param>
        /// <returns></returns>
        Component GetComponentByComponentName(Manifest manifest, string name);

        /// <summary>
        /// Gets the components in the default bundle
        /// </summary>
        /// <param name="manifest">Maifest object</param>
        /// <returns>List of components in the default bundle</returns>
        List<Component> GetDefaultComponents(Manifest manifest);

        /// <summary>
        /// Load manifest from the specified path.
        /// </summary>
        /// <param name="path">The local file path.</param>
        /// <returns>A manifest.</returns>
        Task<Manifest> GetManifestAsync(string path);
    }
}