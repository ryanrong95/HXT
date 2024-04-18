using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class MyContactsView : ContactsTopView<PvWsOrderReponsitory>
    {
        private string enterpriseid;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyContactsView() : base(Business.WarehouseServicing)
        {

        }

        /// <summary>
        /// 当前客户的收货人
        /// </summary>
        /// <param name="EnterpriseID">客户ID</param>
        public MyContactsView(string EnterpriseID) : this()
        {
            this.enterpriseid = EnterpriseID;
        }

        /// <summary>
        /// 查询结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Contact> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterpriseid);
        }
    }
}
