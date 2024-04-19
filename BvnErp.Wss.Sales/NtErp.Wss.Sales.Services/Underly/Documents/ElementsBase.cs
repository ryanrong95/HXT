using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    abstract public class ElementsBase : ElementBase
    {
        internal protected object Value { get; protected set; }

        protected ElementsBase()
        {

        }

        protected ElementsBase(object value)
        {
            this.Value = value;
        }

        sealed public override Elements this[string index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
                if (this is Elements)
                {
                    ((Elements)this).Value = null;
                }
            }
        }
    }
}
