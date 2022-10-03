// -----------------------------------------------------------------------
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
        private static readonly string rootPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Shift");

        public static string GetPacakgeInstallationPath(string packageName, string feedName, string version)
        {
            return Path.Combine(rootPath, packageName, feedName, version) + "\\";
        }

        public static string GetRootPath()
        {
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            return rootPath;
        }
    }
}