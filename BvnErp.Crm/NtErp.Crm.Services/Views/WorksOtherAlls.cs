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
    public class WorksOtherAlls : UniqueView<WorksOther, BvCrmReponsitory>, Needs.Underly.IFkoView<WorksOther>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public WorksOtherAlls()
        {

        }

        /// <summary>
        /// 获取工作记录数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<WorksOther> GetIQueryable()
        {
            AdminTopView topview = new AdminTopView(this.Reponsitory); //人员视图

            return from worksother in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.WorksOther>()
                   join admin in topview on worksother.AdminID equals admin.ID
                   select new WorksOther
                   {
                       ID = worksother.ID,
                       Admin = admin,
                       Context = worksother.Context,
                       StartDate = worksother.StartDate,
                       Status = (Status)worksother.Status,
                       CreateDate = worksother.CreateDate,
                       UpdateDate = worksother.UpdateDate,
                       Subject = worksother.Subject,
                   };
        }
    }
}
