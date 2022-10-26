// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MixedReality.Shift.Cli.Commands;
using Shift.Core;
using Shift.Core.Brokers;
using Shift.Core.Brokers.Executable;
using Shift.Core.Models.Plugins;
using Shift.Core.Providers;
using Shift.Core.Services;
using Shift.Core.Services.Artifacts;
using Shift.Core.Services.Manifests;
using Shift.Core.Services.Manifests.Tasks;
using Shift.Plugins.Common;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

namespace Shift.Cli
{
    internal sealed class Program
    {
        public static int Main(string[] args)
        {
            string programPath = ProgramDataPath.GetWorkingDirectory();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(programPath)
                .Build();

            // define all plugins to build solution w/
            var plugins = new List<PluginDefinition>
            {
                new BasePluginDefinition(),
                new CommonPluginDefinition(),
            };

            try
            {
                var exitCode = new CommandLineBuilder(new ProgramRootCommand(plugins))
                    .UseHost(
                        _ => Host.CreateDefaultBuilder(),
                        host =>
                        {
                            host
                                .UseContentRoot(programPath)
                                .ConfigureAppConfiguration((context, builder) =>
                                {
                                    builder.AddConfiguration(configuration);
                                })
                                .ConfigureServices((context, services) =>
                                {
                                    services
                                        .AddLogging(x =>
                                        {
                                            x.ClearProviders();
                                            x.AddConsole();
                                        });

                                    // needs to be configurable at some point
                                    services.AddSingleton<IAdoTokenBroker, AdoTokenBroker>();
                                    services.AddSingleton<IComponentTaskProvider>(x => new ComponentTaskProvider(plugins));
                                    services.AddSingleton<IComponentService, ComponentService>();
                                    services.AddSingleton<IPackageFeedBrokerFactory, AdoBrokerFactory>();
                                    services.AddSingleton<ExecutableCommandBroker>();
                                    services.AddTransient<PwshExecutableCommandBroker>();
                                    services.AddSingleton<IManifestService, ManifestService>();
                                    services.AddSingleton<IPackageFeedService, PackageFeedService>();
                                    services.AddSingleton<IBundleService, BundleService>();
                                    services.AddSingleton<IInstallationService, InstallationService>();
                                    services.AddSingleton<IReleaseService, ReleaseService>();

                                    foreach (var plugin in plugins)
                                    {
                                        plugin.RegisterServices(services);
                                    }

                                    services.AddMemoryCache();
                                });
                        })
                    .UseDefaults()
                    .Build()
                    .Invoke(args);

                return exitCode;
            }
            catch (OperationCanceledException)
            {
                // User cancellation. Exit gracefully
            }

            return 0;
        }
    }
}