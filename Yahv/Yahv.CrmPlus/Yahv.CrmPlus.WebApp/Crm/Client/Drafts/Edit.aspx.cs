using System;
using System.Collections.Generic;
using System.IO;
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

namespace Yahv.CrmPlus.WebApp.Crm.Client.Drafts
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            var ID = Request.QueryString["enterpriseid"];
            var companyid = Request.QueryString["companyId"];
            var conductType = Request.QueryString["ConductType"];
            if (!string.IsNullOrEmpty(ID))
            {
                var entity = Erp.Current.CrmPlus.Clients[ID];
                this.Model.Entity = Erp.Current.CrmPlus.Clients[ID];
                this.Model.CorCompany = companyid;
                this.Model.Conduct = conductType;
                var relation = new RelationsRoll().Where(x => x.ClientID == ID &&x.CompanyID==companyid).ToArray();
                var ownerArr = relation.Select(x => x.Admin.RealName).Distinct().ToArray();
                string owner = string.Join(",", ownerArr);
                this.Model.Owner = owner;
             // var file = new FilesDescriptionRoll().Where(x => x.EnterpriseID == ID).ToArray();
                //this.Model.LogoFile = file.FirstOrDefault(x => x.Type == CrmFileType.Logo);
               // this.Model.Licenses = file.Where(x => x.Type == CrmFileType.License).Select(x => new { FileName = x.CustomName, Url = x.Url });
            }
            //所属行业
            this.Model.Industry = new EnumsDictionariesRoll().Where(x => x.Enum == "FixedIndustry").Select(x => new { value = x.Description, text = x.Description });

            this.Model.EnterpriseNature = ExtendsEnum.ToDictionary<EnterpriseNature>().Select(item => new
            {
                value = item.Value,
                text = item.Value
            });

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
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
                var isPublic = Request.QueryString["isPublic"] != null;

                bool isSecret = (Yahv.Underly.CrmPlus.ClientType)int.Parse(clientType) == Yahv.Underly.CrmPlus.ClientType.University ? true : false;
                #endregion
                //合作公司  不允许重复
                //var relationsCount = new MyRelationRoll(Erp.Current).Count(item => item.ClientID== Request.QueryString["ID"] && item.CompanyID == companyID);
                //if (relationsCount > 0)
                //{
                //    Easyui.Alert("提示", "合作公司已注册", Web.Controls.Easyui.Sign.Error);
                //    return;
                //}

                #region 企业信息
                var entity = Erp.Current.CrmPlus.Clients[Request.QueryString["enterpriseid"]];
                entity.Name = name;
                entity.District = area;
                entity.Place = place;
                entity.IsDraft = true;
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
                        entity.EnterpriseRegister.Uscc =uscc;
                        entity.EnterpriseRegister.Currency = string.IsNullOrEmpty(currency) ? Currency.CNY : (Currency)int.Parse(currency);
                        entity.EnterpriseRegister.RegistFund = registFund;
                        entity.EnterpriseRegister.RegistCurrency = registCurrency == null ? Currency.Unknown : (Currency)int.Parse(registCurrency);
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

                    //  entity.EnterpriseRegister.IsSecret = false;
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
                var result = false ;
                entity.Conduct = new Service.Models.Origins.Conduct()
                {
                    ConductType = (ConductType)int.Parse(conductType),
                    IsPublic = result,
                };
                //entity.Conduct.ConductType = (ConductType)int.Parse(conductType);
                //entity.Conduct.IsPublic = false;
                #endregion
                #region 我方合作公司
                //我方合作公司
                entity.Relation = new Service.Models.Origins.Relation
                {
                    Type = (ConductType)int.Parse(conductType),
                    OwnerID = Erp.Current.ID,
                    CompanyID = companyID,
                    //一定要Enterprice Enter 之后再执行
                    ClientID = entity.ID,
                };
                #endregion

                entity.ClientType = (Yahv.Underly.CrmPlus.ClientType)int.Parse(clientType);
                entity.Vip = Underly.VIPLevel.NonVIP;
                entity.Source = source;
                //这s三个字段值不确定
                entity.IsMajor = false;
                entity.IsSpecial = false;
                entity.IsSupplier = false;
              //  entity.ID = Request["ID"];
                entity.Industry = product;
                entity.Creator = Erp.Current.ID;
               
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
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }

        }


        //private void Client_EnterError(object sender, ErrorEventArgs e)
        //{
        //    Easyui.Alert("操作提示", e.ToString(), Web.Controls.Easyui.Sign.Error);
        //}

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var client = sender as Yahv.CrmPlus.Service.Models.Origins.Client;
            if (client.ClientStatus == AuditStatus.Voted)
            {
                client.UpdateStatus();
            }
            
            Service.LogsOperating.LogOperating(Erp.Current, client.ID, $"客户:{client.Name}" + "已修改");
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success); ;
        }
    }
}