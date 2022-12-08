// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shift.Core.Models.Common;
using Shift.Core.Models.Manifests;
using Shift.Core.Services;
using Shift.Core.Services.Artifacts;
using Shift.Core.Services.Manifests;

namespace Shift.UnitTests.ServiceTests
{
    [TestClass]
    internal class ReleaseServiceTests
    {
        private Mock<IComponentService> _componentService;
        private Mock<IConfiguration> _configuration;
        private Mock<IBundleService> _bundleService;
        private Mock<IManifestService> _manifestService;
        private Mock<IPackageFeedService> _packageFeedService;

        public ReleaseServiceTests()
        {
            _packageFeedService = new Mock<IPackageFeedService>();
            _packageFeedService
                .Setup(x => x.GetLatestVersionAsStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<string>());
            _componentService = new Mock<IComponentService>();
            _componentService
                .Setup(x => x.DownloadComponentAsync(
                    It.IsAny<Component>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<ShiftResultCode>());
            _manifestService = new Mock<IManifestService>();
            _manifestService
                .Setup(x => x.GetManifestAsync(It.IsAny<string>()))
                .ReturnsAsync(new Manifest
                {
                    Components = new List<Component>() {
                    new Component() { Id = "testComponent1" },
                    new Component() { Id = "testComponent2" },
                    new Component() { Id = "testComponent3" },
                }
                });
            _configuration = new Mock<IConfiguration>();
            _bundleService = new Mock<IBundleService>();
        }

        [TestMethod]
        public async Task ReleaseService_CreateReleaseAsync()
        {
            // arrange
            var releaseService = new ReleaseService(
                _componentService.Object,
                _manifestService.Object,
                _bundleService.Object,
                NullLogger<ReleaseService>.Instance);

            // act
            var result = await releaseService.CreateReleaseAsync("test.json", "test.zip");

            // assert
            Assert.Equals(ShiftResultCode.Success, result);
            _componentService
                .Verify(x => x.DownloadComponentAsync(
                    It.IsAny<Component>(), 
                    It.IsAny<string>(),
                    It.IsAny<string>()), 
                    Times.Exactly(3));
            _packageFeedService
                .Verify(x => x.DownloadArtifactAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                    Times.Exactly(2));
        }


        [TestMethod]
        public async Task ReleaseService_InitReleaseAsync_CanExecuteTest()
        {
            // arrange
            var releaseService = new ReleaseService(
                 _componentService.Object,
                 _manifestService.Object,
                 _bundleService.Object,
                 NullLogger<ReleaseService>.Instance);

            // act
            await releaseService.InitReleaseAsync();

            // assert
            _manifestService
                .Verify(x => x.GetManifestAsync(
                    It.IsAny<string>()),
                    Times.Once);
            _bundleService
                .Verify(x => x.ProcessDefaultBundleFromReleaseAsync(
                    It.IsAny<Manifest>(),
                    It.IsAny<string>()),
                    Times.Once());
        }
    }
}