// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Commands;
using Shift.Core.Models.Common;
using Shift.Core.Services;

namespace MixedReality.Shift.Cli.Commands
{
    public class RunCommandHandler : BaseCommandHandler<RunCommandHandlerInput>
    {
        private readonly IInstallationService _installationService;

        public RunCommandHandler(
            IInstallationService installationService,
            ILogger<RunCommandHandler> logger
        ) : base(logger)
        {
            _installationService = installationService;
        }

        protected override async Task<ShiftResultCode> ExecuteAsyncOverride(
            RunCommandHandlerInput input,
            CancellationToken cancellationToken)
        {
            return await _installationService.RunAsync(
                input.Path, 
                input.DownloadOnly, 
                input.StagingDirectory);
        }
    }
}