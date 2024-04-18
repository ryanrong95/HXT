using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Wl.Models.Hanlders;
using System;
using System.Data;
using System.Linq;

namespace Needs.Wl.Models
{
    //说明：设计缺陷，应该设计成付汇订单，缺少付汇完成时间。

    /// <summary>
    /// 付汇申请
    /// </summary>
    public class PayExchangeApply : ModelBase<Layer.Data.Sqls.ScCustoms.PayExchangeApplies, ScCustomsReponsitory>, IUnique, IPersist
    {
        #region 属性

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 供应商地址
        /// </summary>
        public string SupplierAddress { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行国际代码
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// 使用的汇率类型
        /// </summary>
        public Enums.ExchangeRateType ExchangeRateType { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public Enums.PaymentType PaymentType { get; set; }

        /// <summary>
        /// 期望付款日期
        /// </summary>
        public DateTime? ExpectPayDate { get; set; }

        /// <summary>
        /// 结算截止日期
        /// </summary>
        public DateTime SettlemenDate { get; set; }

        /// <summary>
        /// 其它资料
        /// </summary>
        public string OtherInfo { get; set; }

        /// <summary>
        /// 管理员\跟单员
        /// 跟单员提交的付汇申请
        /// </summary>
        public virtual Admin Admin { get; set; }

        /// <summary>
        /// 会员
        /// 客户提交的付汇申请
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// 付汇申请状态
        /// </summary>
        public Enums.PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }

        /// <summary>
        /// 付汇申请明细
        /// </summary>
        public PayExchangeApplyItems Items { get; set; }

        #endregion

        #region 构造函数

        public PayExchangeApply()
        {
            this.PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Auditing;
            this.Status = (int)Enums.Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.SettlemenDate = DateTime.Now.AddDays(90);
        }

        #endregion

        #region 持久化

        public override void Enter()
        {
            //付汇申请
            this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApply);
            this.Reponsitory.Insert(this.ToLinq());
        }

        /// <summary>
        /// 删除
        /// </summary>
        public override void Abandon()
        {
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);

            //删除明细
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { Status = Enums.Status.Delete }, t => t.PayExchangeApplyID == this.ID);
        }

        #endregion
    }
}