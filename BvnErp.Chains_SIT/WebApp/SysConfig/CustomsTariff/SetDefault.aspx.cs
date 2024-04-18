using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.CustomsTariff
{
    /// <summary>
    /// 设置申报要素默认值
    /// </summary>
    public partial class SetDefault : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 保存默认值
        /// </summary>
        protected void SaveDefaults()
        {
            try
            {
                string Model = HttpUtility.UrlDecode(Request.Form["Model"]).Replace("&quot;", "\'").Replace("amp;", "");
                dynamic model = Model.JsonTo<dynamic>();

                string hsCode = model["HSCode"];
                var tariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs.Where(item => item.HSCode == hsCode.Trim()).FirstOrDefault();
                var elements = tariff.Elements.Split(';');
                var elementsDefaults = tariff.ElementsDefaults;
                ElementsDefault elementsDefault;

                //设置税则申报要素默认值
                foreach (var element in elements)
                {
                    string elementName = element.Split(':')[1];
                    string defaultValue = model[elementName];
                    elementsDefault = elementsDefaults.Where(item => item.ElementName == elementName).FirstOrDefault() ?? new ElementsDefault();
                    if (!string.IsNullOrWhiteSpace(defaultValue))
                    {
                        //新增或修改默认值
                        elementsDefault.CustomsTariffID = tariff.ID;
                        elementsDefault.ElementName = elementName;
                        elementsDefault.DefaultValue = model[elementName];
                        elementsDefault.Enter();
                    }
                    else
                    {
                        //删除之前的默认值
                        if (elementsDefault.ID != null)
                        {
                            elementsDefault.Delete();
                        }
                    }
                }

                //设置其他申报要素默认值
                string otherDefault = model["其他"];
                elementsDefault = elementsDefaults.Where(item => item.ElementName == "其他").FirstOrDefault() ?? new ElementsDefault();
                if (!string.IsNullOrWhiteSpace(otherDefault))
                {
                    //新增或修改默认值
                    elementsDefault.CustomsTariffID = tariff.ID;
                    elementsDefault.ElementName = "其他";
                    elementsDefault.DefaultValue = otherDefault;
                    elementsDefault.Enter();
                }
                else
                {
                    //删除之前的默认值
                    if (elementsDefault.ID != null)
                    {
                        elementsDefault.Delete();
                    }
                }

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }

        }

        /// <summary>
        /// 获取申报要素
        /// </summary>
        /// <returns></returns>
        protected object GetElements()
        {
            string HSCode = Request.Form["HScode"];
            var tariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs.Where(item => item.HSCode == HSCode.Trim()).FirstOrDefault();
            if (tariff != null)
            {
                //查询申报要素默认值
                var elementsDefaults = tariff.ElementsDefaults.Select(item => new { item.ElementName, item.DefaultValue });

                return new
                {
                    //报关申报要素
                    DeclareElements = tariff.Elements,
                    //申报要素默认值 
                    ElementsDefaults = elementsDefaults
                };
            }

            return null;
        }
    }
}