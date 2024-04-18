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
    public class MyWorkWarningsView : WorkWarningsAlls
    {
        //人员对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        MyWorkWarningsView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyWorkWarningsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 获取工作提醒数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<WorkWarning> GetIQueryable()
        {
            return from workwarning in base.GetIQueryable()
                   where workwarning.Admin.ID == this.Admin.ID
                   orderby workwarning.UpdateDate descending
                   select workwarning;
        }
    }
}
