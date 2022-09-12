// -----------------------------------------------------------------------
// <copyright file="IPackagesExposer.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shift.Core.Models.Manifests;

namespace Shift.Core.Exposer
{
    public interface IPackagesExposer
    {
        Task DownloadPackagesAndLog(
            Manifest manifest,
            IEnumerable<string> bundles,
            IEnumerable<string> components,
            IEnumerable<string> versions,
            Action<string> logEvent);

        Task InstallPackagesAndLog(
            Manifest manifest,
            IEnumerable<string> bundles,
            IEnumerable<string> components,
            IEnumerable<string> versions,
            Action<string> logEvent);
    }
}