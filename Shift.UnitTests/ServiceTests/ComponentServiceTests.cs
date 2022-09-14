// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Artifacts;
using Shift.Core.Services.Manifests;
using Shift.Core.Services.Manifests.Tasks;

namespace Shift.UnitTests.ServiceTests
{
    [TestClass]
    public class ComponentServiceTests
    {
        private readonly Mock<IComponentTaskProvider> _componentTaskProvider;
        private readonly Mock<IManifestService> _manifestProcessingService;
        private readonly Mock<IPackageFeedService> _packageFeedService;
        private readonly Mock<IServiceProvider> _serviceProvider;

        public ComponentServiceTests()
        {
            _packageFeedService = new Mock<IPackageFeedService>();
            _componentTaskProvider = new Mock<IComponentTaskProvider>();
            _serviceProvider = new Mock<IServiceProvider>();
            _manifestProcessingService = new Mock<IManifestService>();
        }

        [TestMethod]
        public async Task ComponentService_DownloadComponentAsync_CanDownloadPackage()
        {
            // arrange
            var service = new ComponentService(
                _componentTaskProvider.Object,
                NullLogger<ComponentService>.Instance,
                _packageFeedService.Object,
                _serviceProvider.Object,
                _manifestProcessingService.Object);

            var component = new Component()
            {
                Location = new PackageLocation()
                {
                    Feed = "test-feed",
                    Name = "test-package",
                    Project = "test-project",
                    Version = "test-version",
                    Organization = "test-organization"
                }
            };

            _packageFeedService.Setup(service =>
                service.DownloadArtifactAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()));

            // act
            await service.DownloadComponentAsync(component);

            // assert
            _packageFeedService.Verify(service =>
                service.DownloadArtifactAsync(
                    It.IsAny<string>(),
                    It.Is<string>(feed => feed == ((PackageLocation)component.Location).Feed),
                    It.Is<string>(name => name == ((PackageLocation)component.Location).Name),
                    It.Is<string>(project => project == ((PackageLocation)component.Location).Project),
                    It.Is<string>(version => version == ((PackageLocation)component.Location).Version),
                    It.Is<string>(organization => organization == ((PackageLocation)component.Location).Organization)
                ), Times.Once());
            _packageFeedService.VerifyNoOtherCalls();
        }
    }
}