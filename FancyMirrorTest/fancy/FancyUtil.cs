using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace FancyMirrorTest.Fancy
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
                        Console.WriteLine("Mirror was unable to map the property "+prop.Name+" because it has no route for the source class "+source.GetType().Name);
                    }
                    else
                    {
                        pairs.Add(new Tuple<MirrorAttribute, PropertyInfo>(mirror, prop));
                    }
                }
                catch (Exception e)
                {
                    if (e is NullReferenceException) throw; //we can't clone anything if the entire source object is null
                    throw new Exception("A single MirrorAttribute can only be used to map to one class");
                }
            }

            foreach (var pair in pairs)
            {
                try
                {
                    FancyMirror.MapMirror(pair.Item1, pair.Item2, source, destination);
                }
                catch (Exception e)
                {
                    HandleMirrorExceptions(e);
                }
            }
        }

        private static void HandleMirrorExceptions(Exception e)
        {
            if (e is NullReferenceException)
            {
                //not everything is going to be available to copy at all times
                throw e;
            }
            else
            {
                LogException(e);
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
                                          " because it has no route for the destination class " + destination.GetType().Name);
                    }
                    else
                    {
                        pairs.Add(new Tuple<MirrorAttribute, PropertyInfo>(mirror, prop));
                    }
                }
                catch (Exception e)
                {
                    if (e is NullReferenceException) throw; //we can't clone anything if the entire destination object is null
                    throw new Exception("A single MirrorAttribute can only be used to map to one class");
                }
            }

            foreach (var pair in pairs)
            {
                //try
                //{
                    FancyReflect.MapReflect(pair.Item1, pair.Item2, source, destination);
                /*}
                catch (Exception e)
                {
                    LogException(e);
                }*/
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
        /// Evaluates a provided property and its object to find its value.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static object GetValueOfProperty(PropertyInfo property, object o)
        {
            return property.GetValue(o, null);
        }

        /// <summary>
        /// Sets the value of property to the value of sourceProp in the context of source and destination objects.
        /// 
        /// Differences in nullability (ex int? vs int) can be handled by providing a nullSubstitute value which is
        /// a place holder for null during this transformation. This makes converting between nullable and non-nullable
        /// types easy as long as the substitute value is wisely chosen.
        /// 
        /// If a nullSubstitute value is provided and the target is not a generic type, then that value is always used
        /// for null.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="sourceProp"></param>
        /// <param name="nullSubstitute"></param>
        public static void SetValueOfProperty(PropertyInfo property, object source, object destination, PropertyInfo sourceProp, object nullSubstitute)
        {
            object srcVal = GetValueOfProperty(sourceProp, source);
            //perform null substitution
            if (nullSubstitute != null)
            {
                if (property.GetType().IsGenericType)
                {
                    /*
                     * Null substitution is intended to use the nullSubstitute value in place of null
                     * to resolve differences in the way some pieces of the system store information.
                     * 
                     * If the incoming value is null, and the property is not a nullable type, then the
                     * null substitute value is stored instead.
                     * 
                     * If the incoming value is the null substitute value and the property is a
                     * nullable type, null is used instead.
                     * 
                     * The property must be a generic type for this to work.
                     */
                    if (srcVal == null)
                    {
                        //detect if the destination is a non-nullable type
                        if (property.PropertyType.GetGenericTypeDefinition() != typeof (Nullable<>))
                        {
                            //use null substitute value
                            srcVal = nullSubstitute;
                        }
                    }
                    else if (srcVal == nullSubstitute)
                    {
                        //detect if the destination is a nullable type
                        if (property.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
                        {
                            //use null
                            srcVal = null;
                        }
                    }
                }
                else
                {
                    //Without a generic type the above does not apply. Just substitute the value every time
                    srcVal = nullSubstitute;
                }
            }
            //todo: verify type sanity
            property.SetValue(destination, srcVal); //what could possibly go wrong?
        }

        private static void LogException(Exception e)
        {
            //todo: do something more reasonable
            throw e;
        }
    }
}
