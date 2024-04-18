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
    public class MyIndustriesView:IndustryAlls
    {
        //人员对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        MyIndustriesView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyIndustriesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 获取行业数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Industry> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.Status != Enums.Status.Delete
                   select entity;
        }
    }
}
