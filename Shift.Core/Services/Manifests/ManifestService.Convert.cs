// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Shift.Core.Contracts.Manifests;
using Shift.Core.Contracts.Manifests.Tasks;
using Shift.Core.Models.Manifests;
using Shift.Core.Models.Manifests.Tasks;

namespace Shift.Core.Services.Manifests
{
    /// <summary>
    /// Implements the <see cref="IManifestService"/>.
    /// </summary>
    public partial class ManifestService : IManifestService
    {
        /// <summary>
        /// Converts the stream of bytes into the manifest object
        /// </summary>
        /// <param name="manifestBytes">Stream of bytes of manifest information</param>
        /// <returns>Manifest object</returns>
        public Manifest ConvertBytesToManifest(byte[] manifestBytes)
        {
            var json = Encoding.UTF8.GetString(manifestBytes);

            return Convert(JsonConvert.DeserializeObject<ManifestV1>(json, _converters));
        }

        /// <summary>
        /// Converts the stream of bytes into the manifest object
        /// </summary>
        /// <param name="manifestBytes">Stream of bytes of manifest information</param>
        /// <returns>Manifest object</returns>
        public ManifestPromotionCriteria ConvertBytesToManifestPromotionCriteria(byte[] manifestBytes)
        {
            var json = Encoding.UTF8.GetString(manifestBytes);

            return Convert(JsonConvert.DeserializeObject<ManifestPromotionCriteriaV1>(json, _converters));
        }

        /// <summary>
        /// Converts the manifest object into stream of bytes
        /// </summary>
        /// <param name="targetManifest">Manifest object to convert</param>
        /// <returns>Converted bytes</returns>
        public byte[] ConvertManifestPromotionCriteriaToBytes(ManifestPromotionCriteria targetManifest)
        {
            var contract = Convert(targetManifest);
            var json = JsonConvert.SerializeObject(contract, Formatting.Indented, _converters);

            return Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// Converts the manifest object into stream of bytes
        /// </summary>
        /// <param name="targetManifest">Manifest object to convert</param>
        /// <returns>Converted bytes</returns>
        public byte[] ConvertManifestToBytes(Manifest targetManifest)
        {
            var contract = Convert(targetManifest);
            var json = JsonConvert.SerializeObject(contract, Formatting.Indented, _converters);

            return Encoding.UTF8.GetBytes(json);
        }
        private ManifestV1 Convert(Manifest model)
        {
            return new ManifestV1
            {
                Version = model.Version,
                Bundles = model.Bundles?.ConvertAll(x => (ComponentBundleV1)x),
                Components = model.Components?.ConvertAll(x => new ComponentV1
                {
                    Description = x.Description,
                    Id = x.Id,
                    Owner = x.Owner,
                    DeviceDemands = x.DeviceDemands.ToArray(),
                    Location = x.Location,
                    Task = Convert(x.Task),
                })
            };
        }

        private TaskInfoV1 Convert(TaskInfo model)
        {
            if (model == null)
            {
                return null;
            }
            else
            {
                var task = _taskProvider.GetComponentTask(model.Type);
                return task.ConvertToContract(model) as TaskInfoV1;
            }
        }

        private Manifest Convert(ManifestV1 contract)
        {
            return new Manifest
            {
                Version = contract.Version,
                Bundles = contract.Bundles?.ConvertAll(x => (ComponentBundle)x),
                Components = contract.Components?.ConvertAll(x => new Component
                {
                    Description = x.Description,
                    Id = x.Id,
                    Owner = x.Owner,
                    DeviceDemands = x.DeviceDemands.ToArray(),
                    Location = x.Location,
                    Task = x.Task != null ? Convert(x.Task) : null,
                })
            };
        }

        private TaskInfo Convert(TaskInfoV1 contract)
        {
            var task = _taskProvider.GetComponentTask(contract.Type);
            return task.ConvertToModel(contract) as TaskInfo;
        }

        private ManifestPromotionCriteriaV1 Convert(ManifestPromotionCriteria mpc)
        {
            return new ManifestPromotionCriteriaV1
            {
                Components = mpc?.Components.ToDictionary(x => x.Key, x => new PromotionCriteriaV1
                {
                    Filter = x.Value.Filter.ToString(),
                    Strategy = x.Value.Strategy
                })
            };
        }

        private ManifestPromotionCriteria Convert(ManifestPromotionCriteriaV1 mpc)
        {
            return new ManifestPromotionCriteria
            {
                Components = mpc?.Components.ToDictionary(x => x.Key, x => new PromotionCriteria
                {
                    Filter = new Regex(x.Value.Filter),
                    Strategy = x.Value.Strategy,
                    RequiredViews = x.Value.RequiredViews?.ToArray(),
                })
            };
        }
    }
}