using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls.TraceRecords;
using Yahv.Underly;

namespace Yahv.CrmPlus.WebApp.Uc
{
    public partial class Admins : System.Web.UI.UserControl
    {

        public object[] AdminsList;
        public object[] SelectedAdminList;
        /// <summary>
        /// 角色ID
        /// </summary>
        public string[] RoleIds { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            var traceRecordID = Request.QueryString["TraceRecordID"];
            if (!IsPostBack)
            {
                if (this.RoleIds?.Count() > 0)
                {
                    var admins = new YaHv.CrmPlus.Services.Views.Rolls.AdminsAllRoll().Where(item => RoleIds.Contains(item.RoleID)).Select(item => new
                    {
                        ID = item.ID,
                        Name = item.RealName,
                        RoleName = item.RoleName
                    });

                    this.AdminsList = admins.ToArray();
                }
                else
                {
                    var admins = new YaHv.CrmPlus.Services.Views.Rolls.AdminsAllRoll().Select(item => new
                    {
                        ID = item.ID,
                        Name = item.RealName,
                        RoleName = item.RoleName
                    });
                    this.AdminsList = admins.ToArray();
                }

                var selected = new TraceCommentsRoll().Where(item => item.TraceRecordID == traceRecordID && item.IsPointed == true).Select(item => new
                {
                    ID = item.AdminID,
                    Name = item.Admin.RealName,
                    RoleName = item.Admin.RoleName

                });
                this.SelectedAdminList = selected.ToArray();

            }

        }


    }
}