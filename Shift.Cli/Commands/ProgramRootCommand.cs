// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Shift.Core.Models.Plugins;
using System.Collections.Generic;
using System.CommandLine;

namespace MixedReality.Shift.Cli.Commands
{
    public sealed class ProgramRootCommand : RootCommand
    {
        public ProgramRootCommand(IEnumerable<PluginDefinition> plugins)
        {
            AddGlobalOption(new Option<string>(
                aliases: new[] { "--staging-directory" },
                getDefaultValue: () => "%Temp%\\Shift",
                description: "Staging directory to store all the downloaded artifacts.")
            );

            AddCommand(new RunCommand());
            AddCommand(new VersionCommand());
            AddCommand(new PackCommand());

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