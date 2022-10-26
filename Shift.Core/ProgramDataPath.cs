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
        private static string StagingDirectory => Path.Combine(Path.GetTempPath(), "Shift");

        public static string GetPacakgeInstallationPath(string packageName, string feedName, string version)
        {
            return Path.Combine(StagingDirectory, packageName, feedName, version) + "\\";
        }

        public static string GetStagingDirectory()
        {
            if (!Directory.Exists(StagingDirectory))
            {
                Directory.CreateDirectory(StagingDirectory);
            }
            return StagingDirectory;
        }

        public static string GetWorkingDirectory()
        {
            return AppContext.BaseDirectory;
        }
    }
}