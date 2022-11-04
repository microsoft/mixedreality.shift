// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MixedReality.Shift.Cli.Commands;
using Moq;
using Shift.Core;
using Shift.Core.Models.Manifests;
using Shift.Core.Services;
using Shift.Core.Services.Manifests;

namespace Shift.UnitTests.CommandTests
{
    [TestClass]
    public class RunCommandTests
    {
        [TestMethod]
        public async Task RunCommandHandler_ExecuteAsync_CanUseUserDefinedStagingDirectory()
        {
            // arrange
            var manifest = new Manifest();
            var componentService = new Mock<IComponentService>();
            var bundleService = new Mock<IBundleService>();
            bundleService
                .Setup(x => x.DownloadAndProcessDefaultBundleAsync(It.IsAny<Manifest>(), It.IsAny<string>()));
            var manifestService = new Mock<IManifestService>();
            manifestService
                .Setup(x => x.GetManifestAsync(It.IsAny<string>()))
                .ReturnsAsync(manifest);

            var installationService = new InstallationService(
                componentService.Object,
                bundleService.Object,
                manifestService.Object,
                NullLogger<InstallationService>.Instance);

            var handler = new RunCommandHandler(
                installationService,
                NullLogger<RunCommandHandler>.Instance);
            var stagingDirectory = "%STAGINGDIRECTORY%";

            // act
            await handler.ExecuteAsync(new RunCommandHandlerInput(
                Path: "Data\\manifest.json",
                Bundle: "",
                DownloadOnly: false,
                StagingDirectory: stagingDirectory));

            // assert
            bundleService
                .Verify(x => x.DownloadAndProcessDefaultBundleAsync(
                    It.Is<Manifest>(x => x == manifest),
                    It.Is<string>(x => string.Equals(x, stagingDirectory))),
                    Times.Once());
            Assert.AreEqual(stagingDirectory, ProgramDataPath.GetStagingDirectory());
        }
    }
}
