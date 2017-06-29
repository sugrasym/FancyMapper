using System;
using Fancy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.FancyMapper.Models;
using Testing.FancyMapper.Objects;

namespace Testing.FancyMapper
{
    /// <summary>
    ///     A set of unit tests to test the SimpleModel
    ///     SimpleModel has data annotations defined in its class that will
    ///     instruct on how to mirror it.
    /// </summary>
    [TestClass]
    public class SimpleModelTests
    {
        /// <summary>
        ///     Figuring out this thing
        /// </summary>
        [TestMethod]
        public void TestTest()
        {
            Assert.AreEqual(true, true);
        }

        /// <summary>
        ///     Tests taking a SimpleObject and SimpleModel and mirroring them.
        ///     The SimpleModel's properties should be equivalent to those in
        ///     the SimpleObject after this transformation.
        /// </summary>
        [TestMethod]
        public void TestMirroringSimpleObjectIntoSimpleModelShouldBeEquivalent()
        {
            //create simple object with test data
            var obj = new SimpleObject
            {
                TestInt = 12345,
                TestNullableInt = 65432,
                TestString = "Mirror!",
                SomeString = "I'm an unwanted string!"
            };

            //create a simple model with no data
            var mod = new SimpleModel();

            //mirror
            FancyUtil.Mirror(obj, mod);

            //verify equivalency
            Assert.AreEqual(obj.TestInt, mod.PoorlyNamedInt);
            Assert.AreEqual(obj.TestNullableInt, mod.PoorlyNamedNullableInt);
            Assert.AreEqual(obj.TestString, mod.PoorlyNamedString);
        }

        /// <summary>
        ///     Tests taking a SimpleModel and SimpleObject and reflecting them.
        ///     The SimpleObject's properties should be equivalent to those in
        ///     the SimpleModel after this transformation.
        /// </summary>
        [TestMethod]
        public void TestReflectingSimpleModelIntoSimpleObjectShouldBeEquivalent()
        {
            //create a simple object with no data
            var obj = new SimpleObject();

            //create a simple model with test data
            var mod = new SimpleModel
            {
                PoorlyNamedInt = 7890,
                PoorlyNamedNullableInt = 34567,
                PoorlyNamedString = "Reflect!"
            };

            //reflect
            FancyUtil.Reflect(mod, obj);

            //verify equivalency
            Assert.AreEqual(obj.TestInt, mod.PoorlyNamedInt);
            Assert.AreEqual(obj.TestNullableInt, mod.PoorlyNamedNullableInt);
            Assert.AreEqual(obj.TestString, mod.PoorlyNamedString);
        }

        /// <summary>
        ///     This method tests what happens when you attempt to mirror null into a model.
        ///     It should fail without changing the state of the model, and will throw an
        ///     exception.
        /// </summary>
        [TestMethod]
        public void TestMirroringNullIntoSimpleModelShouldThrowExceptionWithPropertiesUnchanged()
        {
            //create a simple model with original data
            var mod = new SimpleModel
            {
                PoorlyNamedInt = 1,
                PoorlyNamedNullableInt = 5,
                PoorlyNamedString = "This should still be here"
            };

            var thrown = false;

            //mirror
            try
            {
                FancyUtil.Mirror(null, mod);
            }
            catch (Exception e)
            {
                thrown = true;
            }

            //verify equivalency
            Assert.AreEqual("This should still be here", mod.PoorlyNamedString);
            Assert.AreEqual(1, mod.PoorlyNamedInt);
            Assert.AreEqual(5, mod.PoorlyNamedNullableInt);

            //and that the exception was thrown
            Assert.AreEqual(thrown, true);
        }

        /// <summary>
        ///     Tests the ability for null substitution to handle converting incoming nullable values
        ///     into ones that the model can tolerate.
        /// </summary>
        [TestMethod]
        public void TestMirroringObjectWithNullableIntIntoNonNullibleModelShouldUseNullSubstituteValue()
        {
            //create a simple object with nulls
            var obj = new SimpleObjectWithNullables
            {
                AValue = null,
                BValue = null,
                TestString = null
            };

            //create an empty simple model
            var mod = new SimpleModel();

            //mirror
            FancyUtil.Mirror(obj, mod);

            //verify expected null substitutes
            Assert.AreEqual("Null", mod.PoorlyNamedString);
            Assert.AreEqual(5, mod.PoorlyNamedInt);
            Assert.AreEqual(7, mod.PoorlyNamedNullableInt);
        }

        [TestMethod]
        public void TestReflectingModelWithNullableIntIntoNullibleModelShouldReplaceNullSubstituteValuesWithNull()
        {
            //create a test object with some non-null data so we can verify its being written to
            var obj = new SimpleObjectWithNullables
            {
                TestString = "should be overwritten",
                AValue = 53,
                BValue = 99
            };

            //create a model with test data equal to its null substitution values
            var mod = new SimpleModel
            {
                PoorlyNamedString = "Null",
                PoorlyNamedNullableInt = 7,
                PoorlyNamedInt = 5
            };

            //mirror
            FancyUtil.Reflect(mod, obj);

            //verify expected null substitutes
            Assert.AreEqual(null, obj.TestString);
            Assert.AreEqual(null, obj.AValue);
            Assert.AreEqual(null, obj.BValue); //this one can be null because it is nullable in the destination
        }
    }
}