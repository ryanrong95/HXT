using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.PvData.Services.Models;
using YaHv.PvData.Services.Utils;

namespace Yahv.PvData.WebApp.SysConfig.Tariff
{
    public partial class SetDefault : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.HSCode = Request.QueryString["hsCode"];
            }
        }

        /// <summary>
        /// 保存默认值
        /// </summary>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string hsCode = Request["hsCode"].Trim();
            var tariff = Yahv.Erp.Current.PvData.Tariffs[hsCode];
            var elements = tariff.DeclareElements.Split(';');
            var elementsDefaults = tariff.ElementsDefaults;
            ElementsDefault elementsDefault;

            //设置税则申报要素默认值
            foreach (var element in elements)
            {
                string elementName = element.Split(':')[1];
                string defaultValue = Request[elementName];
                elementsDefault = elementsDefaults.Where(item => item.ElementName == elementName).FirstOrDefault() ?? new ElementsDefault();
                if (!string.IsNullOrWhiteSpace(defaultValue))
                {
                    //新增或修改默认值
                    elementsDefault.TariffID = tariff.ID;
                    elementsDefault.ElementName = elementName;
                    elementsDefault.DefaultValue = Request[elementName];
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
            string otherDefault = Request["其他"];
            elementsDefault = elementsDefaults.Where(item => item.ElementName == "其他").FirstOrDefault() ?? new ElementsDefault();
            if (!string.IsNullOrWhiteSpace(otherDefault))
            {
                //新增或修改默认值
                elementsDefault.TariffID = tariff.ID;
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

            Easyui.Dialog.Close("默认值设置成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        /// <summary>
        /// 获取申报要素
        /// </summary>
        /// <returns></returns>
        protected object getElements()
        {
            string hsCode = Request["hsCode"].Trim();
            var tariff = Yahv.Erp.Current.PvData.Tariffs[hsCode];
            if (tariff != null)
            {
                //查询申报要素默认值
                var elementsDefaults = tariff.ElementsDefaults.Select(item => new { item.ElementName, item.DefaultValue });

                return new
                {
                    //报关申报要素
                    DeclareElements = tariff.DeclareElements,
                    //申报要素默认值 
                    ElementsDefaults = elementsDefaults
                };
            }

            return null;
        }
    }
}