using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class TaxManagesModel
    {
        private TaxManage[] TaxManages { get; set; }

        public TaxManagesModel(TaxManage[] taxManages)
        {
            this.TaxManages = taxManages;
        }

        public void BatchInsert()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var layerTaxManages = this.TaxManages.Select(item => new Layer.Data.Sqls.ScCustoms.TaxManage
                {
                    ID = item.ID,
                    InvoiceCode = item.InvoiceCode,
                    InvoiceNo = item.InvoiceNo,
                    InvoiceDate = item.InvoiceDate,
                    SellsName = item.SellsName,
                    Amount = item.Amount,
                    VaildAmount = item.VaildAmount,
                    ConfrimDate = item.ConfrimDate,
                    AuthenticationMonth = item.AuthenticationMonth,
                    IsVaild = (int)item.IsVaild,
                    InvoiceDetailID = item.InvoiceDetailID,
                    InvoiceType = (int)item.InvoiceType,
                    BusinessType = (int)item.BusinessType,
                    Status = (int)item.Status,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate,
                    Summary = item.Summary,
                }).ToArray();
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxManage>(layerTaxManages);
            }
        }
    }
}
