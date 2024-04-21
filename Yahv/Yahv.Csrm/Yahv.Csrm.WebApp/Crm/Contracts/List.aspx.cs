
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.Contracts
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string CompanyID = Request.QueryString["CompanyID"];
                this.Model.CompanyID = CompanyID;
                var wsclient= Erp.Current.Whs.WsClients[CompanyID, Request.QueryString["id"]];
                var contract= wsclient.Contract;
                this.Model.Contract = contract;
                this.Model.File = contract?.ServiceAgreement;
                //换汇时间
                this.Model.ExchageDate = ExtendsEnum.ToArray<ExchangeMode>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //开票类型
                this.Model.BillingType = ExtendsEnum.ToArray<BillingType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //发票税点
                this.Model.InvoiceRate = ExtendsEnum.ToDictionary<InvoiceRate>().Select(item => new
                {
                    value = item.Value,
                    text = item.Value
                });

                this.Model.NotShowBtnSave = Erp.Current.Role.ID == FixedRole.ServiceManager.GetFixedID() && wsclient.WsClientStatus == ApprovalStatus.Normal;
                this.Model.Nonstandard = wsclient.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var clientid = Request.QueryString["id"];
            string companyid = Request.QueryString["CompanyID"];
            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }
           
            var client = Erp.Current.Whs.WsClients[clientid];
            if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
            {
                Easyui.Reload("提示!", "请先规范客户名称", Web.Controls.Easyui.Sign.None);
            }
            Enterprise company = new YaHv.Csrm.Services.Views.Rolls.CompaniesRoll()[companyid].Enterprise;
            var contract = new Contract();
            contract.Company = company;
            contract.StartDate = Convert.ToDateTime(Request["StartDate"]);
            contract.EndDate = Convert.ToDateTime(Request["EndDate"]);
            contract.Enterprise = client.Enterprise;

            contract.MinAgencyFee = decimal.Parse(Request["MinAgencyFee"]);//最低代理费
            contract.AgencyRate = decimal.Parse(Request["AgencyRate"]);//代理费率
            BillingType billingtype = (BillingType)int.Parse(Request["InvoiceType"]);
            contract.InvoiceType = billingtype;
            contract.InvoiceTaxRate = billingtype == BillingType.Full ? decimal.Parse(Request["fullTaxRate"].TrimEnd('%')) / 100 : decimal.Parse(Request["InvoiceTaxRate"].TrimEnd('%')) / 100;
            contract.ExchangeMode = (ExchangeMode)int.Parse(Request["PayExchange"]);
            contract.Summary = Request["Summary"].Trim();

            contract.CreatorID = Yahv.Erp.Current.ID;
            contract.EnterSuccess += Contract_EnterSuccess;
            contract.Enter();
        }
        private void Contract_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            //var url = hidurl.Value;
            //var contract = sender as Contract;
            //var wsclient = Erp.Current.Whs.WsClients[contract.Enterprise.ID];
            //string oldurl = contract.ServiceAgreement == null ? "" : contract.ServiceAgreement.Url;

            //if (!string.IsNullOrWhiteSpace(url) && url != oldurl)
            //{
            //    var file = new FileDescription
            //    {
            //        EnterpriseID = wsclient.ID,
            //        CompanyID = wsclient.Company.ID,
            //        Enterprise = wsclient.Enterprise,
            //        Name = hidName.Value,
            //        Type = FileType.ServiceAgreement,
            //        Url = url,
            //        FileFormat = hidFormat.Value,
            //        CreatorID = Erp.Current.ID,
            //    };
            //    file.EnterSuccess += File_EnterSuccess; ;
            //    file.Enter();
            //}
            //else
            //{
            //    Unify(wsclient);
            //    Easyui.Reload("提示!", "保存成功", Web.Controls.Easyui.Sign.None); ;
            //}
            Easyui.Reload("提示!", "保存成功", Web.Controls.Easyui.Sign.None); ;
        }

        private void File_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            string clientid = (sender as FileDescription).EnterpriseID;
            var client = Erp.Current.Whs.WsClients[clientid];
            Unify(client);
            Easyui.Reload("提示!", "保存成功", Web.Controls.Easyui.Sign.None);
        }
        public object Unify(WsClient client)
        {
            //var client = Erp.Current.Whs.WsClients[companyid, clientid];
            string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            ///只芯达通客户同步 && client.Company.ID == "DBAEAB43B47EB4299DD1D62F764E6B6A"
            if (!string.IsNullOrWhiteSpace(apiurl))
            {
                var contract = client.Contract;
                var json = new
                {
                    Enterprise = contract.Enterprise,
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    AgencyRate = contract.AgencyRate,
                    MinAgencyFee = contract.MinAgencyFee,
                    IsPrePayExchange = contract.ExchangeMode == Underly.ExchangeMode.PrePayExchange,
                    IsLimitNinetyDays = contract.ExchangeMode == Underly.ExchangeMode.LimitNinetyDays,
                    InvoiceType = (int)contract.InvoiceType,
                    InvoiceTaxRate = contract.InvoiceTaxRate,
                    Creator = client.ServiceManager.RealName,
                    Summary = contract.Summary,
                    CreateDate = contract.CreateDate,
                    ClientFile = contract.ServiceAgreement == null ? null : new
                    {
                        Type = (int)FileType.ServiceAgreement,
                        Name = contract.ServiceAgreement.CustomName,
                        Url = contract.ServiceAgreement.Url,
                        CreateDate = contract.ServiceAgreement.CreateDate.ToString(),
                    }
                }.Json();
                var response = HttpClientHelp.HttpPostRaw(apiurl + "/clients/agreement", json);
                return response;
            }
            return null;
        }


        /// <summary>
        /// 导出协议word
        /// </summary>
        protected object ExportAgreement()
        {
            try
            {
                var clientid = Request.Form["clientid"];
                string companyid = Request.Form["CompanyID"];
                var wsclient = Erp.Current.Whs.WsClients[companyid, clientid];
                //创建文件夹
                var fileName = DateTime.Now.Ticks + "服务协议草书.docx";

                //var fileurl = Yahv.Utils.FileServices.Save();
                var dirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\Dowload\\Contracts");
                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                var path = System.IO.Path.Combine(dirPath, fileName);
                string fileurl = $"../../Files/Dowload/Contracts/{fileName}";
                //保存文件
                wsclient.SaveAs(path);

                return new { success = true, message = "导出成功", url = fileurl };
                //return null;
            }
            catch (Exception ex)
            {
                return new { success = false, message = "导出失败" + ex.Message };
            }
        }
        protected object PreviewAgreement()
        {
            try
            {
                var clientid = Request.Form["clientid"];
                string companyid = Request.Form["CompanyID"];
                var wsclient = Erp.Current.Whs.WsClients[companyid, clientid];
                //创建文件夹
                var fileName = DateTime.Now.Ticks + "服务协议草书.html";

                //var fileurl = Yahv.Utils.FileServices.Save();
                var dirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\Dowload\\Contracts");
                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                var path = System.IO.Path.Combine(dirPath, fileName);
                string fileurl = $"../../Files/Dowload/Contracts/{fileName}";
                //保存文件
                wsclient.SaveAs(path, true);

                return new { success = true, message = "成功", url = fileurl };
                //return null;
            }
            catch (Exception ex)
            {
                return new { success = false, message = "失败" + ex.Message };
            }
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        //protected void delete()
        //{
        //    string fileName = Request.QueryString["file"];
        //    var path = System.IO.Path.Combine(Server.MapPath("/files"), fileName);
        //    var fi = new System.IO.FileInfo(path);
        //    if (fi.Exists)
        //    {
        //        fi.Delete();
        //    }
        //    Response.End();
        //}

    }
}