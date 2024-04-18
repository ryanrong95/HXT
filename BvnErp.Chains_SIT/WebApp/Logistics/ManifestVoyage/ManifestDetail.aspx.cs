using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Logistics.ManifestVoyage
{
    /// <summary>
    /// 舱单申报详情
    /// </summary>
    public partial class ManifestDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            var id = Request.QueryString["ID"];
            var manifest = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Manifests[id];

            if (manifest == null)
            {
                this.Model.IsShowManifest = false;
            }
            else
            {
                this.Model.IsShowManifest = true;
            }
        }

        protected void dataManifest()
        {
            var id = Request.Form["ID"];
            var manifest = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Manifests[id];
            var consignments = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ManifestConsignmentInfos.Where(mc => mc.Manifest.ID == id 
            && mc.CusMftStatus != Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Deleted)).ToList();

            Response.Write(new
            {
                Manifest = new
                {
                    VoyageNo = manifest.ID,
                    ManifestType = "进口舱单",
                    CustomsCode = manifest.CustomsCode,
                    LoadingDate = manifest.LoadingDate?.ToShortDateString(),
                    LoadingLocationCode = manifest.LoadingLocationCode,
                    TotalPackNoGrossWt  = consignments.Sum(mc=>mc.PackNum) + "/" + consignments.Sum(mc => mc.GrossWt).ToString("0.##"),
                    TotalAmount = consignments.Sum(mc => mc.GoodsValue),
                    Summary = manifest.Summary
                },
                Consignments = consignments
            }.Json());
        }
    }
}