// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shift.Core.Brokers;

namespace Shift.IntegrationTests
{
    [TestClass]
    public class AdoPackageFeedBrokerTests
    {
        /// <summary>
        /// Azure devOps resource Id
        /// </summary>
        private static readonly string AdoResourceId = "499b84ac-1321-427f-aa17-267ca6975798";

        [TestMethod]
        public void AdoPackageFeedBroker_CanDownloadArtifactTool()
        {
            // arrange
            var tokenBroker = new AdoTokenBroker();
            var broker = new AdoPackageFeedBroker(tokenBroker, NullLogger<AdoPackageFeedBroker>.Instance);
            var path = Path.Combine(Path.GetTempPath(), "artifacttool");

            // act
            Assert.IsTrue(new FileInfo(Path.Combine(path, "artifacttool.exe")).Exists);
        }

        [TestMethod]
        public async Task AdoPackageFeedBroker_CanGetLatestVersion()
        {
            // arrange
            var tokenBroker = new AdoTokenBroker();
            var broker = new AdoPackageFeedBroker(tokenBroker, NullLogger<AdoPackageFeedBroker>.Instance);

            // act
            var versionString = await broker.GetLatestPackageVersionAsync(
                feedName: "MixedReality.Build.Tools",
                packageId: "mrshift-cli-windows",
                organization: "https://microsoft.visualstudio.com/",
                project: "Analog");

            // assert
            Assert.IsNotNull(versionString);
            var version = Version.Parse(versionString);
            Assert.IsTrue(version > new Version(0, 0, 1));
        }

        protected async Task<string> GetPatAsync()
        {
            var tokenCredential = new DefaultAzureCredential();
            var authResult = await tokenCredential.GetTokenAsync(new TokenRequestContext(scopes: new string[] { AdoResourceId + "/.default" }) { });
            return authResult.Token;
        }
    }
}