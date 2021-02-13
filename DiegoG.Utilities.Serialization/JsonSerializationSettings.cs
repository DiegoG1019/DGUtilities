using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiegoG.Utilities.IO
{
    public static class JsonSerializationSettings
    {
        public static void Init()
        {
            Log.Debug("Initializing JsonSerializationSettings");
            Log.Verbose("Adding a new instance of Converter factory DictionaryTKeyEnumTValueConverter");
            JsonSerializerOptions.Converters.Add(new DictionaryTKeyEnumTValueConverter());
            JsonSerializerOptions.WriteIndented = true;
        }

        public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions();

        public class CallbacksJsonConverter<T> : JsonConverter<T>
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                T obj = (T)JsonSerializer.Deserialize(ref reader, typeToConvert, options);
                var methods = from method in typeof(T).GetMethods() where method.GetCustomAttributes(typeof(OnDeserializedAttribute), false).FirstOrDefault() != null select method;
                foreach (var method in methods)
                {
                    method.Invoke(obj, null);
                }

                return obj;
            }
            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                var methods = from method in typeof(T).GetMethods() where method.GetCustomAttributes(typeof(OnSerializedAttribute), false).FirstOrDefault() != null select method;
                foreach (var method in methods)
                {
                    method.Invoke(value, null);
                }

                JsonSerializer.Serialize(writer, value, options);
            }
        }

        /// <summary>
        /// Register the given type T to serialize and deserialize according to the given callbacks
        /// </summary>
        /// <typeparam name="T">The type to register</typeparam>
        public static void RegisterClassCallbacksJsonConverter<T>()
        {
            Log.Verbose($"Adding a new instance of Converter CallbacksJsonConverter of type {typeof(T).Name} ({typeof(T).FullName})");
            JsonSerializerOptions.Converters.Add(new CallbacksJsonConverter<T>());
        }

        /// <summary>
        /// Code taken from https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?view=netcore-3.1#support-dictionary-with-non-string-key
        /// </summary>
        public class DictionaryTKeyEnumTValueConverter : JsonConverterFactory
        {
            public override bool CanConvert(Type typeToConvert)
            {
                if (!typeToConvert.IsGenericType)
                {
                    return false;
                }

                if (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                {
                    return false;
                }

                return typeToConvert.GetGenericArguments()[0].IsEnum;
            }
            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                Type keyType = typeToConvert.GetGenericArguments()[0];
                Type valueType = typeToConvert.GetGenericArguments()[1];

                JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                    typeof(DictionaryEnumConverterInner<,>).MakeGenericType(
                        new Type[] { keyType, valueType }),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: new object[] { options },
                    culture: null);

                return converter;
            }
            private class DictionaryEnumConverterInner<TKey, TValue> : JsonConverter<IDictionary<TKey, TValue>> where TKey : struct, Enum
            {
                private readonly JsonConverter<TValue> _valueConverter;
                private readonly Type _keyType;
                private readonly Type _valueType;

                public DictionaryEnumConverterInner(JsonSerializerOptions options)
                {
                    // For performance, use the existing converter if available.
                    _valueConverter = (JsonConverter<TValue>)options
                        .GetConverter(typeof(TValue));

                    // Cache the key and value types.
                    _keyType = typeof(TKey);
                    _valueType = typeof(TValue);
                }

                public override IDictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                {
                    if (reader.TokenType != JsonTokenType.StartObject)
                    {
                        throw new JsonException();
                    }

                    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            return dictionary;
                        }

                        // Get the key.
                        if (reader.TokenType != JsonTokenType.PropertyName)
                        {
                            throw new JsonException();
                        }

                        string propertyName = reader.GetString();

                        // For performance, parse with ignoreCase:false first.
                        if (!Enum.TryParse(propertyName, ignoreCase: false, out TKey key) && !Enum.TryParse(propertyName, ignoreCase: true, out key))
                        {
                            throw new JsonException($"Unable to convert \"{propertyName}\" to Enum \"{_keyType}\".");
                        }

                        // Get the value.
                        TValue v;
                        if (_valueConverter != null)
                        {
                            reader.Read();
                            v = _valueConverter.Read(ref reader, _valueType, options);
                        }
                        else
                        {
                            v = JsonSerializer.Deserialize<TValue>(ref reader, options);
                        }

                        // Add to dictionary.
                        dictionary.Add(key, v);
                    }
                    throw new JsonException();
                }

                public override void Write(Utf8JsonWriter writer, IDictionary<TKey, TValue> dictionary, JsonSerializerOptions options)
                {
                    writer.WriteStartObject();

                    foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
                    {
                        writer.WritePropertyName(kvp.Key.ToString());
                        if (_valueConverter != null)
                        {
                            _valueConverter.Write(writer, kvp.Value, options);
                        }
                        else
                        {
                            JsonSerializer.Serialize(writer, kvp.Value, options);
                        }
                    }

                    writer.WriteEndObject();
                }
            }
        }

    }
}
