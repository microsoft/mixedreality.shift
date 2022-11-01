// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Shift.Core.Models.Artifacts;

namespace Shift.Core.Brokers
{
    /// <summary>
    /// Defines a <see cref="ISourceCodeBroker"/> that can pull and push content into a remote
    /// source code repository; includes functionality for branching and create remote pull requests.
    /// </summary>
    public interface ISourceCodeBroker
    {
        /// <summary>
        /// Create a pull request to merge <paramref name="sourceBranch"/> into <paramref name="targetBranch"/>.
        /// </summary>
        /// <param name="repositoryName">The name of the target repository.</param>
        /// <param name="sourceBranch">The source branch.</param>
        /// <param name="targetBranch">The target branch.</param>
        /// <param name="enableAutoComplete">Set auto complete status.</param>
        /// <returns>A task.</returns>
        Task CreatePullRequestAsync(
            string repositoryName,
            string sourceBranch,
            string targetBranch,
            bool enableAutoComplete);

        /// <summary>
        /// Branch off of <paramref name="sourceBranch"/> and add additional content.
        /// </summary>
        /// <param name="repositoryName">The name of the target repository.</param>
        /// <param name="sourceBranch">The source branch.</param>
        /// <param name="branchName">The new branch name.</param>
        /// <param name="comment">The commit comment.</param>
        /// <param name="changes">
        /// The list of changes to commit to <paramref name="sourceBranch"/> before creating
        /// <paramref name="branchName"/>.
        /// </param>
        /// <returns>A task.</returns>
        Task CreatePushAsync(
            string repositoryName,
            string sourceBranch,
            string branchName,
            string comment,
            ItemChange[] changes);

        /// <summary>
        /// Download a file from source code repository.
        /// </summary>
        /// <param name="repositoryName">The name of the target repository.</param>
        /// <param name="sourceBranch">The source branch.</param>
        /// <param name="filePath">The file path, relative to the root of the repository.</param>
        /// <returns>File content expressed in bytes.</returns>
        Task<byte[]> DownloadFileAsync(
            string repositoryName,
            string sourceBranch,
            string filePath);
    }
}