namespace Testing.FancyMapper.Objects
{
    public class ComplexObject
    {
        public string Name { get; set; }
        public SimpleObject NestedObject { get; set; }

        public new string ToString()
        {
            return "Name: " + Name + " Nested Object: (" + NestedObject.ToString() + ")";
        }
    }
}