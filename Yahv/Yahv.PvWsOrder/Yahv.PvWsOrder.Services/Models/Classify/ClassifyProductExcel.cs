using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 归类已完成列表导出Excel使用
    /// </summary>
    public class ClassifyProductExcel : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string TariffName { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal VATRate { get; set; }

        /// <summary>
        /// 进口优惠税率
        /// </summary>
        public decimal ImportPreferentialTaxRate { get; set; }

        /// <summary>
        /// 原产地附加税率
        /// </summary>
        public decimal OriginRate { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime CompleteDate { get; set; }

        /// <summary>
        /// 预处理一操作人
        /// </summary>
        public string ClassifyFirstOperatorName { get; set; }

        /// <summary>
        /// 预处理二操作人
        /// </summary>
        public string ClassifySecondOperatorName { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 归类日志
        /// </summary>
        public IEnumerable<string> ClassifyLogs { get; set; }
    }
}
