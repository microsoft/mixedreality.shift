// -----------------------------------------------------------------------
// <copyright file="UnitTest1.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shift.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Shift.Cli.AcceptanceTests
{
    /// <summary>
    /// These tests are intended to run against a built version of shift; it is expected that the machine running these tests
    /// has basic access to internet, etc. The "SHIFT_PATH" variable must be set in default.runsettings before this test is executed.
    /// </summary>
    [TestCategory("LOCAL-TESTS")]
    public partial class LocalFunctionalTests
    {
        [TestMethod]
        public async Task Shift_CanPack_WithHelloWorldSample()
        {
            // arrange
            var outputPath = Path.GetTempFileName() + ".zip";
            var process = CreateProcess($"pack --manifest-path \"./Data/hello-world-manifest.json\" --output-path \"{outputPath}\"");

            // act
            process.Start();
            await process.WaitForExitAsync();
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            // write to output, for posterity
            await Console.Out.WriteLineAsync(output);
            await Console.Error.WriteLineAsync(error);

            // assert
            Assert.AreEqual(0, process.ExitCode);
            Assert.IsTrue(File.Exists(outputPath));
        }
    }
}