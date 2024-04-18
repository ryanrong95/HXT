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
    public partial class CuttedList : Uc.PageBase
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

            var manifest = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure.AsQueryable()
                           .Where(item => item.CutStatus == CutStatus.Cutted);
            if (!string.IsNullOrEmpty(VoyageNo))
            {
                VoyageNo = VoyageNo.Trim();
                manifest = manifest.Where(item => item.ID.Contains(VoyageNo));
            }

            manifest = manifest.OrderByDescending(item => item.CreateTime);

            Func<Needs.Ccs.Services.Models.Voyage, object> convert = item => new
            {
                item.ID,
                VoyageNo = item.ID,
                Carrier = item.Carrier?.Name,
                item.DriverCode,
                item.DriverName,
                item.HKLicense,
                item.CustomsCode,
                CutStatus = item.CutStatus.GetDescription(),
                CreateTime = item.CreateTime.ToString("yyyy-MM-dd")
            };

            this.Paging(manifest, convert);
        }

        


    }
}