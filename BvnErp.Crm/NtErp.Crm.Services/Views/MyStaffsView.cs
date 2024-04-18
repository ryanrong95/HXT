using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class MyStaffsView : AdminTopView
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        MyStaffsView()
        {

        }

        //人员对象
        AdminTop Admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员ID</param>
        public MyStaffsView(IGenericAdmin admin)
        {
            this.Admin = AdminExtends.GetTop(admin.ID);
        }

        public MyStaffsView(AdminTop admin, BvCrmReponsitory reponsitory) : base(reponsitory)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">当前人员对象</param>
        /// <param name="reponsitory">数据库实体</param>
        internal MyStaffsView(IGenericAdmin admin, BvCrmReponsitory reponsitory) : base(reponsitory)
        {
            this.Admin = AdminExtends.GetTop(admin.ID);
        }

        /// <summary>
        /// 获取人员数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<AdminTop> GetIQueryable()
        {
            //获取当前用户的所有子员工
            string[] mystaffids = new DistrictTree(this.Admin.ID).AdminDescendants;

            if(Admin.JobType == JobType.TPM)
            {
                return base.GetIQueryable();
            }
            else
            {
                return base.GetIQueryable().Where(item => mystaffids.Contains(item.ID));
            }
        }
    }
}