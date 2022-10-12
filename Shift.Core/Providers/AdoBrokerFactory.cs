// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;
using Shift.Core.Brokers;

namespace Shift.Core.Providers
{
    public class AdoBrokerFactory : ISourceCodeBrokerFactory, IPackageFeedBrokerFactory
    {
        private readonly ILogger<AdoPackageFeedBroker> _packageFeedLogger;

        private readonly ILogger<AdoSourceCodeBroker> _sourceCodeLogger;

        public AdoBrokerFactory(ILogger<AdoPackageFeedBroker> packageFeedLogger, ILogger<AdoSourceCodeBroker> sourceCodeLogger)
        {
            _packageFeedLogger = packageFeedLogger;
            _sourceCodeLogger = sourceCodeLogger;
        }

        public IPackageFeedBroker CreatePackageFeedBroker(string collectionUri, string projectName, string pat)
        {
            collectionUri = Uri.IsWellFormedUriString(collectionUri, UriKind.Absolute) ? collectionUri : $"https://dev.azure.com/{collectionUri}/";

            return new AdoPackageFeedBroker(_packageFeedLogger, pat, collectionUri, projectName);
        }

        public ISourceCodeBroker CreateSourceCodeBroker(string collectionUri, string projectName, string pat)
        {
            collectionUri = Uri.IsWellFormedUriString(collectionUri, UriKind.Absolute) ? collectionUri : $"https://dev.azure.com/{collectionUri}/";

            return new AdoSourceCodeBroker(_sourceCodeLogger, pat, collectionUri, projectName);
        }
    }
}