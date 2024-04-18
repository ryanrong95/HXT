using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.Approval
{
    /// <summary>
    /// 会员（客户）-客服人员/业务员分配-查询界面
    /// Add by Merlin@20181103
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            var status = EnumUtils.ToDictionary<ClientStatus>().Select(item => new { item.Key, item.Value });
            this.Model.Status = status.Json();

            var ServiceIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "业务员").Select(item => item.Admin.ID).ToArray();
            this.Model.ServiceManager = Needs.Wl.Admin.Plat.AdminPlat.Admins.Where(x => ServiceIDs.Contains(x.ID)).Select(item => new { Key = item.ID, Value = item.RealName }).ToArray().Json();

            this.Model.CurrentName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
            this.Model.DepartmentType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.DepartmentType>()
                                            .Where(t => t.Value.Contains("业务"))
                                            .Select(item => new { Key = item.Value, item.Value }).Json();

            //超100W审批权限
            this.Model.ApproveExceedID = System.Configuration.ConfigurationManager.AppSettings["ApproveExceedID"];
            //总公司额度
            this.Model.ApproveExceedAmount = System.Configuration.ConfigurationManager.AppSettings["ApproveExceedAmount"];
            //当前登录人
            this.Model.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

        }



        protected object getAdminsByDepartmentId()
        {
            var name = Request.Form["DepartmentName"];

            if (string.IsNullOrEmpty(name))
            {
                return new { succes = false, message = "无数据" };

            }

            var admins = Needs.Wl.Admin.Plat.AdminPlat.Admins.Where(x => x.DepartmentName == name.Trim()).Select(item => new { Key = item.ID, Value = item.RealName }).ToArray().Json();
            return new { succes = true, data = admins };

        }
        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string CompanyName = Request.QueryString["CompanyName"];
            string ClientCode = Request.QueryString["ClientCode"];
            string UnPayExchangeAmount = Request.QueryString["UnPayExchangeAmount"];
            string CreateDateFrom = Request.QueryString["CreateDateFrom"];
            string CreateDateTo = Request.QueryString["CreateDateTo"];
            string Status = Request.QueryString["Status"];
            var ClaimStatus = Request.QueryString["ClaimStatus"];
            string Servicemanager = Request.QueryString["Servicemanager"];
            var ReturnedStatus = Request.QueryString["ReturnedStatus"];
            var IsSAEleUpload = Request.QueryString["IsSAEleUpload"];
            var DepartmentType = Request.QueryString["DepartmentType"];

            //var clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView.AsQueryable().Where(x=>x.Status== Needs.Ccs.Services.Enums.Status.Normal);

            var clientsView = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView;

            var clients = clientsView.AsQueryable().Where(x => x.Status == Needs.Ccs.Services.Enums.Status.Normal);

            if (!string.IsNullOrEmpty(CompanyName))
            {
                clients = clients.Where(t => t.Company.Name.Contains(CompanyName.Trim()));
            }
            if (!string.IsNullOrEmpty(ClientCode))
            {
                clients = clients.Where(t => t.ClientCode.Contains(ClientCode.Trim()));
            }
            if (!string.IsNullOrEmpty(UnPayExchangeAmount))
            {
                var amount = decimal.Parse(UnPayExchangeAmount);
                clients = clients.Where(t => t.UnPayExchangeAmount >= amount);
            }
            if (!string.IsNullOrEmpty(CreateDateFrom))
            {
                var from = DateTime.Parse(CreateDateFrom);
                clients = clients.Where(t => t.CreateDate >= from);
            }
            if (!string.IsNullOrEmpty(CreateDateTo))
            {
                var to = DateTime.Parse(CreateDateTo).AddDays(1); ;
                clients = clients.Where(t => t.CreateDate < to);
            }
            if (!string.IsNullOrEmpty(Status))
            {
                int status = Int32.Parse(Status);
                clients = clients.Where(t => t.ClientStatus == (Needs.Ccs.Services.Enums.ClientStatus)status);
            }
            if (Convert.ToBoolean(ClaimStatus))
            {
                clients = clients.Where(x => x.ServiceManager.RealName == null && x.ClientStatus == ClientStatus.Auditing);
            }

            if (!string.IsNullOrEmpty(Servicemanager))
            {
                clients = clients.Where(x => x.ServiceManager.ID == Servicemanager);
            }
            if (Convert.ToBoolean(ReturnedStatus))
            {
                clients = clientsView.SearchByReturned(clients);
            }

            if (Convert.ToBoolean(IsSAEleUpload))
            {
                clients = clientsView.SearchByIsSAEleUpload(clients, !Convert.ToBoolean(IsSAEleUpload));
            }

            clients = clientsView.SearchAddServiceManagerDepart(clients, DepartmentType);

            Func<Needs.Ccs.Services.Models.Client, object> convert = client => new
            {
                client.ID,
                CompanyCode = client.Company.Code,
                CompanyName = client.Company.Name,
                CustomsCode = client.Company.CustomsCode,
                ClientCode = client.ClientCode,
                ClientRankValue = client.ClientRank,
                ClientRank = client.ClientRank.GetDescription(),
                ContactName = client.Company.Contact.Name,
                ContactTel = client.Company.Contact.Mobile,
                CreateDate = client.CreateDate.ToShortDateString(),
                Status = client.ClientStatus.GetDescription(),
                StatusValue = client.ClientStatus,
                MerchandiserName = client.Merchandiser?.RealName,
                ReferrerName = client.Referrer,
                ///== null ? "-" : client.Merchandiser.RealName,
              //  ReferrerName = client.Referrer?.RealName,//== null ? "-" : client.Referrer.RealName,
                ServiceManagerName = client.ServiceManager?.RealName, //== null ? "-" : client.ServiceManager.RealName,
                //StorageServiceManagerName = client.StorageServiceManager?.RealName,// == null ? "-" : client.StorageServiceManager.RealName,
                //StorageMerchandiserName = client.StorageMerchandiser?.RealName,// == null ? "-" : client.StorageMerchandiser.RealName,
                //StorageReferrerName = client.StorageReferrer?.RealName,// == null ? "-" : client.StorageReferrer.RealName,
                RegisterYear = client.RegisterYear,
                RegisterDays = (DateTime.Now - client.CreateDate).Days,
                IsSpecified = client.IsSpecified,
                IsValid = client.IsValid,
                IsStorageValid = client.IsStorageValid,
                client.Summary,
                ServiceType = client.ServiceType.GetDescription(),

                DepartmentCode = client.DepartmentCode,
                //StorageDepartmentCode = client.StorageDepartmentCode
                ///DepartmentCode = client?.Name
                //DepartmentName = client.DepartmentCode != null ?
                //                    ((Needs.Ccs.Services.Enums.DepartmentType)int.Parse(client.DepartmentCode)).GetDescription() : "",

                UnPayExchangeAmount = client.UnPayExchangeAmount.HasValue ? client.UnPayExchangeAmount.Value : 0,
                DeclareAmount = client.DeclareAmount.HasValue ? client.DeclareAmount.Value : 0,
                PayExchangeAmount = client.PayExchangeAmount.HasValue ? client.PayExchangeAmount.Value : 0,
            };

            this.Paging(clients, convert);
        }



        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientStatus_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientStatus_EnterError_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "提交成功", ID = e.Object }).Json());
        }

        /// <summary>
        /// 验证额度
        /// </summary>
        protected void CheckIsExcced() 
        {
            var ID = Request.Form["ID"];

            try {

                //超100W审批权限
                var ApproveExceedID = System.Configuration.ConfigurationManager.AppSettings["ApproveExceedID"];
                //总公司额度
                var ApproveExceedAmountStr = System.Configuration.ConfigurationManager.AppSettings["ApproveExceedAmount"];
                var ApproveExceedAmount = decimal.Parse(ApproveExceedAmountStr);
                //当前登录人
                var AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;


                //客户的所有额度
                var agreementView = new Needs.Ccs.Services.Views.ClientAgreementsView().FirstOrDefault(t => t.ClientID == ID && t.Status == Status.Normal);

                //非报关客户不校验
                if (agreementView == null)
                {
                    Response.Write((new { success = true, message = "" }).Json());
                }
                else
                {
                    var amount = (agreementView.ProductFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.ProductFeeClause.UpperLimit : 0)
                        + (agreementView.TaxFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.TaxFeeClause.UpperLimit : 0)
                        + (agreementView.AgencyFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.AgencyFeeClause.UpperLimit : 0)
                        + (agreementView.IncidentalFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.IncidentalFeeClause.UpperLimit : 0);

                    if (amount <= ApproveExceedAmount)
                    {
                        Response.Write((new { success = true, message = "" }).Json());
                    }
                    else if (ApproveExceedID.Contains(AdminID))
                    {
                        //有权限
                        Response.Write((new { success = true, message = "" }).Json());
                    }
                    else
                    {
                        //无权限
                        Response.Write((new { success = false, message = "" }).Json());
                    }
                }
            }
            catch (Exception ex) {
                Response.Write((new { success = false, message = "校验额度权限出错：" + ex.Message }).Json());

            }

        }
    }
}
