// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MixedReality.Shift.Cli.Commands;
using Moq;
using Shift.Core.Services;

namespace Shift.UnitTests.CommandTests
{
        /*
    [TestClass]
    public class InitLocalCommandTests
    {
        [TestMethod]
        public async Task InitLocalCommandHandler_ExecuteAsync_AssertCompositionRepoNotNull()
        {
            // arrange
            var installationService = new Mock<IInstallationService>();
            var handler = new RunCommandHandler(
                installationService.Object,
                NullLogger<RunCommandHandler>.Instance);

            // act
            await handler.ExecuteAsync(new RunCommandHandlerInput("test-path"));

            // assert
            installationService
                .Verify(x => x.InitLocalAsync(
                    It.Is<string>(x => x == "test-path")),
                    Times.Once());
        }
    }
        */
}