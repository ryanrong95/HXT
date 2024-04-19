using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Products.Coding
{
    abstract public class Coder
    {
        public District District { get; set; }

        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            Coder _obj = obj as Coder;
            if (_obj == null)
            {
                return false;
            }

            return this.GetHashCode() == _obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return (this.District.ToString() + this.Value).GetHashCode();
        }
    }

    abstract public class Coder<T> : Coder where T : Coder, new()
    {
        readonly public static T Free;
        static Coder()
        {
            Free = new T
            {
                District = District.Global,
                Value = null
            };
        }
    }
}
