// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Hosting;
using Shift.Cli;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;

namespace MixedReality.Shift.Cli.Commands
{
    internal class VersionCommand : Command
    {
        public VersionCommand() : base("version", "Get the version of the application.")
        {
            Handler = CommandHandler.Create<RunCommandHandlerInput, IHost, CancellationToken>(
                async (input, host, cancellationToken) =>
                {
                    await Console.Out.WriteLineAsync(typeof(Program).Assembly.GetName().Version.ToString(3));
                    return 0;
                });
        }
    }
}