using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Srm.Approvals.Suppliers
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
                //this.Model.Logo = new FilesDescriptionRoll()[supplier.ID, CrmFileType.Logo].FirstOrDefault();
                //this.Model.Licenses = new FilesDescriptionRoll()[supplier.ID, CrmFileType.License];
                //this.Model.Qualifications = new FilesDescriptionRoll()[supplier.ID, CrmFileType.QualificationCertificate];
            }

        }
        string context = "";
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                bool result = bool.Parse(this.Result.Value);
                context = result ? "审批通过" : "审批不通过";
                var entity = Erp.Current.CrmPlus.MySuppliers[Request.QueryString["enterpriseid"]];
                if (entity == null)
                {
                    return;
                }
                ////审批不通过
                //if (!approveresult)
                //{
                //    entity.Status=entity.Enterprise.Status = AuditStatus.Voted;
                //    entity.EnterpriseRegister.Summary = Request["Idea"];
                //    entity.EnterSuccess += Entity_EnterSuccess1;
                //    entity.Enter();
                //}
                ////审批通过
                //else
                //{
                //entity.Status = entity.Enterprise.Status = AuditStatus.Normal;
                #region 获取页面数据
                string name = Request["SupplierName"].Trim();
                string Area = Request.Form["Area"];
                bool isinternational = Request["IsInternational"] != null;
                string EnterpriseNature = Request.Form["EnterpriseNature"];
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
                //entity.Enterprise = new Enterprise();
                entity.Name = name;
                entity.IsDraft = true;
                entity.District = Request["Area"];
                #endregion

                #region EnterpriseRegister
                entity.EnterpriseRegister.IsInternational = isinternational;
                if (isinternational)
                {
                    entity.EnterpriseRegister.Currency = (Currency)int.Parse(Request.Form["Currency"]);
                    entity.EnterpriseRegister.RegAddress = address;
                }
                else
                {
                    // entity.EnterpriseRegister = new EnterpriseRegister();
                    entity.EnterpriseRegister.Summary = Request["Idea"];
                    entity.EnterpriseRegister.IsSecret = false;
                    
                    entity.EnterpriseRegister.Corperation = Request["Corperation"].Trim();
                    entity.EnterpriseRegister.RegAddress = isinternational ? address : RegAddress;
                    entity.EnterpriseRegister.Uscc = Request["Uscc"].Trim();

                    entity.EnterpriseRegister.RegistFund = Request["RegistFund"];
                    entity.EnterpriseRegister.RegistCurrency = RegistCurrency;
                    entity.EnterpriseRegister.Industry = null;
                    entity.EnterpriseRegister.Currency = null;
                    entity.EnterpriseRegister.BusinessState = BusinessState;
                    entity.EnterpriseRegister.Employees = Employees;
                    entity.EnterpriseRegister.WebSite = website;
                    entity.EnterpriseRegister.Nature = EnterpriseNature;
                    string registDate = Request.Form["RegistDate"];
                    if (string.IsNullOrWhiteSpace(registDate))
                    {
                        entity.EnterpriseRegister.RegistDate = null;
                    }
                    else
                    {
                        entity.EnterpriseRegister.RegistDate = Convert.ToDateTime(registDate);
                    }

                }
                #endregion

                #region 供应商信息
                entity.SupplierGrade = int.Parse(Request["Grade"]);
                entity.Products = Product;
                //entity.Source = null;
                entity.Type = suppliermode;
                entity.SettlementType = (Underly.CrmPlus.SettlementType)int.Parse(Request["SettlementType"]);
                entity.OrderType = ordertype;
                entity.InvoiceType = invoicetype;
                entity.WorkTime = worktime;
                entity.IsFixed = isfixed;
                entity.OrderCompanyID = Request.Form["OrderEnterprise"];
                //entity.CreatorID = Erp.Current.ID;
                #endregion
                if (result)
                {
                    entity.AllowedSuccess += Entity_AllowedSuccess;
                    entity.Allowed(Erp.Current.ID);
                }
                else
                {
                    entity.VotedSuccess += Entity_VotedSuccess;
                    entity.Voted(Erp.Current.ID);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Entity_VotedSuccess(object sender, Usually.SuccessEventArgs e)
        {
            string supplierid = Request.QueryString["enterpriseid"];
            (new ApplyTask
            {
                MainID = supplierid,
                MainType = MainType.Suppliers,
                //ApplierID = this.CreatorID,
                ApproverID = Erp.Current.ID,
                ApplyTaskType = ApplyTaskType.SupplierRegist,
                Status = ApplyStatus.Allowed
            }).Approve();
            Service.LogsOperating.LogOperating(Erp.Current, supplierid, $"供应商:{supplierid}，注册审批否决");
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        private void Entity_AllowedSuccess(object sender, Usually.SuccessEventArgs e)
        {
            string supplierid = Request.QueryString["id"];
            (new ApplyTask
            {
                MainID = supplierid,
                MainType = MainType.Suppliers,
                //ApplierID = this.CreatorID,
                ApproverID = Erp.Current.ID,
                ApplyTaskType = ApplyTaskType.SupplierRegist,
                Status = ApplyStatus.Allowed
            }).Approve();
            Service.LogsOperating.LogOperating(Erp.Current, Request.QueryString["id"], $"供应商:{supplierid}，注册审批通过");
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        

    }
}