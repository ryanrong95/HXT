using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Products
{
    [AttributeUsage(AttributeTargets.Property)]
    sealed class OldNamingAttribute : StandardNamingAttribute
    {
        public OldNamingAttribute(string name) : base(name)
        {
            this.Version = "6.0";
        }

        public OldNamingAttribute(string name, string version) : base(name, version)
        {
        }
    }
}
