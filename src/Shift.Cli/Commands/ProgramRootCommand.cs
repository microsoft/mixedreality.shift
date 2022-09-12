// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.CommandLine;
using Shift.Core.Models.Plugins;

namespace Shift.Cli.Commands
{
    public sealed class ProgramRootCommand : RootCommand
    {
        public ProgramRootCommand(IEnumerable<PluginDefinition> plugins)
        {
            AddGlobalOption(new Option<string>(
                aliases: new[] { "--adoPat" },
                description: "The personal access token used to access ADO")
            );

            AddGlobalOption(new Option<string>(
                aliases: new[] { "--adoUri" },
                getDefaultValue: () => "https://microsoft.visualstudio.com",
                description: "The ADO root uri, eg: https://microsoft.visualstudio.com")
            );

            AddGlobalOption(new Option<string>(
                aliases: new[] { "--adoProject" },
                getDefaultValue: () => "Analog",
                description: "The ADO project, eg: Analog")
            );

            AddGlobalOption(new Option<string>(
                aliases: new[] { "--workingDir" },
                getDefaultValue: () => "%LocalAppData%\\MRShift",
                description: "Working directory to store all the downloaded artifacts.")
            );

            AddGlobalOption(new Option<string>(
                aliases: new[] { "--manifest" },
                description: "The local manifest file to use.")
            );

            AddCommand(new InitCommand());
            AddCommand(new InstallCommand());
            AddCommand(new DownloadCommand());

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