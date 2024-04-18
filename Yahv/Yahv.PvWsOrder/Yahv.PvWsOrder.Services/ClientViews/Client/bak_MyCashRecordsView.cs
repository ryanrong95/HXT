//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using Layers.Data.Sqls;
//using Yahv.Linq.Generic;
//using Yahv.Services.Models;
//using Yahv.Services.Views.Payment;

//namespace Yahv.PvWsOrder.Services.ClientViews
//{
//    public class MyCashRecordsView : CashRecordsTopView<PvWsOrderReponsitory>
//    {
//        private string clientid;

//        /// <summary>
//        /// 无参构造函数
//        /// </summary>
//        private MyCashRecordsView()
//        {

//        }

//        /// <summary>
//        /// 带参构造函数
//        /// </summary>
//        /// <param name="EnterpriseID">客户ID</param>
//        public MyCashRecordsView(string EnterpriseID)
//        {
//            this.clientid = EnterpriseID;
//        }


//        /// <summary>
//        /// 数据结果查询
//        /// </summary>
//        /// <returns></returns>
//        protected override IQueryable<FlowAccount> GetIQueryable()
//        {
//            var companyid = PvClientConfig.CompanyID;

//            return base.GetIQueryable().Where(item => item.Payer == clientid && item.Payee == companyid);
//        }

//        /// <summary>
//        /// 根据传入参数获取订单数据
//        /// </summary>
//        /// <param name="expressions"></param>
//        /// <returns></returns>
//        virtual protected IQueryable<FlowAccount> GetOrders(LambdaExpression[] expressions)
//        {
//            var linq = this.GetIQueryable();
//            if (expressions != null)
//            {
//                foreach (var expression in expressions)
//                {
//                    linq = linq.Where(expression as Expression<Func<FlowAccount, bool>>);
//                }
//            }
//            return linq;
//        }

//        /// <summary>
//        /// 根据传入参数和查询条件获取数据
//        /// </summary>
//        /// <param name="expressions"></param>
//        /// <param name="PageSize"></param>
//        /// <param name="PageIndex"></param>
//        /// <returns></returns>
//        public PageList<FlowAccount> GetPageListOrders(LambdaExpression[] expressions, int PageSize, int PageIndex)
//        {
//            var results = this.GetOrders(expressions).ToArray();
//            int total = results.Count();
//            var linq = results.Skip(PageSize * (PageIndex - 1)).Take(PageSize);
//            return new PageList<FlowAccount>(PageIndex, PageSize, linq, total);
//        }
//    }
//}
