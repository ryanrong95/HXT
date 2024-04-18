using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class MyWorksWeeklyView : WorksWeeklyAlls
    {
        //员工对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        MyWorksWeeklyView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyWorksWeeklyView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 获取工作周报数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<WorksWeekly> GetIQueryable()
        {
            //我的员工视图
            var mystaffs = new MyStaffsView(this.Admin, Reponsitory);

            return from worksweekly in base.GetIQueryable()
                   join mystaff in mystaffs on worksweekly.Admin.ID equals mystaff.ID
                   where worksweekly.Status != Enums.Status.Delete
                   orderby worksweekly.UpdateDate descending
                   select worksweekly;
        }

        /// <summary>
        /// 删除上传文件
        /// </summary>
        /// <param name="WorksWeeklyID">工作周报ID</param>
        public void DeleteFiles(string WorksWeeklyID)
        {
            var workweekly = this[WorksWeeklyID];
            if (workweekly != null)
            {
                Reponsitory.Update<Layer.Data.Sqls.BvCrm.Files>(new
                {
                    Status = Enums.Status.Delete
                }, item => item.WorksWeeklyID == workweekly.ID);
            }
        }
    }
}
