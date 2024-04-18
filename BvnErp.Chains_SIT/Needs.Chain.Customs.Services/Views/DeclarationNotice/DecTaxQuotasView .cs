using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 海关税费额度预警的视图
    /// </summary>
    public class DecTaxQuotasView : UniqueView<Models.DecTaxQuota, ScCustomsReponsitory>
    {
        public DecTaxQuotasView()
        {
        }

        internal DecTaxQuotasView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.DecTaxQuota> GetIQueryable()
        {
           

            return from tax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxQuotas>()
                   select new Models.DecTaxQuota
                   {
                       ID = tax.ID,
                       DeclarationID = tax.DeclarationID,
                       ExciseTax = tax.ExciseTax,
                       AddedValueTax = tax.AddedValueTax,
                       Tariff = tax.Tariff,
                       PayStatus =(Enums.TaxStatus) tax.PayStatus,
                       Status =(Enums.Status) tax.Status,
                       CreateDate = tax.CreateDate,
                       UpdateDate = tax.UpdateDate,
                       Summary = tax.Summary
                   };
        }
    }


   
}
