// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Shift.Core.Contracts.Manifests
{
    public class ManifestV1
    {
        public List<ComponentBundleV1> Bundles { get; set; }

        public List<ComponentV1> Components { get; set; }

        public string Version { get; set; }
    }
}