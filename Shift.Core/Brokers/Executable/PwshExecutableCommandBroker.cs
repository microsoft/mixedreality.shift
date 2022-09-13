// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Shift.Core.Brokers.Executable
{
    public class PwshExecutableCommandBroker : ExecutableCommandBroker
    {
        public PwshExecutableCommandBroker(ILogger<PwshExecutableCommandBroker> logger)
            : base(logger, "pwsh")
        {
        }
    }
}