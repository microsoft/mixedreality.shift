// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.CommandLine;
using Microsoft.Internal.MR.ES.Shift.Cli.Commands;
using Shift.Core.Models.Plugins;

namespace Shift.Cli.Commands
{
    public sealed class ProgramRootCommand : RootCommand
    {
        public ProgramRootCommand(IEnumerable<PluginDefinition> plugins)
        {
            AddGlobalOption(new Option<string>(
                aliases: new[] { "--workingDir" },
                getDefaultValue: () => "%LocalAppData%\\MRShift",
                description: "Working directory to store all the downloaded artifacts.")
            );

            AddCommand(new InitCommand());
            AddCommand(new InstallCommand());
            AddCommand(new DownloadCommand());
            AddCommand(new VersionCommand());
            AddCommand(new CreateReleaseCommand());

            foreach (var p in plugins)
            {
                foreach (var c in p.GetCommands())
                {
                    AddCommand(c);
                }
            }
        }
    }
}