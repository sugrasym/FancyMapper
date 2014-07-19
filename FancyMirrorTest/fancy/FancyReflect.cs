using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FancyMirrorTest.fancy
{
    public static class FancyReflect
    {
        /// <summary>
        /// Maps the provided property with the value of the property in the source object that is referenced
        /// using the path in the mirror.
        /// </summary>
        /// <param name="mirror"></param>
        /// <param name="property"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static void MapReflect(MirrorAttribute mirror, PropertyInfo property, object source, object destination)
        {
            string[] route = mirror.Path.Split('.');
            //the first element is always the class
            string className = mirror.Class;
            //the second element is the property
            string propName = route[1];
            //verify the type of the target object
            if (destination.GetType().Name == className)
            {
                if (route.Length == 2)
                {
                    //get a list of properties on the destination object
                    List<PropertyInfo> destProps = FancyUtil.GetPropertiesOnObject(destination).ToList();
                    //find the one that matches our property name
                    PropertyInfo destProp = destProps.SingleOrDefault(x => x.Name == propName);
                    if (destProp == null)
                    {
                        throw new Exception(
                            "The property specified by this MirrorAttribute does not exist on the destination object");
                    }
                    else
                    {
                        FancyUtil.SetValueOfProperty(destProp, source, destination, property);
                    }
                }
                else
                {
                    //must be solved using recursion
                    var sourceProp = RecursiveRouteReflect(route, 1, 10, destination);
                    FancyUtil.SetValueOfProperty(sourceProp.Item1, source, sourceProp.Item2, property);
                }
            }
            else
            {
                throw new Exception("A single MirrorAttribute can only be used to map to one class");
            }
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
        public static Tuple<PropertyInfo, object> RecursiveRouteReflect(string[] route, int routeIndex, int limit, object source)
        {
            if (routeIndex > limit)
            {
                throw new Exception("Aborted recursive routing, will not attempt to use an index greater than " + limit);
            }

            //get the current route position
            string current = route[routeIndex];
            //get list of properties on source
            List<PropertyInfo> props = FancyUtil.GetPropertiesOnObject(source).ToList();
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
                    return RecursiveRouteReflect(route, routeIndex + 1, limit, FancyUtil.GetValueOfProperty(prop, source));
                }
            }
        }
    }
}
