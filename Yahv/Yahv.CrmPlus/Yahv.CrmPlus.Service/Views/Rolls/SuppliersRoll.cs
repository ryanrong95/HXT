using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class SuppliersRoll : Linq.UniqueView<Supplier, PvdCrmReponsitory>
    {
        public SuppliersRoll()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SuppliersRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Supplier> GetIQueryable()
        {
            return new SuppliersOrigin(this.Reponsitory);
        }
        public IQueryable<Supplier> this[bool IsDraft]
        {
            get
            {
                return this.Where(item => item.IsDraft == IsDraft);
            }
        }
        //public IQueryable<Supplier> this[AuditStatus Status]
        //{
        //    get
        //    {
        //        return this.Where(item => item.Enterprise.IsDraft == false && item.Status == Status);
        //    }
        //}
    }

}


