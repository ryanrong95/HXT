using Needs.Cbs.Services.Enums;
using Needs.Cbs.Services.Models.Origins;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Needs.Cbs.WebApp.BaseData
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Types = EnumUtils.ToDictionary<BaseType>().Select(item => new { item.Key, item.Value }).Json();
                LoadData();
            }
        }

        /// <summary>
        /// 初始化海关基础数据信息
        /// </summary>
        protected void LoadData()
        {
            string settingID = Request.QueryString["ID"];
            var setting = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.CustomsSettings[settingID];

            if (setting != null)
            {
                this.Model.Setting = new
                {
                    setting.ID,
                    setting.Code,
                    setting.Type,
                    setting.Name,
                    setting.EnglishName,
                    setting.Summary
                    
                }.Json();
            }
            else
            {
                this.Model.Setting = setting.Json();
            }
        }

        protected void Save()
        {
            string id = Request.Form["ID"];
            var setting = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.CustomsSettings[id] as CustomsSetting ?? new CustomsSetting();

            setting.Code = Request.Form["Code"];
            setting.Type = (BaseType)Enum.Parse(typeof(BaseType), Request.Form["Type"]);
            setting.Name = Request.Form["Name"];
            setting.EnglishName = Request.Form["EnglishName"];
            setting.Summary = Request.Form["Summary"];

            setting.EnterError += Setting_EnterError;
            setting.EnterSuccess += Setting_EnterSuccess;
            setting.Enter();
        }

        /// <summary>
        /// 保存成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Setting_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}