using Needs.Cbs.Services.Models.Origins;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Needs.Cbs.WebApp.OriginTariff
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {
            this.Model.OriginData = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.CustomsSettings[Cbs.Services.Enums.BaseType.Country].Select(item => new { value = item.Code, text = item.Code + "-" + item.Name }).Json();
        }

        protected void data()
        {
            var origintariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.OriginTariffs.AsQueryable();
            string origin = Request.QueryString["Origin"];
            string name = Request.QueryString["Name"];
            string hsCode = Request.QueryString["HSCode"];

            if (!string.IsNullOrEmpty(origin))
            {
                origintariff = origintariff.Where(item => item.Origin == origin);
            }

            if (!string.IsNullOrEmpty(hsCode))
            {
                origintariff = origintariff.Where(item => item.HSCode == hsCode);
            }

            if (!string.IsNullOrEmpty(name))
            {
                origintariff = origintariff.Where(item => item.Name.Contains(name));
            }

            //对象转化
            Func<Needs.Cbs.Services.Models.Origins.CustomsOriginTariff, object> linq = item => new
            {
                ID = item.ID,
                Name = item.Name,
                HSCode = item.HSCode,
                Origin = item.CountryName,
                Type = item.Type.GetDescription(),
                Rate = item.Rate,
                StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                EndDate = item.EndDate?.ToString("yyyy-MM-dd"),
                item.CreateDate
            };

            this.Paging(origintariff, linq);
        }

        /// <summary>
        /// 数据删除
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.OriginTariffs[id] as CustomsOriginTariff ?? new CustomsOriginTariff();
            if (del != null)
            {
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }

        protected void MutilDelete()
        {
            string ids = Request.Form["IDs"];
            string[] arrid = ids.Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');
            foreach (string id in arrid)
            {
                var del = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.OriginTariffs[id];
                if (del != null)
                {
                    del.Abandon();
                }
            }
            Del_AbandonSuccess(null, null);
        }
    }
}