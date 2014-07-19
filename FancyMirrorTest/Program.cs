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
            DoFancyStuff();
        }

        static void Main(string[] args)
        {
            //I assume I'm supposed to do this Java Style
            Program p = new Program();
        }

        private void DoFancyStuff()
        {
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
            Console.WriteLine("\n");
            Console.WriteLine("-- Post-Reflection --");
            Console.WriteLine("testObject -> " + testObject.ToString());
            Console.Write("testModel -> " + testModel.ToString());

            //keep it from disapearing (I set a breakpoint on this line)
            int nothing = 1;;
        }
    }
}
