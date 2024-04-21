using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    public class FlowAccount : IUnique
    {
        #region 构造函数

        public FlowAccount()
        {
            this.Type = FlowAccountType.BankStatement;
        }
        #endregion

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
        /// 账户类型
        /// </summary>
        public FlowAccountType Type { get; set; }

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
        /// 目标汇率
        /// </summary>
        public decimal? TargetRate { get; set; }

        /// <summary>
        /// 承兑汇票ID
        /// </summary>
        public string MoneyOrderID { get; set; }

        #endregion

        #region 拓展属性

        private string _paymentDateDes { get; set; }

        public string PaymentDateDes
        {
            get
            {
                if (this.PaymentDate == null)
                {
                    return "";
                }
                else
                {
                    return ((DateTime)(this.PaymentDate)).ToString("yyyy-MM-dd");
                }
            }
            set { _paymentDateDes = value; }
        }

        /// <summary>
        /// 账户银行卡号
        /// </summary>
        public string AccountCode { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorName { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.FlowAccounts>().Any(item => item.ID == this.ID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.FlowAccounts()
                    {
                        ID = this.ID,
                        AccountMethord = (int)this.AccountMethord,
                        AccountID = this.AccountID,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Balance = this.Balance,
                        PaymentDate = this.PaymentDate,
                        FormCode = this.FormCode,
                        Currency1 = (int)this.Currency1,
                        ERate1 = this.ERate1,
                        Price1 = this.Price1,
                        Balance1 = this.Balance1,
                        CreatorID = this.CreatorID,
                        CreateDate = DateTime.Now,
                        TargetAccountName = this.TargetAccountName,
                        TargetAccountCode = this.TargetAccountCode,
                        PaymentMethord = (int)this.PaymentMethord,
                        TargetRate = this.TargetRate,
                        Type = (int)this.Type,
                        MoneyOrderID = this.MoneyOrderID,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.FlowAccounts>(new
                    {
                        AccountMethord = (int)this.AccountMethord,
                        AccountID = this.AccountID,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Balance = this.Balance,
                        PaymentDate = this.PaymentDate,
                        FormCode = this.FormCode,
                        Currency1 = (int)this.Currency1,
                        ERate1 = this.ERate1,
                        Price1 = this.Price1,
                        Balance1 = this.Balance1,
                        TargetAccountName = this.TargetAccountName,
                        TargetAccountCode = this.TargetAccountCode,
                        PaymentMethord = (int)this.PaymentMethord,
                        TargetRate = this.TargetRate,
                        Type = (int)this.Type,
                        MoneyOrderID = this.MoneyOrderID,
                    }, item => item.ID == this.ID);
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除流水
        /// </summary>
        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvFinance.FlowAccounts>(item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <remarks>主要是为了使用事务的时候用</remarks>
        public void Add()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                reponsitory.Insert(new Layers.Data.Sqls.PvFinance.FlowAccounts()
                {
                    ID = this.ID,
                    AccountMethord = (int)this.AccountMethord,
                    AccountID = this.AccountID,
                    Currency = (int)this.Currency,
                    Price = this.Price,
                    Balance = this.Balance,
                    PaymentDate = this.PaymentDate,
                    FormCode = this.FormCode,
                    Currency1 = (int)this.Currency1,
                    ERate1 = this.ERate1,
                    Price1 = this.Price1,
                    Balance1 = this.Balance1,
                    CreatorID = this.CreatorID,
                    CreateDate = DateTime.Now,
                    TargetAccountName = this.TargetAccountName,
                    TargetAccountCode = this.TargetAccountCode,
                    PaymentMethord = (int)this.PaymentMethord,
                    TargetRate = this.TargetRate,
                    Type = (int)this.Type,
                    MoneyOrderID = this.MoneyOrderID,
                });
            }

            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }
    }
    #endregion

}
