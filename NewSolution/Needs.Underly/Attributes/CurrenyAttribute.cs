using Needs.Underly.Legals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly.Attributes
{

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    class CurrenyAttribute : LegalAttribute, ICurrency
    {
        readonly string shortName;
        readonly string symbol;
        readonly string shortSymbol;
        readonly string chineseName;

        public CurrenyAttribute(string shortName, string symbol, string shortSymbol)
        {
            this.shortName = shortName;
            this.symbol = symbol;
            this.shortSymbol = shortSymbol;
        }
        public CurrenyAttribute(string chineseName, string shortName, string symbol, string shortSymbol)
        {
            this.shortName = shortName;
            this.symbol = symbol;
            this.shortSymbol = shortSymbol;
            this.chineseName = chineseName;
        }
        /// <summary>
        /// 中国名称
        /// </summary>
        public string ChineseName
        {
            get { return this.chineseName; }
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
