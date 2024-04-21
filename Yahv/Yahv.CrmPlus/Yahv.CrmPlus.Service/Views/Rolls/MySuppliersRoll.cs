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
    public class MySuppliersRoll : Linq.UniqueView<Supplier, PvdCrmReponsitory>
    {
        IErpAdmin admin;
        public MySuppliersRoll(IErpAdmin Admin)
        {
            this.admin = Admin;
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected MySuppliersRoll(PvdCrmReponsitory reponsitory, IQueryable<Supplier> iQueryable) : base(reponsitory, iQueryable)
        {

        }
        protected override IQueryable<Supplier> GetIQueryable()
        {
            return new SuppliersOrigin(this.Reponsitory).Where(item => item.IsDraft == true && item.CreatorID == this.admin.ID);
        }
    }
}


