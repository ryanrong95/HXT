using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.DeclarantCandidates
{
    public partial class AddDeclarantCandidate : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string from = Request.QueryString["From"];

            this.Model.From = from;

            if (from == Needs.Ccs.Services.Enums.DeclarantCandidateType.Checker.ToString())
            {
                this.Model.CandidateData = new Needs.Ccs.Services.Views.SelectableCandidatesView().GetSelectableCandidates(Needs.Ccs.Services.Enums.DeclarantCandidateType.Checker)
                    .Select(item => new { value = item.AdminID, text = item.AdminName }).Json();
            }
            else if (from == Needs.Ccs.Services.Enums.DeclarantCandidateType.DeclareCreator.ToString())
            {
                this.Model.CandidateData = new Needs.Ccs.Services.Views.SelectableCandidatesView().GetSelectableCandidates(Needs.Ccs.Services.Enums.DeclarantCandidateType.DeclareCreator)
                    .Select(item => new { value = item.AdminID, text = item.AdminName }).Json();
            }
            else if (from == Needs.Ccs.Services.Enums.DeclarantCandidateType.CustomSubmiter.ToString())
            {
                this.Model.CandidateData = new Needs.Ccs.Services.Views.SelectableCandidatesView().GetSelectableCandidates(Needs.Ccs.Services.Enums.DeclarantCandidateType.CustomSubmiter)
                    .Select(item => new { value = item.AdminID, text = item.AdminName }).Json();
            }
            else if (from == Needs.Ccs.Services.Enums.DeclarantCandidateType.DoubleChecker.ToString())
            {
                this.Model.CandidateData = new Needs.Ccs.Services.Views.SelectableCandidatesView().GetSelectableCandidates(Needs.Ccs.Services.Enums.DeclarantCandidateType.DoubleChecker)
                    .Select(item => new { value = item.AdminID, text = item.AdminName }).Json();
            }
            else if (from == Needs.Ccs.Services.Enums.DeclarantCandidateType.ManifestDoubleChecker.ToString())
            {
                this.Model.CandidateData = new Needs.Ccs.Services.Views.SelectableCandidatesView().GetSelectableCandidates(Needs.Ccs.Services.Enums.DeclarantCandidateType.ManifestDoubleChecker)
                    .Select(item => new { value = item.AdminID, text = item.AdminName }).Json();
            }

        }

        protected void Save()
        {
            try
            {
                string From = Request.Form["From"];
                string AdminID = Request.Form["AdminID"];

                var declarantCandidate = new Needs.Ccs.Services.Models.DeclarantCandidate();
                declarantCandidate.ID = Guid.NewGuid().ToString("N");
                declarantCandidate.AdminID = AdminID;
                declarantCandidate.Status = Needs.Ccs.Services.Enums.Status.Normal;
                declarantCandidate.CreateTime = DateTime.Now;
                declarantCandidate.UpdateTime = DateTime.Now;

                if (From == Needs.Ccs.Services.Enums.DeclarantCandidateType.Checker.ToString())
                {
                    declarantCandidate.Type = Needs.Ccs.Services.Enums.DeclarantCandidateType.Checker;
                }
                else if (From == Needs.Ccs.Services.Enums.DeclarantCandidateType.DeclareCreator.ToString())
                {
                    declarantCandidate.Type = Needs.Ccs.Services.Enums.DeclarantCandidateType.DeclareCreator;
                }
                else if (From == Needs.Ccs.Services.Enums.DeclarantCandidateType.CustomSubmiter.ToString())
                {
                    declarantCandidate.Type = Needs.Ccs.Services.Enums.DeclarantCandidateType.CustomSubmiter;
                }
                else if (From == Needs.Ccs.Services.Enums.DeclarantCandidateType.DoubleChecker.ToString())
                {
                    declarantCandidate.Type = Needs.Ccs.Services.Enums.DeclarantCandidateType.DoubleChecker;
                }
                else if (From == Needs.Ccs.Services.Enums.DeclarantCandidateType.ManifestDoubleChecker.ToString())
                {
                    declarantCandidate.Type = Needs.Ccs.Services.Enums.DeclarantCandidateType.ManifestDoubleChecker;
                }

                declarantCandidate.Enter();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

    }
}