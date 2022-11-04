// ---------------------------------------------------------------Path.GetDirectoryName(AppContext.BaseDirectory);--------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;

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
    }
}