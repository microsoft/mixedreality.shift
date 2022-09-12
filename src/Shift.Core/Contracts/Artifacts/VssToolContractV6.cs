// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json;

namespace Shift.Core.Contracts.Artifacts
{
    public class VssToolContractV6
    {
        public string Name { get; set; }

        [JsonProperty("rid")]
        public string RuntimeIdentifier { get; set; }

        public Uri Uri { get; set; }

        public string Version { get; set; }
    }
}