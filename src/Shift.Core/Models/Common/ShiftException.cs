// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Shift.Core.Models.Common
{
    /// <inheritdoc/>
    public class ShiftException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftException"/> class.
        /// </summary>
        /// <param name="resultCode">Result code</param>
        /// <param name="message">Message string</param>
        /// <param name="ex">Exception</param>
        /// <param name="isFatal">Fatal or not</param>
        public ShiftException(ShiftResultCode resultCode, string message, Exception ex = null, bool isFatal = true)
            : base(message, ex)
        {
            ResultCode = resultCode;
            IsFatal = isFatal;
        }

        /// <summary>
        /// Gets a value indicating whether the exception is fatal
        /// </summary>
        public bool IsFatal { get; }

        /// <summary>
        /// Gets the result code
        /// </summary>
        public ShiftResultCode ResultCode { get; }
    }
}