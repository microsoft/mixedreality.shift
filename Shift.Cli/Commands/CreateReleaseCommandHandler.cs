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

namespace Microsoft.Internal.MR.ES.Shift.Cli.Commands
{
    public class CreateReleaseCommand : Command
    {
        public CreateReleaseCommand() : base("create-release", "Creates a standalone release artifact from supplied manifest, which does not require internet connectivity to be installed.")
        {
            AddOption(new Option<string>("--manifest-path", "The path to the manifest.") { IsRequired = true });
            AddOption(new Option<string>("--output-path", "The output path of the archive.") { IsRequired = true });

            Handler = CommandHandler.Create<CreateReleaseCommandHandlerInput, IHost, CancellationToken>(async (input, host, cancellationToken) =>
            {
                var handler = ActivatorUtilities.CreateInstance<CreateReleaseCommandHanlder>(host.Services);
                return (int)await handler.ExecuteAsync(input, cancellationToken);
            });
        }
    }
}