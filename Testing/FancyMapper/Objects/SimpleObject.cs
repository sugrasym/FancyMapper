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

namespace Testing.FancyMapper.Objects
{
    /// <summary>
    /// This is a test object designed to test the ability for this to handle
    /// mapping values. It intentionally does not implement any custom data
    /// attributes because Entity framework won't and we'll need to be able
    /// to map to and from those objects.
    /// </summary>
    public class SimpleObject
    {
        public string TestString { get; set; }
        public int TestInt { get; set; }
        public int? TestNullableInt { get; set; }

        public string SomeString { get; set; }

        public new string ToString()
        {
            return "TestString: " + TestString + ", TestInt: " + TestInt + ", TestNullableInt: " + TestNullableInt;
        }
    }
}
