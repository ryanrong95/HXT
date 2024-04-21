using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.TaxRates
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                var entity = new TaxRate();

                if (!string.IsNullOrEmpty(id))
                {
                    entity = Yahv.Erp.Current.Finance.TaxRates[id];
                }

                this.Model.Data = entity;
            }
        }

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            var id = Request.Form["ID"];
            TaxRate entity = null;

            try
            {
                var name = Request.Form["Name"];
                var rate = Request.Form["Rate"];
                var code = Request.Form["Code"];
                var jsonName = Request.Form["JsonName"];

                var taxRates = Erp.Current.Finance.TaxRates;
                if (string.IsNullOrWhiteSpace(id))
                {
                    if (taxRates.Any(item => item.Name == name))
                    {
                        json.success = false;
                        json.data = $"[{name}]名称已存在!";
                        return json;
                    }

                    if (taxRates.Any(item => item.Code == int.Parse(code)))
                    {
                        json.success = false;
                        json.data = $"[{code}]枚举值不能重复!";
                        return json;
                    }

                    if (taxRates.Any(item => item.JsonName == jsonName))
                    {
                        json.success = false;
                        json.data = $"[{jsonName}]Json名称不能重复!";
                        return json;
                    }

                    entity = new TaxRate()
                    {
                        Code = int.Parse(code),
                        CreatorID = Erp.Current.ID,
                        CreateDate = DateTime.Now,
                        JsonName = jsonName,
                        ModifierID = Erp.Current.ID,
                        ModifyDate = DateTime.Now,
                        Name = name,
                        Rate = decimal.Parse(rate),
                    };
                }
                else
                {
                    entity = taxRates[id];

                    entity.Rate = decimal.Parse(rate);
                    entity.JsonName = jsonName;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifierID = Erp.Current.ID;
                }

                entity.Enter();
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.税率管理, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", entity.Json());
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = "添加失败!" + ex.Message;
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.税率管理, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + "失败!", new { entity, ex = ex.ToString() }.Json());
                return json;
            }

            return json;
        }

        #endregion
    }
}