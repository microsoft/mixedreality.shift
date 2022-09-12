// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Shift.Core.Contracts.Manifests;

namespace Shift.Core.Models.Manifests
{
    public class Location
    {
        public static implicit operator Location(LocationV1 contract)
        {
            if (contract == null)
            {
                return null;
            }

            if (contract is PackageLocationV1 packageContract)
            {
                return new PackageLocation
                {
                    Organization = packageContract.Organization,
                    Feed = packageContract.Feed,
                    Project = packageContract.Project,
                    Name = packageContract.Name,
                    Version = packageContract.Version,
                };
            }
            else if (contract is FolderLocationV1 folderContract)
            {
                return new FolderLocation
                {
                    Path = folderContract.Path
                };
            }
            else
            {
                return new Location { };
            }
        }

        public static implicit operator LocationV1(Location model)
        {
            if (model == null)
            {
                return null;
            }

            if (model is FolderLocation folderModel)
            {
                return new FolderLocationV1
                {
                    Path = folderModel.Path
                };
            }
            else if (model is PackageLocation packageModel)
            {
                return new PackageLocationV1
                {
                    Organization = packageModel.Organization,
                    Feed = packageModel.Feed,
                    Project = packageModel.Project,
                    Name = packageModel.Name,
                    Version = packageModel.Version,
                };
            }
            else
            {
                return new LocationV1
                {
                };
            }
        }
    }
}