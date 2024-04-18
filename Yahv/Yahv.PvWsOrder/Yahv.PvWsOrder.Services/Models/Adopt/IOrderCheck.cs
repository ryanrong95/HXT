using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models.Adopt
{
    public interface IOrderCheck
    {      
        bool isVaildOrder(string orderID);

        void UpdateFee(string orderID, decimal orderFee,string adminID);
    }
}
