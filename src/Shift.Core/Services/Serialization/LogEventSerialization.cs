// <copyright file="LogEventSerialization.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace Shift.Core.Services.Serialization
{
    /// <summary>
    /// Static class to handle serialization of logging events.
    /// </summary>
    public static class LogEventSerialization
    {
        /// <summary>
        /// Format a generic log event.
        /// </summary>
        /// <typeparam name="T">The type of the log event.</typeparam>
        /// <param name="state">The log event.</param>
        /// <param name="eventException">The log exception, if provided.</param>
        /// <returns>A serialized string representing the log event.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatState<T>(T state, Exception eventException)
        {
            var jObject = state == null ? new JObject() : JObject.FromObject(state);
            if (eventException != null)
            {
                // exceptions can be nasty to serialize, we should only grab info known to serialize well
                jObject.Add("$Exception", JObject.FromObject(new
                {
                    eventException.Data,
                    eventException.Message,
                    eventException.Source,
                    eventException.StackTrace,
                }));
            }

            return jObject.ToString();
        }
    }
}