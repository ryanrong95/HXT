using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class MyConsigneesView : WsConsigneesTopView<PvbCrmReponsitory>
    {
        private string enterpriseid;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyConsigneesView()
        {

        }

        /// <summary>
        /// 当前客户的收货人
        /// </summary>
        /// <param name="EnterpriseID">客户ID</param>
        public MyConsigneesView(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }

        /// <summary>
        /// 查询结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Consignee> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterpriseid);
        }
    }
}
