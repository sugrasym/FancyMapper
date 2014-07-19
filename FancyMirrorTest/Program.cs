using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyMirrorTest
{
    class Program
    {

        public Program()
        {
            //keep it from disapearing (I set a breakpoint on this line)
            int nothing = 1;
        }

        static void Main(string[] args)
        {
            //I assume I'm supposed to do this Java Style
            Program p = new Program();
        }
    }
}
