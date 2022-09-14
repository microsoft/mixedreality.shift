// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shift.Core.Brokers;
using Shift.Core.Models.Artifacts;
using Shift.Core.Services.Artifacts;

namespace Shift.UnitTests.ServiceTests
{
    [TestClass]
    public class PackageFeedServiceTests
    {
        [TestMethod]
        public async Task PackageFeedService_CanCacheBrokerResults()
        {
            // arrange
            var broker = new Mock<IPackageFeedBroker>();
            broker.Setup(broker => broker.GetPackageVersionsAsync(
                It.Is<string>(x => x == "testFeed"),
                It.Is<string>(x => x == "testPackage"),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(new System.Collections.Generic.List<PackageVersion>
                {
                    new PackageVersion
                    {
                        IsLatest = true,
                        PublishDate = DateTime.UtcNow,
                        Version = "1.0.1",
                        Views = new System.Collections.Generic.List<string>(),
                    }
                });

            var service = new PackageFeedService(new MemoryCache(new MemoryCacheOptions { }), broker.Object);

            // act
            var versions1 = await service.GetPackageVersionsAsListOfStringAsync("https://dev.azure.com/test", "test", "testFeed", "testPackage", 1);
            var versions2 = await service.GetPackageVersionsAsListOfStringAsync("https://dev.azure.com/test", "test", "testFeed", "testPackage", 1);

            // assert
            Assert.IsNotNull(versions2);
            Assert.AreEqual(1, versions2.Count());
            broker.Verify(broker => broker.GetPackageVersionsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }
    }
}