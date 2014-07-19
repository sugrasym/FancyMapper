using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FancyMirrorTest.fancy;
using FancyMirrorTest.test;

namespace FancyMirrorTest
{
    class Program
    {

        public Program()
        {
            MirrorSimpleObject();
            Console.WriteLine("\n");
            MirrorComplexObject();
            Console.WriteLine("\n");
            ReflectSimpleObject();
            //Console.WriteLine("\n");
            //ReflectComplexObject();
            //keep it from disapearing (I set a breakpoint on this line)
            int nothing = 1;
        }

        static void Main(string[] args)
        {
            //I assume I'm supposed to do this Java Style
            Program p = new Program();
        }

        private void MirrorSimpleObject()
        {
            Console.WriteLine("Mirroring a Simple Object to a Simple Model\n");
            Console.WriteLine("-- Pre-Reflection --");
            //create the normal everday test object
            TestObject testObject = new TestObject()
            {
                TestString = "Holy crap I'm reflecting!",
                TestInt = 12345,
                TestNullableInt = 13
            };
            
            //note value in console
            Console.WriteLine("testObject -> "+testObject.ToString());

            //create our empty model
            TestModel testModel = new TestModel();

            //note value in console
            Console.Write("testModel -> "+testModel.ToString());

            Console.WriteLine("\n");
            //pray
            FancyUtil.Mirror(testObject, testModel);

            //note values in console
            Console.WriteLine("-- Post-Reflection --");
            Console.WriteLine("testObject -> " + testObject.ToString());
            Console.Write("testModel -> " + testModel.ToString());
        }

        private void MirrorComplexObject()
        {
            Console.WriteLine("Mirroring a Complex Object to a Simple Model\n");
            Console.WriteLine("-- Pre-Reflection --");
            //this object is slightly harder because it has to be solved using recursion
            ComplexTestObject testObject = new ComplexTestObject()
            {
                Name = "Complex Object Thingy",
                NestedObject = new TestObject()
                {
                    TestInt = 10101,
                    TestNullableInt = 24,
                    TestString = "This is a string"
                }
            };

            //note value in console
            Console.WriteLine("testObject -> " + testObject.ToString());

            //create our empty model
            TestModel testModel = new TestModel();

            //note value in console
            Console.Write("testModel -> " + testModel.ToString());

            Console.WriteLine("\n");
            //pray
            FancyUtil.Mirror(testObject, testModel);

            //note values in console
            Console.WriteLine("-- Post-Reflection --");
            Console.WriteLine("testObject -> " + testObject.ToString());
            Console.Write("testModel -> " + testModel.ToString());
        }

        private void ReflectSimpleObject()
        {
            Console.WriteLine("Reflecting a Simple Model to a Simple Object\n");
            Console.WriteLine("-- Pre-Reflection --");
            //create the normal everday test object
            TestObject testObject = new TestObject();

            //note value in console
            Console.WriteLine("testObject -> " + testObject.ToString());

            //create our empty model
            TestModel testModel = new TestModel()
            {
                PoorlyNamedString = "Holy crap I'm reflecting!",
                PoorlyNamedInt = 12345,
                PoorlyNamedNullableInt = 13
            };

            //note value in console
            Console.Write("testModel -> " + testModel.ToString());

            Console.WriteLine("\n");
            //pray
            FancyUtil.Reflect(testModel, testObject);

            //note values in console
            Console.WriteLine("-- Post-Reflection --");
            Console.WriteLine("testObject -> " + testObject.ToString());
            Console.Write("testModel -> " + testModel.ToString());
        }

        /*private void ReflectComplexObject()
        {
            Console.WriteLine("Reflecting a Complex Model to a Simple Object\n");
            Console.WriteLine("-- Pre-Reflection --");
            //create the normal everday test object
            TestObject testObject = new TestObject();

            //note value in console
            Console.WriteLine("testObject -> " + testObject.ToString());

            //create our empty model
            ComplexTestModel testModel = new ComplexTestModel()
            {
                PoorName = "Bad Name",
                
            };

            //note value in console
            Console.Write("testModel -> " + testModel.ToString());

            Console.WriteLine("\n");
            //pray
            FancyUtil.Reflect(testModel, testObject);

            //note values in console
            Console.WriteLine("-- Post-Reflection --");
            Console.WriteLine("testObject -> " + testObject.ToString());
            Console.Write("testModel -> " + testModel.ToString());
        }*/
    }
}
