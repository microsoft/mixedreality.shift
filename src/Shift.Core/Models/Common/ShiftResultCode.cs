// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Models.Common
{
    /// <summary>
    /// Result code
    /// </summary>
    public enum ShiftResultCode
    {
        Success = 0,
        Unknown = -1,
        InvalidUserInput = 1,
        InvalidCommandLineOption = 2,
        InvalidArgument = 3,
        Unauthorized = 4,
        AdbFailure = 5,
        ExpiredOrMissingTokenFailure = 6,
        FailedRequestToSharePoint = 7,
        HMDError = 8,
        NoCalibrationFile = 9,
        CalibrationError = 10,
        InstallError = 11,
        HMDConnectionError = 12,
        UpdateStarted = 13,
        UpdateNotRequired = 14,
        UpdateCancelled = 15,
        CommandExecutionError = 16,
    }
}