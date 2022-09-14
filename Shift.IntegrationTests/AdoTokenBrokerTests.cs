// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shift.Core.Brokers;

namespace Shift.IntegrationTests
{
    [TestClass]
    public class AdoTokenBrokerTests
    {
        [TestMethod]
        public async Task AdoTokenBroker_CanGetToken_WithOrganizationId()
        {
            // arrange
            var broker = new AdoTokenBroker();

            // act
            var token = await broker.GetTokenCredentialAsync("microsoft");

            // assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public async Task AdoTokenBroker_CanGetToken_WithOrganizationId_FromEnvVar()
        {
            // arrange
            var broker = new AdoTokenBroker();

            // act
            Environment.SetEnvironmentVariable("AZURE_DEVOPS_EXT_PAT_MICROSOFT", "abc");
            var token = await broker.GetTokenCredentialAsync("microsoft");

            // assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
            Assert.AreEqual("abc", token);
        }
    }
}