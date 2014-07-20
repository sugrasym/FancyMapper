using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FancyMirrorTest.Fancy
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
            //verify the type of the target object
            if (destination.GetType().Name == className)
            {
                int index = route.Length > 1 ? 1 : 0;
                //must be solved using recursion
                if (route.Length == 1)
                {
                    //re-evaluate in the context of the child
                    var lp = FancyUtil.GetValueOfProperty(property, source);
                    FancyUtil.Reflect(lp, destination);
                }
                else if (route.Length > 0)
                {
                    var sourceProp = RecursiveRouteReflect(route, index, 10, destination);
                    if (mirror.WalkChildren)
                    {
                        var lp = FancyUtil.GetValueOfProperty(property, source);
                        var sp = FancyUtil.GetValueOfProperty(sourceProp.Item1, sourceProp.Item2);
                        if (sp == null)
                        {
                            //todo: determine if this can be circumvented
                            /*
                             * I really wanted to be able to automatically instantiate properties which were null
                             * using their parameterless constructor, but that is quickly starting to look like
                             * its impossible. The problem seems to be that to convert all but the most trivial
                             * primatives you need to know the type you are converting to at compile time (which
                             * I don't) even if I know the type at runtime (which I do).
                             */
                            throw new NullReferenceException("Unable to map to property "+sourceProp.Item1.Name+" because it is null");
                            /*var t = sourceProp.Item1;
                            var parent = sourceProp.Item2;
                            //It isn't possible to reflect properties into a null object, so it needs to be instantiated
                            var o = Activator.CreateInstance(t.PropertyType); //I hope it has a parameterless constructor
                            var p = parent.GetType().GetProperty(t.Name);
                            p.SetValue(p,o);
                            sp = FancyUtil.GetValueOfProperty(sourceProp.Item1, sourceProp.Item2);*/
                        }
                        FancyUtil.Reflect(lp, sp);
                    }
                    else
                    {
                        FancyUtil.SetValueOfProperty(sourceProp.Item1, source, sourceProp.Item2, property, mirror.NullSubstitute);
                    }
                }
                else
                {
                    throw new Exception("Empty routes cannot be evaluated");
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
