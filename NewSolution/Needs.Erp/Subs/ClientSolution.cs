using Layer.Data.Sqls;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Needs.Erp.Models
{
    public partial class Admin
    {
        public ClientSolution ClientSolutions
        {
            get
            {
                return new ClientSolution(this);
            }
        }
    }
}

namespace Needs.Erp
{

    public class ClientSolution
    {
        IGenericAdmin admin;

        internal ClientSolution(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        public NtErp.Crm.Services.Views.MyClientsView MyClients
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyClientsView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyClientBaseView MyClientsBase
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyClientBaseView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.PublicWarningClientView WarningClients
        {
            get
            {
                return new NtErp.Crm.Services.Views.PublicWarningClientView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyProjectStatisticsView MyProjectStatistics
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyProjectStatisticsView(this.admin);
            }
        }
        public NtErp.Crm.Services.Views.ClientAlls Clients
        { 
            get
            {
                return new NtErp.Crm.Services.Views.ClientAlls();
                //throw new Exception("重新实现");
            }
        }

        public NtErp.Crm.Services.Views.DistrictAlls Districts
        {
            get
            {
                return new NtErp.Crm.Services.Views.DistrictAlls();
            }
        }

        public NtErp.Crm.Services.Views.StandardProductAlls StandardProducts
        {
            get
            {
                return new NtErp.Crm.Services.Views.StandardProductAlls();
            }
        }

        public NtErp.Crm.Services.Views.MyStaffsView MyStaffs
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyStaffsView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.AdminProjectViewBase FAE
        {
            get
            {
                return new NtErp.Crm.Services.Views.FaeView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.AdminProjectViewBase PME
        {
            get
            {
                return new NtErp.Crm.Services.Views.PmView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyPlansView MyPlans
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyPlansView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyAppliesView MyApplies
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyAppliesView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.PublicClientsView PublicClients
        {
            get
            {
                return new NtErp.Crm.Services.Views.PublicClientsView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyManufactureView MyManufactures
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyManufactureView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.CompanyAlls Companys
        {
            get
            {
                return new NtErp.Crm.Services.Views.CompanyAlls();
            }
        }
        public NtErp.Crm.Services.Views.MyWorksWeeklyView WorksWeekly
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyWorksWeeklyView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyWorksOtherView WorksOther
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyWorksOtherView(this.admin);
            }
        }
        public NtErp.Crm.Services.Views.MyWorkWarningsView WorksWarning
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyWorkWarningsView(this.admin);
            }
        }
        public NtErp.Crm.Services.Views.MyBeneficiariesView Beneficiaries
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyBeneficiariesView(this.admin);
            }
        }
        public NtErp.Crm.Services.Views.MyOrdersView Order
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyOrdersView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyIndustriesView MyIndustries
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyIndustriesView(this.admin);
            }
        }
        public NtErp.Crm.Services.Views.ContactAlls Contacts
        {
            get
            {
                return new NtErp.Crm.Services.Views.ContactAlls(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyProjectView MyProjects
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyProjectView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyChargesView MyCharges
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyChargesView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyReadReportView MyReadReports
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyReadReportView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyReportsView MyReports
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyReportsView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.MyClientReportsView MyClientReports
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyClientReportsView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.CatelogueView Catelogues
        {
            get
            {
                return new NtErp.Crm.Services.Views.CatelogueView(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.InvoiceAll MyInvoices
        {
            get
            {
                return new NtErp.Crm.Services.Views.InvoiceAll(this.admin);
            }
        }

        public NtErp.Crm.Services.Views.StaffAlls Staffs
        {
            get
            {
                return new NtErp.Crm.Services.Views.StaffAlls();
            }
        }

        public NtErp.Crm.Services.Views.ConsigneeAlls Consignees
        {
            get
            {
                return new NtErp.Crm.Services.Views.ConsigneeAlls();
            }
        }

        public NtErp.Crm.Services.Views.MyNoticeView MyNotice
        {
            get
            {
                return new NtErp.Crm.Services.Views.MyNoticeView(this.admin);
            }
        }
    }
}

