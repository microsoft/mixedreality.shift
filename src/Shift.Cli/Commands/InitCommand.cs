// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shift.Cli.Commands
{
    internal class InitCommand : Command
    {
        public InitCommand() : base("init", "Install components from a manifest.")
        {
            AddArgument(new Argument("path")
            {
                Description = "The path to the local manifest."
            });

            Handler = CommandHandler.Create<InitCommandHandlerInput, IHost, CancellationToken>(
                async (input, host, cancellationToken) =>
                {
                    var handler = ActivatorUtilities.CreateInstance<InitCommandHandler>(host.Services);
                    return (int)await handler.ExecuteAsync(input, cancellationToken);
                });
        }
    }
}