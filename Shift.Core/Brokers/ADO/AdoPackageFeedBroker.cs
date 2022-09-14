// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Shift.Core.Brokers
{
    /// <summary>
    /// Endpoint for connecting to ADO and accessing ADO information
    /// </summary>
    public class AdoPackageFeedBroker : IPackageFeedBroker
    {
        private const string ArtifactToolName = "ArtifactTool";

        private const string ResourceAreaId = "3fda18ba-dff2-42e6-8d10-c521b23b85fc";

        private readonly string _artifactToolLocation;

        private readonly VssBasicCredential _collectionCredentials;

        private readonly string _collectionPat;

        private readonly ILogger<AdoPackageFeedBroker> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoPackageFeedBroker"/> class.
        /// </summary>
        /// <param name="collectionPat">The collection personal access token.</param>
        /// <param name="collectionUri">The collection URI.</param>
        /// <param name="projectName">The collection project name.</param>
        public AdoPackageFeedBroker(
            IAdoTokenBroker tokenBroker,
            ILogger<AdoPackageFeedBroker> logger)
        {
            _collectionPat = tokenBroker.GetTokenCredentialAsync().Result;
            _collectionCredentials = new VssBasicCredential(string.Empty, _collectionPat);
            _logger = logger;
            _artifactToolLocation = InstallArtifactToolAsync(organization: "https://microsoft.visualstudio.com/").Result;
        }

        /// <summary>
        /// Compares whether the new version is greater than the current version
        /// </summary>
        /// <param name="newVersion">New version</param>
        /// <param name="curVersion">Current version</param>
        /// <returns>Whether the new version is greater or not</returns>
        public static bool IsVersionGreater(string newVersion, string curVersion)
        {
            var version = new Version(newVersion);
            return version.CompareTo(new Version(curVersion)) > 0;
        }

        public async Task DownloadPackageAsync(
            string packageName,
            string feedName,
            string organization,
            string packageVersion,
            string downloadPath,
            string projectName = default)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(_artifactToolLocation, "artifacttool.exe"),
                WorkingDirectory = _artifactToolLocation,
            };

            // set environment variables
            startInfo.Environment.Add("pat", _collectionPat);
            Directory.CreateDirectory(downloadPath);
            var collectionUri = ConvertOrganizationToCollectionUri(organization);

            // create argument list, todo: escaping strings?
            startInfo.Arguments = $"universal download " +
                $"--feed {feedName} " +
                $"--package-name {packageName} " +
                $"--package-version {packageVersion} " +
                $"--service {collectionUri} " +
                $"--path \"{downloadPath.Replace(@"\", @"\\")}\" " +
                $"--patvar pat";

            if (!string.IsNullOrWhiteSpace(projectName))
            {
                startInfo.Arguments += $" --project {projectName}";
            }

            _logger.LogInformation($"Downloading UPack:{Environment.NewLine}" +
                $"{{{Environment.NewLine}" +
                $"\turl: {organization},{Environment.NewLine}" +
                $"\tfeed: {feedName},{Environment.NewLine}" +
                $"\tpackage: {packageName},{Environment.NewLine}" +
                $"\tversion: {packageVersion}{Environment.NewLine}" +
                $"}}");

            // execute artificat tool
            using var process = new Process { StartInfo = startInfo };
            if (process.Start())
            {
                await Task.Run(() => process.WaitForExit());
            }
        }

        /// <summary>
        /// Gets the latest package version from ADO feed
        /// </summary>
        /// <param name="feedName">The ADO feed name</param>
        /// <param name="packageId">The package id</param>
        /// <returns></returns>
        public async Task<string> GetLatestPackageVersionAsync(
            string feedName,
            string packageId,
            string organization,
            string project)
        {
            string collectionUri = ConvertOrganizationToCollectionUri(organization);

            var connection = new VssConnection(
                baseUrl: new Uri(collectionUri),
                credentials: _collectionCredentials);

            using FeedHttpClient feedClient = connection.GetClient<FeedHttpClient>();

            Package packages = await feedClient.GetPackageAsync(
                project,
                feedName,
                protocolType: "UPack",
                packageId);

            return packages.Versions.FirstOrDefault(x => x.IsLatest == true).Version;
        }

        /// <summary>
        /// Gets the latest package version from ADO feed
        /// </summary>
        /// <param name="feedName">The ADO feed name</param>
        /// <param name="packageId">The package id</param>
        /// <returns></returns>
        public async Task<List<Models.Artifacts.PackageVersion>> GetPackageVersionsAsync(
            string feedName,
            string packageId,
            string organization,
            string project)
        {
            string collectionUri = ConvertOrganizationToCollectionUri(organization);

            var connection = new VssConnection(
                baseUrl: new Uri(collectionUri),
                credentials: _collectionCredentials);

            using FeedHttpClient feedClient = connection.GetClient<FeedHttpClient>();

            Package package = await feedClient.GetPackageAsync(
                project,
                feedName,
                protocolType: "UPack",
                packageId,
                includeAllVersions: true);

            return package.Versions.Select(v => new Models.Artifacts.PackageVersion
            {
                PublishDate = v.PublishDate,
                IsLatest = v.IsLatest,
                Version = v.Version,
                Views = v.Views.Select(v => v.Name).ToList(),
            }).ToList();
        }

        private static string ConvertOrganizationToCollectionUri(string organization)
        {
            return Uri.IsWellFormedUriString(organization, UriKind.Absolute) ? organization : $"https://dev.azure.com/{organization}/";
        }

        private void GetEnvironmentInfo(
            out string osName,
            out string arch,
            out string distroName,
            out string distroVersion)
        {
            // effectively unused
            distroName = string.Empty;
            distroVersion = string.Empty;

            // get process architecture
            arch = RuntimeInformation.OSArchitecture switch
            {
                Architecture.Arm => "arm",
                Architecture.Arm64 => "arm64",
                Architecture.X86 => "x86",
                _ => "x86_64",
            };

            osName = GetOSName();
        }

        private string GetOSName() {
            string osName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                osName = "Windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                osName = "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                osName = "OSX";
            }
            else
            {
                osName = string.Empty;
            }
            return osName;
        }

        private async Task<string> InstallArtifactToolAsync(
            string organization,
            string path = default,
            bool forceInstall = false)
        {
            GetEnvironmentInfo(
                out string osName,
                out string arch,
                out string distroName,
                out string distroVersion);

            // discover the clienttools resource area
            string collectionUri = ConvertOrganizationToCollectionUri(organization);

            var vssConnection = new VssConnection(
                baseUrl: new Uri(collectionUri),
                credentials: _collectionCredentials);

            VssBlobHttpClient blobClient = vssConnection.GetClient<VssBlobHttpClient>(Guid.Parse(ResourceAreaId));

            // get extraction path
            var toolInfo = await blobClient.GetToolAsync(
                ArtifactToolName,
                osName,
                arch,
                distroName,
                distroVersion);

            var extractPath = path ?? Path.Combine(ProgramDataPath.GetRootPath(), ArtifactToolName, toolInfo.Version);

            // for install is not set, and path exists, we assume tool is installed
            if (Directory.Exists(extractPath) && !forceInstall)
            {
                return extractPath;
            }

            // download artifact tool from ado itself
            var outputStream = new MemoryStream();
            await blobClient.GetToolAsync(
                ArtifactToolName,
                outputStream,
                osName,
                arch,
                distroName,
                distroVersion);

            // unpack artifact tool into relevant directory
            outputStream.Position = 0;
            using var archive = new ZipArchive(outputStream);

            foreach (var entry in archive.Entries)
            {
                string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));
                var destinationInfo = new FileInfo(destinationPath);

                // ensure the zip file isn't trying to extract outside its path structure, which is possible
                if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                {
                    Directory.CreateDirectory(destinationInfo.DirectoryName);
                    entry.ExtractToFile(destinationPath, true);
                }
            }

            return extractPath;
        }
    }
}