using Needs.Ccs.Services.Enums;
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
    /// <summary>
    /// 运输批次列表界面
    /// </summary>
    public partial class List : Uc.PageBase
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
            this.Model.CutStatus = EnumUtils.ToDictionary<CutStatus>().Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 初始化运输批次数据
        /// </summary>
        protected void data()
        {
            string voyageNo = Request.QueryString["VoyageNo"];
            string carrier = Request.QueryString["Carrier"];
            string cutStatus = Request.QueryString["CutStatus"];

            var manifest = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure.AsQueryable();

            if (!string.IsNullOrEmpty(voyageNo))
            {
                manifest = manifest.Where(item => item.ID.Contains(voyageNo.Trim()));
            }
            if (!string.IsNullOrEmpty(carrier))
            {
                manifest = manifest.Where(item => item.Carrier != null && item.Carrier.Code.Contains(carrier.Trim()));
            }
            if (!string.IsNullOrEmpty(cutStatus))
            {
                int status = Int32.Parse(cutStatus);
                manifest = manifest.Where(item => item.CutStatus == (CutStatus)status);
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
                CutStatusValue = item.CutStatus,
                CutStatus = item.CutStatus.GetDescription(),
                TransportTime = item.TransportTime?.ToString("yyyy-MM-dd") ?? string.Empty,
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