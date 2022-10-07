// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shift.Core.Models.Common;
using Shift.Core.Models.Manifests;
using Shift.Core.Services;
using Shift.Core.Services.Manifests;

namespace Shift.UnitTests.ServiceTests
{
    [TestClass]
    public class InstallationServiceTests
    {
        private Mock<IBundleService> _bundleService;
        private Mock<IComponentService> _componentService;
        private Mock<IManifestService> _manifestService;

        public InstallationServiceTests()
        {
            _componentService = new Mock<IComponentService>();
            _componentService
                .Setup(x => x.DownloadComponentAsync(It.IsAny<Component>(),
                    It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<ShiftResultCode>());
            _manifestService = new Mock<IManifestService>();
            _manifestService
                .Setup(x => x.GetManifestAsync(It.IsAny<string>()))
                .ReturnsAsync(new Manifest { Components = new List<Component>() { } });

            _bundleService = new Mock<IBundleService>();
        }

        [TestMethod]
        public async Task InstallationService_InitAsync_DownloadsCorrectManifestTest()
        {
            // arrange
            var flavor = "test-flavor";
            var organization = "test-organization";
            var project = "test-project";
            var feed = "test-feed";
            var version = "test-version";

            var installationService = new InstallationService(
                _componentService.Object,
                _bundleService.Object,
                _manifestService.Object,
                NullLogger<InstallationService>.Instance);

            // act
            await installationService.InitAsync(flavor, organization, project, feed, version);

            // assert
            _manifestService
                .Verify(x => x.DownloadManifestAndConvertAsync(
                    It.Is<string>(x => x == flavor),
                    It.IsAny<string>(),
                    It.Is<string>(x => x == project),
                    It.Is<string>(x => x == feed),
                    It.Is<string>(x => x == version),
                    It.IsAny<string>()),
                Times.Once());
            _bundleService
                .Verify(x => x.DownloadAndProcessDefaultBundleAsync(
                    It.IsAny<Manifest>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task InstallationService_InitLocalAsync_CanExecuteTest()
        {
            // arrange
            var path = "Data\\manifest.json";

            var installationService = new InstallationService(
                _componentService.Object,
                _bundleService.Object,
                _manifestService.Object,
                NullLogger<InstallationService>.Instance);

            // act
            await installationService.InitLocalAsync(path);

            // assert
            _manifestService
                .Verify(x => x.GetManifestAsync(
                    It.Is<string>(x => x == path)),
                    Times.Once);
            _bundleService
                .Verify(x => x.DownloadAndProcessDefaultBundleAsync(
                    It.IsAny<Manifest>()),
                    Times.Once());
        }

        [TestMethod]
        public async Task InstallationService_InitReleaseAsync_CanExecuteTest()
        {
            // arrange
            var path = "manifest.json";

            var installationService = new InstallationService(
                _componentService.Object,
                _bundleService.Object,
                _manifestService.Object,
                NullLogger<InstallationService>.Instance);

            // act
            await installationService.InitReleaseAsync();

            // assert
            _manifestService
                .Verify(x => x.GetManifestAsync(
                    It.Is<string>(x => x == path)),
                    Times.Once);
            _bundleService
                .Verify(x => x.ProcessDefaultBundleFromReleaseAsync(
                    It.IsAny<Manifest>()),
                    Times.Once());
        }
    }
}