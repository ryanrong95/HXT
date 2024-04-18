using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class MyPayerView : Yahv.Services.Views.wsPayersTopView<PvWsOrderReponsitory>
    {
        string EnterpriseID;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyPayerView()
        {

        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        public MyPayerView(string enterpriseid)
        {
            this.EnterpriseID = enterpriseid;
        }

        /// <summary>
        /// 获取结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<wsPayer> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.EnterpriseID);
        }
    }
}
