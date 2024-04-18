using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class ApplyAlls : UniqueView<Apply, BvCrmReponsitory>, Needs.Underly.IFkoView<Apply>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ApplyAlls()
        {

        }

        internal ApplyAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 获取审批数据
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Apply> GetIQueryable()
        {
            //人员视图
            AdminTopView adminview = new AdminTopView(this.Reponsitory);
            return from applies in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Applies>()
                   join admin in adminview on applies.AdminID equals admin.ID
                   orderby applies.CreateDate descending
                   select new Apply
                   {
                       ID = applies.ID,
                       Type = (ApplyType)applies.Type,
                       MainID = applies.MainID,
                       Admin = admin,
                       Status = (ApplyStatus)applies.Status,
                       CreateDate = applies.CreateDate,
                       UpdateDate = applies.UpdateDate,
                       Summary = applies.Summary
                   };
        }
    }
}
