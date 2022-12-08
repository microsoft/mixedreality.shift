// ---------------------------------------------------------------Path.GetDirectoryName(AppContext.BaseDirectory);--------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Shift.Core.Models.Common;

namespace Shift.Core
{
    public static class ProgramDataPath
    {
        private static string DefaultStagingDirectory => Path.Combine(Path.GetTempPath(), "Shift");

        public static string UserDefinedStagingDirectory { get; set; }

        public static string GetPacakgeInstallationPath(string packageName, string feedName, string version)
        {
            return Path.Combine(DefaultStagingDirectory, packageName, feedName, version) + "\\";
        }

        public static string GetStagingDirectory()
        {
            string stagingDirectory = string.IsNullOrEmpty(UserDefinedStagingDirectory) ? DefaultStagingDirectory : UserDefinedStagingDirectory;
            if (!Directory.Exists(stagingDirectory))
            {
                Directory.CreateDirectory(stagingDirectory);
            }
            return stagingDirectory;
        }

        public static string GetWorkingDirectory()
        {
            return AppContext.BaseDirectory;
        }

        /// <summary>
        /// Gets the manifest path based on the staging directory
        /// Throws Shift Exception if not found
        /// </summary>
        /// <returns></returns>
        public static string GetManifestPathFromStagingDirectory()
        {
            string stagingDirectory = GetStagingDirectory();
            string manifestPath = Directory.GetFiles(stagingDirectory)
                .FirstOrDefault(d => d.Contains("manifest.json"));

            if (string.IsNullOrEmpty(manifestPath))
            {
                throw new ShiftException(
                    resultCode: ShiftResultCode.ManifestNotFound,
                    message: $"No manifest.json file found under {stagingDirectory}");
            }

            return manifestPath;
        }
    }
}