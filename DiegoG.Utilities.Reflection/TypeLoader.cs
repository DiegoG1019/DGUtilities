using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.Reflection
{
    public static class TypeLoader
    {
        private static readonly IEnumerable<Type> NoExclude = Array.Empty<Type>();
        private static Assembly[] ExecAsm => AppDomain.CurrentDomain.GetAssemblies();
        public static IEnumerable<T> InstanceTypesWithAttribute<T>(Type attr, IEnumerable<Type>? exclude, params Assembly[] assemblies) where T : class
        {
            if (!attr.IsAssignableTo(typeof(Attribute)))
                throw new ArgumentException("Type must be an attribute type", nameof(attr));
            var types = new List<T>();

            Type? curtype = null;
            try
            {
                foreach (var ty in ReflectionCollectionMethods.GetAllTypesWithAttributeInAssemblies(attr, false, assemblies.Length > 0 ? assemblies : ExecAsm).Except(exclude ?? NoExclude))
                {
                    curtype = ty;
                    types.Add((T)Activator.CreateInstance(ty)!);
                }
                return types;
            }
            catch (Exception e)
            {
                throw new TypeLoadException($"All classes that are chosen to be loaded must not be generic, abstract, or static, must have a parameterless constructor. Check inner exception for more details. Type that caused the exception: {curtype}", e);
            }
        }
        public static IEnumerable<T> InstanceTypesWithAttribute<T>(Type attr, params Assembly[] assemblies) where T : class
            => InstanceTypesWithAttribute<T>(attr, null, assemblies);
    }
}
