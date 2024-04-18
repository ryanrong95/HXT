using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Services.Views;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class MyPersonInvoicesView : vInvoicesTopView<PvbCrmReponsitory>
    {
        private string enterpriseid;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyPersonInvoicesView()
        {

        }

        /// <summary>
        /// 当前客户的收货人
        /// </summary>
        /// <param name="EnterpriseID">客户ID</param>
        public MyPersonInvoicesView(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }

        /// <summary>
        /// 查询结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<vInvoice> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterpriseid);
        }
    }
}
