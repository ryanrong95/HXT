using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Products.Coding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Products
{
    [Newtonsoft.Json.JsonConverter(typeof(CodersConverter))]

    sealed public class Embargos : Coders<Embargo>
    {
        public Embargos(District district) : base(district)
        {

        }

        public Embargos()
        {

        }
    }
}
