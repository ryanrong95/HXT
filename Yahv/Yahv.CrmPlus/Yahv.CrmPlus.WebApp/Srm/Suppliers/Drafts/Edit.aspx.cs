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

namespace Yahv.CrmPlus.WebApp.Srm.Suppliers.Drafts
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Area = Service.EnumsDictionary<FixedArea>.Current.Select(item => new
                {
                    value = item.ID,
                    text = item.Description
                });
              
                this.Model.EnterpriseNature = ExtendsEnum.ToArray<EnterpriseNature>().Select(item => new
                {
                    value = item.GetDescription(),
                    text = item.GetDescription()
                });
                var supplier = Erp.Current.CrmPlus.MySuppliers[Request.QueryString["enterpriseid"]];
                this.Model.Entity = supplier;
                this.Model.Files = new Service.Views.FilesDescriptionsView().Search(item => item.EnterpriseID == Request.QueryString["enterpriseid"]).ToMyArray();
            }

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var entity = Erp.Current.CrmPlus.MySuppliers[Request.QueryString["enterpriseid"]];
                if (entity == null)
                {
                    return;
                }
                #region 获取页面数据
                string name = Request["SupplierName"].Trim();
                string Area = Request.Form["Area"];
                bool isinternational = Request["IsInternational"] != null;
                string EnterpriseNature = Request.Form["EnterpriseNature"];

                string place = Request["Place"];
                //Origin origin;
                //if (Enum.TryParse(place, out origin))
                //{
                //    place = origin.GetOrigin().Code;
                //}
              
                InvoiceType invoicetype = (InvoiceType)int.Parse(Request.Form["InvoiceType"]);
                Underly.CrmPlus.SupplierType suppliermode = (Underly.CrmPlus.SupplierType)int.Parse(Request.Form["SupplierType"]);
                Underly.CrmPlus.OrderType ordertype = (Underly.CrmPlus.OrderType)int.Parse(Request.Form["OrderType"]);
                string worktime = Request.Form["WorkTime"];
                bool isfixed = Request.Form["IsFixed"] != null;
                string website = Request.Form["WebSite"];
                string Product = Request.Form["Product"];

                string address = Request.Form["Address"];
                string uscc = Request.Form["Uscc"];
                string Corperation = Request.Form["Corperation"];

                //注册币种
                Currency RegistCurrency = Request.Form["RegistCurrency"] == null ? Currency.Unknown : (Currency)int.Parse(Request.Form["RegistCurrency"]);
                string RegistFund = Request.Form["RegistFund"];
                string RegAddress = Request.Form["RegAddress"];
                int Employees = 0;
                int.TryParse(Request.Form["Employees"], out Employees);
                string BusinessState = Request.Form["BusinessState"];
                #endregion

                #region Enterprise
                entity.Name = name;
                //entity.Enterprise.IsDraft = true;
                entity.District = Request["Area"];
                #endregion

                #region Enterprise.EnterpriseRegister
                entity.EnterpriseRegister.IsInternational = isinternational;
                entity.EnterpriseRegister.WebSite = website;
                entity.EnterpriseRegister.IsSecret = false;
                entity.EnterpriseRegister.Corperation = Request["Corperation"].Trim();
                entity.EnterpriseRegister.RegAddress = isinternational? address: RegAddress;
                entity.EnterpriseRegister.Uscc = Request["Uscc"].Trim();

                entity.EnterpriseRegister.RegistFund = Request["RegistFund"];
                entity.EnterpriseRegister.RegistCurrency = RegistCurrency;
                entity.EnterpriseRegister.Industry = null;
                entity.EnterpriseRegister.BusinessState = BusinessState;
                entity.EnterpriseRegister.Employees = Employees;
                entity.EnterpriseRegister.Nature = EnterpriseNature;
                string registDate = Request.Form["RegistDate"];
                if (isinternational)
                {
                    entity.EnterpriseRegister.Currency = (Currency)int.Parse(Request.Form["Currency"]);
                }
                if (string.IsNullOrWhiteSpace(registDate))
                {
                    entity.EnterpriseRegister.RegistDate = null;
                }
                else
                {
                    entity.EnterpriseRegister.RegistDate = Convert.ToDateTime(registDate);
                }
                #endregion

                #region 供应商信息
                entity.Grade = null;
                entity.Products = Product;
                //entity.Source = null;
                entity.Type = suppliermode;
                //entity.SettlementType = null;
                entity.OrderType = ordertype;
                entity.InvoiceType = invoicetype;
                entity.WorkTime = worktime;
                entity.IsFixed = isfixed;
                entity.CreatorID = Erp.Current.ID;
                entity.Place = place;
                #endregion

                #region 文件
                var logo = Request.Form["LogoForJson"];
                entity.Logo = string.IsNullOrWhiteSpace(logo) ? null : logo.JsonTo<List<CallFile>>().FirstOrDefault();
                var licenses = Request.Form["LicensesForJson"];
                entity.Lisences = string.IsNullOrWhiteSpace(licenses) ? null : licenses.JsonTo<List<CallFile>>();
                //var qualification = Request.Form["QualificationForJson"];
                //entity.QualificationCertificates = string.IsNullOrWhiteSpace(qualification) ? null : qualification.JsonTo<List<CallFile>>();
                #endregion
                entity.SupplierStatus = Underly.AuditStatus.Waiting;
                entity.EnterSuccess += Entity_EnterSuccess;
                entity.Enter();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Supplier;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"修改草稿供应商:{entity.Name}");
            (new ApplyTask
            {
                MainID = entity.ID,
                MainType = MainType.Suppliers,
                ApplierID = entity.CreatorID,
                ApplyTaskType = ApplyTaskType.SupplierRegist
            }).Enter();
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        protected object Exist()
        {
            string name = Request["Name"].Trim();
            if (new EnterprisesRoll().Any(item => item.Name == name && item.Status == AuditStatus.Black))
            {
                return new { Exist = true, message = "该企业是黑名单企业", IsDraft = false };
            }
            var suppliers = Erp.Current.CrmPlus.Suppliers.FirstOrDefault(item => item.Name.ToUpper() == name.ToUpper() && item.IsDraft == false && item.Status == AuditStatus.Normal);
            var mysuppliers = Erp.Current.CrmPlus.MySuppliers.FirstOrDefault(item => item.Name.ToUpper() == name.ToUpper() && item.Status == AuditStatus.Waiting);
            if (mysuppliers != null)
            {
                return new { Exist = true, message = "已注册待审批" };
            }
            else if (suppliers != null)
            {
                return new { Exist = true, message = "供应商已存在" };
            }
            else
            {
                return new { Exist = false, message = "可注册" };
            };
        }
       
    }
}