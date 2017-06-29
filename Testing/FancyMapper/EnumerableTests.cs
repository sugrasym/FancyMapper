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
            for (var a = 0; a < 100; a++)
            {
                obj.Objects.Add(new SimpleObject
                {
                    TestString = a.ToString(),
                    TestInt = a
                });
            }

            //create a complex model with no data
            var mod = new ListModel();

            //mirror
            FancyUtil.Mirror(obj, mod);

            //verify equivalency
            for (var a = 0; a < obj.Objects.Count; a++)
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
            for (var a = 0; a < 100; a++)
            {
                mod.Models.Add(new SimpleModel
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
            for (var a = 0; a < mod.Models.Count; a++)
            {
                var i = mod.Models.OrderBy(x => x.PoorlyNamedString);
                var o = obj.Objects.OrderBy(x => x.TestString);
                Assert.AreEqual(i.ToList()[a].PoorlyNamedString, o.ToList()[a].TestString);
            }
        }
    }
}