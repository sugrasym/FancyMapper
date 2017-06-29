using System;
using System.Collections.Generic;
using System.Reflection;

namespace Fancy
{
    /// <summary>
    ///     Resolves object reference issues
    /// </summary>
    public static class FancyResolver
    {
        /// <summary>
        ///     Instantiates a child object nested inside the non-null parent object so that the child
        ///     model's values can be reflected into the child object.
        /// </summary>
        /// <param name="sourceProp"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static bool ResolveNullDestination(Tuple<PropertyInfo, object> sourceProp, ref object sp)
        {
            try
            {
                var t = sourceProp.Item1;
                var parent = sourceProp.Item2;
                //It isn't possible to reflect properties into a null object, so it needs to be instantiated
                CreateInstance(t, parent);
                //Re-evaluate
                sp = FancyUtil.GetValueOfProperty(sourceProp.Item1, sourceProp.Item2);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Instantiates the property on the parent object from the provided
        ///     property info type.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="parent"></param>
        public static void CreateInstance(PropertyInfo t, object parent)
        {
            var o = Activator.CreateInstance(t.PropertyType); //I hope it has a parameterless constructor
            var p = parent.GetType().GetProperty(t.Name);
            p.SetValue(parent, o);
        }

        /// <summary>
        ///     Creates a new instance of the provided type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        /// <summary>
        ///     Instantiates the property on the parent object from the provided
        ///     property info type as an IEnumerable.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="parent"></param>
        public static void CreateListInstance(PropertyInfo t, object parent)
        {
            var p = parent.GetType().GetProperty(t.Name);
            var listType = typeof(List<>);
            var genericArgs = p.PropertyType.GetGenericArguments();
            var concreteType = listType.MakeGenericType(genericArgs);
            var newList = Activator.CreateInstance(concreteType);
            p.SetValue(parent, newList);
        }
    }
}