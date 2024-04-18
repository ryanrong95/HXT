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
        public Permissions Permissions
        {
            get
            {
                return new Permissions(this);
            }
        }
    }
    public class Permissions
    {
        IGenericAdmin Admin;

        public Permissions(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        public Ccs.Services.Views.RoleViews Roles
        {
            get
            {
                return new Ccs.Services.Views.RoleViews();
            }
        }
        public Ccs.Services.Views.AdminRoleViews AdminRoles
        {
            get
            {
                return new Ccs.Services.Views.AdminRoleViews();
            }
        }
        public Ccs.Services.Views.SuggestionViews Suggestions
        {
            get
            {
                return new Ccs.Services.Views.SuggestionViews();
            }
        }
        public Ccs.Services.Views.AdminRolesTopView AdminRolesAll
        {
            get
            {
                return new Ccs.Services.Views.AdminRolesTopView();
            }
        }
    }
}
