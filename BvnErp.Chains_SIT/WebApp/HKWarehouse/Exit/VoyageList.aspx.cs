using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Exit
{
    /// <summary>
    /// 运输批次
    /// </summary>
    public partial class VoyageList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.Carriers = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Where(item => item.CarrierType == CarrierType.InteLogistics && item.Status == Status.Normal)
                                  .Select(item => new { item.Code, item.Name }).Json();
        }

        /// <summary>
        /// 初始化运输批次数据
        /// </summary>
        protected void data()
        {
            string voyageNo = Request.QueryString["VoyageNo"];
            string carrier = Request.QueryString["Carrier"];

            var manifest = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure.Where(v => v.CutStatus == CutStatus.Cutted||v.CutStatus==CutStatus.UnCutting).AsQueryable();

            if (!string.IsNullOrEmpty(voyageNo))
            {
                manifest = manifest.Where(item => item.ID.Contains(voyageNo.Trim()));
            }
            if (!string.IsNullOrEmpty(carrier))
            {
                manifest = manifest.Where(item => item.Carrier != null && item.Carrier.Code.Contains(carrier.Trim()));
            }

            Func<Needs.Ccs.Services.Models.Voyage, object> convert = item => new
            {
                item.ID,
                VoyageNo = item.ID,
                Carrier = item.Carrier?.Name,
                item.DriverCode,
                item.DriverName,
                item.HKLicense,
                item.CustomsCode,
                CreateTime = item.CreateTime.ToString("yyyy-MM-dd"),
                CutStatus = item.CutStatus.GetDescription(),
                TransportTime = item.TransportTime?.ToString("yyyy-MM-dd") ?? string.Empty,
                VoyageType = item.Type.GetDescription(),
            };

            this.Paging(manifest, convert);
        }
    }
}