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

        public static IEnumerable<T> InstanceTypesWithAttribute<T>(Type attr, Func<Type, Attribute[], bool>? validator, IEnumerable<Type>? exclude, params Assembly[] assemblies) where T : class
        {
            if (!attr.IsAssignableTo(typeof(Attribute)))
                throw new ArgumentException("Type must be an attribute type", nameof(attr));
            var types = new List<T>();

            Func<Type, Attribute[], bool> isvalid = validator ?? ((x,y) => true);

            Type? curtype = null;
            try
            {
                foreach (var ty in ReflectionCollectionMethods.GetAllTypesAndAttributeInstanceTupleFromAssembly(attr, false, assemblies.Length > 0 ? assemblies : ExecAsm)
                    .Where(x => (exclude ?? NoExclude).All(y => y != x.Type)))
                {
                    curtype = ty.Type;
                    if (isvalid(ty.Type, ty.Attributes))
                        types.Add((T)Activator.CreateInstance(ty.Type)!);
                }
                return types;
            }
            catch (Exception e)
            {
                throw new TypeLoadException($"All classes that are chosen to be loaded must not be generic, abstract, or static, must have a parameterless constructor. Check inner exception for more details. Type that caused the exception: {curtype}", e);
            }
        }

        public static IEnumerable<T> InstanceTypesWithAttribute<T>(Type attr, IEnumerable<Type>? exclude, params Assembly[] assemblies) where T : class
            => InstanceTypesWithAttribute<T>(attr, null, exclude, assemblies);

        public static IEnumerable<T> InstanceTypesWithAttribute<T>(Type attr, Func<Type, Attribute[], bool>? validator, params Assembly[] assemblies) where T : class
            => InstanceTypesWithAttribute<T>(attr, validator, null, assemblies);

        public static IEnumerable<T> InstanceTypesWithAttribute<T>(Type attr, params Assembly[] assemblies) where T : class
            => InstanceTypesWithAttribute<T>(attr, null, null, assemblies);
    }
}
