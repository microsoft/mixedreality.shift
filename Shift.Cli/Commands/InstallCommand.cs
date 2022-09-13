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
    internal class InstallCommand : Command
    {
        public InstallCommand() : base("install", "Downloads a specified set of components and performs the associated tasks.")
        {
            AddArgument(new Argument("path")
            {
                Description = "The path to the local manifest."
            });

            AddOption(new Option<string>(new string[] { "--components", "-c" })
            {
                Description = "A comma-separated list of components to install."
            });

            AddOption(new Option<string>(new string[] { "--bundle", "-b" })
            {
                Description = "Bundle to install."
            });

            AddOption(new Option<string>(new string[] { "--versions", "-v" })
            {
                Description = "A comma-separated list of component versions. " +
                "To specify only the third component's version, use the following pattern: \",,0.0.1\"."
            });

            Handler = CommandHandler.Create<InstallCommandHandlerInput, IHost, CancellationToken>(
                async (input, host, cancellationToken) =>
                {
                    var handler = ActivatorUtilities.CreateInstance<InstallCommandHandler>(host.Services);
                    return (int)await handler.ExecuteAsync(input, cancellationToken);
                });
        }
    }
}