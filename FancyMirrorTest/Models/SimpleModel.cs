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
        [Mirror("SimpleObjectWithNullables.BValue")]
        public int? PoorlyNamedNullableInt { get; set; }

        public new string ToString()
        {
            return "PoorlyNamedString: " + PoorlyNamedString + ", PoorlyNamedInt: " + PoorlyNamedInt + ", PoorlyNamedNullableInt: " + PoorlyNamedNullableInt;
        }
    }
}
