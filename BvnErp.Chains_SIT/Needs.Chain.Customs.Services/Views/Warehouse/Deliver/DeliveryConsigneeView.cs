using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 提货信息视图
    /// </summary>
    public class DeliveryConsigneeView : UniqueView<Models.DeliveryConsignee, ScCustomsReponsitory>
    {
        public DeliveryConsigneeView()
        {
        }

        internal DeliveryConsigneeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DeliveryConsignee> GetIQueryable()
        {
            return from deliveryConsigneeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveryConsignees>()
                   where deliveryConsigneeItem.Status == (int)Enums.Status.Normal
                   select new Models.DeliveryConsignee
                   {
                       ID = deliveryConsigneeItem.ID,
                       DeliveryNoticeID = deliveryConsigneeItem.DeliveryNoticeID,
                       Supplier = deliveryConsigneeItem.Supplier,
                       PickUpDate = deliveryConsigneeItem.PickUpDate,
                       Address = deliveryConsigneeItem.Address,
                       Contact = deliveryConsigneeItem.Contact,
                       Tel = deliveryConsigneeItem.Tel,
                       Status = (Enums.Status)deliveryConsigneeItem.Status,
                       CreateDate = deliveryConsigneeItem.CreateDate,
                       UpdateDate = deliveryConsigneeItem.UpdateDate,
                       Summary = deliveryConsigneeItem.Summary
                   };
        }
    }

}
