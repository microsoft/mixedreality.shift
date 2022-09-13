// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Models.Events
{
    public class InstallEvent : BaseEvent
    {
        public string ComponentId { get; set; }

        public string TaskType { get; set; }
    }
}