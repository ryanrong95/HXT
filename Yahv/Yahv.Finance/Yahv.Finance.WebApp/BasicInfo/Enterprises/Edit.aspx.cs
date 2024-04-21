using Layers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.Enterprises
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var EnterpriseID = Request.QueryString["id"];

                if (!string.IsNullOrEmpty(EnterpriseID))
                {
                    var enterprise = Yahv.Erp.Current.Finance.Enterprises[EnterpriseID];
                    this.Model.Data = enterprise;

                    //类型
                    if (enterprise?.Type != null)
                    {
                        this.Model.theTypes = Enum.GetValues(typeof(EnterpriseAccountType)).Cast<EnterpriseAccountType>()
                            .Where(item => enterprise.Type.HasFlag(item)).Select(item => new { ID = item }).ToArray();
                    }
                }

                this.Model.Districts = ExtendsEnum.ToDictionary<Origin>().Select(item => new { value = item.Value, text = item.Value });
                this.Model.Types = ExtendsEnum.ToDictionary<EnterpriseAccountType>().Select(item => new { value = item.Key, text = item.Value });
            }
        }

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };

            var EnterpriseID = Request.Form["ID"];
            Enterprise enterprise = null;
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                try
                {
                    var Name = Request.Form["Name"];
                    var District = Request.Form["District"];
                    var Type = GetEpAccountType(Request.Form["EnterpriseAccountType"]);
                    var Summary = Request.Form["Summary"];

                    if (string.IsNullOrEmpty(District))
                    {
                        json.success = false;
                        json.data = "请正确选择地区!";
                        return json;
                    }

                    var enterprises = Erp.Current.Finance.Enterprises;

                    if (string.IsNullOrWhiteSpace(EnterpriseID))
                    {
                        enterprise = new Enterprise()
                        {
                            ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.Enterprise),
                            Name = Name,
                            Type = Type,
                            District = District,
                            CreatorID = Erp.Current.ID,
                            ModifierID = Erp.Current.ID,
                            Summary = Summary,
                        };
                    }
                    else
                    {
                        enterprise = enterprises.Where(t => t.ID == EnterpriseID).FirstOrDefault();
                        enterprise.Name = Name;
                        enterprise.Type = Type;
                        enterprise.District = District;
                        enterprise.ModifierID = Erp.Current.ID;
                        enterprise.ModifyDate = DateTime.Now;
                        enterprise.Summary = Summary;
                    }

                    enterprise.Enter();
                    tran.Commit();

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.往来单位管理, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(EnterpriseID) ? "新增" : "修改", enterprise.Json());
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    json.success = false;
                    json.data = "添加失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.往来单位管理, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(EnterpriseID) ? "新增" : "修改") + " 异常!", new { enterprise, exception = ex.ToString() }.Json());
                    return json;
                }
            }

            return json;
        }

        #endregion

        #region 私有函数
        /// <summary>
        /// 根据字符串返回 组合枚举
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        EnterpriseAccountType GetEpAccountType(string values)
        {
            EnterpriseAccountType epType = default(EnterpriseAccountType);
            foreach (var type in values.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                epType |= (EnterpriseAccountType)int.Parse(type);
            }

            return epType;
        }
        #endregion
    }
}