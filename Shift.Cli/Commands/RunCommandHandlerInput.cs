// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Shift.Core.Commands;

namespace MixedReality.Shift.Cli.Commands
{
    public sealed record RunCommandHandlerInput(
       string Path,
       string Bundle,
       bool DownloadOnly,
       string StagingDirectory=default
       ) : BaseCommandHandlerInput;
}