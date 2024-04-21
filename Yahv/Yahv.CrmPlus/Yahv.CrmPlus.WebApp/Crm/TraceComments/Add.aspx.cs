using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.TraceComments
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    this.Model.ID = Request.QueryString["ID"];
            //}
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["TraceID"];
            var comment = Request.Form["Comments"].Trim();

            var olds = Erp.Current.CrmPlus.MyTraceComments.Where(x => x.TraceRecordID == id).ToArray();
            var entity = olds.FirstOrDefault(item => string.IsNullOrWhiteSpace(item.Comments) && item.AdminID == Erp.Current.ID) ?? new TraceComment
            {
                AdminID = Erp.Current.ID,
                Comments = comment,
                CreateDate = DateTime.Now,
                IsPointed = false,
                ModifyDate = DateTime.Now,
                TraceRecordID = id,
            };

            entity.Comments = comment;
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