using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Services
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed public class GeckoFuntionAttribute : Attribute
    {
        readonly string name;

        public GeckoFuntionAttribute()
        {

        }

        public GeckoFuntionAttribute(string funtionName)
        {
            this.name = funtionName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="funtionName"></param>
        /// <param name="version"></param>
        public GeckoFuntionAttribute(string funtionName, string version) : this(funtionName)
        {
            this.Version = version;
        }

        public string FuntionName
        {
            get { return name; }
        }

        public string Version { get; private set; }
    }


    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed public class GeckoClassAttribute : Attribute
    {
        public GeckoClassAttribute()
        {

        }
    }
}
