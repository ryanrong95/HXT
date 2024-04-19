using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Products
{
    [AttributeUsage(AttributeTargets.Property)]
    class StandardNamingAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Version
        {
            get; protected set;
        }

        public StandardNamingAttribute(string name)
        {
            this.Name = name;
            this.Version = "1.0";
        }

        public StandardNamingAttribute(string name, string version) : this(name)
        {
            this.Version = version;
        }


    }
}
