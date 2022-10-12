// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shift.Core.Models.Common;

namespace Shift.Core.Commands
{
    public abstract record BaseCommandHandlerInput;

    public abstract class BaseCommandHandler<TBaseCommandInput> where TBaseCommandInput : BaseCommandHandlerInput
    {
        public BaseCommandHandler(ILogger logger)
        {
            Logger = logger;
        }

        public BaseCommandHandler(
            ILogger logger,
            IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        protected ILogger Logger { get; }

        protected IServiceProvider ServiceProvider { get; }

        public async Task<ShiftResultCode> ExecuteAsync(
            TBaseCommandInput input,
            CancellationToken cancellationToken = default)
        {
            Logger.LogTrace($"Start_{GetType().Name}");

            var result = ShiftResultCode.Unknown;
            try
            {
                result = await ExecuteAsyncOverride(input, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ShiftException mrex)
            {
                Logger.LogError($"ERROR: {mrex.Message}");
                Logger.LogDebug(mrex, mrex.Message);
                result = mrex.ResultCode;
            }
            catch (Exception ex)
            {
                Logger.LogError($"ERROR: {ex.Message}");
                Logger.LogDebug(ex, ex.Message);
            }
            finally
            {
                Logger.LogTrace($"Stop_{GetType().Name}");
            }

            Logger.LogDebug($"{GetType().Name} completed with exit code: {result}");
            return result;
        }

        protected abstract Task<ShiftResultCode> ExecuteAsyncOverride(
            TBaseCommandInput input,
            CancellationToken cancellationToken);
    }
}