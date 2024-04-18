using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class CostApplyPayeesOrigin : UniqueView<Models.CostApplyPayee, ScCustomsReponsitory>
    {
        public CostApplyPayeesOrigin()
        {
        }

        public CostApplyPayeesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CostApplyPayee> GetIQueryable()
        {
            return from costApplyPayee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplyPayees>()
                   select new Models.CostApplyPayee
                   {
                       ID = costApplyPayee.ID,
                       AdminID = costApplyPayee.AdminID,
                       PayeeName = costApplyPayee.PayeeName,
                       PayeeAccount = costApplyPayee.PayeeAccount,
                       PayeeBank = costApplyPayee.PayeeBank,
                       Status = (Enums.Status)costApplyPayee.Status,
                       CreateDate = costApplyPayee.CreateDate,
                       UpdateDate = costApplyPayee.UpdateDate,
                       Summary = costApplyPayee.Summary,
                   };
        }
    }
}
