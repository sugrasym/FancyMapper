using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace FancyMirrorTest.fancy
{

    public static class FancyUtil
    {

        /// <summary>
        /// Using custom attributes, maps the source object's values
        /// onto the destination object. The destination object
        /// must have mirror attributes.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void Mirror(object source, object destination)
        {
            var pairs = new List<Tuple<MirrorAttribute, PropertyInfo>>();
            //find mirror attributes on the destination object
            List<PropertyInfo> destMirrorProps = GetPropertiesWithMirrors(destination).ToList();
            foreach (var prop in destMirrorProps)
            {
                try
                {
                    MirrorAttribute mirror = ExtractMirrorsFromProperty(prop).SingleOrDefault(x => x.Class == source.GetType().Name);
                    
                    if (mirror == null)
                    {
                        //this property doesn't have a mirror to this object
                        Console.WriteLine("Mirror was unable to map the property "+prop.Name+" because it has no route for the source class");
                    }
                    else
                    {
                        pairs.Add(new Tuple<MirrorAttribute, PropertyInfo>(mirror, prop));
                    }
                }
                catch (Exception)
                {
                    throw new Exception("A property cannot be mapped to more than one property in a single target class");
                }
            }

            foreach (var pair in pairs)
            {
                FancyMirror.MapMirror(pair.Item1, pair.Item2, source, destination);
            }
        }

        /// <summary>
        /// Using custom attributes, maps the source object's values
        /// onto the destination object. The source object must have
        /// MirrorAttributes.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void Reflect(object source, object destination)
        {
            var pairs = new List<Tuple<MirrorAttribute, PropertyInfo>>();
            //find mirror attributes on the source object
            List<PropertyInfo> sourceMirrorProps = GetPropertiesWithMirrors(source).ToList();
            foreach (var prop in sourceMirrorProps)
            {
                try
                {
                    MirrorAttribute mirror =
                        ExtractMirrorsFromProperty(prop).SingleOrDefault(x => x.Class == destination.GetType().Name);

                    if (mirror == null)
                    {
                        //this property doesn't have a mirror to this object
                        Console.WriteLine("Reflect was unable to map the property " + prop.Name +
                                          " because it has no route for the destination class");
                    }
                    else
                    {
                        pairs.Add(new Tuple<MirrorAttribute, PropertyInfo>(mirror, prop));
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("A property cannot be mapped to more than one property in a single target class");
                }
            }

            foreach (var pair in pairs)
            {
                FancyReflect.MapReflect(pair.Item1, pair.Item2, source, destination);
            }
        }

        /// <summary>
        /// Gets a list of all properties on the provided object
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesOnObject(object o)
        {
            PropertyInfo[] attrs = o.GetType().GetProperties();
            return attrs.ToList();
        }

        /// <summary>
        /// Finds all properties which have a mirror attribute on the provided object
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesWithMirrors(object o)
        {
            return o.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(MirrorAttribute))).ToList();
        }

        /// <summary>
        /// Get a list of all mirror attributes on the provided property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<MirrorAttribute> ExtractMirrorsFromProperty(PropertyInfo property)
        {
            List<object> attrs = property.GetCustomAttributes(false).Where(x => x is MirrorAttribute).ToList();
            List<MirrorAttribute> safe = new List<MirrorAttribute>();

            foreach (var attr in attrs)
            {
                MirrorAttribute casted = (MirrorAttribute)attr;
                safe.Add(casted);
            }

            return safe;
        }

        /// <summary>
        /// Evaluates a provided property and its object to find its value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static object GetValueOfProperty(PropertyInfo property, object o)
        {
            return property.GetValue(o, null);
        }

        public static void SetValueOfProperty(PropertyInfo property, object source, object destination, PropertyInfo sourceProp)
        {
            //todo: verify type sanity
            object srcVal = GetValueOfProperty(sourceProp, source);
            property.SetValue(destination, srcVal); //what could possibly go wrong?
        }
    }
}
