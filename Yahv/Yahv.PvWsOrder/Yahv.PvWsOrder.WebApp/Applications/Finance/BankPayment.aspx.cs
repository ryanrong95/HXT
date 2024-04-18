using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Payments.Views;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Applications.Finance
{
    public partial class BankPayment : ErpParticlePage
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
            string ID = Request.QueryString["ID"];
            var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);
            if (application != null)
            {
                this.Model.ApplicationData = new
                {
                    CreateDate = application.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ClientName = application.Client.Name,
                    ClientCode = application.Client.EnterCode,
                    Currency = application.Currency.GetDescription(),
                    AvailableBalance = 0,

                    PayerName = application.Payers.FirstOrDefault()?.EnterpriseName,
                    PayerBankName = application.Payers.FirstOrDefault()?.BankName,
                    PayerBankAccount = application.Payers.FirstOrDefault()?.BankAccount,
                    PayerMethod = application.Payers.FirstOrDefault()?.Method.GetDescription(),
                    PayerCurrency = application.Payers.FirstOrDefault()?.Currency.GetDescription(),

                    PayeeName = application.Payees.FirstOrDefault()?.EnterpriseName,
                    PayeeBankName = application.Payees.FirstOrDefault()?.BankName,
                    PayeeBankAccount = application.Payees.FirstOrDefault()?.BankAccount,
                    PayeeMethod = application.Payees.FirstOrDefault()?.Method.GetDescription(),
                    PayeeCurrency = application.Payees.FirstOrDefault()?.Currency.GetDescription(),

                    InCompanyName = application.InCompanyName,
                    InBankName = application.InBankName,
                    InBankAccount = application.InBankAccount,

                    OutCompanyName = application.OutCompanyName,
                    OutBankName = application.OutBankName,
                    OutBankAccount = application.OutBankAccount,
                };
            }
        }

        protected object data()
        {
            string ID = Request.QueryString["ID"];
            var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);
            var payer = new PayersTopView().Where(item => item.Account == application.OutBankAccount && item.Status == GeneralStatus.Normal && item.Methord != Methord.Cash)
                .FirstOrDefault(t => t.RealEnterpriseID == t.EnterpriseID || t.RealEnterpriseID == null || t.RealEnterpriseID == string.Empty)
                .EnterpriseID;
            var payee = application.Payees.FirstOrDefault().EnterpriseID;
            //付款记录展示
            return Erp.Current.Pays.FlowAccounts
                .Where(item => item.Type == AccountType.BankStatement)
                .Where(item => item.Payer == payer && item.Payee == payee)
                .Where(item => item.Price > 0 && item.Currency == application.Currency)
                .ToArray().OrderByDescending(item => item.CreateDate).Select(item => new
                {
                    item.Business,
                    item?.FormCode,
                    item?.Bank,
                    Currency = item?.Currency.GetDescription(),
                    Price = item?.Price,
                    CreateDate = item?.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    item?.Account,
                });
        }

        /// <summary>
        /// 付款凭证
        /// </summary>
        protected void UploadInvoice()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadInvoice");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            FileDirectory dic = new FileDirectory(file.FileName, FileType.PaymentVoucher);
                            dic.AdminID = Erp.Current.ID;
                            dic.Save(file);
                            fileList.Add(new
                            {
                                ID = dic.uploadResult.FileID,
                                CustomName = dic.FileName,
                                FileName = dic.uploadResult.FileName,
                                Url = dic.uploadResult.Url,
                            });
                        }
                    }
                }
                Response.Write((new { success = true, data = fileList }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 加载付款凭证
        /// </summary>
        protected object LoadInvoice()
        {
            string OrderID = Request.QueryString["OrderID"];
            var query = new PvWsOrder.Services.Views.OrderFilesRoll(OrderID)
                .Where(item => item.Type == (int)FileType.PaymentVoucher)
                .Where(item => item.ApplicationID == null || item.ApplicationID == "");
            var linq = query.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                Url = FileDirectory.ServiceRoot + t.Url,
            });
            return new
            {
                rows = linq.ToArray(),
                total = query.Count()
            };
        }

        protected void Submit()
        {
            try
            {
                //基本信息
                string ID = Request.Form["ID"].Trim();
                decimal Amount = Convert.ToDecimal(Request.Form["Amount"].Trim());
                string SerialNumber = Request.Form["SerialNumber"].Trim();
                DateTime Date = Convert.ToDateTime(Request.Form["Date"]);

                var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);

                var payer = new PayersTopView().Where(item => item.Account == application.OutBankAccount && item.Status == GeneralStatus.Normal && item.Methord != Methord.Cash)
                    .FirstOrDefault(t => t.RealEnterpriseID == t.EnterpriseID || t.RealEnterpriseID == null || t.RealEnterpriseID == string.Empty)
                    .EnterpriseID;
                var payee = application.Payees.FirstOrDefault().EnterpriseID;

                //银行付款
                string result = Yahv.Payments.PaymentManager.Erp(Erp.Current.ID)[payer, payee].Digital
                      .AdvanceToSuppliers(application.Currency, Amount, application.OutBankName, application.OutBankAccount, SerialNumber, Date);

                //付款凭证
                var invoices = Request.Form["invoices"].Replace("&quot;", "'").Replace("amp;", "");
                var invoiceList = invoices.JsonTo<List<Services.Models.CenterFileDescription>>();
                if (!string.IsNullOrWhiteSpace(result) && invoiceList.Count > 0)
                {
                    new PaymentFileAlls(result).SaveFiles(invoiceList);
                }

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}