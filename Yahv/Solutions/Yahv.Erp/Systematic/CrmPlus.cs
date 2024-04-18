using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Erps;
using Yahv.Underly.Logs;
using Layers.Data.Sqls;

namespace Yahv.Systematic
{
    public class CrmPlus : IAction
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public CrmPlus(IErpAdmin admin)
        {
            this.admin = admin;
        }


        #region  views
        #region 标准品牌
        public Yahv.CrmPlus.Service.Views.Rolls.BrandsRoll StandardBrands
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.BrandsRoll();
            }
        }
        #endregion

        #region 标准型号
        public Yahv.CrmPlus.Service.Views.Rolls.StandardPartNumbersRoll PartNumbers
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.StandardPartNumbersRoll();
            }
        }
        #endregion

        #region 所有企业
        public Yahv.CrmPlus.Service.Views.Rolls.EnterprisesRoll Enterprises
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.EnterprisesRoll();
            }
        }
        #endregion

        #region 内部公司
        public Yahv.CrmPlus.Service.Views.Rolls.CompaniesRoll Companies
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.CompaniesRoll();
            }
        }
        #endregion

        #region 客户
        public Yahv.CrmPlus.Service.Views.Rolls.ClientsRoll Clients
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.ClientsRoll();
            }
        }
        #endregion
        /// <summary>
        //我的客户
        /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.MyClientsRoll MyClients
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.MyClientsRoll(this.admin);
            }
        }

        /// <summary>
        /// 我的注册记录
        /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.MyDraftClientsRoll MyDraftClients
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.MyDraftClientsRoll(this.admin);
            }
        }

        /// <summary>
        /// 我的Top10
        /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.MapsTopNRoll MyTops
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.MapsTopNRoll(this.admin);
            }
        }

        #region 供应商
        public Yahv.CrmPlus.Service.Views.Rolls.SuppliersRoll Suppliers
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.SuppliersRoll();
            }
        }
        /// <summary>
        ///我的注册记录
        /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.MySuppliersRoll MySuppliers
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.MySuppliersRoll(this.admin);
            }
        }
       /// <summary>
       /// 我的site记录
       /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.MySiteSupliersRoll MySiteSuppliers
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.MySiteSupliersRoll(this.admin);
            }
        }
        public Yahv.CrmPlus.Service.Views.Rolls.FixedSuppliersRoll FixedSuppliers
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.FixedSuppliersRoll();
            }
        }
        #endregion

        #region 我的联系人
        public Yahv.CrmPlus.Service.Views.Rolls.MyContactsRoll MyContacts
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.MyContactsRoll(this.admin);
            }
        }
        #endregion

        #region 银行账号
        public Yahv.CrmPlus.Service.Views.Rolls.BookAccountsRoll BookAccounts
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.BookAccountsRoll(this.admin);
            }
        }

        #endregion

        #region   所有的企业发票 
        public Yahv.CrmPlus.Service.Views.Rolls.InvoicesRoll Invoices
        {

            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.InvoicesRoll();

            }
        }

        #endregion

        #region   所有的企业地址
        public Yahv.CrmPlus.Service.Views.Rolls.AddressesRoll Addresses
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.AddressesRoll();
            }
        }

        /// <summary>
        ///客户账期申请
        /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.ClientCreditsApplyRoll ClientCreditsApply
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.ClientCreditsApplyRoll();
            }
        }

        /// <summary>
        ///客户授信申请
        /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.ClientCreditFlowsApplyRoll ClientCreditFlowsApply
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.ClientCreditFlowsApplyRoll(this.admin);
            }
        }

        #endregion

        #region 供应链客户
        public Yahv.CrmPlus.Service.Views.Rolls.ChainsClientsRoll Chains
        {
            get { return new Yahv.CrmPlus.Service.Views.Rolls.ChainsClientsRoll(); }
        }
        #endregion

        #region 我的供应链客户
        public Yahv.CrmPlus.Service.Views.Rolls.MyChainsClientsRoll MyChains
        {
            get { return new Yahv.CrmPlus.Service.Views.Rolls.MyChainsClientsRoll(this.admin); }
        }
        #endregion


        #region  我的跟踪点评
        public Yahv.CrmPlus.Service.Views.Rolls.TraceRecords.MyTraceCommentsRoll MyTraceComments
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.TraceRecords.MyTraceCommentsRoll(this.admin);
            }
        }

        #endregion

        #region  我的销售机会
        public Yahv.CrmPlus.Service.Views.Rolls.MyProjectRoll MyProjects
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.MyProjectRoll(this.admin);
            }
        }

        #endregion
        #region  我的报备
        public Yahv.CrmPlus.Service.Views.Rolls.ProjectReports.MyProjectReportRoll MyProjectReports
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.ProjectReports.MyProjectReportRoll(this.admin);
            }
        }

        #endregion

        #region  我的报价
        public Yahv.CrmPlus.Service.Views.Rolls.ProjectReports.MyAgentQuotRoll MyAgentQuots
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.ProjectReports.MyAgentQuotRoll(this.admin);
            }
        }

        #endregion


        #region  我的送样
        public Yahv.CrmPlus.Service.Views.Rolls.Samples.MySampleRoll MySamples
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.Samples.MySampleRoll(this.admin);
            }
        }

        #endregion

        #region 仓库
        public Yahv.CrmPlus.Service.Views.Rolls.WareHousesRoll WareHouses
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.WareHousesRoll();
            }
        }
        #endregion

        /// <summary>
        /// 门牌
        /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.oDoorsRoll oDoors
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.oDoorsRoll();
            }
        }

        /// <summary>
        /// 承运商
        /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.CarriersRoll Carriers
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.CarriersRoll();
            }
        }

        /// <summary>
        /// 司机
        /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.oDriversRoll oDrivers
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.oDriversRoll();
            }
        }

        /// <summary>
        /// 运输工具
        /// </summary>
        public Yahv.CrmPlus.Service.Views.Rolls.oTransportsRoll oTransports
        {
            get
            {
                return new Yahv.CrmPlus.Service.Views.Rolls.oTransportsRoll();
            }
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
