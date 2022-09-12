// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Shift.Core.Models.Manifests;

namespace Shift.Core.Services.Manifests
{
    /// <summary>
    /// Defines a manifest service which operates on the <see cref="Manifest"/> domain object. The
    /// service is responsible for retrieving, deserializing and storing of manifests.
    /// </summary>
    public partial interface IManifestService
    {
        /// <summary>
        /// Convert UT8 encoded bytes into a manifest.
        /// </summary>
        /// <param name="bytes">The manifest bytes.</param>
        /// <returns>A product manifest.</returns>
        Manifest ConvertBytesToManifest(byte[] bytes);

        /// <summary>
        /// Convert UT8 encoded bytes into a manifest promotion criteria.
        /// </summary>
        /// <param name="bytes">The manifest promotion criteria bytes.</param>
        /// <returns>A product manifest.</returns>
        ManifestPromotionCriteria ConvertBytesToManifestPromotionCriteria(byte[] bytes);

        /// <summary>
        /// Convert manifest promotion criteria into UTF8 encoded json string.
        /// </summary>
        /// <param name="manifest">A manifest promotion criteria.</param>
        /// <returns>A utf8 encoded json string represented as bytes.</returns>
        byte[] ConvertManifestPromotionCriteriaToBytes(ManifestPromotionCriteria manifest);

        /// <summary>
        /// Convert manifest promotion criteria into UTF8 encoded json string.
        /// </summary>
        /// <param name="manifest">A manifest promotion criteria.</param>
        /// <returns>A utf8 encoded json string represented as bytes.</returns>
        byte[] ConvertManifestToBytes(Manifest manifest);
    }
}