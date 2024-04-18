using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceOut : IOutGoodsAdd
    {
        /// <summary>
        /// 如果出库月份是之前的插入outGoods
        /// 如果出库月份是当月的，删除outGoods
        /// </summary>
        public override void addOutGoods()
        {
            if (base.isNeedToOutGoods())
            {
                DateTime dt = DateTime.Now;
                string stFirst = dt.ToString("yyyy-MM");
                DateTime dtFirst = Convert.ToDateTime(stFirst + "-01");
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    Dictionary<string, DateTime> dtOut = checkOutDate(reponsitory);                  
                    foreach (var item in OrderItems)
                    {
                        if (dtOut.ContainsKey(item))
                        {
                            if (dtOut[item] >= dtFirst)
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OutGoods>(new
                                {
                                    Status = (int)Enums.Status.Delete,
                                    UpdateDate = DateTime.Now
                                }, t => t.OrderItemID == item);
                            }
                            else
                            {
                                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OutGoods>().Where(t => t.OrderItemID == item).Count();
                                if (count == 0)
                                {
                                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OutGoods>(new Layer.Data.Sqls.ScCustoms.OutGoods
                                    {
                                        ID = ChainsGuid.NewGuidUp(),
                                        ClientID = order.Client.ID,
                                        OrderItemID = item,
                                        StorageDate = dtOut[item],
                                        Status = (int)Enums.Status.Normal,
                                        CreateDate = DateTime.Now,
                                        UpdateDate = DateTime.Now
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检查是否是之前月份出库的
        /// </summary>
        private Dictionary<string, DateTime> checkOutDate(Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            Dictionary<string, DateTime> result = new Dictionary<string, DateTime>();
            var outView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_OutView>().Where(t => OrderItems.Contains(t.OrderItemID)).ToList();
            foreach (var t in outView)
            {
                result.Add(t.OrderItemID, t.CreateDate);
            }

            return result;
        }


    }
}
