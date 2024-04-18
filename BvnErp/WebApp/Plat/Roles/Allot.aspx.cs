using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Plat.Roles
{
    /// <summary>
    /// 为角色分配管理员
    /// </summary>
    public partial class Allot : Needs.Web.Sso.Forms.ErpPage
    {

        /// <summary>
        /// 当前会话角色
        /// </summary>
        protected NtErp.Services.Models.Role CurrentRole
        {
            get
            {
                var id = Request["id"];
                if (string.IsNullOrWhiteSpace(id))
                {
                    return null;
                }
                return new NtErp.Services.Views.RoleView().SingleOrDefault(t => t.ID == id);
            }
        }

        protected IEnumerable<ShowModel> Inners;
        protected IEnumerable<ShowModel> Outers;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadData();
            }
        }

        protected void LoadData()
        {
            if (this.CurrentRole == null)
            {
                return;
            }

            var roles = new NtErp.Services.Views.RoleView().ToArray();
            var maps = new NtErp.Services.Views.MapsAdminRoleView().ToArray();
            var admins = new NtErp.Services.Views.MyStaffsView(Needs.Erp.ErpPlot.Current).Where(t => t.Status == Needs.Erp.Generic.Status.Normal).ToArray();
            // 组内管理员
            var inners = maps.Where(t => t.RoleID == this.CurrentRole.ID).Select(t => t.AdminID).ToArray();

            var arry = admins.Select(t =>
            {
                var map = maps.Where(m => m.AdminID == t.ID).Select(m => m.RoleID);
                var role = roles.Where(r => map.Contains(r.ID));
                return new ShowModel
                {
                    ID = t.ID,
                    UserName = t.UserName,
                    RoleIDs = role.Select(r => r.ID),
                    RoleNames = role.Select(r => $"[{r.Name}]")
                };
            }).Where(t => t.ID != "SA0000000001");

            this.Inners = arry.Where(t => inners.Contains(t.ID)).ToArray();
            this.Outers = arry.Where(t => !inners.Contains(t.ID)).ToArray();

        }

        protected void allot()
        {
            var adminID = Request["adminid"];
            var isCheck = Convert.ToBoolean(Request["ischeck"]);
            if (!string.IsNullOrWhiteSpace(adminID))
            {
                var entity = new NtErp.Services.Models.MapAdminRole
                {
                    AdminID = adminID,
                    RoleID = this.CurrentRole.ID
                };
                if (isCheck)
                {
                    entity.Enter();
                }
                else
                {
                    entity.Abandon();
                }
            }

        }


        protected class ShowModel
        {
            public string ID { get; set; }
            public string UserName { get; set; }
            public IEnumerable<string> RoleIDs { get; set; }
            public IEnumerable<string> RoleNames { get; set; }
        }
    }
}