/*
 * Copyright (C) 2014 Nathan Wiehoff
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 *   IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            string srcTypeName = AttemptToDeproxyName(source);

            foreach (var prop in destMirrorProps)
            {
                try
                {
                    MirrorAttribute mirror = ExtractMirrorsFromProperty(prop).SingleOrDefault(x => x.Class == srcTypeName);

                    if (mirror == null)
                    {
                        //this property doesn't have a mirror to this object
                        Console.WriteLine("Mirror was unable to map the property " + prop.Name + " because it has no route for the source class " + source.GetType().Name);
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
                //todo: determine if these should be silently ignored at this stage - YEP
                //not everything is going to be available to copy at all times
                //throw e;
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

            var destTypeName = AttemptToDeproxyName(destination);
            foreach (var prop in sourceMirrorProps)
            {
                try
                {
                    MirrorAttribute mirror =
                        ExtractMirrorsFromProperty(prop).SingleOrDefault(x => x.Class == destTypeName);

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
        /// Attempts to use the rule that a proxied class has a name in the format of
        /// REALTYPE_garbage where we want REALTYPE.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string AttemptToDeproxyName(object source)
        {
            var entityType = source.GetType();
            string rawName = source.GetType().Name;
            if (entityType.Namespace != null && entityType.Namespace.Contains("System.Data.Entity.DynamicProxies"))
            {
                return entityType.BaseType.Name;
            }
            return rawName;
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
        /// a place holder for null during this transformation. When a null is encountered, null substitute is used. 
        /// When the null substitute is encountered, null is used.
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
                //substitute the value every time
                if (srcVal == null)
                {
                    srcVal = nullSubstitute;
                }
                else if (srcVal.Equals(nullSubstitute))
                {
                    srcVal = null;
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
