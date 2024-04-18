using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecTaxGuaranteeView : UniqueView<Models.DecTaxGuarantee, ScCustomsReponsitory>
    {
        public DecTaxGuaranteeView()
        {
        }
        internal DecTaxGuaranteeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecTaxGuarantee> GetIQueryable()
        {
            return from gua in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxGuarantees>()
                   where gua.Status == (int)Enums.Status.Normal
                   select new Models.DecTaxGuarantee
                   {
                       ID = gua.ID,
                       GuaranteeNo = gua.GuaranteeNo,
                       PutOnCustoms = gua.PutOnCustoms,
                       GuaranteeAmount = gua.GuaranteeAmount,
                       RemainAmount = gua.RemainAmount,
                       BankName = gua.BankName,
                       ApproveDate = gua.ApproveDate,
                       ValidDate = gua.ValidDate,
                       Status = (Enums.Status)gua.Status,
                       CreateDate = gua.CreateDate,
                       UpdateDate = gua.UpdateDate,
                       Summary = gua.Summary

                   };
        }
    }
}
