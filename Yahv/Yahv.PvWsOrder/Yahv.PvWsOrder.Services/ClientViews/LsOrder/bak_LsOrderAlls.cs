//using Layers.Data.Sqls;
//using System;
//using System.Linq;
//using System.Linq.Expressions;
//using Yahv.Linq;
//using Yahv.Linq.Generic;
//using Yahv.PvWsOrder.Services.ClientModels;
//using Yahv.PvWsOrder.Services.ClientViews;
//using Yahv.Services.Models;
//using Yahv.Services.Models.LsOrder;
//using Yahv.Services.Views;
//using Yahv.Underly;

//namespace Yahv.PvWsOrder.Services.ClientViews
//{
//    /// <summary>
//    /// 所有租赁订单视图
//    /// </summary>
//    public class LsOrderAlls : LsOrderTopView<PvLsOrderReponsitory>
//    {
//        private LsOrderType? Type;

//        protected LsOrderAlls()
//        {

//        }

//        protected LsOrderAlls(LsOrderType lsOrderType)
//        {
//            this.Type = lsOrderType;
//        }


//        protected override IQueryable<LsOrder> GetIQueryable()
//        {
//            var linq = base.GetIQueryable();

//            if (Type.HasValue)
//            {
//                linq = linq.Where(item => item.Type == Type.Value);
//            }

//            return linq;
//        }

//        /// <summary>
//        /// 根据传入参数获取订单数据
//        /// </summary>
//        /// <param name="expressions"></param>
//        /// <returns></returns>
//        virtual protected IQueryable<LsOrder> GetOrders(LambdaExpression[] expressions)
//        {
//            var orders = this.GetIQueryable();
//            if (expressions != null)
//            {
//                foreach (var expression in expressions)
//                {
//                    orders = orders.Where(expression as Expression<Func<LsOrder, bool>>);
//                }
//            }
//            return orders;
//        }

//        /// <summary>
//        /// 根据传入参数和查询条件获取数据
//        /// </summary>
//        /// <param name="expressions"></param>
//        /// <param name="PageSize"></param>
//        /// <param name="PageIndex"></param>
//        /// <returns></returns>
//        virtual protected PageList<LsOrder> GetPageListOrders(LambdaExpression[] expressions, int PageSize, int PageIndex)
//        {
//            var orders = this.GetOrders(expressions).ToArray();
//            int total = orders.Count();
//            var linq = orders.Skip(PageSize * (PageIndex - 1)).Take(PageSize);
//            return new PageList<LsOrder>(PageIndex, PageSize, linq, total);
//        }
//    }
//}
