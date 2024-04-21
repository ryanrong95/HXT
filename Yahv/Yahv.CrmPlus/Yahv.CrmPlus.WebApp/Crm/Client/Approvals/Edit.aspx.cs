using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }


        protected void LoadData()
        {
            var ID = Request.QueryString["ID"];
            var entity = Erp.Current.CrmPlus.Clients[ID];
            this.Model.Entity = entity;
            this.Model.Clients = Erp.Current.CrmPlus.Clients.Where(x => x.IsDraft == false).Select(x => new { SubID = x.ID, text = x.Name });
            this.Model.Sources = new EnumsDictionariesRoll().Where(x => x.Enum == "FixedSource").Select(x => new { value = x.ID, text = x.Description });
            this.Model.Areas = new EnumsDictionariesRoll().Where(x => x.Enum == "FixedArea").Select(x => new { value = x.ID, text = x.Description });
            this.Model.Industry = new EnumsDictionariesRoll().Where(x => x.Enum == "FixedIndustry").Select(x => new { value = x.Description, text = x.Description });
            // var admin = new AdminsAllRoll()[Erp.Current.ID];

            var file = new FilesDescriptionRoll().Where(x => x.EnterpriseID == ID).ToArray();

            this.Model.LogoFile = file.FirstOrDefault(x => x.Type == CrmFileType.Logo);
            this.Model.Licenses = file.Where(x => x.Type == CrmFileType.License).Select(x => new { FileName = x.CustomName, Url = x.Url });

            this.Model.ClientType = ExtendsEnum.ToDictionary<Yahv.Underly.CrmPlus.ClientType>().Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value
            });

            this.Model.EnterpriseNature = ExtendsEnum.ToDictionary<EnterpriseNature>().Select(item => new
            {
                value = item.Value,
                text = item.Value
            });

            //this.Model.Currency = ExtendsEnum.ToDictionary<Currency>().Select(item => new
            //{
            //    value = int.Parse(item.Key),
            //    text = item.Value
            //});

            //this.Model.BusinessRelationType = ExtendsEnum.ToDictionary<BusinessRelationType>().Select(item => new
            //{
            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});
            //this.Model.DataStatus = ExtendsEnum.ToDictionary<AuditStatus>().Select(item => new
            //{
            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});
            //this.Model.Places = ExtendsEnum.ToDictionary<Origin>().Select(item => new
            //{
            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});
            //this.Model.ClientGrade = ExtendsEnum.ToDictionary<ClientGrade>().Select(item => new
            //{
            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});
            //this.Model.Vip = ExtendsEnum.ToDictionary<VIPLevel>().Select(item => new
            //{
            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});
            //this.Model.ConductGrade = ExtendsEnum.ToDictionary<ConductGrade>().Select(item => new
            //{

            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});

        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister();
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
            entity.Vip = Underly.VIPLevel.NonVIP;
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
            entity.Creator = Erp.Current.ID;
            #endregion

            #region 附件

            var license = Request.Form["licenseForJson"];
            var logo = Request.Form["logoForJson"];
            if (!string.IsNullOrEmpty(license))
            {
                entity.Lisences = license.JsonTo<List<CallFile>>();
            }
            if (!string.IsNullOrEmpty(logo))
            {
                entity.Logo = logo.JsonTo<List<CallFile>>().FirstOrDefault();
            }
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