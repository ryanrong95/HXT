using Layer.Data.Sqls;
using Needs.Model;
using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 报关单缴税流水
    /// </summary>
    public class DecTaxFlow : ModelBase<Layer.Data.Sqls.ScCustoms.DecTaxFlows, ScCustomsReponsitory>, Needs.Linq.IUnique, Needs.Linq.IPersist
    {
        #region 属性

        /// <summary>
        /// 报关单ID 
        /// DecHeadID
        /// TODO:数据库中修改字段名称 DecHeadID
        /// </summary>
        public string DecTaxID { get; set; }

        /// <summary>
        /// 税费单号
        /// </summary>
        public string TaxNumber { get; set; }

        /// <summary>
        /// 税费类型
        /// </summary>
        public Enums.DecTaxType TaxType { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 缴税日期
        /// </summary>
        public DateTime? PayDate { get; set; }

        /// <summary>
        /// 缴税银行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 海关扣款时间
        /// </summary>
        public DateTime? DeductionTime { get; set; }

        #endregion

        public DecTaxFlow()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
    }
}
