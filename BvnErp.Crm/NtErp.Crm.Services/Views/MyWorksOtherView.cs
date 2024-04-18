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
    public class MyWorksOtherView : WorksOtherAlls
    {
        //人员对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        MyWorksOtherView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyWorksOtherView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 获取工作记录数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<WorksOther> GetIQueryable()
        {
            //我的员工视图
            var mystaffs = new MyStaffsView(this.Admin, Reponsitory);

            return from worksother in base.GetIQueryable()
                   join mystaff in mystaffs on worksother.Admin.ID equals mystaff.ID
                   where worksother.Status != Enums.Status.Delete
                   orderby worksother.UpdateDate descending
                   select worksother;
        }


        /// <summary>
        /// 附件删除
        /// </summary>
        /// <param name="WorksOtherId">工作计划ID</param>
        public void DeleteFiles(string WorksOtherId)
        {
            var worksOther = this[WorksOtherId];
            if (worksOther != null)
            {
                Reponsitory.Update<Layer.Data.Sqls.BvCrm.Files>(new
                {
                    Status = Enums.Status.Delete
                }, item => item.WorksOtherID == worksOther.ID);
            }
        }
    }
}
