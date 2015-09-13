/*
 * Copyright (C) 2015 Nathan Wiehoff, Geoffrey Hibbert
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fancy
{
    public static class FancyReflect
    {
        /// <summary>
        /// Maps the provided property with the value of the property in the source object that is referenced
        /// using the path in the mirror.
        /// </summary>
        /// <param name="mirror"></param>
        /// <param name="sourceProp"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void MapReflect(MirrorAttribute mirror, PropertyInfo sourceProp, object source, object destination)
        {
            string[] route = mirror.Path.Split('.');
            //the first element is always the class
            string className = mirror.Class;
            string destTypeName = FancyUtil.AttemptToDeproxyName(destination);
            //verify the type of the target object
            if (destTypeName == className)
            {
                int index = route.Length > 1 ? 1 : 0;
                //must be solved using recursion
                if (route.Length == 1)
                {
                    //re-evaluate in the context of the child
                    var lp = FancyUtil.GetValueOfProperty(sourceProp, source);
                    FancyUtil.Reflect(lp, destination);
                }
                else if (route.Length > 0)
                {
                    var destProp = RecursiveRouteReflect(route, index, 10, destination);
                    if (mirror.Deep)
                    {
                        var s = FancyUtil.GetValueOfProperty(sourceProp, source);
                        var d = FancyUtil.GetValueOfProperty(destProp.Item1, destProp.Item2);
                        if (d == null)
                        {
                            //attempt to instantiate a new one on the fly so we have somewhere to put these values
                            bool safe = FancyResolver.ResolveNullDestination(destProp, ref d);
                            if (!safe)
                                throw new NullReferenceException("Unable to map to property " + destProp.Item1.Name +
                                                                 " because it is null in the target");
                        }
                        //determine if this property is an IEnumerable or a single model
                        if (sourceProp.PropertyType.GetInterfaces().Contains(typeof (IEnumerable)))
                        {
                            //make sure the target is an IEnumerable as well
                            if (!destProp.Item1.PropertyType.GetInterfaces().Contains(typeof (IEnumerable)))
                                throw new Exception(
                                    "Target property must be an IEnumerable if deep is true and the source is an IEnumerable");
                            //determine the type of the target IEnumerable
                            var destType = d.GetType().GetGenericArguments()[0];
                            //instantiate the target property
                            FancyResolver.CreateListInstance(destProp.Item1, destination);
                            //map each element to the list's model
                            foreach (var element in s as IEnumerable)
                            {
                                //create the model
                                var t = FancyResolver.CreateInstance(destType);
                                FancyUtil.Reflect(element, t);
                                //add it to the target list
                                var o = FancyUtil.GetValueOfProperty(destProp.Item1, destination);
                                MethodInfo addMethod = o.GetType()
                                    .GetMethods().FirstOrDefault(m => m.Name == "Add" && m.GetParameters().Count() == 1);
                                if (addMethod != null) addMethod.Invoke(o, new object[] {t});
                            }
                        }
                        else
                        {
                            FancyUtil.Reflect(s, d);
                        }
                    }
                    else
                    {
                        FancyUtil.SetValueOfProperty(destProp.Item1, source, destProp.Item2, sourceProp,
                            mirror.NullSubstitute);
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
                    var sp = FancyUtil.GetValueOfProperty(prop, source);
                    if (sp == null)
                    {
                        //attempt to instantiate a new one on the fly so we have somewhere to put these values
                        bool safe = FancyResolver.ResolveNullDestination(new Tuple<PropertyInfo, object>(prop, source), ref sp);
                        if (!safe) throw new NullReferenceException("Unable to map to property " + prop.Name + " because it is null in the target");
                    }
                    //go further down the chain
                    return RecursiveRouteReflect(route, routeIndex + 1, limit, sp);
                }
            }
        }
    }
}
