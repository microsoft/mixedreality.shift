// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shift.Core.Brokers.Executable;
using Shift.Core.Models.Common;

namespace Shift.UnitTests.BrokerTests
{
    [TestClass]
    public class ExecutableCommandBrokerTests
    {
         [TestMethod]
        public async Task ExecutableCommandBroker_CanExecuteCommand()
        {
            // arrange
            var executableCommandBroker = new ExecutableCommandBroker(NullLogger<ExecutableCommandBroker>.Instance, "pwsh");
            var output = new List<string>();
            executableCommandBroker.OnOutputReceived += (data) =>
            {
                if (data != null)
                {
                    output.Add(data);
                }
            };

            // act
            var result = await executableCommandBroker.ExecuteAsync(new[] { "-Command", "Write-Host", "abcd" });

            // assert
            Assert.AreEqual(ShiftResultCode.Success, result);
            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("abcd", output[0]);
        }
    }
}