// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shift.Core.Contracts.Manifests.Tasks;
using Shift.Core.Services.Manifests.Tasks;

namespace Shift.Core.Services.Serialization
{
    public class PolymorphicTaskInfoConverter : JsonConverter
    {
        private readonly IComponentTaskProvider _componentTaskProvider;

        public PolymorphicTaskInfoConverter(IComponentTaskProvider componentTaskProvider)
        {
            _componentTaskProvider = componentTaskProvider;
        }

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(TaskInfoV1).IsAssignableFrom(objectType);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jo = JObject.Load(reader);
            var taskType = jo["type"]?.ToString() ?? jo["Type"].ToString();

            var task = _componentTaskProvider.GetComponentTask(taskType);
            object item;
            if (task == null)
            {
                item = new TaskInfoV1();
            }
            else
            {
                item = Activator.CreateInstance(task.ContractType);
            }

            serializer.Populate(jo.CreateReader(), item);

            return item;
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            var o = JToken.FromObject(value);
            o.WriteTo(writer);
        }
    }
}