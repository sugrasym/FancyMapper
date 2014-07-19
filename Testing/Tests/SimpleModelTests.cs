using System;
using FancyMirrorTest.Fancy;
using FancyMirrorTest.Models;
using FancyMirrorTest.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.Tests
{
    /// <summary>
    /// A set of unit tests to test the SimpleModel
    /// 
    /// SimpleModel has data annotations defined in its class that will
    /// instruct on how to mirror it.
    /// </summary>
    [TestClass]
    public class SimpleModelTests
    {

        /// <summary>
        /// Figuring out this thing
        /// </summary>
        [TestMethod]
        public void TestTest()
        {
            Assert.AreEqual(true, true);
        }

        /// <summary>
        /// Tests taking a SimpleObject and SimpleModel and mirroring them.
        /// The SimpleModel's properties should be equivalent to those in 
        /// the SimpleObject after this transformation.
        /// </summary>
        [TestMethod]
        public void TestMirroringSimpleObjectIntoSimpleModelShouldBeEquivalent()
        {
            //create simple object with test data
            SimpleObject obj = new SimpleObject()
            {
                TestInt = 12345,
                TestNullableInt = 65432,
                TestString = "Mirror!"
            };

            //create a simple model with no data
            SimpleModel mod = new SimpleModel();

            //mirror
            FancyUtil.Mirror(obj, mod);
            
            //verify equivalency
            Assert.AreEqual(obj.TestInt, mod.PoorlyNamedInt);
            Assert.AreEqual(obj.TestNullableInt, mod.PoorlyNamedNullableInt);
            Assert.AreEqual(obj.TestString, mod.PoorlyNamedString);
        }
    }
}
