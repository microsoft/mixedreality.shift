// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shift.Cli.Commands;
using Shift.Core.Models.Common;
using Shift.Core.Services;
using Shift.Core.Services.Manifests;

namespace Shift.UnitTests.CommandTests
{
    [TestClass]
    public class InstallCommandTests
    {
        private readonly Mock<IComponentService> _componentService;
        private readonly InstallCommandHandler _handler;
        private readonly Mock<IInstallationService> _installationService;

        public InstallCommandTests()
        {
            _installationService = new Mock<IInstallationService>();
            _componentService = new Mock<IComponentService>();
            _handler = new InstallCommandHandler(
                _installationService.Object,
                _componentService.Object,
                NullLogger<InstallCommandHandler>.Instance);
        }

        [TestMethod]
        public async Task InstallCommandHandler_ExecuteAsync_ComponentWithoutVersionInputTest()
        {
            // arrange
            var input = new InstallCommandHandlerInput("", "testComponent", "");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.InstallComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[1])),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task InstallCommandHandler_ExecuteAsync_ComponentWithVersionInputTest()
        {
            // arrange
            var input = new InstallCommandHandlerInput("", "testComponent", "1.2.3");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.InstallComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[] { "1.2.3" })),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task InstallCommandHandler_ExecuteAsync_MultipleComponentsWithoutVersionsTest()
        {
            // arrange
            var input = new InstallCommandHandlerInput("", "testComponent1,testComponent2,testComponent3", "");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.InstallComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent1", "testComponent2", "testComponent3" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[3])),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task InstallCommandHandler_ExecuteAsync_MultipleComponentsWithSomeVersionsTest()
        {
            // arrange
            var input = new InstallCommandHandlerInput("", "testComponent1,testComponent2,testComponent3", ",2.0.0,");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.InstallComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent1", "testComponent2", "testComponent3" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[] { "", "2.0.0", "" })),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task InstallCommandHandler_ExecuteAsync_MultipleComponentsWithSpacesTest()
        {
            // arrange
            var input = new InstallCommandHandlerInput("", "testComponent1, testComponent2, testComponent3", "");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.InstallComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent1", "testComponent2", "testComponent3" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[3])),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task InstallCommandHandler_ExecuteAsync_MultipleComponentsWithVersionsTest()
        {
            // arrange
            var input = new InstallCommandHandlerInput("", "testComponent1,testComponent2,testComponent3", "1.0.0,2.0.0,3.0.0");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.InstallComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent1", "testComponent2", "testComponent3" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[] { "1.0.0", "2.0.0", "3.0.0" })),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task InstallCommandHandler_ExecuteAsync_BundleInputTest()
        {
            // arrange
            var input = new InstallCommandHandlerInput("", "", "", "default");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _installationService
                .Verify(x => x.InstallBundleAsync(
                    It.Is<string>(bundle => string.Equals(bundle, "default")),
                    It.IsNotNull<string>()),
                    Times.Once());
        }
    }
}