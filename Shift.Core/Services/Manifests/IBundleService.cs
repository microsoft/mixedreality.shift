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
    /// Defines an orchestration service that handles complex download and install operations.
    /// </summary>
    public interface IBundleService
    {
        /// <summary>
        /// Downloads and processes the components in the specified bundle
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        /// <param name="bundle">Bundle name</param>
        Task<ShiftResultCode> DownloadAndProcessBundleAsync(Manifest manifest, string bundle);

        /// <summary>
        /// Downloads and processes the components in the default bundle
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        Task<ShiftResultCode> DownloadAndProcessDefaultBundleAsync(Manifest manifest);

        /// <summary>
        /// Downloads the components in the specified bundle
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        /// <param name="bundle">Bundle name</param>
        Task<ShiftResultCode> DownloadBundleAsync(Manifest manifest, string bundle);

        /// <summary>
        /// Downloads the components in the specified bundle
        /// </summary>
        /// <param name="manifestPath">Path to manifest file</param>
        /// <param name="bundle">Bundle name</param>
        Task<ShiftResultCode> DownloadBundleAsync(string manifestPath, string bundle);

        /// <summary>
        /// Downloads the components in the default bundle
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        Task<ShiftResultCode> DownloadDefaultBundleAsync(Manifest manifest);

        /// <summary>
        /// Processes the components in the specified bundle
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        /// <param name="bundle">Bundle name</param>
        Task<ShiftResultCode> ProcessBundleAsync(Manifest manifest, string bundle);

        /// <summary>
        /// Proceses the components in the default bundle
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        Task<ShiftResultCode> ProcessDefaultBundleAsync(Manifest manifest);

        /// <summary>
        /// This command is called from a release image. Processes the default components in the
        /// release image
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        /// <param name="programPath">Path to the current working directory</param>
        Task<ShiftResultCode> ProcessDefaultBundleFromReleaseAsync(Manifest manifest, string programPath);
    }
}