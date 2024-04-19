using NtErp.Wss.Sales.Services.Extends;
using NtErp.Wss.Sales.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models.Orders.Commodity
{
    public class CommodityOutput
    {
        public CommodityOutput()
        {
            this.CreateDate = DateTime.Now;
        }
        #region 属性
        public string ID { get; set; }
        public string InputID { get; set; }
        public string OrderID { get; set; }
        public string ServiceInputID { get; set; }
        public int Count { get; set; }
        public DateTime CreateDate { get; set; }

        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.CommodityOutput);
                    reponsitory.Insert(this.ToLinq());
                }
            }
        }
        #endregion
    }
}
