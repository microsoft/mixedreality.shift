// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Contracts.Manifests
{
    public class ComponentBundleV1
    {
        public string[] Bundles { get; set; }

        public string[] Components { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }
    }
}