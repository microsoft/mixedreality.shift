// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Shift.Core.Models.Common;

namespace Shift.Core.Brokers.Executable
{
    public interface ICommandBroker
    {
        event Action<string> OnErrorReceived;

        event Action<string> OnOutputReceived;

        Task<ShiftResultCode> ExecuteAsync(
            string[] args,
            string workingDirectory = default,
            bool runAsAdmin = false);
    }
}