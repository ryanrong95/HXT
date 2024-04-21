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
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Register
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();

            }
        }

        protected void LoadData()
        {
            var ID = Request.QueryString["enterpriseid"];

            var client = new WaitingClientsRoll()[ID];
            this.Model.Entity = client;

            //  var file = new FilesDescriptionRoll().Where(x => x.EnterpriseID == ID).ToArray();

            //   this.Model.LogoFile = file.FirstOrDefault(x => x.Type == CrmFileType.Logo);
            //  this.Model.Licenses = file.Where(x => x.Type == CrmFileType.License).Select(x => new { FileName = x.CustomName, Url = x.Url });
            this.Model.Conduct = new ConductsRoll(ID);
            this.Model.Owner = client.Relation.Admin?.RealName;
            this.Model.corCompany = client.Relation.CompanyID;
            this.Model.Industry = Service.EnumsDictionary<FixedIndustry>.Current.Select(x => new { value = x.Description, text = x.Description });
            this.Model.EnterpriseNature = ExtendsEnum.ToDictionary<EnterpriseNature>().Select(item => new
            {
                value = item.Value,
                text = item.Value
            });
        }


        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //try
            //{
            #region  参数
            var name = Request.Form["name"].Trim();
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
            var ID = Request.QueryString["enterpriseid"];


            bool isSecret = (Yahv.Underly.CrmPlus.ClientType)int.Parse(clientType) == Yahv.Underly.CrmPlus.ClientType.University ? true : false;
            var entity = new WaitingClientsRoll()[ID];
            #endregion
            //同一个客户的合作公司, 不允许重复注册
            var enterprise = new EnterprisesRoll().FirstOrDefault(item => item.Name == entity.Name && item.IsDraft == false);
            if (enterprise != null)
            {
                var relationsCount = new RelationsRoll().Count(item => item.ClientID == enterprise.ID && item.CompanyID == companyID && item.Status == AuditStatus.Normal);
                if (relationsCount > 0)
                {
                    //Easyui.Alert("提示","合作公司已注册", Sign.Error);
                    Easyui.Window.Close("审批失败，合作公司已注册!", Web.Controls.Easyui.AutoSign.Error); ;
                    return;
                }
            }
            #region 企业
            entity.Name = name;
            entity.District = area;
            entity.Place = place;
            entity.IsDraft = true;
            entity.Status = AuditStatus.Normal;
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

            entity.Conduct.ConductType = (ConductType)int.Parse(conductType);
            entity.Conduct.IsPublic = false;
            #endregion

            #region 我方合作公司
            entity.Relation.Type = (ConductType)int.Parse(conductType);
            entity.Relation.CompanyID = companyID;
            entity.Relation.ClientID = ID;
            #endregion

            #region  客户
            entity.ClientType = (Yahv.Underly.CrmPlus.ClientType)int.Parse(clientType);
            entity.Vip = Underly.VIPLevel.NonVIP;
            entity.Source = source;
            entity.Industry = product;
            entity.Status = AuditStatus.Normal;
            #endregion

            #region  审批信息

            entity.IsMajor = isMajor;
            entity.IsSpecial = isSpecial;
            entity.ClientGrade = string.IsNullOrEmpty(grade) ? ClientGrade.None : (ClientGrade)int.Parse(grade);
            entity.Vip = string.IsNullOrEmpty(vip) ? (VIPLevel.NonVIP) : (VIPLevel)int.Parse(Request.Form["Vip"]);
            entity.Conduct.Grade = !string.IsNullOrEmpty(conductGrade) ? (ConductGrade)int.Parse(conductGrade) : ConductGrade.None;
            if (!string.IsNullOrEmpty(profitRate))
            {
                entity.ProfitRate = Convert.ToDecimal(profitRate);
            }
            entity.Summary = Request.Form["Summary"];
            #endregion
         
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Approve();
            //}
            //catch (Exception ex)
            //{
            //Easyui.Window.Close("审批成功!", Web.Controls.Easyui.AutoSign.Success); ;
            //  Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            //}

        }
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var client = sender as Yahv.CrmPlus.Service.Models.Origins.WatingClient;
            Service.LogsOperating.LogOperating(Erp.Current, client.ID, $"客户:{client.Name}" + "注册审批通过");
            (new ApplyTask
            {
                MainID = client.ID,
                MainType = Underly.MainType.Clients,
                ApproverID = Erp.Current.ID,
                ApplyTaskType = Underly.ApplyTaskType.ClientRegist,
                Status = Underly.ApplyStatus.Allowed

            }).Approve();
            Easyui.Window.Close("审批成功!", Web.Controls.Easyui.AutoSign.Success); ;
        }


        /// <summary>
        /// 否决
        /// </summary>
        /// <returns></returns>
        protected object Reject()
        {
            var id = Request.Form["id"];
            var summary = Request.Form["summary"];
            try
            {
                var entity = new WaitingClientsRoll()[id]; ;
                entity.Summary = summary;
                entity.Reject();
                return new { success = true, data = "", message = "" };
            }
            catch (Exception ex)
            {
                return new { success = false, message = "操作失败：" + ex };
            }



        }
    }
}