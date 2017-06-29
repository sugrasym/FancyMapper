using System.Collections.Generic;
using Fancy;

namespace Testing.FancyMapper.Models
{
    public class ListModel
    {
        public ListModel()
        {
            Models = new List<SimpleModel>();
        }

        [Mirror("ListOfSimpleObject.Objects", Deep = true)]
        public List<SimpleModel> Models { get; set; }
    }
}