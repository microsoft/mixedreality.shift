// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Shift.Core.Models.Common;
using Shift.Core.Models.Manifests;

namespace Shift.Core.Services.Manifests
{
    /// <summary>
    /// Defines the service responsible for downloading and installing components.
    /// </summary>
    public partial interface IComponentService
    {
        /// <summary>
        /// Download the specified component.
        /// </summary>
        /// <param name="component">The component to download.</param>
        Task<ShiftResultCode> DownloadComponentAsync(Component component, string downloadRoot = null);

        /// <summary>
        /// Downloads specified versions of components
        /// </summary>
        /// <param name="components">Array of components to download</param>
        /// <param name="versions">Array of versions of components</param>
        /// <param name="packageName">The manifest package name, the product build flavor</param>
        /// <param name="organization">The organization for the manifest</param>
        /// <param name="project">The project for the manifest</param>
        /// <param name="feed">The feed for the manifest</param>
        /// <returns>Shift result code</returns>
        Task<ShiftResultCode> DownloadComponentsAsync(
            string[] components,
            string[] versions,
            string packageName,
            string organization,
            string project,
            string feed);

        /// <summary>
        /// Downloads specified versions of components from a local manifest
        /// </summary>
        /// <param name="components">Array of components to download</param>
        /// <param name="versions">Array of versions of components</param>
        /// <param name="manifestPath">Path to local manifest</param>
        /// <returns>Shift result code</returns>
        Task<ShiftResultCode> DownloadComponentsAsync(
            string[] components,
            string[] versions,
            string manifestPath);
    }
}