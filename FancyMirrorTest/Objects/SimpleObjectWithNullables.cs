using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyMirrorTest.Objects
{
    public class SimpleObjectWithNullables
    {
        public string TestString { get; set; }
        public int? AValue { get; set; }
        public int? BValue { get; set; }

        public new string ToString()
        {
            return "TestString: " + TestString + ", AValue: " + AValue + ", BValue: " + BValue;
        }
    }
}
