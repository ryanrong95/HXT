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

namespace Yahv.Finance.WebApp.BasicInfo.SpecialGoldStores
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];

                var entity = new GoldStore();
                if (!string.IsNullOrWhiteSpace(id))
                {
                    entity = Yahv.Erp.Current.Finance.GoldStores.Where(t => t.ID == id).FirstOrDefault();

                }

                this.Model.Data = entity;
                this.Model.Admins = Yahv.Erp.Current.Finance.Admins
                    .Where(t => t.Status != Underly.AdminStatus.Closed)
                    .OrderBy(t => t.RealName)
                    .Select(item => new { value = item.ID, text = item.RealName })
                    .ToArray();
            }
        }

        #region 提交保存
        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            var id = Request.QueryString["id"];
            GoldStore store = null;
            try
            {
                string Name = Request.Form["Name"];
                string Summary = Request.Form["Summary"];
                string Owner = Request.Form["OwnerID"];

                if (string.IsNullOrEmpty(Name.Trim()))
                {
                    json.success = false;
                    json.data = "金库名称不能为空!";
                    return json;
                }

                if (string.IsNullOrEmpty(Owner.Trim()))
                {
                    json.success = false;
                    json.data = "请正确选择金库主管!";
                    return json;
                }
                else
                {
                    var selectedAdmin = Yahv.Erp.Current.Finance.Admins.Where(t => t.ID == Owner).FirstOrDefault();
                    if (selectedAdmin == null)
                    {
                        json.success = false;
                        json.data = "请正确选择金库主管!";
                        return json;
                    }
                }

                var goldStores = Erp.Current.Finance.GoldStores;

                if (string.IsNullOrWhiteSpace(id))
                {
                    store = new GoldStore()
                    {
                        Name = Name,
                        Summary = Summary,
                        IsSpecial = true,
                        OwnerID = Owner,
                        CreatorID = Erp.Current.ID,
                        ModifierID = Erp.Current.ID,
                    };

                    if (store != null && goldStores.Any(item => item.Name == store.Name))
                    {
                        json.success = false;
                        json.data = "该名称已经存在，不能重复添加!";
                        return json;
                    }
                }
                else
                {
                    store = goldStores.Where(t => t.ID == id).FirstOrDefault();
                    store.Name = Name;
                    store.Summary = Summary;
                    store.OwnerID = Owner;
                    store.ModifierID = Erp.Current.ID;
                    store.ModifyDate = DateTime.Now;

                    if (store != null && goldStores.Any(item => item.ID != id && item.Name == store.Name))
                    {
                        json.success = false;
                        json.data = "该名称已经存在，不能重复添加!";
                        return json;
                    }
                }

                store.Enter();
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.特殊金库, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", store.Json());
                return json;
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.特殊金库, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + " 异常!", new { store, exception = ex.ToString() }.Json());
                json.success = false;
                json.data = $"提交异常!{ex.Message}";
                return json;
            }
        }

        #endregion

    }
}