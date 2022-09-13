// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Brokers.Executable;
using Shift.Core.Models.Manifests;
using Shift.Core.Models.Manifests.Tasks;

namespace Shift.Core.Services.Manifests.Tasks
{
    public class PwshComponentTaskHandler : IComponentTaskHandler
    {
        private readonly ILogger<PwshComponentTaskHandler> _logger;
        private readonly PwshExecutableCommandBroker _pwshBroker;

        public PwshComponentTaskHandler(ILogger<PwshComponentTaskHandler> logger, PwshExecutableCommandBroker pwshBroker)
        {
            _logger = logger;
            _pwshBroker = pwshBroker;
        }

        public async Task ExecuteAsync(string path, Component component)
        {
            _pwshBroker.OnOutputReceived += OnOutputReceived;
            _pwshBroker.OnErrorReceived += OnErrorReceived;

            // save script next to the component bits
            var scriptPath = Path.Combine(path, $"{component.Id}-script.ps1");
            _logger.LogInformation($"Executing PowerShell script: {scriptPath}");
            File.WriteAllText(scriptPath, ((PwshTaskInfo)component.Task).Script);

            await _pwshBroker.ExecuteAsync(
                args: new[] { "-File", scriptPath },
                workingDirectory: Path.GetDirectoryName(scriptPath),
                runAsAdmin: ((PwshTaskInfo)component.Task).Elevate);
        }

        private void OnErrorReceived(string obj)
        {
            _logger.LogError(obj);
        }

        private void OnOutputReceived(string obj)
        {
            _logger.LogInformation(obj);
        }
    }
}