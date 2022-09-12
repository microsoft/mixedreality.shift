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

namespace Shift.Cli.Commands
{
    public sealed record InitCommandHandlerInput(
       string Path
       ) : BaseCommandHandlerInput;

    public class InitCommandHandler : BaseCommandHandler<InitCommandHandlerInput>
    {
        private readonly IInstallationService _installationService;

        public InitCommandHandler(
            IInstallationService installationService,
            ILogger<InitCommandHandler> logger
        ) : base(logger)
        {
            _installationService = installationService;
        }

        protected override async Task<ShiftResultCode> ExecuteAsyncOverride(
            InitCommandHandlerInput input,
            CancellationToken cancellationToken)
        {
            return await _installationService.InitLocalAsync(input.Path);
        }
    }
}