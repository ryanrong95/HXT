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
//    /// 账户应付款
//    /// </summary>
//    public class MyAccountPayableView : AccountsPayableTopView<PvWsOrderReponsitory>
//    {
//        //客户ID
//        private string Clientid;

//        /// <summary>
//        /// 无参构造函数，不可访问
//        /// </summary>
//        private MyAccountPayableView()
//        {

//        }

//        /// <summary>
//        /// 带参构造函数，可访问
//        /// </summary>
//        /// <param name="ClientID"></param>
//        public MyAccountPayableView(string ClientID)
//        {
//            this.Clientid = ClientID;
//        }


//        protected override IQueryable<Balance> GetIQueryable()
//        {
//            //获取当前平台公司ID
//            var CompanyID = PvClientConfig.CompanyID;

//            var linq = base.GetIQueryable() //&& item.Currency == Currency.CNY)
//                .Where(item => item.Payee == CompanyID && item.Payer == this.Clientid);

//            return linq;
//        }
//    }
//}
