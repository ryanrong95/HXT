using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.Order
{
    public class BoxStrategy : BaseStrategy
    {
        public BoxStrategy(List<InsideOrderItem> Models):base(Models)
        {

        }

       
        public override List<OrderModel> SplitOrder()
        {
            List<OrderModel> orderOne = new List<OrderModel>();
            List<dyjPackingModel> wouldBeOrders = new List<dyjPackingModel>();
           
            //按箱子拆分订单
            //   箱子里的型号数不超过50，凑
            //   箱子里的型号数超过50，则一个箱子就是一个wouldbeOrder，输出型号的时候再分50一单
            var caseNos = base.models.Select(item => item.PackNo).Distinct().ToList();
            int allcount = 0;
            foreach (var caseNo in caseNos)
            {
                bool isfind = false;
                allcount = 0;
                allcount = base.models.Where(item => item.PackNo == caseNo).Count();
                foreach (var t in wouldBeOrders)
                {
                    if (t.AllCount + allcount <= 50)
                    {
                        t.AllCount += allcount;
                        t.boxes.Add(caseNo);
                        isfind = true;
                        break;
                    }
                }

                if (!isfind)
                {
                    dyjPackingModel packmodel = new dyjPackingModel();
                    packmodel.AllCount = allcount;
                    packmodel.boxes.Add(caseNo);
                    wouldBeOrders.Add(packmodel);
                }
            }

            // 按ProductUniqueCode 输出
            foreach (var p in wouldBeOrders)
            {
                var ProcessModel = base.models.Where(item => p.boxes.Contains(item.PackNo)).ToList();
                if (ProcessModel.Count <= 50)
                {
                    List<string> ProductUniqueCodes = new List<string>();
                    foreach(var t in ProcessModel)
                    {
                        ProductUniqueCodes.Add(t.PreProductID);
                    }
                    OrderModel om = new OrderModel();
                    om.ProductUniqueCodes = ProductUniqueCodes;
                    orderOne.Add(om);
                }
                else
                {
                    var limit = 50;
                    for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(ProcessModel.Count() / 50M)); i++)
                    {
                        var mc = ProcessModel.Skip(i * limit).Take(limit).ToList();
                        List<string> ProductUniqueCodes = new List<string>();
                        foreach (var t in mc)
                        {
                            ProductUniqueCodes.Add(t.PreProductID);
                        }
                        OrderModel om = new OrderModel();
                        om.ProductUniqueCodes = ProductUniqueCodes;
                        orderOne.Add(om);
                    }
                }
            }


            return orderOne;
        }
    }
}
