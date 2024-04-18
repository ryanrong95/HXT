using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class StockOut : IOutGoodsAdd
    {
        /// <summary>
        /// 如果出库的时候，没有开票，则将记录插入到OutGoods
        /// </summary>
        public override void addOutGoods()
        {           
            if (base.isNeedToOutGoods())
            {
                 using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var invoiceNotices = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>().Where(t => OrderItems.Contains(t.OrderItemID)).ToList();
                    foreach (var item in OrderItems)
                    {
                        if (!invoiceNotices.Any(t => t.OrderItemID == item))
                        {
                            int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OutGoods>().Where(t => t.OrderItemID == item).Count();
                            if (count == 0)
                            {
                                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OutGoods>(new Layer.Data.Sqls.ScCustoms.OutGoods
                                {
                                    ID = ChainsGuid.NewGuidUp(),
                                    ClientID = order.Client.ID,
                                    OrderItemID = item,
                                    StorageDate = DateTime.Now,
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
}
