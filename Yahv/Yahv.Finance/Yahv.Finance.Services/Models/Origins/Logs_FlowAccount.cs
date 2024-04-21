using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 流水日志表
    /// </summary>
    public class Logs_FlowAccount : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        #endregion

        #region 数据库属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 账户行为
        /// </summary>
        public AccountMethord AccountMethord { get; set; }

        /// <summary>
        /// 对方户名
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 我方账户ID
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 收入/支出 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal? Balance { get; set; }

        /// <summary>
        /// 支付日期（收款日期或付款日期）
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 本位币种1
        /// </summary>
        public Currency Currency1 { get; set; }

        /// <summary>
        /// 本位币种汇率1
        /// </summary>
        public decimal ERate1 { get; set; }

        /// <summary>
        /// 本位币种 金额
        /// </summary>
        public decimal Price1 { get; set; }

        /// <summary>
        /// 本位币种 余额
        /// </summary>
        public decimal? Balance1 { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 对方名称
        /// </summary>
        public string TargetAccountName { get; set; }

        /// <summary>
        /// 对方银行帐号
        /// </summary>
        public string TargetAccountCode { get; set; }

        /// <summary>
        /// 收款方式
        /// </summary>
        public PaymentMethord PaymentMethord { get; set; }

        /// <summary>
        /// 流水表ID
        /// </summary>
        public string SourceID { get; set; }

        /// <summary>
        /// 流水表创建日期
        /// </summary>
        public DateTime SourceCreateDate { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvFinance.Logs_FlowAccount()
                {
                    ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.LogsFlow),
                    AccountMethord = (int)this.AccountMethord,
                    AccountID = this.AccountID,
                    Currency = (int)this.Currency,
                    Price = this.Price,
                    Balance = this?.Balance ?? 0,
                    PaymentDate = this.PaymentDate,
                    FormCode = this.FormCode,
                    Currency1 = (int)this.Currency1,
                    ERate1 = this.ERate1,
                    Price1 = this.Price1,
                    Balance1 = this?.Balance1 ?? 0,
                    CreatorID = this.CreatorID,
                    CreateDate = DateTime.Now,
                    TargetAccountName = this.TargetAccountName,
                    TargetAccountCode = this.TargetAccountCode,
                    PaymentMethord = (int)this.PaymentMethord,
                    SourceID = this.SourceID,
                    SourceCreateDate = this.SourceCreateDate,
                });
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion
    }
}