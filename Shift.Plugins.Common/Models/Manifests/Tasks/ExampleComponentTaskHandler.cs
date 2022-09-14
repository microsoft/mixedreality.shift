// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Manifests.Tasks;
using System.Threading.Tasks;

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