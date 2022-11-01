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
    [TestClass]
    public class PackCommandTests
    {
        [TestMethod]
        public async Task PackCommandHandler_ExecuteAsync_AssertCompositionRepoNotNull()
        {
            // arrange
            var releaseService = new Mock<IReleaseService>();
            var handler = new PackCommandHandler(
                releaseService.Object,
                NullLogger<PackCommandHandler>.Instance);

            // act
            await handler.ExecuteAsync(new PackCommandHandlerInput("test.json", "test.zip"));

            // assert
            releaseService
                .Verify(x => x.CreateReleaseAsync(
                    It.IsNotNull<string>(),
                    It.IsNotNull<string>()),
                    Times.Once());
        }
    }
}