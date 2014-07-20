using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyMirrorTest.Fancy
{
    /// <summary>
    /// A mirror attribute defines the location of a target value in another class as an
    /// relative path.
    /// 
    /// Valid mapping
    /// Class.Property.ChildProperty ... FinalProperty
    /// 
    /// More than one mirror can be placed on any property.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class MirrorAttribute : Attribute
    {
        public string Path { get; set; }

        /// <summary>
        /// Returns the name of the class by splitting the path.
        /// The first element in the route is always the class.
        /// </summary>
        public string Class
        {
            get
            {
                string[] route = Path.Split('.');
                return route[0];
            }
        }

        /// <summary>
        /// Value to use instead of null when transforming between the source and
        /// destination property. Must be a primate or generic type that is
        /// immutable at compile time.
        /// </summary>
        public object NullSubstitute { get; set; }

        /// <summary>
        /// When true, this mirror will be evaluated in its own context and any child
        /// properties it contains will be evaluated against the object in its path.
        /// 
        /// Use this if you are nesting a model inside a model and the nested model
        /// has mirror attributes on its own properties.
        /// </summary>
        public bool WalkChildren { get; set; }

        public MirrorAttribute(String path)
        {
            Path = path;
            NullSubstitute = null;
            WalkChildren = false;
        }
    }
}
