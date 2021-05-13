using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#pragma warning disable CS8619
#pragma warning disable CS8600

namespace DiegoG.Utilities.Reflection
{
    /// <summary>
    /// A class full of methods to effectively and easily create collections using reflection
    /// </summary>
    public static class ReflectionCollectionMethods<Ttype>
    {
        private static readonly Type typeinfo = typeof(Ttype);
        private static readonly string? typenamespace = ((TypeInfo)typeinfo).Namespace;

        public static IEnumerable<PropertyInfo> GetAllStaticProperties() => ReflectionCollectionMethods.GetAllStaticProperties(typeinfo);

        public static IEnumerable<PropertyInfo> GetAllInstanceProperties() => ReflectionCollectionMethods.GetAllInstanceProperties(typeinfo);

        public static IEnumerable<object> GetAllStaticPropertyValues() => ReflectionCollectionMethods.GetAllStaticPropertyValues(typeinfo);

        public static IEnumerable<object> GetAllInstancePropertyValues(Ttype instance) => ReflectionCollectionMethods.GetAllInstancePropertyValues(typeinfo, instance!);

        public static IEnumerable<Tout> GetAllMatchingTypeStaticPropertyValues<Tout>() => ReflectionCollectionMethods.GetAllMatchingTypeStaticPropertyValues<Tout>(typeinfo);

        public static IEnumerable<Tout> GetAllMatchingTypeInstancePropertyValues<Tout>(Ttype instance) => ReflectionCollectionMethods.GetAllMatchingTypeInstancePropertyValues<Tout>(typeinfo, instance!);

        public static IEnumerable<(string Name, object Value)> GetAllStaticPropertyNameValueTuple() => ReflectionCollectionMethods.GetAllStaticPropertyNameValueTuple(typeinfo);

        public static IEnumerable<(string Name, object Value)> GetAllInstancePropertyNameValueTuple(Ttype instance) => ReflectionCollectionMethods.GetAllInstancePropertyNameValueTuple(typeinfo, instance!);

        public static IEnumerable<(string Name, Tout Value)> GetAllMatchingTypeStaticPropertyNameValueTuple<Tout>() => ReflectionCollectionMethods.GetAllMatchingTypeStaticPropertyNameValueTuple<Tout>(typeinfo);

        public static IEnumerable<(string Name, Tout Value)> GetAllMatchingTypeInstancePropertyNameValueTuple<Tout>(Ttype instance) => ReflectionCollectionMethods.GetAllMatchingTypeInstancePropertyNameValueTuple<Tout>(typeinfo, instance!);

        //
        public static IEnumerable<PropertyInfo> GetAllStaticPropertiesOfAllNamespaceClasses() => ReflectionCollectionMethods.GetAllStaticPropertiesOfAllNamespaceClasses(typenamespace!);

        //
        public static IEnumerable<object> GetAllStaticPropertyValuesOfAllNamespaceClasses() => ReflectionCollectionMethods.GetAllStaticPropertyValuesOfAllNamespaceClasses(typenamespace!);

        //
        public static IEnumerable<Tout> GetAllMatchingTypeStaticPropertyValuesOfAllNamespaceClasses<Tout>() => ReflectionCollectionMethods.GetAllMatchingTypeStaticPropertyValuesOfAllNamespaceClasses<Tout>(typenamespace!);

        //
        public static IEnumerable<(string Name, object Value)> GetAllStaticPropertyNameValueTupleOfAllNamespaceClasses() => ReflectionCollectionMethods.GetAllStaticPropertyNameValueTupleOfAllNamespaceClasses(typenamespace!);

        //
        public static IEnumerable<(string Name, Tout Value)> GetAllStaticMatchingTypePropertyNameValueTupleOfAllNamespaceClasses<Tout>() => ReflectionCollectionMethods.GetAllStaticMatchingTypePropertyNameValueTupleOfAllNamespaceClasses<Tout>(typenamespace!);

        public static IEnumerable<(PropertyInfo Property, IEnumerable<Attribute> Attributes)> GetAllInstancePropertiesWithAttribute(Type attribute) => ReflectionCollectionMethods.GetAllInstancePropertiesWithAttribute(typeinfo, attribute);
    }
    public static class ReflectionCollectionMethods
    {
        public static IEnumerable<PropertyInfo> GetAllStaticProperties(TypeInfo typeinfo) => typeinfo.GetProperties(BindingFlags.Public | BindingFlags.Static);

        public static IEnumerable<PropertyInfo> GetAllInstanceProperties(TypeInfo typeinfo) => typeinfo.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //
        public static IEnumerable<PropertyInfo> GetAllInstanceProperties(Type typeinfo) => GetAllInstanceProperties((TypeInfo)typeinfo);

        public static IEnumerable<PropertyInfo> GetAllStaticProperties(Type typeinfo) => GetAllStaticProperties((TypeInfo)typeinfo);

        //
        public static IEnumerable<object> GetAllStaticPropertyValues(Type typeinfo) => GetAllStaticProperties(typeinfo).Select(d => d.GetValue(null));

        public static IEnumerable<object> GetAllInstancePropertyValues(Type typeinfo, object instance) => GetAllInstanceProperties(typeinfo).Select(d => d.GetValue(instance));

        //
        public static IEnumerable<Tout> GetAllMatchingTypeStaticPropertyValues<Tout>(Type typeinfo) => GetAllStaticProperties(typeinfo).Where(p => p.PropertyType == typeof(Tout)).Select(d => d.GetValue(null)).Cast<Tout>();

        public static IEnumerable<Tout> GetAllMatchingTypeInstancePropertyValues<Tout>(Type typeinfo, object instance) => GetAllInstanceProperties(typeinfo).Where(p => p.PropertyType == typeof(Tout)).Select(d => d.GetValue(instance)).Cast<Tout>();

        //
        public static IEnumerable<(string Name, object Value)> GetAllStaticPropertyNameValueTuple(Type typeinfo) => GetAllStaticProperties(typeinfo).Select(d => (d.Name, d.GetValue(null)));

        public static IEnumerable<(string Name, object Value)> GetAllInstancePropertyNameValueTuple(Type typeinfo, object instance) => GetAllInstanceProperties(typeinfo).Select(d => (d.Name, d.GetValue(instance)));

        //
        public static IEnumerable<(string Name, Tout Value)> GetAllMatchingTypeStaticPropertyNameValueTuple<Tout>(Type typeinfo) => from item in GetAllStaticProperties(typeinfo) where item.PropertyType == typeof(Tout) select (item.Name, (Tout)item.GetValue(null));

        public static IEnumerable<(string Name, Tout Value)> GetAllMatchingTypeInstancePropertyNameValueTuple<Tout>(Type typeinfo, object instance) => from item in GetAllInstanceProperties(typeinfo) where item.PropertyType == typeof(Tout) select (item.Name, (Tout)item.GetValue(instance));

        //
        public static IEnumerable<PropertyInfo> GetAllStaticPropertiesOfAllNamespaceClasses(string @namespace) => Assembly.GetExecutingAssembly().GetTypes().Where(w => w.IsClass && w.Namespace == @namespace).SelectMany(d => GetAllStaticProperties(d));

        //
        public static IEnumerable<object> GetAllStaticPropertyValuesOfAllNamespaceClasses(string @namespace) => Assembly.GetExecutingAssembly().GetTypes().Where(w => w.IsClass && w.Namespace == @namespace).SelectMany(d => GetAllStaticPropertyValues(d));

        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0038:Use pattern matching", Justification = "Scope problems")]
        public static IEnumerable<Tout> GetAllMatchingTypeStaticPropertyValuesOfAllNamespaceClasses<Tout>(string @namespace) => from item in GetAllStaticPropertyValuesOfAllNamespaceClasses(@namespace) where item is Tout select (Tout)item;

        //
        public static IEnumerable<(string Name, object Value)> GetAllStaticPropertyNameValueTupleOfAllNamespaceClasses(string @namespace) => from item in GetAllStaticPropertiesOfAllNamespaceClasses(@namespace) select (item.Name, item.GetValue(null));

        //
        public static IEnumerable<(string Name, Tout Value)> GetAllStaticMatchingTypePropertyNameValueTupleOfAllNamespaceClasses<Tout>(string @namespace) => from item in GetAllStaticPropertiesOfAllNamespaceClasses(@namespace) where item is Tout select (item.Name, (Tout)item.GetValue(null));

        public static IEnumerable<(PropertyInfo Property, IEnumerable<Attribute> Attributes)> GetAllInstancePropertiesWithAttribute(Type typeinfo, Type attribute) => from prop in GetAllInstanceProperties(typeinfo) where Attribute.IsDefined(prop, attribute) select (prop, prop.GetCustomAttributes(attribute));

        public static IEnumerable<(PropertyInfo Property, IEnumerable<Attribute> Attributes)> GetAllStaticPropertiesWithAttribute(Type typeinfo, Type attribute) => from prop in GetAllStaticProperties(typeinfo) where Attribute.IsDefined(prop, attribute) select (prop, prop.GetCustomAttributes(attribute));

        public static IEnumerable<Type> GetAllTypesWithAttribute(Type attribute, bool inherit) => GetAllTypesWithAttributeInAssemblies(attribute, inherit, AppDomain.CurrentDomain.GetAssemblies());

        public static IEnumerable<(Type Type, Attribute[] Attributes)> GetAllTypesAndAttributeInstanceTuple(Type attribute, bool inherit) => GetAllTypesAndAttributeInstanceTupleFromAssembly(attribute, inherit, AppDomain.CurrentDomain.GetAssemblies());

        public static IEnumerable<(Type Type, Attribute[] Attributes)> GetAllTypesAndAttributeInstanceTupleFromAssembly(Type attribute, bool inherit, Assembly[] asm) => from a in asm
                                                                                                                                                                         from t in a.GetTypes()
                                                                                                                                                                         let attributes = t.GetCustomAttributes(attribute, inherit)
                                                                                                                                                                         where attributes != null && attributes.Length > 0
                                                                                                                                                                         select (t, attributes as Attribute[]);

        public static IEnumerable<Type> GetAllTypesWithAttributeInAssemblies(Type attribute, bool inherit, params Assembly[] asm) => from a in asm
                                                                                                                                     from t in a.GetTypes()
                                                                                                                                     let attributes = t.GetCustomAttributes(attribute, inherit)
                                                                                                                                     where attributes != null && attributes.Length > 0
                                                                                                                                     select t;
    }
}
