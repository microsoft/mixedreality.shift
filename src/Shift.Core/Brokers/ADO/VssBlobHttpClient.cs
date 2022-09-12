// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Shift.Core.Contracts.Artifacts;

namespace Shift.Core.Brokers
{
    public class VssBlobHttpClient : VssHttpClientBase, IVssBlobHttpClient
    {
        protected static readonly ApiResourceVersion ApiVersion = new ApiResourceVersion("6.0-preview.1");

        /// <summary>
        /// Corresponds to the following template: _apis/{area}/{toolName}/{resource}
        /// </summary>
        private static readonly Guid GetToolId = new Guid("187ec90d-dd1e-4ec6-8c57-937d979261e5");

        public VssBlobHttpClient(
            Uri baseUrl,
            VssCredentials credentials
        ) : base(baseUrl, credentials)
        {
        }

        public VssBlobHttpClient(
            Uri baseUrl,
            VssCredentials credentials,
            VssHttpRequestSettings settings
        ) : base(baseUrl, credentials, settings)
        {
        }

        public VssBlobHttpClient(
            Uri baseUrl,
            VssCredentials credentials,
            params DelegatingHandler[] handlers
        ) : base(baseUrl, credentials, handlers)
        {
        }

        public VssBlobHttpClient(
            Uri baseUrl,
            HttpMessageHandler pipeline,
            bool disposeHandler
        ) : base(baseUrl, pipeline, disposeHandler)
        {
        }

        public VssBlobHttpClient(
            Uri baseUrl,
            VssCredentials credentials,
            VssHttpRequestSettings settings,
            params DelegatingHandler[] handlers
        ) : base(baseUrl, credentials, settings, handlers)
        {
        }

        /// <summary>
        /// Get Azure DevOps tool metadata.
        /// </summary>
        /// <param name="toolName">The tool name, eg: ArtifactTool.</param>
        /// <param name="osName">The system/OS name, such as 'Linux', 'Darwin', 'Java', 'Windows'.</param>
        /// <param name="arch">The processor architecture, such as 'x86_64', 'arm64', 'amd64'.</param>
        /// <param name="distroName">The distribution name, not required for windows.</param>
        /// <param name="distroVersion">The distribution version, not required for windows.</param>
        /// <returns></returns>
        public async Task<VssToolContractV6> GetToolAsync(
            string toolName,
            string osName = null,
            string arch = null,
            string distroName = null,
            string distroVersion = null)
        {
            object routeValues = new
            {
                toolName,
            };

            var queryParameters = new Dictionary<string, string> { };
            if (!string.IsNullOrWhiteSpace(osName))
            {
                queryParameters.Add(nameof(osName), osName);
            }

            if (!string.IsNullOrWhiteSpace(arch))
            {
                queryParameters.Add(nameof(arch), arch);
            }

            if (!string.IsNullOrWhiteSpace(distroName))
            {
                queryParameters.Add(nameof(distroName), distroName);
            }

            if (!string.IsNullOrWhiteSpace(distroVersion))
            {
                queryParameters.Add(nameof(distroVersion), distroVersion);
            }

            var responseMessage = await GetAsync(GetToolId, routeValues, ApiVersion, queryParameters);
            return await ReadContentAsAsync<VssToolContractV6>(responseMessage);
        }

        /// <summary>
        /// Get Azure DevOps tool metadata and download tool bits to output stream.
        /// </summary>
        /// <param name="toolName">The tool name, eg: ArtifactTool.</param>
        /// <param name="osName">The system/OS name, such as 'Linux', 'Darwin', 'Java', 'Windows'.</param>
        /// <param name="arch">The processor architecture, such as 'x86_64', 'arm64', 'amd64'.</param>
        /// <param name="distroName">The distribution name, not required for windows.</param>
        /// <param name="distroVersion">The distribution version, not required for windows.</param>
        /// <returns></returns>
        public async Task<VssToolContractV6> GetToolAsync(
            string toolName,
            Stream outputStream,
            string osName = null,
            string arch = null,
            string distroName = null,
            string distroVersion = null)
        {
            var toolInfo = await GetToolAsync(toolName, osName, arch, distroName, distroVersion);
            using var client = new HttpClient();
            using var inputStream = await client.GetStreamAsync(toolInfo.Uri);
            await inputStream.CopyToAsync(outputStream);

            return toolInfo;
        }
    }
}