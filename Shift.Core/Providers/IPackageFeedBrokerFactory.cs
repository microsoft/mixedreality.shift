// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Shift.Core.Brokers;

namespace Shift.Core.Providers
{
    public interface IPackageFeedBrokerFactory
    {
        public IPackageFeedBroker CreatePackageFeedBroker(string collectionUri, string projectName, string pat);
    }
}