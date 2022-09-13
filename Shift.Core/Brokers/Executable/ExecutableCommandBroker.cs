// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Common;

namespace Shift.Core.Brokers.Executable
{
    public class ExecutableCommandBroker : ICommandBroker
    {
        protected readonly ILogger _logger;

        public ExecutableCommandBroker(ILogger<ExecutableCommandBroker> logger, string executablePath)
        {
            ExecutablePath = executablePath;
            _logger = logger;
        }

        protected ExecutableCommandBroker(ILogger logger, string executablePath)
        {
            ExecutablePath = executablePath;
            _logger = logger;
        }

        public event System.Action<string> OnErrorReceived;

        public event System.Action<string> OnOutputReceived;

        protected string ExecutablePath { get; set; }

        public async Task<ShiftResultCode> ExecuteAsync(
            string[] args,
            string workingDirectory = default,
            bool runAsAdmin = false)
        {
            string errorOccured = string.Empty;
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExecutablePath,
                    WorkingDirectory = workingDirectory,
                }
            };

            foreach (var a in args)
            {
                process.StartInfo.ArgumentList.Add(a);
            }

            if (runAsAdmin)
            {
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";
            }
            else
            {
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
            }

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    OnOutputReceived?.Invoke(args.Data);
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    OnErrorReceived?.Invoke(args.Data);
                    errorOccured += args.Data + "\n";
                }
            };

            await Task.Run(() =>
            {
                process.Start();
                if (!runAsAdmin)
                {
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                }
                process.WaitForExit();
            });

            if (!string.IsNullOrEmpty(errorOccured.Trim()))
            {
                throw new ShiftException(ShiftResultCode.CommandExecutionError, errorOccured);
            }

            return ShiftResultCode.Success;
        }
    }
}