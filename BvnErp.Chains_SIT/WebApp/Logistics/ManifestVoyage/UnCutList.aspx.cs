using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Logistics.ManifestVoyage
{
    public partial class UnCutList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected void data()
        {
            string VoyageNo = Request.QueryString["VoyageNo"];

            var manifest = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure.Where(t => t.CutStatus == CutStatus.UnCutting).AsQueryable();

            if (!string.IsNullOrEmpty(VoyageNo))
            {
                VoyageNo = VoyageNo.Trim();
                manifest = manifest.Where(item => item.ID.Contains(VoyageNo));
            }

            manifest = manifest.OrderByDescending(t => t.CreateTime);

            Func<Needs.Ccs.Services.Models.Voyage, object> convert = item => new
            {
                item.ID,
                VoyageNo = item.ID,
                Carrier = item.Carrier?.Name,
                item.DriverCode,
                item.DriverName,
                item.HKLicense,
                item.CustomsCode,
                //Type = item.Type.GetDescription(),
                CreateTime = item.CreateTime.ToString("yyyy-MM-dd"),
                TransportTime = (item.TransportTime != null) ? item.TransportTime?.ToString("yyyy-MM-dd") : string.Empty,
                VoyageType = item.Type.GetDescription(),
            };

            this.Paging(manifest, convert);
        }

        /// <summary>
        /// 截单操作
        /// </summary>
        protected void SureCut()
        {
            try
            {
                string id = Request.Form["ID"];
                var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[id];
                if (entity != null)
                {
                    entity.SureCut();
                }
                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }


    }
}