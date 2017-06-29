using System.Collections.Generic;

namespace Testing.FancyMapper.Objects
{
    public class ListOfSimpleObject
    {
        public ListOfSimpleObject()
        {
            Objects = new List<SimpleObject>();
        }

        public List<SimpleObject> Objects { get; set; }
    }
}