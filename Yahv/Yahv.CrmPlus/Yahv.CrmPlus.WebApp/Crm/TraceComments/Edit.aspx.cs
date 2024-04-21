using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls.TraceRecords;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.TraceComments
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["ID"];
                this.Model.Readers = new YaHv.CrmPlus.Services.Views.Rolls.AdminsAllRoll().Where(item => item.RoleID == FixedRole.SaleManager.GetFixedID() || item.RoleID == FixedRole.PM.GetFixedID()).Select(item => new
                {
                    value = item.ID,
                    text = $"{item.RealName}-{item.RoleName}"
                });
                this.Model.ReaderIDs = string.Join(",", new TraceCommentsRoll().Where(x => x.TraceRecordID == id && x.IsPointed == true).Select(x => x.Admin.RealName).Distinct().ToArray());
                var entity = new TraceRecordsRoll()[id];
                this.Model.Entity = entity;
                this.Model.Contact = Erp.Current.CrmPlus.MyContacts[entity.ClientContactID];
                this.Model.files = new FilesDescriptionRoll()[entity.Enterprise.ID, entity.ID, CrmFileType.TraceRecords];
            }
        }

        protected object data()
        {
            var id = Request.QueryString["ID"];
            // string id = Request.Form["ID"];
            var comments = new MyTraceCommentsRoll(Erp.Current).Where(x => x.TraceRecordID == id).
                OrderByDescending(item => item.CreateDate).
                ToArray();

            var result = this.Paging(comments.Select(item => new
            {
                item.ID,
                Reader = item.Admin.RealName,
                item.Comments,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;

        }

    }
}