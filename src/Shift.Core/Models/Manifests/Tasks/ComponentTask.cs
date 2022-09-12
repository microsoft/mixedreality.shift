// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Shift.Core.Services.Manifests.Tasks;

namespace Shift.Core.Models.Manifests.Tasks
{
    public abstract class ComponentTask<TContract, TModel, THandler> : IComponentTask where THandler : IComponentTaskHandler
    {
        public Type ContractType => typeof(TContract);

        public Type HandlerType => typeof(THandler);

        public Type ModelType => typeof(TModel);

        public abstract string TaskType { get; }

        public abstract TModel Convert(TContract contract);

        public abstract TContract Convert(TModel model);

        public object ConvertToContract(object model) => Convert((TModel)model);

        public object ConvertToModel(object contract) => Convert((TContract)contract);
    }
}