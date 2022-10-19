// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Shift.Core.Brokers
{
    public interface IAdoTokenBroker
    {
        Task<string> GetTokenCredentialAsync(string tenantId);
    }
}