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

using System.Linq;
using Fancy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.FancyMapper.Models;
using Testing.FancyMapper.Objects;

namespace Testing.FancyMapper
{
    [TestClass]
    public class EnumerableTests
    {
        [TestMethod]
        public void TestMirroringListToListOfModels()
        {
            //create test data
            var obj = new ListOfSimpleObject();
            for (int a = 0; a < 100; a++)
            {
                obj.Objects.Add(new SimpleObject()
                {
                    TestString = a.ToString(),
                    TestInt = a
                });
            }

            //create a complex model with no data
            ListModel mod = new ListModel();

            //mirror
            FancyUtil.Mirror(obj, mod);

            //verify equivalency
            for (int a = 0; a < obj.Objects.Count; a++)
            {
                var i = obj.Objects.OrderBy(x => x.TestString);
                var o = mod.Models.OrderBy(x => x.PoorlyNamedString);
                Assert.AreEqual(i.ToList()[a].TestString, o.ToList()[a].PoorlyNamedString);
            }
        }

        [TestMethod]
        public void TestReflectingListOfModelsToList()
        {
            //create test data
            var mod = new ListModel();
            for (int a = 0; a < 100; a++)
            {
                mod.Models.Add(new SimpleModel()
                {
                    PoorlyNamedString = a.ToString(),
                    PoorlyNamedInt = a
                });
            }

            //create a complex object with no data
            var obj = new ListOfSimpleObject();

            //mirror
            FancyUtil.Reflect(mod, obj);

            //verify equivalency
            for (int a = 0; a < mod.Models.Count; a++)
            {
                var i = mod.Models.OrderBy(x => x.PoorlyNamedString);
                var o = obj.Objects.OrderBy(x => x.TestString);
                Assert.AreEqual(i.ToList()[a].PoorlyNamedString, o.ToList()[a].TestString);
            }
        }
    }
}
