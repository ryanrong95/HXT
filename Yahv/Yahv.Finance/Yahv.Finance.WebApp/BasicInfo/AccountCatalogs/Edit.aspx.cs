using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Finance.WebApp.BasicInfo.AccountCatalogs
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        private void InitData()
        {
            using (var view = new AccountCatalogsRoll())
            {
                string id = Request.QueryString["id"];
                var entity = view.FirstOrDefault(item => item.ID == id);

                if (entity != null)
                {
                    this.Model.Data = new
                    {
                        ID = id,
                        FatherID = entity.FatherID,
                        Name = entity.Name,
                        FatherName = view[entity.FatherID]?.Name,
                    };
                }
                else
                {
                    string fatherId = Request.QueryString["fatherid"];

                    this.Model.Data = new
                    {
                        FatherID = fatherId,
                        FatherName = view[fatherId]?.Name,
                    };
                }
            }
        }
        #endregion

        #region 提交保存
        protected object Submit()
        {
            JMessage json = new JMessage() { success = true, data = "提交成功!" };
            var id = Request.QueryString["id"];
            AccountCatalog catalog = null;
            try
            {
                var fatherId = Request.Form["FatherId"];
                var text = Request.Form["Name"];
                var accountCatalogs = Erp.Current.Finance.AccountCatalogs.ToArray();
                //新增
                if (string.IsNullOrWhiteSpace(id))
                {
                    //判断是否为批量(根据分号来定)
                    var names = text.Split(new string[] { ";", "；" }, StringSplitOptions.RemoveEmptyEntries);
                    var list = new List<AccountCatalog>();

                    foreach (string name in names)
                    {
                        catalog = new AccountCatalog()
                        {
                            Name = name,
                            CreatorID = Erp.Current.ID,
                            ModifierID = Erp.Current.ID,
                            FatherID = fatherId,
                        };

                        var model = accountCatalogs.FirstOrDefault(item => item.Name == name && item.FatherID == fatherId);
                        if (model != null)
                        {
                            if (model.Status == GeneralStatus.Normal)
                            {
                                json.success = false;
                                json.data = $"{name}已经存在，不能重复添加!";
                                return json;
                            }
                            else
                            {
                                catalog.ID = model.ID;
                                catalog.Status = GeneralStatus.Normal;

                                //直接进行更新
                                catalog.Enter();
                                continue;
                            }
                        }
                        list.Add(catalog);
                    }

                    //添加
                    Erp.Current.Finance.AccountCatalogs.Add(list);
                }
                //修改
                else
                {
                    catalog = accountCatalogs.FirstOrDefault(item => item.ID == id);
                    catalog.Name = text;
                    catalog.ModifierID = Erp.Current.ID;
                    catalog.ModifyDate = DateTime.Now;
                    catalog.FatherID = fatherId;

                    if (accountCatalogs != null &&
                        accountCatalogs.Any(item => item.Name == catalog.Name && item.FatherID == fatherId && item.ID != id))
                    {
                        json.success = false;
                        json.data = "该名称已经存在，不能重复添加!";
                        return json;
                    }
                    catalog.Enter();
                }

                Yahv.Finance.Services.Oplogs.Oplog(Erp.Current.ID, LogModular.账款类型管理, Yahv.Finance.Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(id) ? "新增" : "修改", catalog.Json());
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"提交异常!{ex.Message}";
                Yahv.Finance.Services.Oplogs.Oplog(Erp.Current.ID, LogModular.账款类型管理, Yahv.Finance.Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(id) ? "新增" : "修改") + "异常!", new { catalog, exception = ex.ToString() }.Json());
            }

            return json;
        }
        #endregion
    }
}