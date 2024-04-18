using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models.Adopt
{
    public class WsOrderCheck : IOrderCheck
    {
        public bool isVaildOrder(string orderID)
        {
            bool isVaild = false;
            try
            {
                using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
                {
                    int count = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>().Where(t => t.ID == orderID).Count();
                    if (count > 0)
                    {
                        isVaild = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return isVaild;
        }

        public void UpdateFee(string orderID, decimal orderFee, string adminID)
        {
            
        }
    }
}
