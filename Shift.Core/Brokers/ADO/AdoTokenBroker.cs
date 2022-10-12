// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Shift.Core.Models.Common;

namespace Shift.Core.Brokers
{
    public class AdoTokenBroker : IAdoTokenBroker
    {
        private static readonly string AdoResourceId = "499b84ac-1321-427f-aa17-267ca6975798";

        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        /// <summary>
        /// Initializes the access token
        /// </summary>
        /// <param name="organization">The organization to authenticate to, eg: FABRIKAM.</param>
        /// <param name="logger">Logger to use</param>
        public async Task<string> GetTokenCredentialAsync(string organization )
        {
            organization = organization.ToUpperInvariant();
            if (_cache.TryGetValue(organization, out string pat))
            {
                return pat;
            }
            // load from env var
            var token = Environment.GetEnvironmentVariable($"AZURE_DEVOPS_EXT_PAT_{organization}");
            if (!string.IsNullOrWhiteSpace(token))
            {
                _cache[organization] = token;
                return token;
            }
            try
            {
                var tokenCredential = new DefaultAzureCredential();
                var authResult = await tokenCredential.GetTokenAsync(new TokenRequestContext(scopes: new string[] { AdoResourceId + "/.default" }));
                _cache[organization] = authResult.Token;
                return authResult.Token;
            }
            catch (Exception ex)
            {
                var msg = "ERROR: Unable to acquire Azure Authentication token! Ensure successful azure login by running 'az login' from the command prompt.";
                throw new ShiftException(ShiftResultCode.Unauthorized, msg, ex, isFatal: true);
            }
        }
    }
}