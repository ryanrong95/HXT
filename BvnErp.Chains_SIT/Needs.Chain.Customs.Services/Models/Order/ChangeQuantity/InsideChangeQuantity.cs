using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 内单修改数量 Do
    /// </summary>
    public class InsideChangeQuantity : IChangeQuantityDo
    {
        private Layer.Data.Sqls.ScCustomsReponsitory Reponsitory { get; set; }

        private string OrderItemID { get; set; } = string.Empty;

        private decimal NewQuantity { get; set; }

        private decimal UnitPrice { get; set; }

        private string SortingID { get; set; } = string.Empty;

        private string DecListID { get; set; }

        private decimal DeclPrice { get; set; }

        private Layer.Data.Sqls.ScCustoms.OrderControls[] OldOrderControls { get; set; }

        private Layer.Data.Sqls.ScCustoms.OrderControlSteps[] OldOrderControlSteps { get; set; }

        public InsideChangeQuantity(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, string orderItemID, decimal newQuantity, decimal unitPrice, string sortingID,
            string decListID, decimal declPrice)
        //Layer.Data.Sqls.ScCustoms.OrderControls[] oldOrderControls,
        //Layer.Data.Sqls.ScCustoms.OrderControlSteps[] oldOrderControlSteps)
        {
            this.Reponsitory = reponsitory;
            this.OrderItemID = orderItemID;
            this.NewQuantity = newQuantity;
            this.UnitPrice = unitPrice;
            this.SortingID = sortingID;
            this.DecListID = decListID;
            this.DeclPrice = declPrice;
            //this.OldOrderControls = oldOrderControls;
            //this.OldOrderControlSteps = oldOrderControlSteps;
        }

        public void Do()
        {
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
            {
                Quantity = this.NewQuantity,
                TotalPrice = this.NewQuantity * this.UnitPrice,
            }, t => t.ID == this.OrderItemID);

            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new
            {
                Quantity = this.NewQuantity,
            }, t => t.ID == this.SortingID);

            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecLists>(new
            {
                GQty = this.NewQuantity,
                DeclTotal = this.NewQuantity * this.DeclPrice,
            }, t => t.ID == this.DecListID);


        }
    }
}
