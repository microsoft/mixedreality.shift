// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shift.Core.Brokers;

namespace Shift.UnitTests.BrokerTests
{
    [TestClass]
    public class AdoPackageFeedBrokerTests
    {
        [TestMethod]
        public async Task AdoPackageFeedBroker_InstallArtifactToolLockingTest()
        {
            var organization = "microsoft";
            var pat = await new AdoTokenBroker().GetTokenCredentialAsync(organization);
            var adoPackageFeedBroker = new AdoPackageFeedBroker(
                NullLogger<AdoPackageFeedBroker>.Instance,
                pat: pat,
                collectionUri: organization,
                projectName: "testProject");

            string artifactToolLocation = string.Empty;
            for (int i = 0; i < 30; i++)
            {
                artifactToolLocation = await adoPackageFeedBroker.InstallArtifactToolAsync(organization);
            }

            Assert.AreNotEqual(artifactToolLocation, string.Empty);
        }
    }
}
