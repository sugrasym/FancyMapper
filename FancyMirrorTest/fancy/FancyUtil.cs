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
            var pairs = new List<Tuple<MirrorAttribute, PropertyInfo>>();
            //find mirror attributes on the destination object
            List<PropertyInfo> destMirrorProps = GetPropertiesWithMirrors(destination).ToList();
            foreach (var prop in destMirrorProps)
            {
                try
                {
                    MirrorAttribute mirror =
                        ExtractMirrorsFromProperty(prop).SingleOrDefault(x => x.Class == source.GetType().Name);
                    
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
                MapMirror(pair.Item1, pair.Item2, source, destination);
            }
        }

        public static void Reflect(object source, object destination)
        {

        }

        /// <summary>
        /// Maps the provided property with the value of the property in the source object that is referenced
        /// using the path in the mirror.
        /// </summary>
        /// <param name="mirror"></param>
        /// <param name="property"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private static void MapMirror(MirrorAttribute mirror, PropertyInfo property, object source, object destination)
        {
            string[] route = mirror.Path.Split('.');
            //the first element is always the class
            string className = mirror.Class;
            //the second element is the property
            string propName = route[1];
            //verify the type of the target object
            if (source.GetType().Name == className)
            {
                if (route.Length == 2)
                {
                    //get a list of properties on the source object
                    List<PropertyInfo> srcProps = GetPropertiesOnObject(source).ToList();
                    //find the one that matches our property name
                    PropertyInfo sourceProp = srcProps.SingleOrDefault(x => x.Name == propName);
                    if (sourceProp == null)
                    {
                        throw new Exception(
                            "The property specified by this MirrorAttribute does not exist on the source object");
                    }
                    else
                    {
                        SetValueOfProperty(property, source, destination, sourceProp);
                    }
                }
                else
                {
                    //must be solved using recursion
                    var sourceProp = RecursiveRouteMirror(route, 1, 10, source);
                    SetValueOfProperty(property, sourceProp.Item2 , destination, sourceProp.Item1);
                }
            }
            else
            {
                throw new Exception("A single MirrorAttribute can only be used to map to one class");
            }
        }

        private static void SetValueOfProperty(PropertyInfo property, object source, object destination, PropertyInfo sourceProp)
        {
            //todo: verify type sanity
            object srcVal = GetValueOfProperty(sourceProp, source);
            property.SetValue(destination, srcVal); //what could possibly go wrong?
        }

        /// <summary>
        /// Walks down the route recursively locating properties until the end point is reached, and then returns
        /// that final property.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="routeIndex"></param>
        /// <param name="limit"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Tuple<PropertyInfo, object> RecursiveRouteMirror(string[] route, int routeIndex, int limit, object source)
        {
            if (routeIndex > limit)
            {
                throw new Exception("Aborted recursive routing, will not attempt to use an index greater than "+limit);
            }

            //get the current route position
            string current = route[routeIndex];
            //get list of properties on source
            List<PropertyInfo> props = GetPropertiesOnObject(source).ToList();
            //select the one that matches this step of the route
            PropertyInfo prop = props.SingleOrDefault(x => x.Name == current);
            if (prop == null)
            {
                throw new Exception(
                    "The route provided by this MirrorAttribute is not a valid path to the target property");
            }
            else
            {
                //is this the end of the chain?
                if (route.Length - 1 == routeIndex)
                {
                    return new Tuple<PropertyInfo, object>(prop, source);
                }
                else
                {
                    //go further down the chain
                    return RecursiveRouteMirror(route, routeIndex + 1, limit, GetValueOfProperty(prop, source));
                }
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
