using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly.Legals
{
    public interface ICurrency : ILegal
    {
        string ShortName { get; }
        string Symbol { get; }
        string ShortSymbol { get; }
    }

}
