using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PaySupplierInfoView : UniqueView<Models.PaySupplierInfo, ScCustomsReponsitory>
    {
        public PaySupplierInfoView()
        {

        }

        public PaySupplierInfoView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PaySupplierInfo> GetIQueryable()
        {
            var paySupplierInfos = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaySupplierInfos>();

            return from paySupplierInfo in paySupplierInfos
                   select new Models.PaySupplierInfo
                   {
                       ID = paySupplierInfo.ID,
                       DecHeadID = paySupplierInfo.DecHeadID,
                       OrderID = paySupplierInfo.OrderID,
                       ClientID = paySupplierInfo.ClientID,
                       Amount = paySupplierInfo.Amount,
                       Currency = paySupplierInfo.Currency,
                       SupplierName = paySupplierInfo.SupplierName,
                       PayType = (Enums.PaySupplierPayType)paySupplierInfo.PayType,
                       Status = (Enums.Status)paySupplierInfo.Status,
                       CreateDate = paySupplierInfo.CreateDate,
                       UpdateDate = paySupplierInfo.UpdateDate,
                       Summary = paySupplierInfo.Summary,
                   };
        }
    }
}
