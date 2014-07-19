namespace FancyMirrorTest.test
{
    public class ComplexTestObject
    {
        public string Name { get; set;}
        public TestObject NestedObject { get; set; }

        public new string ToString()
        {
            return "Name: " + Name + " Nested Object: (" + NestedObject.ToString()+")";
        }
    }
}
