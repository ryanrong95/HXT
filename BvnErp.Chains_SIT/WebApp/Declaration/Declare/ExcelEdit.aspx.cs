using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class ExcelEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void UpdateEntryID()
        {
            try
            {
                string DeclarationID = Request.Form["ID"];
                string EntryID = Request.Form["EntryID"];
                string SeqNo = Request.Form["SeqNo"];
                Needs.Ccs.Services.Models.DecHead headinfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];
                headinfo.EntryId = EntryID;
                if (!string.IsNullOrEmpty(SeqNo))
                {
                    headinfo.SeqNo = SeqNo;
                }
                headinfo.ExcelDeclareDone();
                // 暂用 不是实际流程
                headinfo.DeclareSucceess();

                Response.Write(new { result = true,info="保存成功" }.Json());
            }
            catch(Exception ex)
            {
                Response.Write(new { result = false, info = "保存错误" + ex.ToString() }.Json());
            }
        }


    }
}