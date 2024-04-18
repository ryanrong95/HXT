using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class OrderTracesView : UniqueView<XDTOrderTrace, ScCustomReponsitory>
    {
        public OrderTracesView()
        {

        }

        internal OrderTracesView(ScCustomReponsitory res, IQueryable<XDTOrderTrace> iQuery) : base(res, iQuery)
        {

        }

        protected override IQueryable<XDTOrderTrace> GetIQueryable()
        {
            return from trace in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderTraces>()
                   orderby trace.CreateDate descending
                   select new XDTOrderTrace
                   {
                       ID = trace.ID,
                       OrderID = trace.OrderID,
                       Step = (XDTOrderTraceStep)trace.Step,
                       CreateDate = trace.CreateDate,
                       Summary = trace.Summary,
                   };
        }


        /// <summary>
        /// 根据OrderID查询订单轨迹数据
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public OrderTracesView SearchByOrderID(string OrderID)
        {
            var iQuery = this.IQueryable.Where(item => item.OrderID == OrderID);

            return new OrderTracesView(this.Reponsitory, iQuery);
        }
    }

    public class XDTOrderTrace : IUnique
    {
        #region 属性
        public string ID { get; set; }

        public string OrderID { get; set; }

        public string AdminID { get; set; }

        public string UserID { get; set; }

        public XDTOrderTraceStep Step { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }
        #endregion
    }

    public enum XDTOrderTraceStep
    {
        [Description("已下单")]
        Submitted = 1,

        [Description("订单处理中")]
        Processing = 2,

        [Description("香港仓库处理中")]
        HKProcessing = 3,

        [Description(" 报关中")]
        Declaring = 4,

        [Description("运输中")]
        InTransit = 5,

        [Description("深圳库房处理中")]
        SZProcessing = 6,

        [Description("派送中")]
        Delivering = 7,

        [Description("已提货")]
        PickUp = 8,

        [Description("订单已完成")]
        Completed = 9,

        [Description("订单异常")]
        Anomaly = 10, s
    }
}
