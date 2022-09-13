// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Shift.Core.Models.Manifests.Tasks
{
    public interface IComponentTask

    {
        Type ContractType { get; }

        Type HandlerType { get; }

        Type ModelType { get; }

        string TaskType { get; }

        object ConvertToContract(object model);

        object ConvertToModel(object contract);
    }
}