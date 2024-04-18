using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DeliveryNoticeView : UniqueView<Models.DeliveryNotice, ScCustomsReponsitory>
    {
        public DeliveryNoticeView()
        {
        }

        internal DeliveryNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        
        /// <summary>
        /// 提货通知视图
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.DeliveryNotice> GetIQueryable()
        {
            var orderView = new Views.OrdersView(this.Reponsitory);
            var deliveryConsigneeView = new Views.DeliveryConsigneeView(this.Reponsitory);
            var adminView = new Views.AdminsTopView(this.Reponsitory);

            var result = from deliveryNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveryNotices>()
                         join deliveyConsignee in deliveryConsigneeView on deliveryNotice.ID equals deliveyConsignee.DeliveryNoticeID
                         join order in orderView on deliveryNotice.OrderID equals order.ID
                         join admin in adminView on deliveryNotice.AdminID equals admin.ID
                         where deliveryNotice.Status == (int)Enums.Status.Normal
                         select new Models.DeliveryNotice
                         {
                             ID = deliveryNotice.ID,
                             Order = order,
                             DeliveryConsignees = deliveyConsignee,
                             DeliveryNoticeStatus = (Enums.DeliveryNoticeStatus)deliveryNotice.DeliverNoticeStatus,
                             Status = (Enums.Status)deliveryNotice.Status,
                             CreateDate = deliveryNotice.CreateDate,
                             UpdateDate = deliveryNotice.UpdateDate,
                             Summary = deliveryNotice.Summary,
                             Admin = admin,

                         };
            return result;
        }
    }
}
