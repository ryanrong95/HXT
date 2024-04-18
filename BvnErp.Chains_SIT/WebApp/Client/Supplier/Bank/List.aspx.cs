using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Client.Supplier.Bank
{
    public partial class List : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["SupplierID"];
            this.Model.ID = id;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["SupplierID"];
            string Name = Request.QueryString["Name"];
            string Account = Request.QueryString["Account"];

            var accounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierBanks.AsQueryable();
            accounts = accounts.Where(t => t.ClientSupplierID == id && t.Status != Needs.Ccs.Services.Enums.Status.Delete);

            Func<Needs.Ccs.Services.Models.ClientSupplierBank, object> convert = account => new
            {
                account.ID,
                //Name = account.Beneficiary.Name,
                Account = account.BankAccount,
                BankName = account.BankName,
                BankAddress = account.BankAddress,
                SwiftCode = account.SwiftCode,
                CreateDate = account.CreateDate.ToString("yyyy-MM-dd"),
                Status = account.Status.GetDescription(),
                Summary = account.Summary,
                account.Place,
               // Currency= account.Currency==null?null: account.Currency.GetDescription(),
                Methord= account.Methord==null?null: account.Methord.GetDescription()
            };
            this.Paging(accounts, convert);
        }

        /// <summary>
        /// 删除银行账户
        /// </summary>
        protected void DeleteClientSupplierAccount()
        {
            //    string ids = Request.Form["ID"];
            //    ids.Split(',').ToList().ForEach(t =>
            //    {
            //        var invoice = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierBanks[t];
            //        invoice.Abandon();
            //    });
            string id = Request.Form["ID"];
            var supplieraccount = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierBanks[id];
            if (string.IsNullOrEmpty(URL))
            {
                supplieraccount.Abandon();
            }
            else {
                #region 调用后
                try
                {
                    string requestUrl = URL + "/CrmUnify/DelBeneficiary";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var supplier = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliers[supplieraccount.ClientSupplierID];
                    var apiclient = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[supplier.ClientID];
                    var entity = new ApiBeneficiary()
                    {
                        Enterprise = new EnterpriseObj
                        {
                            AdminCode = " ",
                            District = " ",
                            Corporation = " ",
                            Name = supplier.Name,
                            RegAddress = " ",
                            Uscc = " ",
                            Status = 200
                        },
                        WsClient = new EnterpriseObj
                        {
                            AdminCode = " ",
                            District = "",
                            Corporation = apiclient.Company.Corporate,
                            Name = apiclient.Company.Name,
                            RegAddress = apiclient.Company.Address,
                            Uscc = apiclient.Company.Code,
                            Status = 200
                        },
                        InvoiceType = 1,
                        RealName = apiclient.Company.Name,
                        Bank = supplieraccount.BankName,
                        BankAddress = supplieraccount.BankAddress,
                        Account = supplieraccount.BankAccount,
                        SwiftCode = supplieraccount.SwiftCode,
                        Methord = (int)supplieraccount.Methord,
                        Currency =(int)supplieraccount.Currency,
                        District = 1,
                        Name = apiclient.Company.Contact.Name,
                        Tel = apiclient.Company.Contact.Tel,
                        Mobile = apiclient.Company.Contact.Mobile,
                        Email = apiclient.Company.Contact.Email,
                        Status = 200,
                        Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                        CreateDate = DateTime.Now.ToString(),
                        UpdateDate = DateTime.Now.ToString(),
                        IsDefault = false
                    };

                    string apiSupplier = JsonConvert.SerializeObject(entity);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiSupplier);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                        return;
                    }
                    supplieraccount.Abandon();
                }
                catch (Exception)
                {

                    throw;
                }
                #endregion
            }
        }
    }
}