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

namespace Yahv.CrmPlus.WebApp.Chains
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ClientType = ExtendsEnum.ToArray<ClientType>(ClientType.OEM, ClientType.Unknown).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //this.Model.Area = new EnumsDictionariesRoll().Where(item => item.Enum == "FixedArea").Select(item => new
                //{
                //    value = item.ID,
                //    text = item.Description
                //});
                this.Model.Place = ExtendsEnum.ToArray(Origin.NG, Origin.Unknown).Select(item => new
                {
                    value = item.ToString(),
                    text = item.GetDescription()
                });
                this.Model.ServiceType = ExtendsEnum.ToArray(ServiceType.Unknown).Select(item => new
                {
                    value=(int)item,
                    text=item.GetDescription()
                });
                this.Model.Currency = ExtendsEnum.ToArray<Currency>(Currency.Unknown).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try {
               
                
                string name = Request["ClientName"].Trim();
                ChainsClient entity =  new ChainsClient();

                #region 获取页面数据
                string Area = Request.Form["Area"];
                 string worktime = Request.Form["WorkTime"];
                string website = Request.Form["WebSite"];
                
                string uscc = Request.Form["Uscc"];
                string Corperation = Request.Form["Corperation"];
                string stype = Request.Form["ServiceType"];
                string nature = Request.Form["Nature"];
                //服务类型
                var servicetype = ServiceType.Unknown;
                if (!string.IsNullOrWhiteSpace(stype))
                {
                    Enum.TryParse(stype, out servicetype);
                }
                
                Currency RegistCurrency = Request.Form["RegistCurrency"] == null ? Currency.Unknown : (Currency)int.Parse(Request.Form["RegistCurrency"]);
                string RegistFund = Request.Form["RegistFund"];
                string RegAddress = Request.Form["RegAddress"];
                int Employees = int.Parse(Request.Form["Employees"]); ;
                string BusinessState = Request.Form["BusinessState"];
                #endregion

                #region Enterprise
                entity.Enterprise = entity.Enterprise ?? new Enterprise();
                entity.Enterprise.Name = name;
                entity.Enterprise.IsDraft = false;
                //entity.Enterprise.District = Request["Area"];
                entity.Enterprise.Place = Request["Place"];
                #endregion

                #region EnterpriseRegister
               
                    entity.EnterpriseRegister = entity.EnterpriseRegister ?? new EnterpriseRegister();
                    entity.EnterpriseRegister.IsSecret = false;
                    //entity.EnterpriseRegister.IsInternational = isinternational;
                    entity.EnterpriseRegister.Corperation = Request["Corperation"].Trim();
                    entity.EnterpriseRegister.RegAddress = RegAddress;
                    entity.EnterpriseRegister.Uscc = Request["Uscc"].Trim();

                    entity.EnterpriseRegister.RegistFund = Request["RegistFund"];
                    entity.EnterpriseRegister.RegistCurrency = RegistCurrency;
                    entity.EnterpriseRegister.Industry = null;
                    entity.EnterpriseRegister.Currency = null;
                    entity.EnterpriseRegister.BusinessState = BusinessState;
                    entity.EnterpriseRegister.Employees = Employees;
                    entity.EnterpriseRegister.WebSite = website;
                    //entity.EnterpriseRegister.Nature = EnterpriseNature;
                    string registDate = Request.Form["RegistDate"];
                    if (string.IsNullOrWhiteSpace(registDate))
                    {
                        entity.EnterpriseRegister.RegistDate = null;
                    }
                    else
                    {
                        entity.EnterpriseRegister.RegistDate = Convert.ToDateTime(registDate);
                    }

                
                #endregion

                #region 客户信息
                entity.Grade = null;
                entity.CustomCode = Request.Form["CustomCode"].Trim();
                entity.ServiceType = servicetype;
                entity.Nature = (ClientType)Enum.Parse(typeof(Nature), nature);
                entity.OwnerID = Erp.Current.ID;
                #endregion

                #region 文件

                var licenses = Request.Form["LicensesForJson"];
                entity.Lisences = string.IsNullOrWhiteSpace(licenses) ? null : licenses.JsonTo<List<CallFile>>();
                var hklicenses = Request.Form["hkLicensesForJson"];
                entity.HkLisences = string.IsNullOrWhiteSpace(hklicenses) ? null : hklicenses.JsonTo<CallFile>();
                #endregion

                entity.EnterSuccess += Entity_EnterSuccess; ;
                //entity.Enter();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            throw new NotImplementedException();
        }
       /// <summary>
       /// 是否已存在
       /// </summary>
       /// <returns></returns>
        protected object Exist()
        {
            string name = Request["Name"].Trim();
            if (new EnterprisesRoll().Any(item => item.Status == AuditStatus.Black))
            {
                return new { Exist = true, message = "该企业是黑名单企业", IsDraft = false };
            }
            var chains = Erp.Current.CrmPlus.Chains.FirstOrDefault(item => item.Enterprise.Name.ToUpper() == name.ToUpper());
            if (chains != null)
            {
                return new { Exist = true, message = "已注册" };
            }
            else
            {
                return new { Exist = false, message = "可注册" };
            };
        }
    }
}