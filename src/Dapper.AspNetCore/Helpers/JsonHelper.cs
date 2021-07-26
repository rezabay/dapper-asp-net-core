using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dapper.AspNetCore.Converters;

namespace Dapper.AspNetCore.Helpers
{
    internal static class JsonHelper
    {
        private static readonly object InstanceLock = new object();
        private static JsonSerializerOptions _options;

        public static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj, GetOptions());
        }

        public static Task SerializeAsync<T>(Stream stream, T value)
        {
            return JsonSerializer.SerializeAsync(stream, value, options: GetOptions());
        }

        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json, GetOptions());
        }

        public static object Deserialize(string json, Type returnType)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize(json, returnType, GetOptions());
        }

        public static ValueTask<T> DeserializeAsync<T>(Stream stream)
        {
            return JsonSerializer.DeserializeAsync<T>(stream, options: GetOptions());
        }

        private static void SetSerializerOptions(JsonSerializerOptions options)
        {
            options.PropertyNameCaseInsensitive = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.Converters.Add(new TimeSpanConverter());
            options.Converters.Add(new DateTimeConverter());
        }

        private static JsonSerializerOptions GetOptions()
        {
            if (_options != null) return _options;

            lock (InstanceLock)
            {
                if (_options == null)
                {
                    var options = new JsonSerializerOptions();
                    SetSerializerOptions(options);
                    _options = options;
                }
            }

            return _options;
        }
    }
}