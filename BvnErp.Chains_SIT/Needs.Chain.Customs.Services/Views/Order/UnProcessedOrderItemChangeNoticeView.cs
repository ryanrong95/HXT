using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class UnProcessedOrderItemChangeNoticeView : UniqueView<UnProcessedOrderItemChangeNoticeViewModel, ScCustomsReponsitory>
    {
        public IQueryable<UnProcessedOrderItemChangeNoticeViewModel> GetQueryable(string orderID)
        {
            var orderItemChangeNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>();
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();

            var linq = from orderItemChangeNotice in orderItemChangeNotices
                       join orderItem in orderItems
                            on new
                            {
                                OrderItemID = orderItemChangeNotice.OrderItemID,
                                OrderItemChangeNoticeDataStatus = orderItemChangeNotice.Status,
                                OrderItemDataStatus = (int)Enums.Status.Normal,
                                OrderID = orderID,
                            }
                            equals new
                            {
                                OrderItemID = orderItem.ID,
                                OrderItemChangeNoticeDataStatus = (int)Enums.Status.Normal,
                                OrderItemDataStatus = orderItem.Status,
                                OrderID = orderItem.OrderID,
                            }
                       where orderItemChangeNotice.ProcessStatus != (int)Enums.ProcessState.Processed
                       orderby orderItemChangeNotice.UpdateDate
                       select new UnProcessedOrderItemChangeNoticeViewModel
                       {
                           Model = orderItem.Model,
                           OrderItemChangeType = (Enums.OrderItemChangeType)orderItemChangeNotice.Type,
                       };

            return linq;
        }

        protected override IQueryable<UnProcessedOrderItemChangeNoticeViewModel> GetIQueryable()
        {
            throw new NotImplementedException();
        }
    }

    public class UnProcessedOrderItemChangeNoticeViewModel : IUnique
    {
        public string ID { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public Enums.OrderItemChangeType OrderItemChangeType { get; set; }
    }
}
