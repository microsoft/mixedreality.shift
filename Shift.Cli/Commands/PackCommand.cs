// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;

namespace MixedReality.Shift.Cli.Commands
{
    public class PackCommand : Command
    {
        public PackCommand() : base("pack", "Pack a manifest, with its dependencies into an archive that can be ran offline.")
        {
            AddOption(new Option<string>("--manifest-path", "The path to the manifest.") { IsRequired = true });
            AddOption(new Option<string>("--output-path", "The output path of the archive.") { IsRequired = true });

            Handler = CommandHandler.Create<PackCommandHandlerInput, IHost, CancellationToken>(async (input, host, cancellationToken) =>
            {
                var handler = ActivatorUtilities.CreateInstance<PackCommandHandler>(host.Services);
                return (int)await handler.ExecuteAsync(input, cancellationToken);
            });
        }
    }

}