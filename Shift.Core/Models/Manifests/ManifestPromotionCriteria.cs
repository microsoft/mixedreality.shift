// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Shift.Core.Models.Manifests
{
    public class ManifestPromotionCriteria
    {
        public Dictionary<string, PromotionCriteria> Components { get; set; }
    }
}