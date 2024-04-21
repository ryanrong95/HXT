using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Wms.Services.Models;

namespace Wms.Services.Views
{
    public class SZPickingNoticesRoll : QueryView<Models.DataPickingNotice, PvWmsRepository>
    {

        public SZPickingNoticesRoll()
        {

        }

        public SZPickingNoticesRoll(PvWmsRepository reponsitory) : base(reponsitory)
        {

        }

        public SZPickingNoticesRoll(PvWmsRepository reponsitory, IQueryable<DataPickingNotice> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<DataPickingNotice> GetIQueryable()
        {
            //var data= from notice in new Yahv.Services.Views.PickingNoticesView<PvWmsRepository>(this.Reponsitory)
            //          //join waybill in new ServicesWaybillsTopView()
            //          //on notice.WaybillID equals waybill.ID into waybills
            //          //from waybill in waybills.DefaultIfEmpty()
            //          orderby notice.CreateDate descending
            //          select new DataPickingNotice
            //          {
            //              ExcuteStatus = (int)notice.ExcuteStatus,
            //              BoxCode = notice.BoxCode,
            //              Checked = notice.Checked,
            //              Conditions = notice.Conditions,
            //              CreateDate = notice.CreateDate,
            //              DateCode = notice.DateCode,
            //              Files = notice.Files,
            //              ID = notice.ID,
            //              InputID = notice.InputID,
            //              NetWeight = notice.NetWeight,
            //              Output = notice.Output,
            //              OutputID = notice.OutputID,
            //              Picking = notice.Picking,
            //              Product = notice.Product,
            //              ProductID = notice.ProductID,
            //              Quantity = notice.Quantity,
            //              ShelveID = notice.ShelveID,
            //              Source = notice.Source,
            //              Status = notice.Status,
            //              StockQuantity = notice.StockQuantity,
            //              Supplier = notice.Supplier,
            //              Target = notice.Target,
            //              TotalPieces = notice.TotalPieces,
            //              Type = notice.Type,
            //              Visable = notice.Visable,
            //              Volume = notice.Volume,
            //              WareHouseID = notice.WareHouseID,
            //              Waybill = notice.Waybill,
            //              WaybillID = notice.WaybillID,
            //              Weight = notice.Weight
            //          };
            return from notice in new Yahv.Services.Views.PickingNoticesView<PvWmsRepository>(this.Reponsitory)
                   //join waybill in new ServicesWaybillsTopView()
                   //on notice.WaybillID equals waybill.ID
                   orderby notice.CreateDate descending
                   select new DataPickingNotice
                   {
                       ExcuteStatus = (int)notice.ExcuteStatus,
                       BoxCode = notice.BoxCode,
                       Checked = notice.Checked,
                       Conditions = notice.Conditions,
                       CreateDate = notice.CreateDate,
                       DateCode = notice.DateCode,
                       Files = notice.Files,
                       ID = notice.ID,
                       InputID = notice.InputID,
                       NetWeight = notice.NetWeight,
                       Output = notice.Output,
                       OutputID = notice.OutputID,
                       Picking = notice.Picking,
                       Product = notice.Product,
                       ProductID = notice.ProductID,
                       Quantity = notice.Quantity,
                       ShelveID = notice.ShelveID,
                       Source = notice.Source,
                       Status = notice.Status,
                       StockQuantity = notice.StockQuantity,
                       Supplier = notice.Supplier,
                       Target = notice.Target,
                       TotalPieces = notice.TotalPieces,
                       Type = notice.Type,
                       Visable = notice.Visable,
                       Volume = notice.Volume,
                       WareHouseID = notice.WareHouseID,
                       WaybillID = notice.WaybillID,
                       Weight = notice.Weight
                   };
        }

        /// <summary>
        /// 根据库房编号查询分拣通知
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public SZPickingNoticesRoll SearchByWarehouseID(string warehouseID)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                where notice.WareHouseID == warehouseID
                                select notice.WaybillID;

            var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new SZPickingNoticesRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据key查询分拣通知
        /// </summary>
        /// <param name="key">型号/制造商</param>
        /// <returns></returns>
        public SZPickingNoticesRoll SearchByKey(string key)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                join product in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>()
                                on notice.ProductID equals product.ID
                                where product.Manufacturer.Contains(key)||product.PartNumber.Contains(key)
                                select notice.WaybillID;

            var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new SZPickingNoticesRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据waybillID查询分拣通知
        /// </summary>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        public SZPickingNoticesRoll SearchByWaybillID(string waybillID)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.WaybillID == waybillID
                         select waybill;

            return new SZPickingNoticesRoll(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据waybillID查询分拣通知
        /// </summary>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        public SZPickingNoticesRoll SearchByTinyOrderID(string tinyOrderID)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                join output in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                                on notice.OutputID equals output.ID
                                where output.TinyOrderID == tinyOrderID
                                select notice.WaybillID;

            var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new SZPickingNoticesRoll(this.Reponsitory, iQuery);
        }
        /// <summary>
        /// 根据waybillID查询分拣通知
        /// </summary>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        public SZPickingNoticesRoll SearchByVastOrderID(string vastOrderID)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                join output in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                                on notice.OutputID equals output.ID
                                where output.OrderID == vastOrderID
                                select notice.WaybillID;

            var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new SZPickingNoticesRoll(this.Reponsitory, iQuery);
        }

      
    }
    static public class PickingNoticesExtends
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="iquery"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        static public object ToPage(this IQueryable<DataPickingNotice> iquery, int pageIndex = 1, int pageSize = 20)
        {
            int total = iquery.Count();
            var query = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = query.Select(item => (DataPickingNotice)item).ToArray()
            };
        }
    }
}
