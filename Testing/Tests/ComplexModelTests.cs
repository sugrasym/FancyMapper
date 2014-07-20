using System;
using FancyMirrorTest.Fancy;
using FancyMirrorTest.Models;
using FancyMirrorTest.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.Tests
{
    /// <summary>
    /// A set of unit tests to test the ComplexModel
    /// 
    /// ComplexModel has data annotations defined in its class that will
    /// instruct on how to mirror it.
    /// </summary>
    [TestClass]
    public class ComplexModelTests
    {
        /// <summary>
        /// Tests mirroring a single string property from a simple object into
        /// a single string property on a complex model. These strings should
        /// be equivalent after this transformation.
        /// </summary>
        [TestMethod]
        public void TestMirroringSinglePropertyFromSimpleObjectIntoComplexModelShouldBeEquivalent()
        {
            //create a simple object with test data
            SimpleObject obj = new SimpleObject()
            {
                SomeString = "I'm some other string!"
            };

            //create a complex model with no data
            ComplexModel mod = new ComplexModel();

            //mirror
            FancyUtil.Mirror(obj, mod);

            //verify equivalency
            Assert.AreEqual(obj.SomeString, mod.PoorName);
        }

        /// <summary>
        /// Tests mirroring the properties of a simple object into
        /// a simple model nested inside this complex model. The
        /// nested simple model and simple object should be equivalent
        /// after this transformation.
        /// </summary>
        [TestMethod]
        public void TestMirroringSimpleObjectIntoNestedSimpleModelInComplexModelShouldBeEquivalent()
        {
            //create a simple object with test data
            SimpleObject obj = new SimpleObject()
            {
                TestInt = 56223,
                TestNullableInt = 30998,
                TestString = "I'm a Mirrored String!"
            };

            //create a complex model with no data
            ComplexModel mod = new ComplexModel();

            //mirror
            FancyUtil.Mirror(obj, mod);

            //verify equivalency
            Assert.AreEqual(obj.TestInt, mod.NestedModel.PoorlyNamedInt);
            Assert.AreEqual(obj.TestNullableInt, mod.NestedModel.PoorlyNamedNullableInt);
            Assert.AreEqual(obj.TestString, mod.NestedModel.PoorlyNamedString);
        }

        /// <summary>
        /// Tests reflecting a single string property from a complex model into a simple object.
        /// These properties should be equivalent after this transformation.
        /// </summary>
        [TestMethod]
        public void TestReflectingSinglePropertyFromComplexModelIntoSimpleObjectShouldBeEquivalent()
        {
            //create a simple object with no data
            SimpleObject obj = new SimpleObject();
            
            //create a complex model with test data
            ComplexModel mod = new ComplexModel()
            {
                PoorName = "Reflected String",
            };

            //reflect
            FancyUtil.Reflect(mod, obj);

            //verify equivalency
            Assert.AreEqual(obj.SomeString, mod.PoorName);
        }

        /// <summary>
        /// Tests reflecting a simple model nested inside a complex model into a 
        /// simple object. The nested simple model and the simple object should be
        /// equivalent after this transformation.
        /// </summary>
        [TestMethod]
        public void TestReflectingNestedSimpleObjectIntoSimpleModelShouldBeEquivalent()
        {
            //create a simple object with no data
            SimpleObject obj = new SimpleObject();

            //create a complex model with test data
            ComplexModel mod = new ComplexModel()
            {
                PoorName = "Complex Object Name",
                NestedModel = new SimpleModel()
                {
                    PoorlyNamedString = "Reflected string!",
                    PoorlyNamedInt = 2003,
                    PoorlyNamedNullableInt = 1024
                }
            };

            //reflect
            FancyUtil.Reflect(mod, obj);

            //verify equivalency
            Assert.AreEqual(obj.TestInt, mod.NestedModel.PoorlyNamedInt);
            Assert.AreEqual(obj.TestNullableInt, mod.NestedModel.PoorlyNamedNullableInt);
            Assert.AreEqual(obj.TestString, mod.NestedModel.PoorlyNamedString);
        }

        /// <summary>
        /// Tests mirroring a complex object into a complex model. The properties on
        /// the model should be equivalent to those in the object after this
        /// transformation.
        /// 
        /// Note that the routing for the nested model must use the WalkChildren flag
        /// because it needs to be re-evaluated against its target to determine what
        /// its properties mirror against.
        /// </summary>
        [TestMethod]
        public void TestMirroringComplexObjectIntoComplexModelShouldBeEquivalent()
        {
            //create a complex object with test data
            ComplexObject obj = new ComplexObject()
            {
                Name = "A complex object",
                NestedObject = new SimpleObject()
                {
                    TestInt = 500,
                    TestNullableInt = 300,
                    TestString = "abcdef"
                }
            };

            //create a complex model with no data
            ComplexModel mod = new ComplexModel();

            //mirror
            FancyUtil.Mirror(obj, mod);

            //verify equivalency
            Assert.AreEqual(obj.Name, mod.PoorName);
            Assert.AreEqual(obj.NestedObject.TestInt, mod.NestedModel.PoorlyNamedInt);
            Assert.AreEqual(obj.NestedObject.TestNullableInt, mod.NestedModel.PoorlyNamedNullableInt);
            Assert.AreEqual(obj.NestedObject.TestString, mod.NestedModel.PoorlyNamedString);
        }

        /// <summary>
        /// This test actually demonstrates a weakness in this code.
        /// </summary>
        [TestMethod]
        public void TestReflectingComplexModelIntoComplexObjectShouldBeEquivalent()
        {
            //create a complex object with no data
            ComplexObject obj = new ComplexObject()
            {
                /*
                 * Due to constraints on reflection this test would actually fail if this object
                 * were not instantiated. That makes sense, since you can't set a child property
                 * if the parent property is null, but it kind of sucks that I can't find a way
                 * to dynamically instantiate null references at runtime (unless they are an
                 * object - and ONLY an object)
                 */
                NestedObject = new SimpleObject()
            };

            //create a complex model with test data
            ComplexModel mod = new ComplexModel()
            {
                PoorName = "Reflected name",
                NestedModel = new SimpleModel()
                {
                    PoorlyNamedInt = 456,
                    PoorlyNamedNullableInt = 1234,
                    PoorlyNamedString = "Yo!"
                }
            };

            //reflect
            FancyUtil.Reflect(mod, obj);

            //verify equivalency
            Assert.AreEqual(obj.Name, mod.PoorName);
            Assert.AreEqual(obj.NestedObject.TestInt, mod.NestedModel.PoorlyNamedInt);
            Assert.AreEqual(obj.NestedObject.TestNullableInt, mod.NestedModel.PoorlyNamedNullableInt);
            Assert.AreEqual(obj.NestedObject.TestString, mod.NestedModel.PoorlyNamedString);
        }
    }
}
