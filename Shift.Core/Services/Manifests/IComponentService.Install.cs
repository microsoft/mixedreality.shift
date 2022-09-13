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
        /// Install a component that has been previously downloaded.
        /// </summary>
        /// <param name="component">The component to install.</param>
        /// <param name="componentLocation">The local path where the component is located.</param>
        /// <returns>The result code.</returns>
        Task<ShiftResultCode> InstallComponentAsync(Component component, string componentLocation = null);

        /// <summary>
        /// Given a list of components, installs them
        /// </summary>
        /// <param name="components">Array of components to download</param>
        /// <param name="versions">Array of versions of components</param>
        /// <param name="packageName">The manifest package name, the product build flavor</param>
        /// <param name="organization">The organization for the manifest</param>
        /// <param name="project">The project for the manifest</param>
        /// <param name="feed">The feed for the manifest</param>
        /// <returns>Shift result code</returns>
        Task<ShiftResultCode> InstallComponentsAsync(
            string[] components,
            string[] versions,
            string packageName,
            string organization,
            string project,
            string feed);

        /// <summary>
        /// Given a list of components, installs them
        /// </summary>
        /// <param name="components">Array of components to download</param>
        /// <param name="versions">Array of versions of components</param>
        /// <param name="manifestPath">Path to local manifest</param>
        /// <returns>Shift result code</returns>
        Task<ShiftResultCode> InstallComponentsAsync(
            string[] components,
            string[] versions,
            string manifestPath);
    }
}