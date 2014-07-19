namespace FancyMirrorTest.Objects
{
    public class ComplexTestObject
    {
        public string Name { get; set;}
        public SimpleObject NestedObject { get; set; }

        public new string ToString()
        {
            return "Name: " + Name + " Nested Object: (" + NestedObject.ToString()+")";
        }
    }
}
