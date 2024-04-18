using System;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    /// <summary>
    /// 代付货款委托书扩展实体
    /// </summary>
    public class PrePayApplyExtends : IUnique
    {
        /// <summary>
        /// 代付申请ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 供应商名称(英文)
        /// </summary>
        public string SupplierEnglishName { get; set; }
        /// <summary>
        /// 供应商地址
        /// </summary>
        public string SupplierAddress { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 本次申请金额
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// 供应商银行名称(英文)
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 供应商银行地址(英文)
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }
        /// <summary>
        /// 银行代码
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 委托方
        /// </summary>
        public string ClientName { get; set; }
    }
}
