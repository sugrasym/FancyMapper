using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyMirrorTest.fancy
{
    /// <summary>
    /// A mirror attribute defines the location of a target value in another class as an
    /// relative path.
    /// 
    /// Valid mapping
    /// Class.Property
    /// 
    /// More than one mirror can be placed on any property.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class MirrorAttribute : System.Attribute
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
        /// todo: implement this really cool feature so the documentation isn't lying
        /// Null substitute will be used whenever an incoming or outgoing value is nullable
        /// in one target but not in another. This will be used instead.
        /// 
        /// Ex mirroring an int? onto an int would use nullSubstitute for the value if the
        /// source is null.
        /// </summary>
        protected object NullSubstitute { get; set; }

        public MirrorAttribute(String path, object nullSubstitute = null)
        {
            this.Path = path;
            this.NullSubstitute = nullSubstitute;
        }
    }
}
