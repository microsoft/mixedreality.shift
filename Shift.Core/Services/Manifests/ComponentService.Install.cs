// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Common;
using Shift.Core.Models.Events;
using Shift.Core.Models.Manifests;
using Shift.Core.Services.Manifests.Tasks;
using Shift.Core.Services.Serialization;

namespace Shift.Core.Services.Manifests
{
    public partial class ComponentService : IComponentService
    {
        public async Task<ShiftResultCode> InstallComponentAsync(Component component, string componentLocation = null)
        {
            var telemetryEvent = new InstallEvent();
            telemetryEvent.ComponentId = component.Id;

            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;
            ShiftResultCode resultCode = ShiftResultCode.Unknown;

            try
            {
                componentLocation ??= GetComponentDownloadLocation(component);
                if (component.Task != null)
                {
                    telemetryEvent.TaskType = component.Task.Type;

                    var componentTask = _componentTaskProvider.GetComponentTask(component.Task.Type);

                    var handler = ActivatorUtilities.CreateInstance(_serviceProvider, componentTask.HandlerType) as IComponentTaskHandler;
                    await handler.ExecuteAsync(componentLocation, component);
                }

                resultCode = ShiftResultCode.Success;
                return resultCode;
            }
            catch (ShiftException ex)
            {
                telemetryEvent.ExceptionOcurred = true;
                telemetryEvent.ResultCode = ex.ResultCode.ToString();
                exception = ex;
                throw;
            }
            catch (Exception ex)
            {
                telemetryEvent.ExceptionOcurred = true;
                exception = ex;
                throw;
            }
            finally
            {
                telemetryEvent.DurationMS = stopwatch.ElapsedMilliseconds;
                telemetryEvent.ResultCode = resultCode.ToString();

                _logger.Log(telemetryEvent.ExceptionOcurred ?
                    LogLevel.Critical : LogLevel.Information,
                    new EventId(),
                    telemetryEvent,
                    exception,
                    LogEventSerialization.FormatState);
            }
        }

        public async Task<ShiftResultCode> InstallComponentsAsync(
            string[] components,
            string[] versions,
            string packageName,
            string organization,
            string project,
            string feed)
        {
            Manifest manifest = await _manifestProcessingService.DownloadManifestAndConvertAsync(packageName, organization, project, feed);

            return await InstallComponentsAsync(components, versions, manifest);
        }

        public async Task<ShiftResultCode> InstallComponentsAsync(
            string[] components,
            string[] versions,
            string manifestPath)
        {
            Manifest manifest = await _manifestProcessingService.GetManifestAsync(manifestPath);

            return await InstallComponentsAsync(components, versions, manifest);
        }

        private async Task<ShiftResultCode> InstallComponentsAsync(
            string[] components,
            string[] versions,
            Manifest manifest)
        {
            List<Component> componentsToProcess = GetComponentFromManifestByComponentIds(manifest, components, versions);

            foreach (var component in componentsToProcess)
            {
                _logger.LogTrace($"Processing component [{component.Id}].");

                await DownloadComponentAsync(component);

                await InstallComponentAsync(component);
            }
            return ShiftResultCode.Success;
        }
    }
}