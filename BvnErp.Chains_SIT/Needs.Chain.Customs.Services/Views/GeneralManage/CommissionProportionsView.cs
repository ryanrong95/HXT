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
    public class CommissionProportionsView : UniqueView<Models.CommissionProportion, ScCustomsReponsitory>
    {
        public CommissionProportionsView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public CommissionProportionsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<CommissionProportion> GetIQueryable()
        {
            var result = from commissionproportion in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CommissionProportions>()
                         where commissionproportion.Status == (int)Enums.Status.Normal
                         select new Models.CommissionProportion
                         {
                             ID = commissionproportion.ID,
                             Proportion = commissionproportion.Proportion,
                             RegeisterMonth = commissionproportion.RegeisterMonth,
                             Status = (Enums.Status)commissionproportion.Status,
                             Summary = commissionproportion.Summary,
                             UpdateDate = commissionproportion.UpdateDate,
                             CreateDate = commissionproportion.CreateDate,
                         };
            return result;
        }
    }
}
