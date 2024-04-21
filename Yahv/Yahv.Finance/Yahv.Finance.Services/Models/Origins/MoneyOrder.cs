using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 承兑汇票
    /// </summary>
    public class MoneyOrder : IUnique, IDataEntity
    {
        #region 事件

        public event SuccessHanlder AddSuccess;
        #endregion

        #region 构造函数
        public MoneyOrder()
        {
            Currency = Currency.CNY;
        }
        #endregion

        #region 数据库属性
        public string ID { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public MoneyOrderType Type { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 票据号码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankCode { get; set; }

        /// <summary>
        /// 开户行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 开户行行号
        /// </summary>
        public string BankNo { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 出票人账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 是否允许转让
        /// </summary>
        public bool IsTransfer { get; set; }

        /// <summary>
        /// 当前持票人账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 出票日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 汇票到期日
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 承兑性质
        /// </summary>
        public MoneyOrderNature Nature { get; set; }

        /// <summary>
        /// 兑换日期
        /// </summary>
        public DateTime? ExchangeDate { get; set; }

        /// <summary>
        /// 兑换金额
        /// </summary>
        public decimal? ExchangePrice { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改人ID
        /// </summary>
        public string ModifierID { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public MoneyOrderStatus Status { get; set; }

        /// <summary>
        /// 是否能贴现
        /// </summary>
        public bool IsMoney { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (!repository.ReadTable<Layers.Data.Sqls.PvFinance.MoneyOrders>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.MoneyOrders);

                    repository.Insert(new Layers.Data.Sqls.PvFinance.MoneyOrders()
                    {
                        ID = this.ID,
                        Code = this.Code,
                        Name = this.Name,
                        Currency = (int)this.Currency,
                        CreateDate = DateTime.Now,
                        PayerAccountID = this.PayerAccountID,
                        CreatorID = this.CreatorID,
                        Price = this.Price,
                        BankCode = this.BankCode,
                        BankName = this.BankName,
                        BankNo = this.BankNo,
                        EndDate = this.EndDate,
                        ExchangeDate = this.ExchangeDate,
                        ExchangePrice = this.ExchangePrice,
                        IsTransfer = this.IsTransfer,
                        ModifierID = this.ModifierID,
                        ModifyDate = this.ModifyDate,
                        Nature = (int)this.Nature,
                        PayeeAccountID = this.PayeeAccountID,
                        StartDate = this.StartDate,
                        Status = (int)this.Status,
                        Type = (int)this.Type,
                        IsMoney = this.IsMoney,
                    });

                    this.AddSuccess?.Invoke(this, new SuccessEventArgs(this));
                }
                //修改
                else
                {
                    repository.Update<Layers.Data.Sqls.PvFinance.MoneyOrders>(new
                    {
                        Name = this.Name,
                        PayerAccountID = this.PayerAccountID,
                        Price = this.Price,
                        BankCode = this.BankCode,
                        BankName = this.BankName,
                        BankNo = this.BankNo,
                        EndDate = this.EndDate,
                        ExchangeDate = this.ExchangeDate,
                        ExchangePrice = this.ExchangePrice,
                        IsTransfer = this.IsTransfer,
                        ModifierID = this.ModifierID,
                        ModifyDate = this.ModifyDate,
                        Nature = (int)this.Nature,
                        PayeeAccountID = this.PayeeAccountID,
                        StartDate = this.StartDate,
                        Status = (int)this.Status,
                        Type = (int)this.Type,
                        IsMoney = this.IsMoney,
                    }, item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <remarks>主要是为了使用事务的时候用</remarks>
        public void Add()
        {
            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                this.ID = PKeySigner.Pick(PKeyType.MoneyOrders);
                repository.Insert(new Layers.Data.Sqls.PvFinance.MoneyOrders()
                {
                    ID = this.ID,
                    Code = this.Code,
                    Name = this.Name,
                    Currency = (int)this.Currency,
                    CreateDate = DateTime.Now,
                    PayerAccountID = this.PayerAccountID,
                    CreatorID = this.CreatorID,
                    Price = this.Price,
                    BankCode = this.BankCode,
                    BankName = this.BankName,
                    BankNo = this.BankNo,
                    EndDate = this.EndDate,
                    ExchangeDate = this.ExchangeDate,
                    ExchangePrice = this.ExchangePrice,
                    IsTransfer = this.IsTransfer,
                    ModifierID = this.ModifierID,
                    ModifyDate = this.ModifyDate,
                    Nature = (int)this.Nature,
                    PayeeAccountID = this.PayeeAccountID,
                    StartDate = this.StartDate,
                    Status = (int)this.Status,
                    Type = (int)this.Type,
                    IsMoney = this.IsMoney,
                });
            }
        }
        #endregion
    }
}