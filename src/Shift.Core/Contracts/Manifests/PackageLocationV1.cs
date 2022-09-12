// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Contracts.Manifests
{
    public class PackageLocationV1 : LocationV1
    {
        public string Feed { get; set; }

        public string Name { get; set; }

        public string Organization { get; set; }

        public string Project { get; set; }

        public string Version { get; set; }
    }
}