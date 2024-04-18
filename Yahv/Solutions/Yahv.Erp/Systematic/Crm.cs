using Yahv.Underly.Erps;
using Yahv.Underly.Logs;
using Layers.Data.Sqls;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Systematic
{
    /// <summary>
    /// Crm 领域 客户关系 供应商关系
    /// </summary>
    public class Crm : IAction
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Crm(IErpAdmin admin)
        {
            this.admin = admin;
        }

        #region Views
        /// <summary>
        /// 所有管理员
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.AdminsAppendRoll Admins
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.AdminsAppendRoll();
            }
        }
        /// <summary>
        /// 所有公司
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll Enterprises
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll();
            }
        }
        /// <summary>
        /// 内部公司
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.CompaniesRoll Companies
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.CompaniesRoll();
            }
        }
        /// <summary>
        /// 所有客户
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.TradingClientsRoll Clients
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.TradingClientsRoll();
            }
        }
        /// <summary>
        /// 我的客户
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.MyTradingClientsRoll MyClients
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.MyTradingClientsRoll(this.admin);
            }
        }

        /// <summary>
        /// 库房
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.WareHousesRoll WareHouses
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.WareHousesRoll(this.admin);
            }
        }

        /// <summary>
        /// 我的联系人
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.MyTradingContactsRoll MyContacts
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.MyTradingContactsRoll(this.admin);
            }
        }
        /// <summary>
        /// 我的到货地址
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.MyTradingConsigneesRoll MyConsignees
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.MyTradingConsigneesRoll(this.admin);
            }
        }

        /// <summary>
        /// 流水账
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.FlowAccountsTopView FlowAccounts
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.FlowAccountsTopView();
            }
        }

        /// <summary>
        /// 信用视图统计
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.CreditsStatisticsView Credits
        {
            get
            {
                return new YaHv.Csrm.Services.Views.Rolls.CreditsStatisticsView();
            }
        }

        /// <summary>
        /// 财务科目
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.SubjectsRoll FinanceSubjects
        {
            get { return new YaHv.Csrm.Services.Views.Rolls.SubjectsRoll(); }
        }
        /// <summary>
        /// 承运商
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.CarriersRoll Carriers
        {
            get { return new YaHv.Csrm.Services.Views.Rolls.CarriersRoll(this.admin); }
        }

        /// <summary>
        /// 账期条款
        /// </summary>
        public YaHv.Csrm.Services.Views.Rolls.DebtTermsTopView DebtTerms
        {
            get { return new YaHv.Csrm.Services.Views.Rolls.DebtTermsTopView(); }
        }
        #endregion

        #region Action
        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="log"></param>
        public void Logs_Error(Logs_Error log)
        {
            using (var repository = new HvRFQReponsitory())
            {
                repository.Insert<Layers.Data.Sqls.HvRFQ.Logs_Errors>(new Layers.Data.Sqls.HvRFQ.Logs_Errors
                {
                    AdminID = admin.ID,
                    Page = log.Page,
                    Message = log.Message,
                    Codes = log.Codes,
                    Source = log.Source,
                    Stack = log.Stack,
                    CreateDate = log.CreateDate
                });
            }
        }
        #endregion
    }
}
