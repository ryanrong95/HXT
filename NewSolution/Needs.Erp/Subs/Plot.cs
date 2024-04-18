using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Needs.Erp.Models
{
    /// <summary>
    /// Admin 
    /// </summary>
    public partial class Admin
    {
        /// <summary>
        /// 小块基址
        /// </summary>
        public Plot Plots
        {
            get
            {
                return new Plot(this);
            }
        }
    }
}

namespace Needs.Erp
{
    public partial class Plot
    {
        IGenericAdmin admin;

        internal Plot(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        /// <summary>
        /// 菜单
        /// </summary>
        public NtErp.Services.Views.MyMenusView MyMenus
        {
            get
            {
                return new NtErp.Services.Views.MyMenusView(this.admin);
            }
        }
        /// <summary>
        /// 颗粒化
        /// </summary>
        public NtErp.Services.Views.MyUnitesView MyUnites
        {
            get
            {
                return new NtErp.Services.Views.MyUnitesView(this.admin);
            }
        }

        /// <summary>
        /// 我的员工
        /// </summary>
        public NtErp.Services.Views.MyStaffsView MyStaffs
        {
            get
            {
                return new NtErp.Services.Views.MyStaffsView(this.admin);
            }
        }


        //public Views.PlotMyStaffsView Staffs(IGenericAdmin currentadmin)
        //{

        //      return new Views.PlotMyStaffsView(currentadmin);

        //}

        //public MyClientsView MyClients
        //{
        //    get
        //    {
        //        return new MyClientsView(this.admin);
        //    }
        //}


        public NtErp.Services.Views.AdminsAlls Admins
        {
            get
            {
                return new NtErp.Services.Views.AdminsAlls();
            }
        }

        public NtErp.Services.Views.AdminClientsView MapAdminClients
        {
            get { return new NtErp.Services.Views.AdminClientsView(null); }
        }

        public NtErp.Services.Views.AdminClientsView_En MapAdminClients_En
        {
            get { return new NtErp.Services.Views.AdminClientsView_En(null); }
        }



        //public NtErp.Services.Views.AppliesAll Applies
        //{
        //    get { return new NtErp.Services.Views.AppliesAll(); }
        //}



        //public NtErp.Services.Views.MapsAdminClientView AdminClients
        //{

        //    get
        //    {
        //        return new NtErp.Services.Views.MapsAdminClientView();
        //    }
        //}
    }
}
