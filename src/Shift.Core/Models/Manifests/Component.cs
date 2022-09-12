// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Shift.Core.Models.Manifests.Tasks;

namespace Shift.Core.Models.Manifests
{
    public class Component
    {
        public string Description { get; set; }

        public string[] DeviceDemands { get; set; } = Array.Empty<string>();

        public string Id { get; set; }

        public Location Location { get; set; }

        public string Owner { get; set; }

        public TaskInfo Task { get; set; }
    }
}