using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client
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
            //客户所有人
            this.Model.Owner = Erp.Current.RealName;
            //所属行业
            this.Model.Industry = new EnumsDictionariesRoll().Where(x => x.Enum == "FixedIndustry").Select(x => new { value = x.Description, text = x.Description });
            //客户类型
            this.Model.ClientType = ExtendsEnum.ToDictionary<Yahv.Underly.CrmPlus.ClientType>().Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value
            });
            //企业性质
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
                var conductType = Request.Form["ConductType"].Trim();
                var companyID = Request.Form["Company"].Trim();
                var source = Request.Form["Source"].Trim();
                var area = Request.Form["area"].Trim();
                var clientType = Request.Form["clientType"].Trim();
                var nature = Request.Form["nature"].Trim();
                var industry = Request.Form["industry"].Trim();
                var website = Request.Form["website"].Trim();
                var product = Request.Form["Product"].Trim();
                //工商信息
                var uscc = Request.Form["Uscc"].Trim();
                var corperation = Request.Form["Corperation"];
                var registDate = Request.Form["RegistDate"];
                var businessState = Request.Form["BusinessState"];
                Currency registCurrency = string.IsNullOrEmpty(Request.Form["RegistCurrency"]) ? Currency.Unknown : (Currency)int.Parse(Request.Form["RegistCurrency"]);
                var currency = Request.Form["Currency"];
                var registFund = Request.Form["RegistFund"];
                var regAddress = Request.Form["RegAddress"];
                var employees = Request.Form["Employees"];
                var place = Request.Form["Place"];
                var adderss = Request.Form["Address"];
                bool isSecret = (Yahv.Underly.CrmPlus.ClientType)int.Parse(clientType) == Yahv.Underly.CrmPlus.ClientType.University ? true : false;
                #endregion


                #region 代理线业务只能注册1次，贸易可以注册三次
                var enterprise = new EnterprisesRoll().FirstOrDefault(x => x.Name.Trim() == name && x.IsDraft == false);
                if (enterprise != null)
                {
                    //合作公司  不允许重复
                    var relationsCount = new RelationsRoll().Count(item => item.ClientID == enterprise.ID && item.CompanyID == companyID);
                    if (relationsCount > 0)
                    {
                        Easyui.Alert("提示", "合作公司已注册", Web.Controls.Easyui.Sign.Error);
                        return;
                    }
                    var relationCount = new RelationsRoll(enterprise.ID).Where(x => x.Type == (ConductType)int.Parse(conductType) && x.Status == AuditStatus.Normal).Select(x => x.CompanyID).Distinct().ToArray();
                    if ((ConductType)int.Parse(conductType) == ConductType.Trade)
                    {
                        if (relationCount.Length == 3)
                        {
                            Easyui.Alert("提示", "该客户贸易类型已注册3个!", Web.Controls.Easyui.Sign.Error);
                            return;
                        }

                    }
                    else if (relationCount.Length == 1)
                    {
                        Easyui.Alert("提示", "该客户代理线业务已注册!", Web.Controls.Easyui.Sign.Error);
                        return;

                    }



                }
                #endregion
                #region  客户信息
                var entity = new Yahv.CrmPlus.Service.Models.Origins.Client();


                entity.ClientType = (Yahv.Underly.CrmPlus.ClientType)int.Parse(clientType);
                entity.Vip = VIPLevel.NonVIP;
                entity.Source = source;
                //这s三个字段值不确定
                entity.IsMajor = false;
                entity.IsSpecial = false;
                entity.IsSupplier = false;
                // entity.Owner = Erp.Current.ID; 保护人
                entity.Industry = product;
                #endregion

                #region   企业
                entity.IsDraft = true;
                entity.Name = name;
                entity.Place = place;
                entity.District = area;
                entity.Status = AuditStatus.Waiting;
                #endregion

                #region   工商信息
                if (!isInternation)
                {
                    //是高校或涉密 
                    if (isSecret)
                    {
                        entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister()
                        {
                            IsInternational = false,
                            IsSecret = true,
                            Uscc = uscc,
                            Industry = industry,
                            Nature = nature,
                            WebSite = website,
                        };
                    }
                    else
                    {
                        entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister();
                        entity.EnterpriseRegister.IsSecret = isSecret;
                        entity.EnterpriseRegister.IsInternational = isInternation;
                        entity.EnterpriseRegister.Corperation = corperation;
                        entity.EnterpriseRegister.RegAddress = regAddress;
                        entity.EnterpriseRegister.Uscc = uscc;
                        entity.EnterpriseRegister.Currency = null;
                        entity.EnterpriseRegister.RegistFund = registFund;
                        entity.EnterpriseRegister.RegistCurrency = registCurrency;
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
                    entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister()
                    {
                        IsSecret = false,
                        IsInternational = true,
                        Currency = (Currency)int.Parse(currency),
                        RegAddress = adderss,
                        Industry = industry,
                        Nature = nature,
                        WebSite = website

                    };
                }
                #endregion

                #region  业务类型
                //添加业务类型
                entity.Conduct = new Service.Models.Origins.Conduct()
                {
                    ConductType = (ConductType)int.Parse(conductType),
                    // IsPublic = Erp.Current.ID == "SA01" ? true : false,
                    IsPublic = false
                };
                #endregion

                #region   我方合作公司

                //我方合作公司
                entity.Relation = new Service.Models.Origins.Relation
                {
                    Type = (ConductType)int.Parse(conductType),
                    OwnerID = Erp.Current.ID,
                    CompanyID = companyID,
                    ClientID = entity.ID,
                };
                #endregion

                #region 附件
                entity.Creator = Erp.Current.ID;
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
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            // var client = (Yahv.CrmPlus.Service.Models.Origins.Client)e.Object;
            var client = sender as Yahv.CrmPlus.Service.Models.Origins.Client;
            Service.LogsOperating.LogOperating(Erp.Current, client.ID, $"新增草稿客户:{client.Name}");
            (new ApplyTask
            {
                MainID = client.ID,
                MainType = MainType.Clients,
                ApplierID = Erp.Current.ID,
                ApplyTaskType = ApplyTaskType.ClientRegist,

            }).Enter();
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }



        protected object GetEnterpriseName()
        {
            var name = Request.Form["Name"];
            var arr = new EnterprisesRoll().Where(x => x.Name == name && x.IsDraft == false).ToArray();
            var client = Erp.Current.CrmPlus.Clients.FirstOrDefault(x => x.Name == name);

            if (arr.Where(x => x.Status == AuditStatus.Black).Count() > 0)
            {

                return new { succes = false, message = "黑名单客户不允许注册" };

            }
            else if (client != null)
            {
                return new { succes = false, Entity = client.Json(), isDraft = false };
            }

            return new { succes = true };
        }

    }
}