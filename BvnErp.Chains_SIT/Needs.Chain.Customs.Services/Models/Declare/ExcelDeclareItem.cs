using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ExcelDeclareItem
    {
        /// <summary>
        /// 商品序号/项号
        /// </summary>
        public int GNo { get; set; }
        /// <summary>
        /// 备案序号
        /// </summary>
        public decimal? ContrItem { get; set; }
        /// <summary>
        /// 10位商编
        /// </summary>
        public string CodeTS { get; set; }
        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CiqCode { get; set; }
        /// <summary>
        /// 报关品名
        /// </summary>
        public string GName { get; set; }
        /// <summary>
        /// 规格型号（申报要素）
        /// </summary>
        public string GModel { get; set; }
        /// <summary>
        /// 成交数量
        /// </summary>
        public decimal GQty { get; set; }
        /// <summary>
        /// 成交单位
        /// </summary>
        public string GUnit { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string FirstUnit { get; set; }
        /// <summary>
        /// 法定第一数量
        /// </summary>
        private decimal? firstQty { get; set; }
        /// <summary>
        /// 法定第二单位
        /// </summary>
        public string SecondUnit { get; set; }

        /// <summary>
        /// 法定第二数量
        /// </summary>        
        private decimal? secondQty { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal DeclPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal DeclTotal { get; set; }

        /// <summary>
        /// 成交币制
        /// </summary>
        public string TradeCurr { get; set; }

    }
}
