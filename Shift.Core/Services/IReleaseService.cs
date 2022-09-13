// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Shift.Core.Models.Common;

namespace Shift.Core.Services
{
    public interface IReleaseService
    {
        /// <summary>
        /// Creates a release image by downloading all the components specified in the manifest
        /// </summary>
        /// <param name="organization">The organization for the manifest</param>
        /// <param name="project">The project for the manifest</param>
        /// <param name="feed">The feed for the manifest</param>
        /// <param name="packageName">The manifest package name, the product build flavor</param>
        Task<ShiftResultCode> CreateReleaseAsync(
            string organization,
            string project,
            string feed,
            string packageName);
    }
}