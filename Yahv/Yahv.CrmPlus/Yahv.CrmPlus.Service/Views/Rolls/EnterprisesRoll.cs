using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class EnterprisesRoll : Linq.UniqueView<Enterprise, PvdCrmReponsitory>
    {
        bool isdraft;
        public EnterprisesRoll(bool IsDraft = false)
        {
            this.isdraft = IsDraft;
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected EnterprisesRoll(PvdCrmReponsitory reponsitory, IQueryable<Enterprise> iQueryable) : base(reponsitory, iQueryable)
        {

        }
        protected override IQueryable<Enterprise> GetIQueryable()
        {
            return new EnterprisesOrigin(this.Reponsitory).Where(item => item.IsDraft == isdraft);
        }
    }
}


