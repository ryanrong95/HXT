using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.ApiModels.Warehouse
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    class CurrenyAttribute : Attribute, ICurrency
    {
        readonly string shortName;
        readonly string symbol;
        readonly string shortSymbol;

        public CurrenyAttribute(string shortName, string symbol, string shortSymbol)
        {
            this.shortName = shortName;
            this.symbol = symbol;
            this.shortSymbol = shortSymbol;
        }

        public string ShortName
        {
            get { return this.shortName; }
        }

        public string Symbol
        {
            get { return this.symbol; }
        }

        public string ShortSymbol
        {
            get { return this.shortSymbol; }
        }

        public override string ToString()
        {
            return this.ShortSymbol;
        }
    }
}
