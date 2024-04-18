using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class OutputReportTopView : UniqueView<ReportClass, PvWsOrderReponsitory>
    {

        string enterpriseid;

        public OutputReportTopView(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }

        private OutputReportTopView(PvWsOrderReponsitory reponsitory, IQueryable<ReportClass> IQuery) : base(reponsitory, IQuery)
        {

        }

        protected override IQueryable<ReportClass> GetIQueryable()
        {
            return from report in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.CgOutputReportTopView>()
                   join order in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>() on report.OrderID equals order.ID
                   where order.ClientID == this.enterpriseid
                   orderby report.PickingDate descending
                   select new ReportClass
                   {
                       ID = report.ID,
                       PartNumber = report.PartNumber,
                       Manufacturer = report.Manufacturer,
                       Currency = report.outCurrency,
                       UnitPrice = report.outUnitPrice,
                       Quantity = report.PickingQuantity,
                       TotalPrice = report.outUnitPrice * report.PickingQuantity,
                       Supplier = report.Supplier,
                       WarehouseID = report.WareHouseID,
                       OrderID = report.OrderID,
                       OrderType = order.Type,
                       CreateDate = report.PickingDate,
                       WaybillID = report.WaybillID,
                   };
        }

        #region 根据查询条件过滤
        /// <summary>
        /// 根据收货人信息搜索
        /// </summary>
        /// <param name="ConsgineeName"></param>
        /// <returns></returns>
        public OutputReportTopView SearchByConsigneeName(string ConsgineeName)
        {
            var linq = from entity in this.IQueryable
                       join waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.WaybillsTopView>()
                       on entity.WaybillID equals waybill.wbID
                       where waybill.coeCompany.Contains(ConsgineeName)
                       select entity;

            return new OutputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据出库时间搜索
        /// </summary>
        /// <param name="StartDate"></param>
        /// <returns></returns>
        public OutputReportTopView SearchByStartDate(DateTime StartDate)
        {
            var linq = from entity in this.IQueryable
                       where entity.CreateDate >= StartDate
                       select entity;

            return new OutputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据出库时间搜索
        /// </summary>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public OutputReportTopView SearchByEndDate(DateTime EndDate)
        {
            var linq = from entity in this.IQueryable
                       where entity.CreateDate <= EndDate
                       select entity;

            return new OutputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据订单号搜索
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public OutputReportTopView SearchByOrderID(string OrderID)
        {
            var linq = from entity in this.IQueryable
                       where entity.OrderID.Contains(OrderID)
                       select entity;

            return new OutputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据型号搜索
        /// </summary>
        /// <param name="PartNumber"></param>
        /// <returns></returns>
        public OutputReportTopView SearchByPartNumber(string PartNumber)
        {
            var linq = from entity in this.IQueryable
                       where entity.PartNumber.Contains(PartNumber)
                       select entity;

            return new OutputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据库房搜索
        /// </summary>
        /// <param name="WarehouseID"></param>
        /// <returns></returns>
        public OutputReportTopView SearchByWareHouseID(string WareHouseID)
        {
            var linq = from entity in this.IQueryable
                       where entity.WarehouseID.Contains(WareHouseID)
                       select entity;

            return new OutputReportTopView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据订单类型搜索
        /// </summary>
        /// <param name="OrderType"></param>
        /// <returns></returns>
        public OutputReportTopView SearchByOrderType(int OrderType)
        {
            var linq = from entity in this.IQueryable
                       where entity.OrderType == OrderType
                       select entity;

            return new OutputReportTopView(this.Reponsitory, linq);
        }
        #endregion
    }
}
