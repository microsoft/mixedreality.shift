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

namespace MixedReality.Shift.Cli.Commands
{
    internal class RunCommand : Command
    {
        public RunCommand() : base(
            name: "run", 
            description: "Run the tasks specified in the manifest; " +
            "path can be to a packaged manifest (.zip) or " +
            "manifest file (.json) or release folder containing manifest file.")
        {
            AddArgument(new Argument("path")
            {
                Description = "The path to the local manifest, release folder, or zip archive."
            });

            AddOption(new Option<string>("--bundle", "If set, runs the specified bundle tasks"));

            AddOption(new Option<bool>("--download-only", "If set, packages are downloaded "));

            Handler = CommandHandler.Create<RunCommandHandlerInput, IHost, CancellationToken>(
                async (input, host, cancellationToken) =>
                {
                    var handler = ActivatorUtilities.CreateInstance<RunCommandHandler>(host.Services);
                    return (int)await handler.ExecuteAsync(input, cancellationToken);
                });
        }
    }
}