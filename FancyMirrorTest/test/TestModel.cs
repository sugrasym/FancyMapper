using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FancyMirrorTest.fancy;

namespace FancyMirrorTest.test
{
    /// <summary>
    /// This is a model that represents some magic object we want to push to 
    /// a view or something. It uses custom data annotations to tell FancyUtil
    /// where to reflect / mirror values.
    /// </summary>
    class TestModel
    {
        [Mirror("TestObject.TestString")]
        public string PoorlyNamedString { get; set; }

        [Mirror("TestObject.TestInt")]
        public int PoorlyNamedInt { get; set; }

        [Mirror("TestObject.TestNullableInt", 0)]
        public int? PoorlyNamedNullableInt { get; set; }

        public new string ToString()
        {
            return "PoorlyNamedString: " + PoorlyNamedString + ", PoorlyNamedInt: " + PoorlyNamedInt + ", PoorlyNamedNullableInt: " + PoorlyNamedNullableInt;
        }
    }
}
