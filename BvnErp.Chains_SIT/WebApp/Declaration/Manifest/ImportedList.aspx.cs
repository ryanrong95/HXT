using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Manifest
{
    public partial class ImportedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Data();
        }
        protected void Load_Data()
        {
            this.Model.CusReceiptCodeData = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CusMftStatus>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }
        protected void data()
        {
            string VoyageNo = Request.QueryString["VoyageNo"];
            string BillNo = Request.QueryString["BillNo"];
            string CusDecStatus = Request.QueryString["BaseCusReceiptCode"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string ContrNo = Request.QueryString["ContrNo"];
            var ManifestBills = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ManifestConsignmentLists.AsQueryable();

            //已申报及其之后的状态
            ManifestBills = ManifestBills.Where(item => item.CusMftStatus != Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Draft)
                            && item.CusMftStatus != Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Make));

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
            if (!string.IsNullOrEmpty(CusDecStatus))
            {
                CusDecStatus = CusDecStatus.Trim();
                ManifestBills = ManifestBills.Where(t => t.CusMftStatus == CusDecStatus);
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
                Status = head.CusMftStatus,
                StatusName = head.StatusName,
            };
            this.Paging(ManifestBills, convert);
        }

        /// <summary>
        /// 批量申报
        /// </summary>
        protected void Declare()
        {
            try
            {
                string[] ManifestIDs = Request.Form["ManifestIDs"].Split(',');
                foreach (string s in ManifestIDs)
                {
                    var manifest = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ManifestConsignments.First(c => c.ID == s);
                    manifest.Apply();
                }

                Response.Write((new { success = true, message = "保存成功，等待发送海关！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}