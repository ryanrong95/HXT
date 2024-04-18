using System;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    /// <summary>
    /// 应付账目
    /// </summary>
    public class PayerLeft : IUnique
    {
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// AccountSource: Keeper 库房 1, Tracker 跟单 2
        /// </summary>
        public AccountSource Source { get; set; }

        /// <summary>
        /// 内部来自内部公司 默认：芯达通
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 供应商、承运商等
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 收款接收人, 收款人可能还存在：司机、个人等其他, Payee==null时代表对个人应付, 关联Takers
        /// </summary>
        public string TakerID { get; set; }

        /// <summary>
        /// 枚举 Conduct, 本期默认业务为仓储
        /// </summary>
        public Conduct Conduct { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 币种, 枚举
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 总额
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 结算日期
        /// </summary>
        public DateTime CutDate { get; set; }

        /// <summary>
        /// 结算期号, 月结账单提取的依据 [202012]
        /// </summary>
        public int CutDateIndex { get; set; }

        /// <summary>
        /// 发生通知ID
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 发生单据ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 发生运单ID
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string AdminID { get; set; }
        #endregion
    }
}
