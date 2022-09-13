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
    internal class DownloadCommand : Command
    {
        public DownloadCommand() : base("download", "Downloads a specified set of components.")
        {
            AddArgument(new Argument("path")
            {
                Description = "The path to the local manifest."
            });

            AddOption(new Option<string>(new string[] { "--components", "-c" })
            {
                Description = "A comma-separated list of components to download."
            });

            AddOption(new Option<string>(new string[] { "--bundle", "-b" })
            {
                Description = "Bundle to download."
            });

            AddOption(new Option<string>(new string[] { "--versions", "-v" })
            {
                Description = "A comma-separated list of component versions. " +
                "To specify only the third component's version, use the following pattern: \",,0.0.1\"."
            });

            Handler = CommandHandler.Create<DownloadCommandHandlerInput, IHost, CancellationToken>(
                (async (input, host, cancellationToken) =>
                {
                    var handler = ActivatorUtilities.CreateInstance<DownloadCommandHandler>(host.Services);
                    return (int)await handler.ExecuteAsync(input, cancellationToken);
                }));
        }
    }
}