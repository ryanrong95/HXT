using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   public class CheckContext
    {
        public List<CollectData> CheckData { get; set; }
        public CheckContext(List<CollectData> data)
        {
            this.CheckData = data;
        }

        public CheckReturnData Check()
        {
            CheckReturnData data = new CheckReturnData();
            data.Data = "";
            bool isSuccess = true;
            List<string> orderIds = this.CheckData.Select(t => t.OrderID).ToList();
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var PayExchangeItems = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                                        where orderIds.Contains(c.OrderID)
                                        select new
                                        {
                                            OrderID = c.OrderID,
                                            PayExchangeID = c.PayExchangeApplyID,
                                            PayExchangeItemID = c.ID
                                        }).ToList();

                var PayExchangeIds = PayExchangeItems.Select(t => t.PayExchangeID).ToList();

                var PayExchanges = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>()
                                    where PayExchangeIds.Contains(c.ID)
                                    select new
                                    {
                                        PayExchangeID = c.ID,
                                        PayExchangeApplyStatus = c.PayExchangeApplyStatus
                                    }).ToList();


                if(PayExchanges.Any(t=>t.PayExchangeApplyStatus==(int)Enums.PayExchangeApplyStatus.Auditing))
                {
                    isSuccess = false;
                    var payExchangeIds = PayExchanges.Where(t => t.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Auditing).Select(t => t.PayExchangeID).ToList();
                    var OrderIds = PayExchangeItems.Where(t => payExchangeIds.Contains(t.PayExchangeID)).Select(t => t.OrderID).Distinct().ToList();
                    data.Data = string.Join(",", OrderIds.ToArray());
                }
            }

            data.Success = isSuccess;

            return data;
        }
    }
}
