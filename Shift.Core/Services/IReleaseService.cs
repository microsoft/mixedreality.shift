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
        /// <param name="manifestPath">The path to the manifest.</param>
        /// <param name="outputPath">The path to the output zip.</param>
        Task<ShiftResultCode> CreateReleaseAsync(string manifestPath, string outputPath);
    }
}