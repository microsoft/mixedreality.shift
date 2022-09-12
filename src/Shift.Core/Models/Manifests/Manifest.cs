// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Shift.Core.Models.Manifests
{
    public class Manifest
    {
        public List<ComponentBundle> Bundles { get; set; }

        public List<Component> Components { get; set; }

        public string Version { get; set; }
    }
}