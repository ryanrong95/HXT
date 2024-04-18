using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.DeclarantCandidates
{
    public partial class ModifyManifestDoubleChecker : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string VoyageNo = Request.QueryString["VoyageNo"];
            string BillNo = Request.QueryString["BillNo"];
            string Status = Request.QueryString["Status"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string ContrNo = Request.QueryString["ContrNo"];
            var ManifestBills = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ManifestConsignmentLists.AsQueryable();

            //草稿状态
            ManifestBills = ManifestBills.Where(item => item.CusMftStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Draft));

            if (!string.IsNullOrEmpty(VoyageNo))
            {
                VoyageNo = VoyageNo.Trim();
                ManifestBills = ManifestBills.Where(item => item.VoyageNo.Contains(VoyageNo));
            }
            if (!string.IsNullOrEmpty(ContrNo))
            {
                ContrNo = ContrNo.Trim();
                ManifestBills = ManifestBills.Where(item => item.ContrNO.Contains(ContrNo));
            }
            if (!string.IsNullOrEmpty(BillNo))
            {
                BillNo = BillNo.Trim();
                ManifestBills = ManifestBills.Where(item => item.BillNo.Contains(BillNo));
            }
            if (!string.IsNullOrEmpty(Status))
            {
                Status = Status.Trim();
                ManifestBills = ManifestBills.Where(t => t.CusMftStatus == Status);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                var from = DateTime.Parse(StartDate);
                ManifestBills = ManifestBills.Where(t => t.CreateTime >= from);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                var to = DateTime.Parse(EndDate);
                ManifestBills = ManifestBills.Where(t => t.CreateTime <= to);
            }

            Func<Needs.Ccs.Services.Models.ManifestConsignmentList, object> convert = head => new
            {
                ID = head.ID,
                VoyageNo = head.VoyageNo,
                BillNo = head.BillNo,
                Port = head.Port,
                PackNo = head.PackNo,
                ConsigneeName = head.ConsigneeName,
                CreateTime = head.CreateTime.ToShortDateString(),
                AdminName = head.AdminName,
                StatusName = head.StatusName,
                DoubleCheckerName = head.DoubleCheckerName,
            };
            this.Paging(ManifestBills, convert);
        }

    }
}