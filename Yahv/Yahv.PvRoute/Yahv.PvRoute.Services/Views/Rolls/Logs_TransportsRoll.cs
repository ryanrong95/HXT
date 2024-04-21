using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvRoute.Services.Models;
using Yahv.PvRoute.Services.Views.Origins;

namespace Yahv.PvRoute.Services.Views.Rolls
{
    public class Logs_TransportsRoll : QueryView<Logs_Transport, PvRouteReponsitory>
    {
        public Logs_TransportsRoll()
        {

        }
        public Logs_TransportsRoll(PvRouteReponsitory reponsitory, IQueryable<Logs_Transport> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<Logs_Transport> GetIQueryable()
        {
            try
            {


                var logs_TransportOrigin = new Logs_TransportOrigin(this.Reponsitory);
                var transportConsigneeOrigin = new TransportConsigneeOrigin(this.Reponsitory);

                var iQuery = from tl in logs_TransportOrigin
                             join tc in transportConsigneeOrigin on tl.ConsigneeID equals tc.ID into transports
                             from tc in transports.DefaultIfEmpty()
                             select new Logs_Transport
                             {
                                 ID = tl.ID,
                                 FaceOrderID = tl.FaceOrderID,
                                 Summary = tl.Summary,
                                 CreateDate = tl.CreateDate,
                                 Json = tl.Json,
                                 Phone = tl.Phone,
                                 Contact = tl.Contact,
                                 ConsigneeID = tl.ConsigneeID,

                                 //TransportConsignee = tc,
                                 ConsigneeName=tc.Name
                             };

                return iQuery;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Logs_Transport> iquery = this.IQueryable.Cast<Logs_Transport>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_transportLog = iquery.ToArray();

            //if (ienum_transportLog.Count() == 0)
            //{
            //    //请求顺丰接口
            //}

            Func<Logs_Transport, object> convert = item => new
            {
                ID = item.ID,
                FaceOrderID = item.FaceOrderID,
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Json = item.Json,
                Phone = item.Phone,
                Contact = item.Contact,
                ConsigneeID = item.ConsigneeID,

                //TransportConsignee = item.TransportConsignee
                ConsigneeName=item.ConsigneeName

            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {

                };

                return ienum_transportLog.Select(convert).Select(convertAgain).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = ienum_transportLog.Select(convert).ToArray(),
            };
        }

        public Logs_TransportsRoll SearchByFaceOrderID(string faceOrderID)
        {

            //先去查询Logs_Transport表中是否有数据，有数据直接返回搜索结果（忽略，该订单的日志消息可能有更新）
            /*
             逻辑流程：显示的时候显示数据库的所有数据。通过faceOrderID查询的时候：
             1.先请求FaceOrder表的数据获得类型是顺丰还是跨越；
             2.根据不同的类型去分别请求顺丰，跨越的获取路由数据的接口，然后插入数据库；
             3.获取最新的查询结果。
             */

            using (var faceOrderRoll = new FaceOrdersRoll())
            {
                var faceOrders = from query in faceOrderRoll
                                 where query.ID.Contains(faceOrderID)
                                 select query;

                var faceOrder = faceOrders.SingleOrDefault();
                //if (faceOrder == null)
                //{
                //    throw new Exception("不存在该运单！！");
                //}
                if (faceOrder.Source == Enums.PrintSource.SF)
                {
                    //请求顺丰接口
                    SFHelper helper = new SFHelper();
                    helper.CreateFaceOrderLog(faceOrderID);

                }
                else if (faceOrder.Source == Enums.PrintSource.KYSY)
                {
                    //请求跨越速运接口
                    KYSYHelper helper = new KYSYHelper();
                    helper.CreateFaceOrderLog(faceOrderID);
                }
                //else
                //{
                //    throw new NotSupportedException($"目前只支持顺丰/跨越速运的接口，请重试!");
                //}

            }

            var linq = from query in this.IQueryable
                       where query.FaceOrderID.Contains(faceOrderID)
                       select query;

            var view = new Logs_TransportsRoll(this.Reponsitory, linq);
            return view;
        }
    }
}
