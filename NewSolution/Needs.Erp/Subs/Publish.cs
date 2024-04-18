using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Needs.Erp.Models
{
    /// <summary>
    /// Admin 
    /// </summary>
    public partial class Admin
    {
        /// <summary>
        /// 公共块
        /// </summary>
        public Publish Publishs
        {
            get
            {
                return new Publish(this);
            }
        }
    }
}

namespace Needs.Erp
{
    /// <summary>
    /// 刘芳 与 田洁茹目前专用
    /// </summary>
    public class Publish
    {
        IGenericAdmin admin;

        internal Publish(IGenericAdmin admin)
        {
            this.admin = admin;
        }
        public NtErp.Vrs.Services.Views.VendersView MyVenders
        {
            get
            {
                //Adapter<ISalesOrder, Views.SalesOrderAlls>.Current;
                return new NtErp.Vrs.Services.Views.VendersView();
            }
        }
        public NtErp.Vrs.Services.Views.MyCompaniesView MyAdminsComapnies
        {
            get
            {
                return new NtErp.Vrs.Services.Views.MyCompaniesView(null);
            }
        }
        public NtErp.Vrs.Services.Views.CompaniesView CompaniesAll
        {
            get { return new NtErp.Vrs.Services.Views.CompaniesView(); }
        }
        public NtErp.Vrs.Services.Views.InvoicesView InvoicesAll
        {
            get { return new NtErp.Vrs.Services.Views.InvoicesView(); }
        }
        public NtErp.Vrs.Services.Views.ContactsView MyContacts
        {
            get { return new NtErp.Vrs.Services.Views.ContactsView(); }
        }
        public NtErp.Vrs.Services.Views.MyContactView MyContactViews
        {
            get
            {
                return new NtErp.Vrs.Services.Views.MyContactView(this.admin);
            }
        }
        public NtErp.Vrs.Services.Views.BeneficiariesView MyBeneficiaries
        {
            get { return new NtErp.Vrs.Services.Views.BeneficiariesView(); }
        }
        /// <summary>
        /// Bom单
        /// </summary>
        public  NtErp.Wss.Services.Views.BomsView  MyBoms
        {
            get { return new NtErp.Wss.Services.Views.BomsView(); }
        }
    }
}
