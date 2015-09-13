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

namespace Fancy
{
    /// <summary>
    /// A mirror attribute defines the location of a target value in another class as an
    /// relative path.
    /// 
    /// Valid mapping
    /// Class.Property.ChildProperty ... FinalProperty
    /// 
    /// More than one mirror can be placed on any property.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class MirrorAttribute : Attribute
    {
        public string Path { get; set; }

        /// <summary>
        /// Returns the name of the class by splitting the path.
        /// The first element in the route is always the class.
        /// </summary>
        public string Class
        {
            get
            {
                string[] route = Path.Split('.');
                return route[0];
            }
        }

        /// <summary>
        /// Value to use instead of null when transforming between the source and
        /// destination property. Must be a primate or generic type that is
        /// immutable at compile time.
        /// </summary>
        public object NullSubstitute { get; set; }

        /// <summary>
        /// When true, this mirror will be applied to the child
        /// elements of the property.
        /// </summary>
        public bool Deep { get; set; }

        public MirrorAttribute(String path)
        {
            Path = path;
            NullSubstitute = null;
            Deep = false;
        }
    }
}
