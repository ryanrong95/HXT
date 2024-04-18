using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Views
{
    public class TaxManageOriginView : UniqueView<Models.TaxManage, ScCustomsReponsitory>
    {
        public TaxManageOriginView()
        {
        }

        public TaxManageOriginView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.TaxManage> GetIQueryable()
        {
            return from taxManage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManage>()
                   select new Models.TaxManage
                   {
                       ID = taxManage.ID,
                       InvoiceCode = taxManage.InvoiceCode,
                       InvoiceNo = taxManage.InvoiceNo,
                       InvoiceDate = taxManage.InvoiceDate,
                       SellsName = taxManage.SellsName,
                       Amount = taxManage.Amount,
                       VaildAmount = taxManage.VaildAmount,
                       ConfrimDate = taxManage.ConfrimDate,
                       AuthenticationMonth = taxManage.AuthenticationMonth,
                       IsVaild = (Enums.InvoiceVaildStatus)taxManage.IsVaild,
                       InvoiceDetailID = taxManage.InvoiceDetailID,
                       InvoiceType = (Enums.InvoiceType)taxManage.InvoiceType,
                       BusinessType = (Enums.BusinessType)taxManage.BusinessType,
                       Status = (Enums.Status)taxManage.Status,
                       CreateDate = taxManage.CreateDate,
                       UpdateDate = taxManage.UpdateDate,
                       Summary = taxManage.Summary,
                   };
        }
    }
}
