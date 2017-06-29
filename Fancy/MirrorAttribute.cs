using System;

namespace Fancy
{
    /// <summary>
    ///     A mirror attribute defines the location of a target value in another class as an
    ///     relative path.
    ///     Valid mapping
    ///     Class.Property.ChildProperty ... FinalProperty
    ///     More than one mirror can be placed on any property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MirrorAttribute : Attribute
    {
        public MirrorAttribute(string path)
        {
            Path = path;
            NullSubstitute = null;
            Deep = false;
        }

        public string Path { get; set; }

        /// <summary>
        ///     Returns the name of the class by splitting the path.
        ///     The first element in the route is always the class.
        /// </summary>
        public string Class
        {
            get
            {
                var route = Path.Split('.');
                return route[0];
            }
        }

        /// <summary>
        ///     Value to use instead of null when transforming between the source and
        ///     destination property. Must be a primate or generic type that is
        ///     immutable at compile time.
        /// </summary>
        public object NullSubstitute { get; set; }

        /// <summary>
        ///     When true, this mirror will be applied to the child
        ///     elements of the property.
        /// </summary>
        public bool Deep { get; set; }
    }
}