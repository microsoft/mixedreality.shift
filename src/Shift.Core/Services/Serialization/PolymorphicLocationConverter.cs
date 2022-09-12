// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shift.Core.Contracts.Manifests;

namespace Shift.Core.Services.Serialization
{
    public class PolymorphicLocationConverter : JsonConverter
    {
        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(LocationV1).IsAssignableFrom(objectType);
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

            LocationV1 item;
            if (jo.ContainsKey("path") || jo.ContainsKey("Path"))
            {
                item = new FolderLocationV1();
            }
            else
            {
                item = new PackageLocationV1();
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