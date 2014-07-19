namespace FancyMirrorTest.Models
{
    public class ComplexTestModel
    {
        public ComplexTestModel()
        {
            NestedModel = new SimpleModel();
        }

        public string PoorName { get; set; }

        public SimpleModel NestedModel { get; set; }

        public new string ToString()
        {
            return "PoorName: " + PoorName + ", NestedModel: ("+NestedModel.ToString()+")";
        }
    }
}
