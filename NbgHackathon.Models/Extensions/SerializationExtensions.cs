using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbgHackathon.Models
{
    internal static class SerializationExtensions
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T ReadObject<T>(this IDictionary<string, EntityProperty> properties, string prefix) where T : new()
        {
            var keyPrefix = $"{prefix}_";
            var nestedProperties = properties.Where(kvp => kvp.Key.StartsWith(keyPrefix))
                .ToDictionary(kvp => kvp.Key.Replace(keyPrefix, string.Empty), kvp => kvp.Value);

            if (!nestedProperties.Any())
                return default(T);

            var instance = TableEntity.ConvertBack<T>(nestedProperties, null);
            return instance;
        }

        public static TEnum? ReadEnum<TEnum>(this IDictionary<string, EntityProperty> properties, string key) where TEnum : struct
        {
            if (!properties.TryGetValue(key, out EntityProperty property) || !Enum.TryParse<TEnum>(property.StringValue, out TEnum value))
            {
                return default(TEnum?);
            }

            return value;
        }
    }
}
