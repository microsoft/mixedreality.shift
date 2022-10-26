// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shift.Core.Models.Common;
using Shift.Core.Services.Manifests;

namespace Shift.UnitTests.CommandTests
{
    /*
    [TestClass]
    public class DownloadCommandTests
    {
        private readonly DownloadCommandHandler _handler;
        private readonly Mock<IBundleService> _bundleService;
        private readonly Mock<IComponentService> _componentService;

        public DownloadCommandTests()
        {
            _bundleService = new Mock<IBundleService>();
            _componentService = new Mock<IComponentService>();
            _handler = new DownloadCommandHandler(
                _bundleService.Object,
                _componentService.Object,
                NullLogger<DownloadCommandHandler>.Instance);
        }

        [TestMethod]
        public async Task DownloadCommandHandler_ExecuteAsync_ComponentWithoutVersionInputTest()
        {
            // arrange
            var input = new DownloadCommandHandlerInput("", "testComponent", "");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.DownloadComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[1])),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task DownloadCommandHandler_ExecuteAsync_ComponentWithVersionInputTest()
        {
            // arrange
            var input = new DownloadCommandHandlerInput("", "testComponent", "1.2.3");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.DownloadComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[] { "1.2.3" })),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task DownloadCommandHandler_ExecuteAsync_MultipleComponentsWithoutVersionsTest()
        {
            // arrange
            var input = new DownloadCommandHandlerInput("", "testComponent1,testComponent2,testComponent3", "");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.DownloadComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent1", "testComponent2", "testComponent3" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[3])),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task DownloadCommandHandler_ExecuteAsync_MultipleComponentsWithSomeVersionsTest()
        {
            // arrange
            var input = new DownloadCommandHandlerInput("", "testComponent1,testComponent2,testComponent3", ",2.0.0,");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.DownloadComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent1", "testComponent2", "testComponent3" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[] { "", "2.0.0", "" })),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task DownloadCommandHandler_ExecuteAsync_MultipleComponentsWithSpacesTest()
        {
            // arrange
            var input = new DownloadCommandHandlerInput("", "testComponent1, testComponent2, testComponent3", "");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.DownloadComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent1", "testComponent2", "testComponent3" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[3])),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task DownloadCommandHandler_ExecuteAsync_MultipleComponentsWithVersionsTest()
        {
            // arrange
            var input = new DownloadCommandHandlerInput("", "testComponent1,testComponent2,testComponent3", "1.0.0,2.0.0,3.0.0");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.DownloadComponentsAsync(
                    It.Is<string[]>(components => Enumerable.SequenceEqual(components, new string[] { "testComponent1", "testComponent2", "testComponent3" })),
                    It.Is<string[]>(versions => Enumerable.SequenceEqual(versions, new string[] { "1.0.0", "2.0.0", "3.0.0" })),
                    It.IsNotNull<string>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task DownloadCommandHandler_ExecuteAsync_BundleInputTest()
        {
            // arrange
            var input = new DownloadCommandHandlerInput("", "", "", "default");

            // act
            var result = await _handler.ExecuteAsync(input);

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            _bundleService
                .Verify(x => x.DownloadBundleAsync(
                    It.IsNotNull<string>(),
                    It.Is<string>(bundle => string.Equals(bundle, "default"))),
                    Times.Once());
        }
    }
    */
}