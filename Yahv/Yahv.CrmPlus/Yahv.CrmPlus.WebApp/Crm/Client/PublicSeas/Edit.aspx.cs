using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.PublicSeas
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.Claiman = Erp.Current.RealName;
            this.Model.Companys = new CompaniesRoll().Where(x => x.CompanyStatus == DataStatus.Normal).Select(x => new { ID = x.ID, Name = x.Name });
            this.Model.ConductType = ExtendsEnum.ToDictionary<ConductType>().Select(x => new { value = x.Key, text = x.Value });
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            #region  参数
            bool isInternation = Request.Form["IsInternational"] != null;
            var conductType = Request.Form["ConductType"];
            var companyID = Request.Form["Company"];
            var source = Request.Form["Source"];
            var area = Request.Form["area"];
            var clientType = Request.Form["clientType"];
            var nature = Request.Form["nature"];
            var industry = Request.Form["industry"];
            var website = Request.Form["website"];
            var product = Request.Form["Product"];
            //工商信息
            var uscc = Request.Form["Uscc"];
            var corperation = Request.Form["Corperation"];

            var businessState = Request.Form["BusinessState"];
            var registCurrency = Request.Form["RegistCurrency"];
            var currency = Request.Form["Currency"];
            var registFund = Request.Form["RegistFund"];
            var regAddress = Request.Form["RegAddress"];
            var employees = Request.Form["Employees"];
            var place = Request.Form["Place"];
            var adderss = Request.Form["Address"];
            //审批信息
            var grade = Request.Form["Grade"];
            var vip = Request.Form["Vip"];
            var conductGrade = Request.Form["ConductGrade"];
            var isMajor = Request.Form["IsMajor"] != null;
            var isSpecial = Request.Form["IsSpecial"] != null;
            var profitRate = Request.Form["ProfitRate"];


            bool isSecret = (Yahv.Underly.CrmPlus.ClientType)int.Parse(clientType) == Yahv.Underly.CrmPlus.ClientType.University ? true : false;
            var entity = Erp.Current.CrmPlus.Clients[Request.QueryString["id"]];
            #endregion
            #region 企业
            entity.District = area;
            entity.Place = place;
            entity.IsDraft = false;
            #endregion

            #region  工商信息
            if (!isInternation)
            {
                //是高校或涉密 
                if (isSecret)
                {
                    entity.EnterpriseRegister.ID = entity.ID;
                    entity.EnterpriseRegister.IsInternational = isInternation;
                    entity.EnterpriseRegister.IsSecret = isSecret;
                    entity.EnterpriseRegister.Uscc = uscc;
                    entity.EnterpriseRegister.Industry = industry;
                    entity.EnterpriseRegister.Nature = nature;
                    entity.EnterpriseRegister.WebSite = website;
                }
                else
                {
                    entity.EnterpriseRegister.ID = entity.ID;
                    // entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister();
                    entity.EnterpriseRegister.IsSecret = isSecret;
                    entity.EnterpriseRegister.IsInternational = false;
                    entity.EnterpriseRegister.Corperation = corperation;
                    entity.EnterpriseRegister.RegAddress = regAddress;
                    entity.EnterpriseRegister.Uscc = uscc;
                    entity.EnterpriseRegister.Currency = null;
                    entity.EnterpriseRegister.RegistFund = registFund;
                    entity.EnterpriseRegister.RegistCurrency = (Currency)int.Parse(registCurrency);
                    entity.EnterpriseRegister.Industry = industry;
                    entity.EnterpriseRegister.Employees = !string.IsNullOrEmpty(employees) ? int.Parse(employees) : 0;
                    entity.EnterpriseRegister.WebSite = website;
                    entity.EnterpriseRegister.Nature = nature;
                    entity.EnterpriseRegister.BusinessState = businessState;
                    if (!string.IsNullOrEmpty(Request.Form["RegistDate"]))
                    {
                        entity.EnterpriseRegister.RegistDate = Convert.ToDateTime(Request.Form["RegistDate"]);
                    }

                }

            }
            else
            {
                entity.EnterpriseRegister.ID = entity.ID;
                entity.EnterpriseRegister.IsInternational = true;
                entity.EnterpriseRegister.Currency = (Currency)int.Parse(currency);
                entity.EnterpriseRegister.RegAddress = adderss;
                entity.EnterpriseRegister.Nature = nature;
                entity.EnterpriseRegister.Industry = industry;
                entity.EnterpriseRegister.WebSite = website;
                entity.EnterpriseRegister.IsSecret = isSecret;
            }
            #endregion

            #region 业务类型
            //entity.Conduct = new Service.Models.Origins.Conduct()
            //{
            //    ConductType = (ConductType)int.Parse(conductType),
            //    IsPublic = false
            //};
            #endregion

            #region 我方合作公司
            //entity.Relation = new Service.Models.Origins.Relation
            //{
            //    Type = (ConductType)int.Parse(conductType),
            //    OwnerID = Erp.Current.ID,
            //    CompanyID = companyID,
            //    ClientID = entity.Enterprise.ID,
            //};
            #endregion

            #region  客户
            entity.ClientType = (Yahv.Underly.CrmPlus.ClientType)int.Parse(clientType);
            entity.Vip = VIPLevel.NonVIP;
            entity.Source = source;
            entity.IsMajor = isMajor;
            entity.IsSpecial = isSpecial;
            entity.IsSupplier = false;
            entity.Industry = product;//主要产品
            entity.Status = AuditStatus.Normal;
            #endregion

            #region  审批信息

            entity.IsMajor = isMajor;
            entity.IsSpecial = isSpecial;
            entity.ClientGrade = string.IsNullOrEmpty(grade) ? ClientGrade.None : (ClientGrade)int.Parse(grade);
            entity.Vip = string.IsNullOrEmpty(vip) ? (VIPLevel.NonVIP) : (VIPLevel)int.Parse(Request.Form["Vip"]);
            // entity.Conduct.Grade = !string.IsNullOrEmpty(conductGrade) ? (ConductGrade)int.Parse(conductGrade) : ConductGrade.None;
            if (!string.IsNullOrEmpty(profitRate))
            {
                entity.ProfitRate = Convert.ToDecimal(profitRate);
            }
            entity.Summary = Request.Form["Summary"];
            #endregion
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();

        }
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var client = sender as Yahv.CrmPlus.Service.Models.Origins.Client;
            Service.LogsOperating.LogOperating(Erp.Current, client.ID, $"{Erp.Current}编辑了客户:{client.Name}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

    }
}