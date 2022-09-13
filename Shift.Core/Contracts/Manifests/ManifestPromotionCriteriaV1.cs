// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Shift.Core.Contracts.Manifests
{
    public class ManifestPromotionCriteriaV1
    {
        public Dictionary<string, PromotionCriteriaV1> Components { get; set; }
    }
}