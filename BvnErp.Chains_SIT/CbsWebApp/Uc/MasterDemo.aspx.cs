using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Needs.Cbs.WebApp.Uc
{
    public partial class MasterDemo : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.Types = EnumUtils.ToDictionary<Needs.Cbs.Services.Enums.BaseType>().Select(item => new { item.Key, item.Value }).Json();
            Page.Title = "海关基础数据";
        }

        protected void data()
        {
            string code = Request.QueryString["Code"];
            string type = Request.QueryString["Type"];
            var customsSettings = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.CustomsSettings.AsQueryable();

            if (!string.IsNullOrEmpty(code))
            {
                customsSettings = customsSettings.Where(item => item.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(type))
            {
                customsSettings = customsSettings.Where(item => (int)item.Type == int.Parse(type));
            }

            Func<Needs.Cbs.Services.Models.Origins.CustomsSetting, object> convert = setting => new
            {
                setting.ID,
                setting.Code,
                TypeValue = setting.Type,
                Type = setting.Type.GetDescription(),
                setting.Name,
                setting.EnglishName,
                setting.Summary
            };

            this.Paging(customsSettings, convert);
        }
    }
}