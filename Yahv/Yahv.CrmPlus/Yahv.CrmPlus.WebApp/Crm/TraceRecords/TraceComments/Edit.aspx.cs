using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls.TraceRecords;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.TraceRecords.TraceComments
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        //protected object data()
        //{

        //    string id = Request.QueryString["TraceID"];
        //    var comments = new TraceCommentsRoll().Where(x => x.TraceRecordID == id && x.Comments != null).ToArray();
        //    var result = this.Paging(comments.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
        //    {
        //        item.ID,
        //        Reader = item.Admin.RealName,
        //        item.Comments,
        //        CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
        //    }));

        //    return result;

        //}
      
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var ID = Request.QueryString["TraceID"];
            var comment = Request.Form["Comments"].Trim();
            var entity = new TraceComment();
            entity.Comments = comment;
            entity.TraceRecordID = ID;
            entity.IsPointed = false;
            entity.AdminID = Erp.Current.ID;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var traceComment = sender as TraceComment;
            Service.LogsOperating.LogOperating(Erp.Current, traceComment.ID, $"点评了跟踪记录，ID:{traceComment.TraceRecordID}");
            Easyui.Dialog.Close("提交成功!", Web.Controls.Easyui.AutoSign.Success); ;
        }
    }
}