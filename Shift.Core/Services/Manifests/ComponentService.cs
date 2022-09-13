// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Artifacts;
using Shift.Core.Services.Manifests.Tasks;

namespace Shift.Core.Services.Manifests
{
    /// <summary>
    /// Implements <see cref="IComponentService"/> to download and install components.
    /// </summary>
    public partial class ComponentService : IComponentService
    {
        private readonly IComponentTaskProvider _componentTaskProvider;
        private readonly ILogger _logger;
        private readonly IManifestService _manifestProcessingService;
        private readonly IPackageFeedService _packageFeedService;
        private readonly IServiceProvider _serviceProvider;
        public ComponentService(
            IComponentTaskProvider componentTaskProvider,
            ILogger<ComponentService> logger,
            IPackageFeedService packageFeedService,
            IServiceProvider serviceProvider,
            IManifestService manifestProcessingService)
        {
            _componentTaskProvider = componentTaskProvider;
            _logger = logger;
            _packageFeedService = packageFeedService;
            _serviceProvider = serviceProvider;
            _manifestProcessingService = manifestProcessingService;
        }

        private static void CopyDirectory(
            string sourceDir,
            string destinationDir,
            bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        private string GetComponentDownloadLocation(Component component)
        {
            var downloadRoot = ProgramDataPath.GetRootPath();
            string downloadDir = string.Empty;

            if (component.Location is PackageLocation packageLocation)
            {
                downloadDir = $@"{downloadRoot}\{component.Id}\{packageLocation.Version}";
            }
            else if (component.Location is FolderLocation folderLocation)
            {
                downloadDir = $@"{downloadRoot}\{component.Id}";
            }

            return downloadDir;
        }

        /// <summary>
        /// Gets a list of Component objects from the Manifest given a list of component ids
        /// </summary>
        /// <param name="manifest">Manifest object</param>
        /// <param name="components">Array of component ids</param>
        /// <param name="versions">Array of component versions</param>
        /// <returns></returns>
        private List<Component> GetComponentFromManifestByComponentIds(
            Manifest manifest,
            string[] components,
            string[] versions)
        {
            var componentsToProcess = new List<Component>();

            foreach (var ogComponent in manifest.Components)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (ogComponent.Id == components[i])
                    {
                        if (!string.IsNullOrEmpty(versions[i]))
                        {
                            ((PackageLocation)ogComponent.Location).Version = versions[i];
                        }
                        componentsToProcess.Add(ogComponent);
                    }
                }
            }

            return componentsToProcess;
        }
    }
}