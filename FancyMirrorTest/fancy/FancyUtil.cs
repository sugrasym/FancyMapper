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
        /// onto the destination object.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void Mirror(object source, object destination)
        {
            //get a list of properties on the source object
            IEnumerable<PropertyInfo> sourceProperties = GetPropertiesOnObject(source);

            //get a list of properties on the destination object
            IEnumerable<PropertyInfo> destinationProperties = GetPropertiesOnObject(destination);

            //debug
            Console.WriteLine("\n\nFound source");
            foreach (var sourceProperty in sourceProperties)
            {
                Console.WriteLine(GetValueOfProperty(sourceProperty, source));
            }

            //debug
            IEnumerable<PropertyInfo> ptr = GetPropertiesWithMirrors(destination);
            Console.WriteLine("\n\nFound dest");
            foreach (var destinationProperty in ptr)
            {
                Console.WriteLine(ExtractMirrorsFromProperty(destinationProperty).First().Path);
            }

            //find mirror attributes on the destination object
        }

        public static void Reflect(object source, object destination)
        {

        }

        /// <summary>
        /// Provided a mirror attribute, source value, and destination object, this will map the
        /// value onto the appropriate property in the destination
        /// </summary>
        /// <param name="mirror"></param>
        /// <param name="sourceVal"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private static void MapMirror(MirrorAttribute mirror, object sourceVal, object destination)
        {
            string[] route = mirror.Path.Split('.');
            //the first element is always the class
            string className = route[0];
            //the second element is the property (todo: use recursion to handle chaining properties ex Person.Address.StreetName)
            string propName = route[1];
            //verify the type of the target object
            if (destination.GetType().Name == className)
            {
                //get a list of properties on the target object
                List<PropertyInfo> props = GetPropertiesOnObject(destination).ToList();
                //find the one that matches our property name
                PropertyInfo targetProp = props.SingleOrDefault(x => x.Name == propName);
                if (targetProp == null)
                {
                    throw new Exception(
                        "The property specified by this MirrorAttribute does not exist on the destination object");
                }
                else
                {
                    
                }
            }
            else
            {
                throw new Exception("A single MirrorAttribute can only be used to map to one class");
            }
        }

        /// <summary>
        /// Gets a list of all properties on the provided object
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetPropertiesOnObject(object o)
        {
            PropertyInfo[] attrs = o.GetType().GetProperties();
            return attrs.ToList();
        }

        /// <summary>
        /// Finds all properties which have a mirror attribute on the provided object
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetPropertiesWithMirrors(object o)
        {
            return o.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(MirrorAttribute))).ToList();
        }

        /// <summary>
        /// Get a list of all mirror attributes on the provided property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static IEnumerable<MirrorAttribute> ExtractMirrorsFromProperty(PropertyInfo property)
        {
            List<object> attrs = property.GetCustomAttributes(false).Where(x => x is MirrorAttribute).ToList();
            List<MirrorAttribute> safe = new List<MirrorAttribute>();

            foreach (var attr in attrs)
            {
                MirrorAttribute casted = (MirrorAttribute) attr;
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
        private static object GetValueOfProperty(PropertyInfo property, object o)
        {
            return property.GetValue(o, null);
        }

    }
}
