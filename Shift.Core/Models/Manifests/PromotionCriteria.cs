// -----------------------------------------------------------------------
// <copyright file="PromotionCriteria.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Shift.Core.Models.Manifests
{
    public class PromotionCriteria
    {
        public Regex Filter { get; set; }

        public string[] RequiredViews { get; set; }

        public string Strategy { get; set; }
    }
}