// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Internal.MR.ES.Shift.Cli.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shift.Core.Services;

namespace Microsoft.Internal.MR.ES.Shift.UnitTests.CommandTests
{
    [TestClass]
    public class CreateReleaseCommandTests
    {
        [TestMethod]
        public async Task CreateReleaseCommandHandler_ExecuteAsync_AssertCompositionRepoNotNull()
        {
            // arrange
            var releaseService = new Mock<IReleaseService>();
            var handler = new CreateReleaseCommandHanlder(
                releaseService.Object,
                NullLogger<CreateReleaseCommandHanlder>.Instance);

            // act
            await handler.ExecuteAsync(new CreateReleaseCommandHandlerInput("test.json", "test.zip"));

            // assert
            releaseService
                .Verify(x => x.CreateReleaseAsync(
                    It.IsNotNull<string>(),
                    It.IsNotNull<string>()),
                    Times.Once());
        }
    }
}