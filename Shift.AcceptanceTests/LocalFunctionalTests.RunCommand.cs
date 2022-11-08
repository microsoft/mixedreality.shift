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
        public async Task Shift_CanRunManifest_WithHelloWorldSample()
        {
            // arrange
            var process = CreateProcess("run \"./Data/hello-world-manifest.json\"");

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
        }

        [TestMethod]
        public async Task Shift_CanRunManifest_WithBundleOptionAndHelloWorldSample()
        {
            // arrange
            var process = CreateProcess("run \"./Data/hello-world-manifest.json\" --bundle supplemental");

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
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(ProgramDataPath.GetStagingDirectory(), "hello-world-2"), "readme.md")));
        }

        [TestMethod]
        public async Task Shift_CanDownloadManifest_WithHelloWorldSample()
        {
            // arrange
            var stagingDirectory = Path.Combine(Path.GetTempPath(), nameof(Shift_CanDownloadManifest_WithHelloWorldSample));
            var process = CreateProcess($"run \"./Data/hello-world-manifest.json\" --download-only --staging-directory {stagingDirectory}");

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
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(stagingDirectory, "hello-world"), "readme.md")));
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(stagingDirectory, "hello-world-2"), "readme.md")));
        }

        [TestMethod]
        public async Task Shift_CanRunManifest_WithPathToArchiveHelloWorldSample()
        {
            // arrange
            var process = CreateProcess($"run \"./Data/archive.zip\"");

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
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine("./Data/archive", "hello-world"), "readme.md")));

            // cleanup
            Directory.Delete("./Data/archive", recursive: true);
        }
    }
}