using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 员工工资
    /// </summary>
    public class PayWageItemRoll : QueryView<Yahv.Erm.Services.Models.Rolls.PayWageItem1, PvbErmReponsitory>
    {
        public PayWageItemRoll()
        {
        }

        protected override IQueryable<Yahv.Erm.Services.Models.Rolls.PayWageItem1> GetIQueryable()
        {
            var result = from pi in Reponsitory.ReadTable<PayItems>()
                         join pb in Reponsitory.ReadTable<PayBills>() on pi.PayID equals pb.ID
                         join wi in Reponsitory.ReadTable<WageItems>() on pi.Name equals wi.Name
                         select new PayWageItem1()
                         {
                             Name = pi.Name,
                             Value = pi.Value,
                             Type = (WageItemType)wi.Type,
                             Order = wi.CalcOrder,
                             Formula = wi.Formula,
                             DateIndex = pi.DateIndex,
                             PayID = pi.PayID,
                         };

            return result;
        }
    }
}
