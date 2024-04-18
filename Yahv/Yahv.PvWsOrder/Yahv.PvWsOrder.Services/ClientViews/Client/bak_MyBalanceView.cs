//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Layers.Data.Sqls;
//using Yahv.Services.Models;
//using Yahv.Services.Views;
//using Yahv.Underly;

//namespace Yahv.PvWsOrder.Services.ClientViews
//{
//    /// <summary>
//    /// 账户余额
//    /// </summary>
//    public class MyBalanceView : BalancesTopView<PvWsOrderReponsitory>
//    {
//        //客户ID
//        private string clientid;

//        /// <summary>
//        /// 无参构造函数
//        /// </summary>
//        private MyBalanceView()
//        {

//        }

//        /// <summary>
//        /// 可访问的带参构造函数
//        /// </summary>
//        /// <param name="EnterpriseID">当前客户ID</param>
//        public MyBalanceView(string EnterpriseID)
//        {
//            this.clientid = EnterpriseID;
//        }

//        /// <summary>
//        /// 查询结果集
//        /// </summary>
//        /// <returns></returns>
//        protected override IQueryable<Balance> GetIQueryable()
//        {
//            //获取当前平台公司ID
//            string CompanyID = PvClientConfig.CompanyID;

//            var linq = base.GetIQueryable().Where(item => item.Business == "代仓储"  || item.Business=="代报关")// && item.Currency == Currency.CNY)
//                .Where(item => item.Payee == CompanyID && item.Payer == this.clientid);

//            return linq;
//        }
//    }
//}
