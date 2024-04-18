using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceDetailData:IUnique
    {
        public string ID { get; set; }
        public string InvoiceResultID { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int? lineNum { get; set; }
        /// <summary>
        /// 商品名称 *物流辅助服务*收派服务费
        /// </summary>
        public string goodserviceName { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? number { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? price { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? sum { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public string taxRate { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        public decimal? tax { get; set; }
        /// <summary>
        /// 是否为清单行 Y：是，N：否（是的时候，货物名称为“请详见货物清单”，“折扣额合计”，“原价合计”，可以不用做账）
        /// </summary>
        public string isBillLine { get; set; }
        /// <summary>
        /// 零税率标志字段（空:非零税率， 1:税率栏位显示“免税”， 2:税率栏位显示“不征收”， 3:零税率）
        /// </summary>
        public string zeroTaxRateSign { get; set; }
        /// <summary>
        /// 零税率标志名称
        /// </summary>
        public string zeroTaxRateSignName { get; set; }

        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
