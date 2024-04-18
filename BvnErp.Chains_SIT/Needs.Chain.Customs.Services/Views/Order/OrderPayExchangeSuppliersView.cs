using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理订单付汇供应商的视图
    /// </summary>
    public class OrderPayExchangeSuppliersView : UniqueView<Models.OrderPayExchangeSupplier, ScCustomsReponsitory>
    {
        public OrderPayExchangeSuppliersView()
        {

        }

        internal OrderPayExchangeSuppliersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.OrderPayExchangeSupplier> GetIQueryable()
        {
            var clientSuppliersView = new ClientSuppliersView(this.Reponsitory);

            return from payExchangeSupplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers>()
                   join clientSupplier in clientSuppliersView on payExchangeSupplier.ClientSupplierID equals clientSupplier.ID
                   where payExchangeSupplier.Status == (int)Enums.Status.Normal
                   select new Models.OrderPayExchangeSupplier
                   {
                       ID = payExchangeSupplier.ID,
                       OrderID = payExchangeSupplier.OrderID,
                       ClientSupplier = clientSupplier,
                       Status = (Enums.Status)payExchangeSupplier.Status,
                       CreateDate = payExchangeSupplier.CreateDate,
                       UpdateDate = payExchangeSupplier.UpdateDate,
                       Summary = payExchangeSupplier.Summary,
                   };
        }
    }
}
