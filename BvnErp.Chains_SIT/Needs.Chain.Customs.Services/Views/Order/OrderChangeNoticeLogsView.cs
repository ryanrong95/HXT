using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class OrderChangeNoticeLogsListView
    {
        private ScCustomsReponsitory Reponsitory;

        public OrderChangeNoticeLogsListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public OrderChangeNoticeLogsListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public class OrderChangeNoticeLogsListModel
        {
            /// <summary>
            /// ID
            /// </summary>
            public string ID { get; set; } = string.Empty;

            /// <summary>
            /// OrderID
            /// </summary>
            public string OrderID { get; set; } = string.Empty;

            /// <summary>
            /// OrderItemID
            /// </summary>
            public string OrderItemID { get; set; } = string.Empty;

            /// <summary>
            /// AdminID
            /// </summary>
            public string AdminID { get; set; } = string.Empty;

            /// <summary>
            /// 管理员姓名
            /// </summary>
            public string AdminName { get; set; } = string.Empty;

            /// <summary>
            /// CreateDate
            /// </summary>
            public DateTime CreateDate { get; set; }

            /// <summary>
            /// Summary
            /// </summary>
            public string Summary { get; set; } = string.Empty;

            public string OrderChangeNoticeID { get; set; }
        }

        public IEnumerable<OrderChangeNoticeLogsListModel> GetCommon(params LambdaExpression[] expressions)
        {
            var orderChangeNoticeLogs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNoticeLogs>();

            var baseLists = from orderChangeNoticeLog in orderChangeNoticeLogs
                            select new OrderChangeNoticeLogsListModel()
                            {
                                ID = orderChangeNoticeLog.ID,
                                OrderID = orderChangeNoticeLog.OrderID,
                                OrderItemID = orderChangeNoticeLog.OrderItemID,
                                AdminID = orderChangeNoticeLog.AdminID,
                                CreateDate = orderChangeNoticeLog.CreateDate,
                                Summary = orderChangeNoticeLog.Summary,
                                OrderChangeNoticeID = orderChangeNoticeLog.OrderChangeNoticeID
                            };

            foreach (var expression in expressions)
            {
                baseLists = baseLists.Where(expression as Expression<Func<OrderChangeNoticeLogsListModel, bool>>);
            }

            baseLists = baseLists.OrderByDescending(t => t.CreateDate);

            return baseLists;
        }


    }
}
