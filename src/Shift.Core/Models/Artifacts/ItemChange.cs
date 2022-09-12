// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Shift.Core.Models.Artifacts
{
    /// <summary>
    /// Implements an item change; used in conjunction with <see cref="ISourceCodeBroker"/>.
    /// </summary>
    public abstract class ItemChange
    {
        /// <summary>
        /// Gets or sets the new content of the item.
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets the relative path of the item.
        /// </summary>
        public string Path { get; set; }
    }
}