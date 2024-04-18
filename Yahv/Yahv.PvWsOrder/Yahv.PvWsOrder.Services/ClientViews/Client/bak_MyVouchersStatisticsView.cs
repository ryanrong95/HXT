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
//    /// 客户账单视图
//    /// </summary>
//    public class MyVouchersStatisticsView : VouchersStatisticsView<PvWsOrderReponsitory>
//    {
//        private string clientid;

//        private MyVouchersStatisticsView() { }

//        public MyVouchersStatisticsView(string EnterpriseID)
//        {
//            this.clientid = EnterpriseID;
//        }

//        protected override IQueryable<VoucherStatistic> GetIQueryable()
//        {
//            //获取当前平台公司ID
//            string CompanyID = PvClientConfig.CompanyID;

//            var linq = base.GetIQueryable().Where(item =>item.Payee == CompanyID && item.Payer == this.clientid);

//            return linq;
//        }
//    }
//}
