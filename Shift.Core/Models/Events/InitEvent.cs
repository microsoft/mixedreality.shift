// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Models.Events
{
    public class InitEvent : BaseEvent
    {
        public string ManifestPath { get; set; }

        public bool DownloadOnly { get; set; }

        public string StagingDirectory { get; set; }
    }
}