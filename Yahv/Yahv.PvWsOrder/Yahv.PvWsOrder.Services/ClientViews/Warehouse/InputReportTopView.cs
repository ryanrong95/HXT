using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 入库单
    /// </summary>
    public class InputReportTopView : UniqueView<ReportClass, PvWsOrderReponsitory>
    {
        string Enterpriseid;

        public InputReportTopView(string EnterPriseID)
        {
            this.Enterpriseid = EnterPriseID;
        }

        private InputReportTopView(PvWsOrderReponsitory reponsitory, IQueryable<ReportClass> IQuery) : base(reponsitory, IQuery)
        {

        }

        protected override IQueryable<ReportClass> GetIQueryable()
        {
            return from report in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.CgInputReportTopView>()
                   join order in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>() on report.OrderID equals order.ID
                   where order.ClientID == this.Enterpriseid
                   orderby report.EnterDate descending
                   select new ReportClass
                   {
                       ID = report.SortingID,
                       PartNumber = report.PartNumber,
                       Manufacturer = report.Manufacturer,
                       Currency = report.Currency,
                       UnitPrice = report.UnitPrice,
                       Quantity = report.EnterQuantity,
                       TotalPrice = report.UnitPrice * report.EnterQuantity,
                       Supplier = report.Supplier,
                       WarehouseID = report.WareHouseID,
                       OrderID = report.OrderID,
                       OrderType = order.Type,
                       CreateDate = report.EnterDate,
                   };
        }


        #region 根据查询条件过滤
        /// <summary>
        /// 根据供应商搜索
        /// </summary>
        /// <param name="Supplier"></param>
        /// <returns></returns>
        public InputReportTopView SearchBySupplier(string Supplier)
        {
            var linq = from entity in this.IQueryable
                       where entity.Supplier.Contains(Supplier)
                       select entity;

            return new InputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据出库时间搜索
        /// </summary>
        /// <param name="StartDate"></param>
        /// <returns></returns>
        public InputReportTopView SearchByStartDate(DateTime StartDate)
        {
            var linq = from entity in this.IQueryable
                       where entity.CreateDate >= StartDate
                       select entity;

            return new InputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据出库时间搜索
        /// </summary>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public InputReportTopView SearchByEndDate(DateTime EndDate)
        {
            var linq = from entity in this.IQueryable
                       where entity.CreateDate <= EndDate
                       select entity;

            return new InputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据订单号搜索
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public InputReportTopView SearchByOrderID(string OrderID)
        {
            var linq = from entity in this.IQueryable
                       where entity.OrderID.Contains(OrderID)
                       select entity;

            return new InputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据型号搜索
        /// </summary>
        /// <param name="PartNumber"></param>
        /// <returns></returns>
        public InputReportTopView SearchByPartNumber(string PartNumber)
        {
            var linq = from entity in this.IQueryable
                       where entity.PartNumber.Contains(PartNumber)
                       select entity;

            return new InputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据库房搜索
        /// </summary>
        /// <param name="WarehouseID"></param>
        /// <returns></returns>
        public InputReportTopView SearchByWareHouseID(string WareHouseID)
        {
            var linq = from entity in this.IQueryable
                       where entity.WarehouseID.Contains(WareHouseID)
                       select entity;

            return new InputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据订单类型搜索
        /// </summary>
        /// <param name="OrderType"></param>
        /// <returns></returns>
        public InputReportTopView SearchByOrderType(int OrderType)
        {
            var linq = from entity in this.IQueryable
                       where entity.OrderType == OrderType
                       select entity;

            return new InputReportTopView(this.Reponsitory, linq);
        }
        #endregion
    }

    public class ReportClass : IUnique
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public int? Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal? TotalPrice { get; set; }

        /// <summary>
        /// 库房ID
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime CreateDate { get; set; }


        /// <summary>
        /// 运单号
        /// </summary>
        internal string WaybillID { get; set; }
    }
}
