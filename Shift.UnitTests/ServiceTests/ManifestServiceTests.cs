// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shift.Core.Models.Manifests;
using Shift.Core.Models.Plugins;
using Shift.Core.Services.Artifacts;
using Shift.Core.Services.Manifests;
using Shift.Core.Services.Manifests.Tasks;
using Shift.Plugins.Common;

namespace Shift.UnitTests.ServiceTests
{
    [TestClass]
    public class ManifestServiceTests
    {
        private readonly Mock<IPackageFeedService> _packageFeedService;
        private readonly ManifestService _service;

        public ManifestServiceTests()
        {
            _packageFeedService = new Mock<IPackageFeedService>();

            _service = new ManifestService(
                new ComponentTaskProvider(
                    new BasePluginDefinition(),
                    new CommonPluginDefinition()),
                _packageFeedService.Object,
                NullLogger<ManifestService>.Instance
                );
        }

        [TestMethod]
        public async Task ManifestService_GetBundleComponents_CanGetCorrectComponents()
        {
            // arrange
            var manifest = await _service.GetManifestAsync("Data\\manifest.json");

            // act
            var components = _service.GetBundleComponents(manifest, "bundle1");

            // assert
            Assert.AreEqual(1, components.Count());
        }

        [TestMethod]
        public async Task ManifestService_GetBundleComponents_TestIncorrectBundleName()
        {
            // arrange
            var manifest = await _service.GetManifestAsync("Data\\manifest.json");

            // act
            var components = _service.GetBundleComponents(manifest, "incorrectBundleName");

            // assert
            Assert.AreEqual(0, components.Count());
        }

        [TestMethod]
        public async Task ManifestService_GetDefaultComponents_CanGetCorrectComponents()
        {
            // arrange
            var manifest = await _service.GetManifestAsync("Data\\manifest.json");

            // act
            var components = _service.GetDefaultComponents(manifest);

            // assert
            Assert.AreEqual(4, components.Count());
        }

        [TestMethod]
        public async Task ManifestService_GetManifestAsync_CanLoadManifest()
        {
            // act
            var manifest = await _service.GetManifestAsync("Data\\manifest.json");

            // assert
            Assert.IsNotNull(manifest);
            Assert.AreEqual(4, manifest.Components.Count);

            // assert -- check data is translated correctly in polymorphic cases
            Assert.AreEqual(2, manifest.Components.Count(x => x.Task == null));
            Assert.AreEqual(1, manifest.Components.Count(x => x.Task?.Type == "exampleTask"));
            Assert.AreEqual(1, manifest.Components.Count(x => x.Task?.Type == "pwsh"));
            Assert.AreEqual(3, manifest.Components.Count(x => x.Location is PackageLocation));
            Assert.AreEqual(1, manifest.Components.Count(x => x.Location is FolderLocation));
        }
    }
}