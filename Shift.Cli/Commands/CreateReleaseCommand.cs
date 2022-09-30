// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Shift.Core.Commands;
using Shift.Core.Models.Common;
using Shift.Core.Services;
using System.Threading.Tasks;
using System.Threading;

namespace Microsoft.Internal.MR.ES.Shift.Cli.Commands
{
    public sealed record CreateReleaseCommandHandlerInput(string ManifestPath, string OutputPath) : BaseCommandHandlerInput;

    public class CreateReleaseCommandHanlder : BaseCommandHandler<CreateReleaseCommandHandlerInput>
    {
        private readonly IReleaseService _releaseService;

        public CreateReleaseCommandHanlder(IReleaseService releaseService, ILogger<CreateReleaseCommandHanlder> logger) : base(logger)
        {
            _releaseService = releaseService;
        }

        protected override async Task<ShiftResultCode> ExecuteAsyncOverride(CreateReleaseCommandHandlerInput input, CancellationToken cancellationToken)
        {
            return await _releaseService.CreateReleaseAsync(input.ManifestPath, input.OutputPath);
        }
    }
}
