// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Models.Events
{
    public class DownloadEvent : BaseEvent
    {
        public string ComponentId { get; set; }

        public string ComponentVersion { get; set; }

        public bool DownloadSkipped { get; set; }
    }
}