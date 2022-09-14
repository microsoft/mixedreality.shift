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
using Shift.Core.Models.Plugins;
using Shift.Core.Services.Artifacts;
using Shift.Core.Services.Manifests;
using Shift.Core.Services.Manifests.Tasks;
using Shift.Plugins.Common;

namespace Shift.UnitTests.ServiceTests
{
    [TestClass]
    public class BundleServiceTests
    {
        private readonly BundleService _bundleService;
        private readonly Mock<IComponentService> _componentService;
        private readonly Manifest _manifest;
        private readonly Mock<IPackageFeedService> _packageFeedService;

        public BundleServiceTests()
        {
            _packageFeedService = new Mock<IPackageFeedService>();

            _componentService = new Mock<IComponentService>();

            var manifestService = new ManifestService(
                new ComponentTaskProvider(
                    new BasePluginDefinition(),
                    new CommonPluginDefinition()),
                _packageFeedService.Object,
                NullLogger<ManifestService>.Instance
                );

            _bundleService = new BundleService(
                _componentService.Object,
                manifestService,
                NullLogger<BundleService>.Instance
                );

            _manifest = manifestService.GetManifestAsync("Data\\manifest.json").Result;
        }

        [TestMethod]
        public async Task BundleService_DownloadBundleAsync_CanDownloadCorrectComponents()
        {
            // act
            var results = await _bundleService.DownloadBundleAsync(_manifest, "bundle1");

            // assert
            Assert.AreEqual(ShiftResultCode.Success, results);
            _componentService
                .Verify(x => x.DownloadComponentAsync(
                    It.Is<Component>(component => component.Id == "noTaskTestComponent"),
                    It.IsAny<string>()
                    ), Times.Once());
        }

        [TestMethod]
        public async Task BundleService_DownloadBundleAsync_CanDownloadCorrectComponents2()
        {
            // act
            var results = await _bundleService.DownloadBundleAsync(_manifest, "bundle2");

            // assert
            Assert.AreEqual(ShiftResultCode.Success, results);
            _componentService
                .Verify(x => x.DownloadComponentAsync(
                    It.Is<Component>(component => component.Id == "pluginTestComponent" || component.Id == "pwshTestComponent"),
                    It.IsAny<string>()
                    ), Times.Exactly(2));
        }

        [TestMethod]
        public async Task BundleService_ProcessBundleAsync_CanProcessCorrectComponents()
        {
            // act
            var results = await _bundleService.ProcessBundleAsync(_manifest, "bundle1");

            // assert
            Assert.AreEqual(ShiftResultCode.Success, results);
            _componentService
                .Verify(x => x.InstallComponentAsync(
                    It.Is<Component>(component => component.Id == "noTaskTestComponent"),
                    It.IsAny<string>()
                    ), Times.Once());
        }

        [TestMethod]
        public async Task BundleService_ProcessBundleAsync_CanProcessCorrectComponents2()
        {
            // act
            var results = await _bundleService.ProcessBundleAsync(_manifest, "bundle2");

            // assert
            Assert.AreEqual(ShiftResultCode.Success, results);
            _componentService
                .Verify(x => x.InstallComponentAsync(
                    It.Is<Component>(component => component.Id == "pluginTestComponent" || component.Id == "pwshTestComponent"),
                    It.IsAny<string>()
                    ), Times.Exactly(2));
        }
    }
}