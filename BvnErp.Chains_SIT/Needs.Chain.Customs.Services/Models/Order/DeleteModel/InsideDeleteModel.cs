using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 内单删除型号 Do
    /// </summary>
    public class InsideDeleteModel : DeleteModelDo
    {
        private Layer.Data.Sqls.ScCustomsReponsitory Reponsitory { get; set; }

        private string OrderItemID { get; set; } = string.Empty;

        private string SortingID { get; set; } = string.Empty;

        private string OrderID { get; set; } = string.Empty;

        private Layer.Data.Sqls.ScCustoms.OrderControls[] OldOrderControls { get; set; }

        private Layer.Data.Sqls.ScCustoms.OrderControlSteps[] OldOrderControlSteps { get; set; }

        public InsideDeleteModel(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, string orderItemID, string sortingID, string orderID,
            Layer.Data.Sqls.ScCustoms.OrderControls[] oldOrderControls,
            Layer.Data.Sqls.ScCustoms.OrderControlSteps[] oldOrderControlSteps)
        {
            this.Reponsitory = reponsitory;
            this.OrderItemID = orderItemID;
            this.SortingID = sortingID;
            this.OrderID = orderID;
            this.OldOrderControls = oldOrderControls;
            this.OldOrderControlSteps = oldOrderControlSteps;
        }

        public override void Do()
        {
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderPremiums>(item => item.OrderItemID == this.OrderItemID);
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(item => item.OrderItemID == this.OrderItemID);
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(item => item.OrderItemID == this.OrderItemID);
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(item => item.OrderItemID == this.OrderItemID);
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItems>(item => item.ID == this.OrderItemID);
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecLists>(item => item.OrderItemID == this.OrderItemID);

            if (!string.IsNullOrEmpty(this.SortingID))
            {
                this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.PackingItems>(item => item.SortingID == this.SortingID);
                this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>(item => item.SortingID == this.SortingID);
                this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.Sortings>(item => item.ID == this.SortingID);
            }

            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.StoreStorages>(item => item.OrderItemID == this.OrderItemID);

            //删除与该型号有关的 OrderControl 、OrderControlStep
            this.AbandonThisOrderItemControl(this.Reponsitory, this.OrderItemID, this.OrderID, this.OldOrderControls, this.OldOrderControlSteps);
        }
    }
}
