using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DeclaresTopView : UniqueView<DeclareTopModel, ScCustomsReponsitory>
    {
        protected override IQueryable<DeclareTopModel> GetIQueryable()
        {
            var linq = from sort in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclaresTopView>()
                       select new DeclareTopModel
                       {
                           ID = sort.UnqiueID,
                           TinyOrderID = sort.TinyOrderID,
                           PartNumber = sort.PartNumber,
                           Manufacturer = sort.Manufacturer,
                           ItemID = sort.ItemID,
                           BoxCode = sort.BoxCode,
                           Weight = sort.Weight.Value,
                           NetWeight = sort.NetWeight.Value,
                           Quantity = sort.Quantity,
                           Origin = sort.Origin,
                           EnterCode = sort.EnterCode,
                           LotNumber = sort.LotNumber,
                           BoxingDate = sort.BoxingDate,
                           Packer = sort.Packer,
                           Summary = sort.Summary,
                           OrderItemID = sort.OrderItemID,
                           InputID = sort.InputID,
                           OutputID = sort.OutputID,
                           StorageID = sort.StorageID
                       };

            return linq;
        }
    }

    public class DeclareTopModel : IUnique
    {
        /// <summary>
        /// 分拣/拣货ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 子订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 项ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 入库Code？
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxCode { get; set; }

        /// <summary>
        /// 位置？库位？
        /// </summary>
        public string LotNumber { get; set; }

        /// <summary>
        /// 装箱日期
        /// </summary>
        public DateTime BoxingDate { get; set; }

        /// <summary>
        /// 装箱人/分拣拣货人
        /// </summary>
        public string Packer { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal? NetWeight { get; set; }

        /// <summary>
        /// 进项ID
        /// </summary>
        public string InputID { get; set; }

        /// <summary>
        /// 销项ID
        /// </summary>
        public string OutputID { get; set; }
    }
}
