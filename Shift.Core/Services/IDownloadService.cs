using System.Threading.Tasks;
using Shift.Core.Models.Common;

namespace Shift.Core.Services
{
    public interface IDownloadService
    {
        Task<ShiftResultCode> DownloadAsync(
            string bundle,
            string manifestPath,
            string stagingDirectory = null,
            string adoPat = null);

        Task<ShiftResultCode> DownloadAsync(
            string bundle,
            string packageName,
            string organization,
            string project,
            string feed,
            string stagingDirectory = null,
            string adoPat = null);

        Task<ShiftResultCode> DownloadAsync(
            string[] components,
            string[] versions,
            string manifestPath,
            string stagingDirectory = null,
            string adoPat = null);

        Task<ShiftResultCode> DownloadAsync(
            string[] components,
            string[] versions,
            string packageName,
            string organization,
            string project,
            string feed,
            string stagingDirectory = null,
            string adoPat = null);
    }
}