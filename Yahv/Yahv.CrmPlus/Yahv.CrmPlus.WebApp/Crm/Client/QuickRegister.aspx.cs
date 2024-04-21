using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client
{
    public partial class QuickRegister : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                #region  参数
                //基本信息
                var name = Request.Form["Name"].Trim();
                var conductType = Request.Form["ConductType"].Trim();
                bool isInternation = Request.Form["IsInternational"] != null;
                var area = Request.Form["Area"].Trim();
                //证照
                var license = Request.Form["licenseForJson"];
                //工商信息
                var uscc = Request.Form["Uscc"].Trim();
                var place = Request.Form["Place"];
                var currency = Request.Form["Currency"];
                var adderss = Request.Form["Address"];
                #endregion

                var entity = new Yahv.CrmPlus.Service.Models.Origins.Client();

                #region 客户信息
                entity.ClientType = Yahv.Underly.CrmPlus.ClientType.Trader;
                entity.Vip = VIPLevel.NonVIP;
                entity.IsMajor = false;
                entity.IsSpecial = false;
                entity.IsSupplier = false;
                #endregion

                #region 企业信息
                entity.IsDraft = true;
                entity.Name = name;
                entity.Place = place;
                entity.District = area;
                entity.Status = AuditStatus.Waiting;
                #endregion

                #region 工商信息
                if (!isInternation)
                {
                    entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister()
                    {
                        IsSecret = false,
                        IsInternational = false,
                        Uscc = uscc,
                    };
                }
                else
                {
                    entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister()
                    {
                        IsSecret = false,
                        IsInternational = true,
                        Currency = (Currency)int.Parse(currency),
                        RegAddress = adderss,
                    };
                }
                #endregion

                #region 业务类型
                entity.Conduct = new Service.Models.Origins.Conduct()
                {
                    ConductType = (ConductType)int.Parse(conductType),
                    IsPublic = false
                };
                #endregion

                #region 附件
                entity.Creator = Erp.Current.ID;
                if (!string.IsNullOrEmpty(license))
                {
                    entity.Lisences = license.JsonTo<List<CallFile>>();
                }
                #endregion

                entity.EnterSuccess += Client_EnterSuccess;
                entity.Enter();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        private void Client_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var client = sender as Yahv.CrmPlus.Service.Models.Origins.Client;
            //记录操作日志
            Service.LogsOperating.LogOperating(Erp.Current, client.ID, $"快速注册客户:{client.Name}");
            //新增客户注册审批
            var applyTask = new ApplyTask
            {
                MainID = client.ID,
                MainType = MainType.Clients,
                ApplierID = Erp.Current.ID,
                ApplyTaskType = ApplyTaskType.ClientRegist,
            };
            applyTask.Enter();

            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}