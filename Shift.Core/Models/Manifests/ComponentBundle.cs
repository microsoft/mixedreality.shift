// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Shift.Core.Contracts.Manifests;

namespace Shift.Core.Models.Manifests
{
    public class ComponentBundle
    {
        /// <summary>
        /// Gets or sets the dependent bundle information
        /// </summary>
        public List<string> Bundles { get; set; }

        /// <summary>
        /// Gets or sets the components information
        /// </summary>
        public List<string> Components { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }

        public static implicit operator ComponentBundle(ComponentBundleV1 contract)
        {
            if (contract == null)
            {
                return null;
            }

            return new ComponentBundle
            {
                Bundles = contract.Bundles?.ToList(),
                Components = contract.Components?.ToList(),
                Description = contract.Description,
                Id = contract.Id,
            };
        }

        public static implicit operator ComponentBundleV1(ComponentBundle model)
        {
            if (model == null)
            {
                return null;
            }

            return new ComponentBundleV1
            {
                Bundles = model.Bundles?.ToArray(),
                Components = model.Components?.ToArray(),
                Description = model.Description,
                Id = model.Id,
            };
        }
    }
}