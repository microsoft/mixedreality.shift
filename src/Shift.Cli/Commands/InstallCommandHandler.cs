// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Commands;
using Shift.Core.Models.Common;
using Shift.Core.Services;
using Shift.Core.Services.Manifests;

namespace Shift.Cli.Commands
{
    public sealed record InstallCommandHandlerInput(
        string components,
        string versions,
        string manifestPath,
        string bundle = null
        ) : BaseCommandHandlerInput;

    public class InstallCommandHandler : BaseCommandHandler<InstallCommandHandlerInput>
    {
        private readonly IInstallationService _installationService;
        private readonly IComponentService _componentService;

        public InstallCommandHandler(
            IInstallationService installationService,
            IComponentService componentService,
            ILogger<InstallCommandHandler> logger
        ) : base(logger)
        {
            _installationService = installationService;
            _componentService = componentService;
        }

        protected override async Task<ShiftResultCode> ExecuteAsyncOverride(
            InstallCommandHandlerInput input,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(input.bundle))
            {
                return await _installationService.InstallBundleAsync(input.bundle, input.manifestPath);
            }
            else if (!string.IsNullOrEmpty(input.components))
            {
                // Parse the inputs
                var components = input.components.Split(",").Select(x => x.Trim()).ToArray();
                string[] versions;
                if (string.IsNullOrEmpty(input.versions))
                {
                    versions = new string[components.Length];
                }
                else
                {
                    versions = input.versions.Split(",").Select(x => x.Trim()).ToArray();
                    if (components.Length != versions.Length)
                    {
                        throw new ShiftException(ShiftResultCode.InvalidArgument, $"Arguments are invalid:\ncomponents: {input.components}\nversions: {input.versions}");
                    }
                }
                return await _componentService.InstallComponentsAsync(components, versions, input.manifestPath);
            }
            return ShiftResultCode.InvalidCommandLineOption;
        }
    }
}