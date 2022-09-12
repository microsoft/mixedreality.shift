// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Shift.Core.Contracts.Manifests.Tasks;

namespace Shift.Core.Contracts.Manifests
{
    public class ComponentV1
    {
        public string Description { get; set; }

        public string[] DeviceDemands { get; set; } = Array.Empty<string>();

        public string Id { get; set; }

        public LocationV1 Location { get; set; }

        public string Owner { get; set; }

        public TaskInfoV1 Task { get; set; }
    }
}