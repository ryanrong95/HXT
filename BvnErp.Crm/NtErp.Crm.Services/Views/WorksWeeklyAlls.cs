using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class WorksWeeklyAlls : UniqueView<WorksWeekly, BvCrmReponsitory>, Needs.Underly.IFkoView<WorksWeekly>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public WorksWeeklyAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public WorksWeeklyAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 获取工作周报数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<WorksWeekly> GetIQueryable()
        {
            AdminTopView topview = new AdminTopView(this.Reponsitory); //人员视图

            return from worksweekly in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.WorksWeekly>()
                   join admin in topview on worksweekly.AdminID equals admin.ID
                   select new WorksWeekly
                   {
                       ID = worksweekly.ID,
                       Admin = admin,
                       Context = worksweekly.Context,
                       WeekOfYear = worksweekly.WeekOfYear,
                       Status = (Enums.Status)worksweekly.Status,
                       CreateDate = worksweekly.CreateDate,
                       UpdateDate = worksweekly.UpdateDate,
                   };
        }
    }
}
