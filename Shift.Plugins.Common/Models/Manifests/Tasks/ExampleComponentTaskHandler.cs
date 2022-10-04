// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Manifests.Tasks;

namespace Shift.Plugins.Common.Models.Manifest.Tasks
{
    public class ExampleComponentTaskHandler : IComponentTaskHandler
    {
        private readonly ILogger<ExampleComponentTaskHandler> _logger;

        public ExampleComponentTaskHandler(
            ILogger<ExampleComponentTaskHandler> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(string path, Component component)
        {
            _logger.LogInformation($"Running example component task handler.");
        }
    }
}