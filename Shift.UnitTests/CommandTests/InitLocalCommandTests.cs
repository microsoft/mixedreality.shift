// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shift.Cli.Commands;
using Shift.Core.Services;

namespace Shift.UnitTests.CommandTests
{
    [TestClass]
    public class InitLocalCommandTests
    {
        [TestMethod]
        public async Task InitLocalCommandHandler_ExecuteAsync_AssertCompositionRepoNotNull()
        {
            // arrange
            var installationService = new Mock<IInstallationService>();
            var handler = new InitCommandHandler(
                installationService.Object,
                NullLogger<InitCommandHandler>.Instance);

            // act
            await handler.ExecuteAsync(new InitCommandHandlerInput("test-path"));

            // assert
            installationService
                .Verify(x => x.InitLocalAsync(
                    It.Is<string>(x => x == "test-path")),
                    Times.Once());
        }
    }
}