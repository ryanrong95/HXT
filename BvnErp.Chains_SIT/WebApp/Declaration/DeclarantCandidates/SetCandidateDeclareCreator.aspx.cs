using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.DeclarantCandidates
{
    /// <summary>
    /// 设置候选制单人
    /// </summary>
    public partial class SetCandidateDeclareCreator : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var declarantCandidatesView = new Needs.Ccs.Services.Views.DeclarantCandidatesView().AsQueryable();

            declarantCandidatesView = declarantCandidatesView.OrderBy(t => t.AdminName);

            declarantCandidatesView = declarantCandidatesView.Where(t => t.Type == Needs.Ccs.Services.Enums.DeclarantCandidateType.DeclareCreator);

            Func<Needs.Ccs.Services.Models.DeclarantCandidate, object> convert = item => new
            {
                DeclarantCandidateID = item.ID,
                AdminName = item.AdminName,
                CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            this.Paging(declarantCandidatesView, convert);
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void Delete()
        {
            try
            {
                string DeclarantCandidateID = Request.Form["DeclarantCandidateID"];

                var declarantCandidate = new Needs.Ccs.Services.Models.DeclarantCandidate();
                declarantCandidate.ID = DeclarantCandidateID;
                declarantCandidate.Abandon();

                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }

    }
}