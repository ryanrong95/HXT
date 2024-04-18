using System;
using Yahv.Finance.Services.Views;
using Yahv.Underly.Erps;
using Yahv.Underly.Logs;

namespace Yahv.Systematic
{
    /// <summary>
    /// 中心财务
    /// </summary>
    public class Finance : IAction
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Finance(IErpAdmin admin)
        {
            this.admin = admin;
        }

        #region Views

        /// <summary>
        /// admins
        /// </summary>
        public Yahv.Finance.Services.Views.AdminsTopView Admins
        {
            get
            {
                return new Yahv.Finance.Services.Views.AdminsTopView();
            }
        }

        /// <summary>
        /// 收付款类型视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.AccountCatalogsRoll AccountCatalogs
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.AccountCatalogsRoll();
            }
        }

        /// <summary>
        /// 金库视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.GoldStoresRoll GoldStores
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.GoldStoresRoll();
            }
        }

        /// <summary>
        /// 银行视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.BanksRoll Banks
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.BanksRoll();
            }
        }

        /// <summary>
        /// 银行风险地区
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.BankRiskAreasRoll BankRiskAreas
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.BankRiskAreasRoll();
            }
        }

        /// <summary>
        /// 企业
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.EnterprisesRoll Enterprises
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.EnterprisesRoll();
            }
        }

        /// <summary>
        /// 账户
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.AccountsRoll Accounts
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.AccountsRoll();
            }
        }

        /// <summary>
        /// 银行账户类型
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.AccountTypesRoll AccountTypes
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.AccountTypesRoll();
            }
        }

        /// <summary>
        /// 账户用途
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.AccountPurposesRoll AccountPurposes
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.AccountPurposesRoll();
            }
        }

        /// <summary>
        /// MapsAccountTypeRoll
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.MapsAccountTypeRoll MapsAccountType
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.MapsAccountTypeRoll();
            }
        }

        /// <summary>
        /// MapsPurposeRoll
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.MapsPurposeRoll MapsPurpose
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.MapsPurposeRoll();
            }
        }

        /// <summary>
        /// 个人
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.PersonsRoll Persons
        {
            get
            {
                return new Yahv.Finance.Services.Views.Rolls.PersonsRoll();
            }
        }

        /// <summary>
        /// CompaniesTopView
        /// </summary>
        public Yahv.Finance.Services.Views.CompaniesTopView CompaniesTopView
        {
            get
            {
                return new Yahv.Finance.Services.Views.CompaniesTopView();
            }
        }

        /// <summary>
        /// 货款申请对账单视图
        /// </summary>
        public Yahv.Finance.Services.Views.PayerStatementsView PayerStatementsView
        {
            get { return new Yahv.Finance.Services.Views.PayerStatementsView(); }
        }

        /// <summary>
        /// 货款申请
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.PayerAppliesRoll PayerAppliesView
        {
            get { return new Yahv.Finance.Services.Views.Rolls.PayerAppliesRoll(); }
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.LogsApplyStepRoll LogsApplyStepView
        {
            get { return new Yahv.Finance.Services.Views.Rolls.LogsApplyStepRoll(); }
        }

        /// <summary>
        /// 文件上传视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.FilesDescriptionRoll FilesDescriptionView
        {
            get { return new Yahv.Finance.Services.Views.Rolls.FilesDescriptionRoll(); }
        }

        /// <summary>
        /// 流水表视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.FlowAccountsRoll FlowAccounts
        {
            get { return new Yahv.Finance.Services.Views.Rolls.FlowAccountsRoll(); }
        }

        /// <summary>
        /// 资金调拨视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.SelfAppliesRoll SelfAppliesView
        {
            get { return new Yahv.Finance.Services.Views.Rolls.SelfAppliesRoll(); }
        }

        /// <summary>
        /// 资金申请视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.CostAppliesRoll CostApplies
        {
            get { return new Yahv.Finance.Services.Views.Rolls.CostAppliesRoll(); }
        }

        /// <summary>
        /// 资金申请项视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.CostApplyItemsRoll CostApplyItems
        {
            get { return new Yahv.Finance.Services.Views.Rolls.CostApplyItemsRoll(); }
        }

        /// <summary>
        /// 费用申请视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.ChargeAppliesRoll ChargeApplies
        {
            get { return new Yahv.Finance.Services.Views.Rolls.ChargeAppliesRoll(); }
        }

        /// <summary>
        /// 费用申请项视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.ChargeApplyItemsRoll ChargeApplyItems
        {
            get { return new Yahv.Finance.Services.Views.Rolls.ChargeApplyItemsRoll(); }
        }

        /// <summary>
        /// 收款视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.PayeeLeftsRoll PayeeLefts
        {
            get { return new Yahv.Finance.Services.Views.Rolls.PayeeLeftsRoll(); }
        }

        /// <summary>
        /// 税率视图
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.TaxRatesRoll TaxRates
        {
            get { return new Yahv.Finance.Services.Views.Rolls.TaxRatesRoll(); }
        }

        /// <summary>
        /// 工资申请
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.SalaryAppliesRoll SalaryApplies
        {
            get { return new Yahv.Finance.Services.Views.Rolls.SalaryAppliesRoll(); }
        }

        /// <summary>
        /// 工资申请项
        /// </summary>
        public Yahv.Finance.Services.Views.Rolls.SalaryApplyItemsRoll SalaryApplyItems
        {
            get { return new Yahv.Finance.Services.Views.Rolls.SalaryApplyItemsRoll(); }
        }
        #endregion

        #region Action
        public void Logs_Error(Logs_Error log)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
