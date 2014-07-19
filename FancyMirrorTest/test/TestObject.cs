using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyMirrorTest.test
{
    /// <summary>
    /// This is a test object designed to test the ability for this to handle
    /// mapping values. It intentionally does not implement any custom data
    /// attributes because Entity framework won't and we'll need to be able
    /// to map to and from those objects.
    /// </summary>
    public class TestObject
    {
        public string TestString { get; set; }
        public int TestInt { get; set; }
        public int? TestNullableInt { get; set; }

        public new string ToString()
        {
            return "TestString: " + TestString + ", TestInt: " + TestInt + ", TestNullableInt: " + TestNullableInt;
        }
    }
}
