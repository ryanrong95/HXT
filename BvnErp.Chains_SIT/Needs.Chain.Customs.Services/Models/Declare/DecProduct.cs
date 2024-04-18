using Needs.Linq;
using Needs.Wl.Models;
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
    /// 报关单产品
    /// </summary>
    public class DecProduct : Interfaces.IProduct
    {
        public string ID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 产品品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 产品批次
        /// </summary>
        public string Batch { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public string NO { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxIndex { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal NetWeight { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal GrossWeight { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        #region 订单的单价和总价  2020-09-03 by yeshuangshuang

        /// <summary>
        /// 单价
        /// </summary>
        public decimal OrderUnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal OrderTotalPrice { get; set; }

        #endregion
    }
}