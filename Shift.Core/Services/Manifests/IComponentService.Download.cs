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
        Task<ShiftResultCode> DownloadComponentAsync(
            Component component,
            string stagingDirectory = null,
            string adoPat = null);

        /// <summary>
        /// Downloads specified versions of components in the Manifest
        /// </summary>
        /// <param name="components">Array of components to download</param>
        /// <param name="versions">Array of versions of components</param>
        /// <param name="manifest">Manifest object</param>
        /// <param name="stagingDirectory">Staging directory</param>
        /// <param name="adoPat">ADO PAT</param>
        /// <returns>Shift result code</returns>
        Task<ShiftResultCode> DownloadComponentsAsync(
            string[] components,
            string[] versions,
            Manifest manifest,
            string stagingDirectory = null,
            string adoPat = null);
    }
}