using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otr
{
    public interface IPricebreak
    {
        int Currency { get; set; }

        int Moq { get; set; }

        decimal Price { get; set; }
    }
}
