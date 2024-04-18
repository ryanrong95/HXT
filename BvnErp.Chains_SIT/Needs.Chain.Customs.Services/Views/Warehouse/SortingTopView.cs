using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class SortingTopView : UniqueView<SortingTopModel, ScCustomsReponsitory>
    {
        protected override IQueryable<SortingTopModel> GetIQueryable()
        {
            var linq = from sort in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SortingTopView>()
                       select new SortingTopModel
                       {
                           ID = sort.UnqiueID,
                           VastOrderID = sort.VastOrderID,
                           TinyOrderID = sort.TinyOrderID,
                           PartNumber = sort.PartNumber,
                           Manufacturer = sort.Manufacturer,
                           ItemID = sort.ItemID,
                           BoxCode = sort.BoxCode,
                           Weight = sort.Weight,
                           NetWeight = sort.NetWeight,
                           Quantity = sort.Quantity,
                           Origin = sort.Origin,
                           DateCode = sort.DateCode,
                           WarehouseID = sort.WareHouseID,
                           StoInputID = sort.StoInputID,
                           StorageID = sort.StorageID,
                           UnitPrice = sort.UnitPrice,
                           ProductID = sort.ProductID,
                           Volume = sort.Volume
                       };

            return linq;
        }
    }

    public class SortingTopModel : IUnique
    {
        /// <summary>
        /// 分拣ID：UniqueID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 主订单号
        /// </summary>
        public string VastOrderID { get; set; }

        /// <summary>
        /// 子订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 项ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxCode { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal? NetWeight { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 库房ID
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 进项ID
        /// </summary>
        public string StoInputID { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }

    }
}
