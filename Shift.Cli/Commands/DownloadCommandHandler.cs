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
using Shift.Core.Services.Manifests;

namespace Shift.Cli.Commands
{
    public sealed record DownloadCommandHandlerInput(
        string path,
        string components,
        string versions,
        string bundle = null
        ) : BaseCommandHandlerInput;

    public class DownloadCommandHandler : BaseCommandHandler<DownloadCommandHandlerInput>
    {
        private readonly IBundleService _bundleService;
        private readonly IComponentService _componentService;

        public DownloadCommandHandler(
            IBundleService bundleService,
            IComponentService installationService,
            ILogger<DownloadCommandHandler> logger
        ) : base(logger)
        {
            _bundleService = bundleService;
            _componentService = installationService;
        }

        protected override async Task<ShiftResultCode> ExecuteAsyncOverride(
            DownloadCommandHandlerInput input,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(input.bundle))
            {
                return await _bundleService.DownloadBundleAsync(input.path, input.bundle);
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
                return await _componentService.DownloadComponentsAsync(components, versions, input.path);
            }
            return ShiftResultCode.InvalidCommandLineOption;
        }
    }
}