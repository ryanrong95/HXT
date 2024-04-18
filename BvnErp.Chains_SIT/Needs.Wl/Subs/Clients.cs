using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;

namespace Needs.Wl.Admin.Plat.Models
{
    public partial class Admin
    {
        public Clients Clients
        {
            get
            {
                return new Clients(this);
            }
        }
    }

    public class Clients
    {
        IGenericAdmin Admin;

        public Clients(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 客户
        /// </summary>
        public Views.MyClientsView MyClients
        {
            get
            {
                return new Views.MyClientsView(this.Admin);
            }
        }

        public Needs.Ccs.Services.Views.ClientsView ClientsView
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientsView();
            }
        }


        public Needs.Ccs.Services.Views.SuperAdminClientsView SuperAdminClientsView
        {
            get
            {
                return new Needs.Ccs.Services.Views.SuperAdminClientsView();
            }
        }

        public Needs.Ccs.Services.Views.ClientControlsView ClientControlsView
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientControlsView();
            }
        }

        /// <summary>
        /// 付汇管控客户列表
        /// </summary>
        public Needs.Ccs.Services.Views.PayExControlListView PayExControlListView
        {
            get
            {
                return new Needs.Ccs.Services.Views.PayExControlListView();
            }
        }

        /// <summary>
        /// 客户的所有账户
        /// </summary>
        public Needs.Ccs.Services.Views.UsersView Users
        {
            get
            {
                return new Needs.Ccs.Services.Views.UsersView();
            }
        }

        /// <summary>
        /// 会员补充协议信息
        /// </summary>
        public Needs.Ccs.Services.Views.ClientAgreementsView ClientAgreements
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientAgreementsView();
            }
        }

        /// <summary>
        /// 发票信息
        /// </summary>
        public Needs.Ccs.Services.Views.ClientInvoicesView ClientInvoices
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientInvoicesView();
            }
        }

        /// <summary>
        /// 发票收件地址
        /// </summary>
        public Needs.Ccs.Services.Views.ClientInvoiceConsigneesView ClientInvoiceConsignees
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientInvoiceConsigneesView();
            }
        }

        /// <summary>
        /// 客户的供应商
        /// </summary>
        public Needs.Ccs.Services.Views.ClientSuppliersView ClientSuppliers
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientSuppliersView();
            }
        }

        /// <summary>
        /// 客户的供应商地址
        /// </summary>
        public Needs.Ccs.Services.Views.ClientSupplierAddressesView ClientSuppliersAddresses
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientSupplierAddressesView();
            }
        }

        /// <summary>
        /// 客户的收货人
        /// </summary>
        public Needs.Ccs.Services.Views.ClientConsigneesView ClientConsignees
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientConsigneesView();
            }
        }

        /// <summary>
        /// 客服人员信息
        /// </summary>
        public Needs.Ccs.Services.Views.ClientAdminsView ClientAdmins
        {
            get
            {
                return new Ccs.Services.Views.ClientAdminsView();
            }
        }

        /// <summary>
        /// 注册申请
        /// </summary>
        public Needs.Ccs.Services.Views.ServiceAppliesView ServiceApplies
        {
            get
            {
                return new Needs.Ccs.Services.Views.ServiceAppliesView();
            }
        }

        /// <summary>
        /// 注册申请
        /// </summary>
        public Views.MyServiceAppliesView MyServiceApplies
        {
            get
            {
                return new Views.MyServiceAppliesView(this.Admin);
            }
        }

        /// <summary>
        /// 付汇申请(管理端使用)
        /// </summary>
        public Needs.Ccs.Services.Views.AdminPayExchangeApplyView AdminPayExchangeApply
        {
            get
            {
                return new Needs.Ccs.Services.Views.AdminPayExchangeApplyView();
            }
        }

        /// <summary>
        /// 我的付汇申请
        /// </summary>
        public Views.MyPayExchangeApplysView MyPayExchangeApply
        {
            get
            {
                return new Views.MyPayExchangeApplysView(this.Admin);
            }
        }

        /// <summary>
        /// 我的付汇申请（待审核）
        /// </summary>
        public Views.MyUnAuditedPayExchangeApplysView MyUnAuditedPayExchangeApply
        {
            get
            {
                return new Views.MyUnAuditedPayExchangeApplysView(this.Admin);
            }
        }

        /// <summary>
        /// 付汇申请（待审批）
        /// </summary>
        public Needs.Ccs.Services.Views.UnApprovalPayExchangeApplyView UnApprovalPayExchangeApply
        {
            get
            {
                return new Needs.Ccs.Services.Views.UnApprovalPayExchangeApplyView();
            }
        }

        /// <summary>
        /// 付汇申请文件
        /// </summary>
        public Needs.Ccs.Services.Views.PayExchangeApplyFileView PayExchangeApplyFile
        {
            get
            {
                return new Needs.Ccs.Services.Views.PayExchangeApplyFileView();
            }
        }

        /// <summary>
        /// 付汇申请日志
        /// </summary>
        public Needs.Ccs.Services.Views.PayExchangeLogsView PayExchangeLogs
        {
            get
            {
                return new Needs.Ccs.Services.Views.PayExchangeLogsView();
            }
        }

        /// <summary>
        /// 付汇申请记录
        /// </summary>
        public Needs.Ccs.Services.Views.OrderPayExchangeItemsView OrderPayExchangeItems
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderPayExchangeItemsView();
            }
        }

        /// <summary>
        /// 会员日志
        /// </summary>
        public Needs.Ccs.Services.Views.ClientLogsView ClientLogs
        {
            get
            {
                return new Needs.Ccs.Services.Views.ClientLogsView();
            }
        }


        /// <summary>
        /// 客户自定义产品税务归类
        /// </summary>
        public Views.MyClientProductTaxCategoriesView MyClientProductTaxCategories
        {
            get
            {
                return new Views.MyClientProductTaxCategoriesView(this.Admin);

            }
        }

        public Views.MyStorageTopView MyStorageTopView
        {
            get
            {
                return new Views.MyStorageTopView(this.Admin);
            }
        }
    }
}
