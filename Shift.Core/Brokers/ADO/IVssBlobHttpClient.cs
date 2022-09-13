// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using Shift.Core.Contracts.Artifacts;

namespace Shift.Core.Brokers
{
    public interface IVssBlobHttpClient
    {
        Task<VssToolContractV6> GetToolAsync(
            string toolName,
            string osName = null,
            string arch = null,
            string distroName = null,
            string distroVersion = null);

        Task<VssToolContractV6> GetToolAsync(
            string toolName,
            Stream outputStream,
            string osName = null,
            string arch = null,
            string distroName = null,
            string distroVersion = null);
    }
}