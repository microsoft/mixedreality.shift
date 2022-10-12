// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Shift.Core.Models.Artifacts;

namespace Shift.Core.Brokers
{
    /// <summary>
    /// Implements the <see cref="ISourceCodeBroker"/> for Azure DevOps based source code repositories.
    /// </summary>
    public class AdoSourceCodeBroker : ISourceCodeBroker
    {
        private readonly VssBasicCredential _collectionCredentials;
        private readonly string _collectionPat;
        private readonly ILogger<AdoSourceCodeBroker> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoSourceCodeBroker"/> class.
        /// </summary>
        public AdoSourceCodeBroker(ILogger<AdoSourceCodeBroker> logger, string pat, string collectionUri, string projectNamer)
        {
            _collectionPat = pat;
            _collectionCredentials = new VssBasicCredential(string.Empty, _collectionPat);
            _logger = logger;
        }

        /// <summary>
        /// Create a pull request to merge <paramref name="sourceBranch"/> into <paramref name="targetBranch"/>.
        /// </summary>
        /// <param name="repositoryName">The name of the target repository.</param>
        /// <param name="sourceBranch">The source branch.</param>
        /// <param name="targetBranch">The target branch.</param>
        /// <param name="enableAutoComplete">Set auto complete status.</param>
        /// <returns>A task.</returns>
        public async Task CreatePullRequestAsync(
            string organization,
            string projectName,
            string repositoryName,
            string sourceBranch,
            string targetBranch,
            bool enableAutoComplete)
        {
            string collectionUri = ConvertOrganizationToCollectionUri(organization);
            var connection = new VssConnection(new Uri(collectionUri), _collectionCredentials);
            using var gitClient = connection.GetClient<GitHttpClient>();

            // Get data about a specific repository
            var repository = await gitClient.GetRepositoryAsync(projectName, repositoryName);

            var pullRequest = await gitClient.CreatePullRequestAsync(
                new GitPullRequest
                {
                    Title = $"Test pull request {sourceBranch} -> {targetBranch}",
                    SourceRefName = $"refs/heads/{sourceBranch}",
                    TargetRefName = $"refs/heads/{targetBranch}",
                    AutoCompleteSetBy = new IdentityRef { },
                }, repository.Id);

            if (enableAutoComplete)
            {
                var userId = new IdentityRef { Id = connection.AuthorizedIdentity.Id.ToString() };
                await gitClient.UpdatePullRequestAsync(
                    new GitPullRequest { AutoCompleteSetBy = userId, CompletionOptions = new GitPullRequestCompletionOptions { DeleteSourceBranch = true } },
                    repository.Id,
                    pullRequest.PullRequestId);
            }
        }

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
        public async Task CreatePushAsync(
            string organization,
            string projectName,
            string repositoryName,
            string sourceBranch,
            string branchName,
            string comment,
            ItemChange[] changes)
        {
            // Get a GitHttpClient to talk to the Git endpoints
            string collectionUri = ConvertOrganizationToCollectionUri(organization);

            var connection = new VssConnection(new Uri(collectionUri), _collectionCredentials);
            using var gitClient = connection.GetClient<GitHttpClient>();

            // Get data about a specific repository
            var repository = await gitClient.GetRepositoryAsync(projectName, repositoryName);
            var refs = await gitClient.GetRefsAsync(repository.Id, filter: $"heads/{sourceBranch}");

            var newObjectId = (Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n")).Substring(0, 40);

            var refUpdate = new GitRefUpdate
            {
                OldObjectId = refs.Single(x => x.Name == $"refs/heads/{sourceBranch}").ObjectId,
                Name = $"refs/heads/{branchName}",
            };

            var changeset = changes.Select(c =>
            {
                return new GitChange
                {
                    ChangeType = c is AddItemChange ? VersionControlChangeType.Add : VersionControlChangeType.Edit,
                    Item = new GitItem { Path = c.Path },
                    NewContent = new ItemContent
                    {
                        Content = Convert.ToBase64String(c.Content),
                        ContentType = ItemContentType.Base64Encoded,
                    },
                };
            });

            await gitClient.CreatePushAsync(
                new GitPush
                {
                    RefUpdates = new[] { refUpdate },
                    Commits = new[] { new GitCommitRef { Comment = comment, Changes = changeset.ToArray() } },
                },
                repository.Id);

            // var results = await gitClient.UpdateRefsAsync(new[] { refUpdate }, repository.Id);
        }

        /// <summary>
        /// Download a file from source code repository as array of bytes.
        /// </summary>
        /// <param name="repositoryName">The name of the target repository.</param>
        /// <param name="branch">The branch to download file from.</param>
        /// <param name="filePath">The file path, relative to the root of the repository.</param>
        /// <returns>File content expressed in bytes.</returns>
        public async Task<byte[]> DownloadFileAsync(
            string organization,
            string projectName,
            string repositoryName,
            string branch,
            string filePath)
        {
            string collectionUri = ConvertOrganizationToCollectionUri(organization);

            var connection = new VssConnection(new Uri(collectionUri), _collectionCredentials);
            using var gitClient = connection.GetClient<GitHttpClient>();

            var repository = await gitClient.GetRepositoryAsync(projectName, repositoryName);

            using var outputStream = new MemoryStream();
            using var contentStream = await gitClient.GetItemContentAsync(
                repository.Id,
                filePath,
                versionDescriptor: new GitVersionDescriptor { Version = $"{branch}", VersionType = GitVersionType.Branch },
                includeContent: true);

            await contentStream.CopyToAsync(outputStream);

            return outputStream.ToArray();
        }

        private static string ConvertOrganizationToCollectionUri(string organization)
        {
            return Uri.IsWellFormedUriString(organization, UriKind.Absolute) ? organization : $"https://dev.azure.com/{organization}/";
        }
    }
}