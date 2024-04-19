using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Wss.Sales.Services.Model;
using NtErp.Wss.Sales.Services.Models.Orders.Commodity;
using NtErp.Wss.Sales.Services.Models.SsoUsers;
using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Views
{
    public class CommodityInputsView : QueryView<CommodityInput, Layer.Data.Sqls.BvOrdersReponsitory>
    {

        string userid;
        public CommodityInputsView()
        {
        }
        public CommodityInputsView(string userid)
        {
            this.userid = userid;
        }

        protected override IQueryable<CommodityInput> GetIQueryable()
        {
            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOrders.CommodityInputs>()
                        select new CommodityInput
                        {
                            ID = entity.ID,
                            UserID = entity.UserID,
                            OrderID = entity.OrderID,
                            ServiceOuputID = entity.ServiceOuputID,
                            Count = entity.Count,
                            CreateDate = entity.CreateDate
                        };
            if (string.IsNullOrEmpty(this.userid))
            {
                return linqs;
            }
            else
            {
                linqs = linqs.Where(t => t.UserID == this.userid);
                return linqs;
            }
        }
    }
}
