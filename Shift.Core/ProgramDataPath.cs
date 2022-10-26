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
        private static readonly string stagingDirectory = "%Temp%\\Shift";

        public static string GetPacakgeInstallationPath(string packageName, string feedName, string version)
        {
            return Path.Combine(stagingDirectory, packageName, feedName, version) + "\\";
        }

        public static string GetStagingDirectory()
        {
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