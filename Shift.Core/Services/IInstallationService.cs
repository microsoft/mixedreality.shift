// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Shift.Core.Models.Common;

namespace Shift.Core.Services
{
    /// <summary>
    /// Defines the service responsible for downloading and installing components for a specific manifest
    /// </summary>
    public interface IInstallationService
    {
        Task<ShiftResultCode> RunAsync(
            string manifestPath,
            string bundle,
            bool downloadOnly,
            string stagingDirectory);

        /// <summary>
        /// Given the location of the manifest file on the feed, downloads the appropriate
        /// components and performs the associated tasks
        /// </summary>
        /// <param name="flavor">Product build flavor</param>
        /// <param name="organization">The organization for the manifest</param>
        /// <param name="project">The project for the manifest</param>
        /// <param name="feed">The feed for the manifest</param>
        /// <param name="version">The build version</param>
        /// <returns>Shift result code</returns>
        Task<ShiftResultCode> InitAsync(
            string flavor,
            string organization,
            string project,
            string feed,
            string version);

        /// <summary>
        /// Given the local manifest file, downloads the appropriate components and performs the
        /// associated tasks
        /// </summary>
        /// <param name="manifestPath"></param>
        /// <returns></returns>
        Task<ShiftResultCode> InitLocalAsync(string manifestPath);

        /// <summary>
        /// Installs the components in the given bundle
        /// </summary>
        /// <param name="bundle">Bundle name</param>
        /// <param name="packageName">The manifest package name, the product build flavor</param>
        /// <param name="organization">The organization for the manifest</param>
        /// <param name="project">The project for the manifest</param>
        /// <param name="feed">The feed for the manifest</param>
        /// <param name="manifestPath">Optional parameter to use local manifest file</param>
        /// <returns></returns>
        Task<ShiftResultCode> InstallBundleAsync(
            string bundle,
            string packageName,
            string organization,
            string project,
            string feed,
            string manifestPath = null,
            string stagingDirectory = null);

        /// <summary>
        /// Installs the components in the given bundle
        /// </summary>
        /// <param name="bundle">Bundle name</param>
        /// <param name="manifestPath">Path to local manifest file</param>
        /// <returns></returns>
        Task<ShiftResultCode> InstallBundleAsync(
            string bundle,
            string manifestPath,
            string stagingDirectory = null);
    }
}