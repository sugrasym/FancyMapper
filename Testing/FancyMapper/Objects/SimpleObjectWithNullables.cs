namespace Testing.FancyMapper.Objects
{
    public class SimpleObjectWithNullables
    {
        public string TestString { get; set; }
        public int? AValue { get; set; }
        public int? BValue { get; set; }

        public new string ToString()
        {
            return "TestString: " + TestString + ", AValue: " + AValue + ", BValue: " + BValue;
        }
    }
}