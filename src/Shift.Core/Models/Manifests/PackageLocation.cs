// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Models.Manifests
{
    public class PackageLocation : Location
    {
        public string Feed { get; set; }

        public string Name { get; set; }

        public string Organization { get; set; }

        public string Project { get; set; }

        public string Version { get; set; }
    }
}