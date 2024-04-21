using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls.TraceRecords;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.TraceRecords
{
    public partial class Detail : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["subid"];
                var entity = new TraceRecordsRoll()[id];
                this.Model.Readers = string.Join(",", new TraceCommentsRoll().Where(x => x.TraceRecordID == id && x.IsPointed==true).Select(x => x.Admin.RealName).ToArray());
                this.Model.Contact = Erp.Current.CrmPlus.MyContacts[entity.ClientContactID];
                this.Model.Entity = entity;
                this.Model.Comments= new TraceCommentsRoll().Where(x => x.TraceRecordID == id && x.Comments != null).ToArray();
            }
        }


        protected object data()
        {

            string id = Request.QueryString["subid"];
            var comments = new TraceCommentsRoll().Where(x => x.TraceRecordID == id && x.Comments != null).ToArray();
            var result = this.Paging(comments.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
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