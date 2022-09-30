﻿// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

namespace Shift.Core.Models.Events
{
    public class BaseEvent
    {
        public float DurationMS { get; set; }
        public string EventName => GetType().Name;
        public bool ExceptionOcurred { get; set; }
        public string ResultCode { get; set; }
        public string ShiftVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
    }
}