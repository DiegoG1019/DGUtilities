﻿using DiegoG.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.Settings
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FromEnvironmentVariable : Attribute
    {
        private static readonly Dictionary<Type, Converter<string?, object?>> Converters = new()
        {
            { typeof(bool), x => (x = x?.ToLower()) is null
                            ? null
                            : x is "yes" or "true" or "enabled" ? true : x is "no" or "false" or "disabled" ? false : null
            }
        };

        public static Converter<string?, object?> AddConverter(Type type, Converter<string?, object?> converter) => Converters[type] = converter;
        private static Converter<string?, object?> GetConverter(Type type) => Converters.GetValueOrDefault(type) ?? AddConverter(type, new DefaultConverter(type).Convert);

        private class DefaultConverter 
        {
            private readonly Type Type;
            public DefaultConverter(Type type) => Type = type;
            public object? Convert(string? value) => System.Convert.ChangeType(value, Type);
        }

        public string VariableName { get; private set; }
        
        public bool IsJson { get; private set; }
        
        public EnvironmentVariableTarget EnvironmentVariableTarget { get; set; } = EnvironmentVariableTarget.Process;
        
        /// <summary>
        /// Denotes that this attribute is required and should throw if not found. Defaults to <see cref="true"/>
        /// </summary>
        public bool Required { get; set; } = true;

        /// <summary>
        /// Denotes that this attribute takes precedence over non-null reference types, does nothing for value types. Defaults to <see cref="false"/>
        /// </summary>
        public bool Precedence { get; set; } = false;

        public object? FetchValue(Type type)
            => IsJson ? GetJson(type) : GetValue(type);

        private string? GetVariable() => Environment.GetEnvironmentVariable(VariableName, EnvironmentVariableTarget);

        private object? GetJson(Type type)
        {
            string? val = GetVariable();
            return val is null ? null : Serialization.Deserialize.Json(val, type);
        }

        private object? GetValue(Type type)
        {
            string? val = GetVariable();
            return val is null
                ? null
                : GetConverter(type)?.Invoke(val) ?? throw new InvalidOperationException($"Could not convert {VariableName} with value {val ?? "null"} to {type?.Name ?? "null"}");
        }

        public FromEnvironmentVariable([CallerMemberName]string? variableName = null, bool isJson = false)
        {
            VariableName = variableName ?? throw new InvalidOperationException("VariableName cannot be null. Either don't pass an argument for this variable, or provide a valid variable name");
            IsJson = isJson;
        }

        public FromEnvironmentVariable(bool isJson, [CallerMemberName] string? variableName = null)
            : this(variableName, isJson) { }
    }
}
