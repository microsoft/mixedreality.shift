// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Contracts.Manifests
{
    public class PromotionCriteriaV1
    {
        public string Filter { get; set; }

        public string[] RequiredViews { get; set; }

        public string Strategy { get; set; }
    }
}