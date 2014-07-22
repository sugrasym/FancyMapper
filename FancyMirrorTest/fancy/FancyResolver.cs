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
        /// Instantiates the property on the parent object from the provided
        /// property info type.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="parent"></param>
        public static void CreateInstance(PropertyInfo t, object parent)
        {
            var o = Activator.CreateInstance(t.PropertyType); //I hope it has a parameterless constructor
            var p = parent.GetType().GetProperty(t.Name);
            p.SetValue(parent, o);
        }
    }
}
