using System;
using System.Reflection;

namespace FancyMirrorTest.Fancy
{
    /// <summary>
    /// Resolves object reference issues
    /// </summary>
    public static class FancyResolver
    {
        /// <summary>
        /// Instantiates a child object nested inside the non-null parent object so that the child
        /// model's values can be reflected into the child object.
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
                var o = Activator.CreateInstance(t.PropertyType); //I hope it has a parameterless constructor
                var p = parent.GetType().GetProperty(t.Name);
                p.SetValue(parent, o);
                sp = FancyUtil.GetValueOfProperty(sourceProp.Item1, sourceProp.Item2);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
