﻿// -----------------------------------------------------------------------
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
    [TestClass]
    [TestCategory("LOCAL-TESTS")]
    public class LocalFunctionalTests
    {
        private static string _filepath;

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _filepath = Environment.GetEnvironmentVariable("SHIFT_PATH") ?? testContext.Properties["SHIFT_PATH"].ToString();
        }

        [TestMethod]
        public async Task Shift_CanShowHelp()
        {
            // arrange
            var process = CreateProcess("-h");

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
            Assert.IsTrue(output.Contains("Options:"));
            Assert.IsTrue(output.Contains("Usage:"));
            Assert.IsTrue(output.Contains("Commands:"));
        }

        [TestMethod]
        public async Task Shift_CanShowVersion()
        {
            // arrange
            var process = CreateProcess("version");

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
            Assert.IsTrue(output.Contains("1.0.0"));
        }

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

        private static Process CreateProcess(string args)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo(_filepath, args)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                }
            };

            return process;
        }
    }
}