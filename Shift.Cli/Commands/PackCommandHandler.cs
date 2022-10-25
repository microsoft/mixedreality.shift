// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Shift.Core.Commands;
using Shift.Core.Models.Common;
using Shift.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MixedReality.Shift.Cli.Commands
{
    public class PackCommandHandler : BaseCommandHandler<PackCommandHandlerInput>
    {
        private readonly IReleaseService _releaseService;

        public PackCommandHandler(IReleaseService releaseService, ILogger<PackCommandHandler> logger) : base(logger)
        {
            _releaseService = releaseService;
        }

        protected override async Task<ShiftResultCode> ExecuteAsyncOverride(PackCommandHandlerInput input, CancellationToken cancellationToken)
        {
            var result = await _releaseService.CreateReleaseAsync(input.ManifestPath, input.OutputPath);

            if (result == ShiftResultCode.Success)
            {
                Console.WriteLine(input.OutputPath);
            }

            return result;
        }
    }
}