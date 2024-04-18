using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class MyChargesView : ChargeAlls
    {
        //人员对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyChargesView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyChargesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 获取费用数据
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Charge> GetIQueryable()
        {
            var data = from charges in base.GetIQueryable()
                       select charges;
            return data;
        }
    }
}
