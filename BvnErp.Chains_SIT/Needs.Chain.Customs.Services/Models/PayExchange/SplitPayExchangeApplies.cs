using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SplitPayExchangeApplies : IUnique
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 供应商名称
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
        /// 金额
        /// </summary>
        public decimal TotalAmount { get; set; }

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
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        public string OrderID { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string PayExchangeApplyID { get; set; }
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

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        /// <summary>
        /// 美国付汇账号
        /// </summary>
        public string ABA { get; set; }
        /// <summary>
        /// 欧美付汇账号
        /// </summary>
        public string IBAN { get; set; }

        /// <summary>
        /// 拆分前的申请付汇ID
        /// </summary>
        public string FatherID { get; set; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierID { get; set; }
        /// <summary>
        /// 银行ID
        /// </summary>
        public string BankID { get; set; }

        public int AdvanceMoney { get; set; }

        #endregion

        /// <summary>
        /// 当新增或修改成功时发生
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        #region 构造函数

        public SplitPayExchangeApplies()
        {
            this.PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Auditing;
            this.Status = Enums.Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.SettlemenDate = DateTime.Now.AddDays(90);
        }

        #endregion

        #region 持久化

        public virtual void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //付汇申请

                //获取供应商信息
                var clientSuppliers = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>().
                    Where(item => item.ID == this.SupplierID && item.Status == (int)Enums.Status.Normal).FirstOrDefault();

                //获取供应商对应的地址
                var clientSuppliersAddresses = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>().
                    Where(item => item.ID == this.SupplierID && item.Status == (int)Enums.Status.Normal).FirstOrDefault();

                //获取供应商的银行信息
                var clientSupplierBanks = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSupplierBanks>().
                     Where(item => item.ID == this.BankID && item.Status == (int)Enums.Status.Normal).FirstOrDefault();

                var apply = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>().
                    Where(item => item.ID == this.FatherID).FirstOrDefault();
                try
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new Layer.Data.Sqls.ScCustoms.PayExchangeApplies
                    {
                        ID = this.ID,
                        AdminID = this.Admin.ID,
                        ClientID = clientSuppliers.ClientID,
                        SupplierName = clientSuppliers.ChineseName,
                        SupplierEnglishName = clientSuppliers.Name,
                        SupplierAddress = clientSuppliersAddresses == null ? null : clientSuppliersAddresses.Address,
                        BankAccount = clientSupplierBanks.BankAccount,
                        BankAddress = clientSupplierBanks.BankAddress,
                        BankName = clientSupplierBanks.BankName,
                        SwiftCode = clientSupplierBanks.SwiftCode,
                        ExchangeRateType = (int)apply.ExchangeRateType,
                        Currency = apply.Currency,
                        ExchangeRate = this.ExchangeRate,
                        PaymentType = (int)this.PaymentType,
                        ExpectPayDate = this.ExpectPayDate,
                        SettlemenDate = (DateTime)apply.SettlemenDate,
                        OtherInfo = apply.OtherInfo,
                        PayExchangeApplyStatus = (int)apply.PayExchangeApplyStatus,
                        Status = (int)apply.Status,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = apply.Summary,
                        ABA = this.ABA,
                        IBAN = this.IBAN,
                        FatherID = this.FatherID,
                        IsAdvanceMoney = this.AdvanceMoney
                    });
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }
        public virtual void ApplyItemEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //插入Items
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyItem),
                    PayExchangeApplyID = this.PayExchangeApplyID,
                    OrderID = this.OrderID,
                    Amount = this.TotalAmount,
                    Status = (int)Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    ApplyStatus = (int)Enums.ApplyItemStatus.Appling,
                });

            }
        }
        public virtual void UnSplitEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //付汇申请
                var apply = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>().
                    Where(item => item.ID == this.FatherID).FirstOrDefault();

                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new Layer.Data.Sqls.ScCustoms.PayExchangeApplies
                {
                    ID = this.ID,
                    AdminID = this.Admin.ID,
                    ClientID = apply.ClientID,
                    SupplierName = apply.SupplierName,
                    SupplierEnglishName = apply.SupplierEnglishName,
                    SupplierAddress = apply.SupplierAddress,
                    BankAccount = apply.BankAccount,
                    BankAddress = apply.BankAddress,
                    BankName = apply.BankName,
                    SwiftCode = apply.SwiftCode,
                    ExchangeRateType = apply.ExchangeRateType,
                    Currency = apply.Currency,
                    ExchangeRate = this.ExchangeRate,
                    PaymentType = apply.PaymentType,
                    ExpectPayDate = apply.ExpectPayDate == null ? null : apply.ExpectPayDate,
                    SettlemenDate = apply.SettlemenDate,
                    OtherInfo = apply.OtherInfo,
                    PayExchangeApplyStatus = (int)apply.PayExchangeApplyStatus,
                    Status = (int)apply.Status,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = apply.Summary,
                    ABA = apply.ABA,
                    IBAN = apply.IBAN,
                    FatherID = this.FatherID,
                    IsAdvanceMoney = this.AdvanceMoney
                });

            }
        }
        public virtual void UnSplitItemEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //插入Items
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyItem),
                    PayExchangeApplyID = this.PayExchangeApplyID,
                    OrderID = this.OrderID,
                    Amount = this.TotalAmount,
                    Status = (int)Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    ApplyStatus = (int)Enums.ApplyItemStatus.Appling,
                });
            }
        }
        public virtual void StatusEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new { PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Audited, IsAdvanceMoney = this.AdvanceMoney }, item => item.ID == FatherID);
            }
        }
        #endregion
    }
}
