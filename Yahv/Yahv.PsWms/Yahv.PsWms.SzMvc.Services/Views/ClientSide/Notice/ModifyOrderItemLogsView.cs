using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Views
{
    public class ModifyOrderItemLogsView : QueryView<ModifyOrderItemLogsViewModel, PsOrderRepository>
    {
        private string _BatchID { get; set; }

        public ModifyOrderItemLogsView()
        {
        }

        public ModifyOrderItemLogsView(string batchID)
        {
            this._BatchID = batchID;
        }

        protected ModifyOrderItemLogsView(PsOrderRepository reponsitory, IQueryable<ModifyOrderItemLogsViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ModifyOrderItemLogsViewModel> GetIQueryable()
        {
            var orderItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.OrderItems>();

            var iQuery = from orderItem in orderItems
                         select new ModifyOrderItemLogsViewModel
                         {
                             OrderItemID = orderItem.ID,
                             OrderID = orderItem.OrderID,
                             ProductID = orderItem.ProductID,
                             CustomCode = orderItem.CustomCode,
                             StocktakingType = (StocktakingType)orderItem.StocktakingType,
                             Mpq = orderItem.Mpq,
                             PackageNumber = orderItem.PackageNumber,
                             Total = orderItem.Total,
                             CreateDate = orderItem.CreateDate,
                             ModifyDate = orderItem.ModifyDate,
                             Status = (GeneralStatus)orderItem.Status,
                             BakPartnumber = orderItem.BakPartnumber,
                             BakBrand = orderItem.BakBrand,
                             BakPackage = orderItem.BakPackage,
                             BakDateCode = orderItem.BakDateCode,
                             NoticeID = orderItem.NoticeID,
                             NoticeItemID = orderItem.NoticeItemID,
                         };

            return iQuery;
        }

        /// <summary>
        /// topview 的 OrderItem
        /// </summary>
        /// <returns></returns>
        public ModifyOrderItemLogsViewModel[] GetTopviewOrderItem()
        {
            IQueryable<ModifyOrderItemLogsViewModel> iquery = this.IQueryable.Cast<ModifyOrderItemLogsViewModel>();

            string flag = this._BatchID + "_" + BakOrderItem.DeliveryTopView.GetDescription();
            iquery = iquery.Where(t => t.OrderItemID.Contains(flag)).OrderBy(t => t.OrderItemID);

            return iquery.ToArray();
        }

        /// <summary>
        /// origin 的 OrderItem
        /// </summary>
        /// <returns></returns>
        public ModifyOrderItemLogsViewModel[] GetOriginOrderItem()
        {
            IQueryable<ModifyOrderItemLogsViewModel> iquery = this.IQueryable.Cast<ModifyOrderItemLogsViewModel>();

            string flag = this._BatchID + "_" + BakOrderItem.OriginOrderItem.GetDescription();
            iquery = iquery.Where(t => t.OrderItemID.Contains(flag)).OrderBy(t => t.OrderItemID);

            return iquery.ToArray();
        }

        /// <summary>
        /// new 的 OrderItem
        /// </summary>
        /// <returns></returns>
        public ModifyOrderItemLogsViewModel[] GetNewOrderItem()
        {
            IQueryable<ModifyOrderItemLogsViewModel> iquery = this.IQueryable.Cast<ModifyOrderItemLogsViewModel>();

            string flag = this._BatchID + "_" + BakOrderItem.NewOrderItem.GetDescription();
            iquery = iquery.Where(t => t.OrderItemID.Contains(flag)).OrderBy(t => t.OrderItemID);

            return iquery.ToArray();
        }
    }

    public class ModifyOrderItemLogsViewModel
    {
        public string OrderItemID { get; set; }

        public string OrderID { get; set; }

        public string ProductID { get; set; }

        public string CustomCode { get; set; }

        public StocktakingType StocktakingType { get; set; }

        public int Mpq { get; set; }

        public int PackageNumber { get; set; }

        public int Total { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public GeneralStatus Status { get; set; }

        public string BakPartnumber { get; set; }

        public string BakBrand { get; set; }

        public string BakPackage { get; set; }

        public string BakDateCode { get; set; }

        public string NoticeID { get; set; }

        public string NoticeItemID { get; set; }
    }
}
