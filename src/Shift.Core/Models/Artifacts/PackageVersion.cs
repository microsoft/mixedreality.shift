// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Shift.Core.Models.Artifacts
{
    public class PackageVersion
    {
        public bool? IsLatest { get; set; }
        public DateTime? PublishDate { get; set; }
        public string Version { get; set; }
        public List<string> Views { get; set; }
    }
}