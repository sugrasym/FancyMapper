using Fancy;

namespace Testing.FancyMapper.Models
{
    public class ComplexModel
    {
        public ComplexModel()
        {
            NestedModel = new SimpleModel();
        }

        [Mirror("SimpleObject.SomeString")]
        [Mirror("ComplexObject.Name")]
        [Mirror("OverlyComplexObject.NestedComplexObject.Name")]
        public string PoorName { get; set; }

        [Mirror("SimpleObject")]
        [Mirror("ComplexObject.NestedObject", Deep = true)]
        [Mirror("OverlyComplexObject.NestedComplexObject.NestedObject", Deep = true)]
        public SimpleModel NestedModel { get; set; }

        public new string ToString()
        {
            return "PoorName: " + PoorName + ", NestedModel: (" + NestedModel.ToString() + ")";
        }
    }
}