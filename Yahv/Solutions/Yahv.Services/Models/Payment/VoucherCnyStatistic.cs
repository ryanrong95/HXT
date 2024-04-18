using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class VoucherCnyStatistic
    {
        /// <summary>
        /// 代仓储订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 代收付款申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 应收ID
        /// </summary>
        public string ReceivableID { get; set; }
        
        /// <summary>
        /// 付款人ID
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 收款人ID
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Business { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }
        
        /// <summary>
        /// 录入币种
        /// </summary>
        public Currency OriginCurrency { get; set; }
        
        /// <summary>
        /// 录入金额
        /// </summary>
        public decimal OriginPrice { get; set; }

        /// <summary>
        /// 汇率(录入币种与本位币之间的汇率)
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 本位币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 应收本位币金额
        /// </summary>
        public decimal LeftPrice { get; set; }
       
        /// <summary>
        /// 实收本位币金额
        /// </summary>
        public decimal? RightPrice { get; set; }

        /// <summary>
        /// 减免本位币金额
        /// </summary>
        public decimal? ReducePrice { get; set; }

        public DateTime? OriginalDate { get; set; }

        public DateTime LeftDate { get; set; }

        public DateTime? RightDate { get; set; }
      
        public GeneralStatus Status { get; set; }

        public string AdminID { get; set; }

        #region 扩展属性

        /// <summary>
        /// 剩余应付
        /// </summary>
        public decimal Remains
        {
            get
            {
                return this.LeftPrice - (this.RightPrice ?? 0m) - (this.ReducePrice ?? 0m);
            }
        }

        public Admin Admin { get; set; }

        public Enterprise Payer { get; set; }

        public Enterprise Payee { get; set; }

        public decimal HKDTOCNYRate { get; set; }

        public decimal HKDLeftPrice { get; set; }

        public decimal? HKDRightPrice { get; set; }

        #endregion
    }
}
