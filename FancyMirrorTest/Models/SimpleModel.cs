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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FancyMirrorTest.Fancy;

namespace FancyMirrorTest.Models
{
    /// <summary>
    /// This is a model that represents some magic object we want to push to 
    /// a view or something. It uses custom data annotations to tell FancyUtil
    /// where to reflect / mirror values.
    /// </summary>
    public class SimpleModel
    {
        [Mirror("SimpleObject.TestString")]
        [Mirror("SimpleObjectWithNullables.TestString", NullSubstitute = "Null")]
        public string PoorlyNamedString { get; set; }

        [Mirror("SimpleObject.TestInt")]
        [Mirror("SimpleObjectWithNullables.AValue", NullSubstitute = 5)]
        public int PoorlyNamedInt { get; set; }

        [Mirror("SimpleObject.TestNullableInt")]
        [Mirror("SimpleObjectWithNullables.BValue", NullSubstitute = 7)]
        public int? PoorlyNamedNullableInt { get; set; }

        public new string ToString()
        {
            return "PoorlyNamedString: " + PoorlyNamedString + ", PoorlyNamedInt: " + PoorlyNamedInt + ", PoorlyNamedNullableInt: " + PoorlyNamedNullableInt;
        }
    }
}
