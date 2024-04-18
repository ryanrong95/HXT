using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单的裝箱信息\分拣后的装箱单
    /// 到货信息装箱单
    /// </summary>
    public sealed class OrderPackingBill : IUnique
    {
        public string ID { get; set; }

        public string OrderID { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxIndex { get; set; }

        public Sorting Sorting { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string CustomsName { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal NetWeight { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal GrossWeight { get; set; }

        /// <summary>
        /// 装箱数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        public PackingStatus PackingStatus { get; set; }

        public DateTime PackingDate { get; set; }

        public string AdminName { get; set; }

        public string WaybillCode { get; set; }

        public OrderPackingBill()
        {

        }

        /// <summary>
        /// 导出PDF
        /// </summary>
        public void ToPDF()
        {
        }

        /// <summary>
        /// 生成装箱单
        /// </summary>
        public void GenerateBill()
        {
        }
    }

}