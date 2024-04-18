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
    public class WorkWarningsAlls : UniqueView<WorkWarning, BvCrmReponsitory>, Needs.Underly.IFkoView<WorkWarning>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public WorkWarningsAlls()
        {

        }

        /// <summary>
        /// 获取工作提醒数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<WorkWarning> GetIQueryable()
        {
            AdminTopView adminview = new AdminTopView(this.Reponsitory);

            return from workwarning in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Warnings>()
                   join admin in adminview on workwarning.AdminID equals admin.ID
                   select new WorkWarning
                   {
                       ID = workwarning.ID,
                       Type = (WarningType)workwarning.Type,
                       Status = (WarningStatus)workwarning.Status,
                       CreateDate = workwarning.CreateDate,
                       UpdateDate = workwarning.UpdateDate,
                       Summary = workwarning.Summary,
                       Admin = admin,
                       MainID = workwarning.MainID,
                       Resource = workwarning.Resource,
                   };
        }
    }
}
