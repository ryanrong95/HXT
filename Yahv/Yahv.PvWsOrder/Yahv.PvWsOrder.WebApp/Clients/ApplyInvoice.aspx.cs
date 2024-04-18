using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views.Alls;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Clients
{
    public partial class ApplyInvoice : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }
        private void LoadData()
        {

            string clientId = Request.QueryString["clientId"];
            var client = Erp.Current.WsOrder.MyWsClients.SingleOrDefault(t => t.ID == clientId);
            var vinvoices = Erp.Current.WsOrder.vInvoices.Where(t => t.EnterpriseID == client.ID);
            var invoices = Erp.Current.WsOrder.Invoices.Where(t => t.EnterpriseID == client.ID);

            //开票类型选项
            List<object> invoiceTypeOptionList = new List<object>();
            invoiceTypeOptionList.Add(new { value = InvoiceType.None, text = InvoiceType.None.GetDescription(), });
            invoiceTypeOptionList.Add(new { value = InvoiceType.VAT, text = InvoiceType.VAT.GetDescription(), });
            this.Model.InvoiceTypeOption = invoiceTypeOptionList.Json();

            //客户为国内公司，抬头固定客户公司；
            //客户为个人，抬头可以选择开票公司，同时根据下拉框的选择，变更开票信息和邮寄信息
            var isPersonal = client.StorageType == WsIdentity.Personal ? true : false;
            var invoiceResult = isPersonal ? vinvoices.ToArray().Select(item => new
            {
                Value = item.ID,
                Text = item.Title,

                InvoiceType = item.Type,
                InvoiceTypeDesc = item.Type.GetDescription(),
                DeliveryType = item.DeliveryType,
                DeliveryTypeDesc = item.DeliveryType.GetDescription(),
                CompanyName = item.Title,
                TaxCode = item.TaxNumber,
                BankName = item.BankName,
                BankAccount = item.BankAccount,
                Address = item.RegAddress,
                Tel = item.Tel,
                ReceipterCompany = item.Title,
                ReceipterName = item.PostRecipient,
                ReceipterTel = item.PostTel,
                DetailAddress = item.PostAddress,
                ClientID = clientId,
                IsPersonal = isPersonal,
            })
            : invoices.ToArray().Select(item => new
            {
                Value = item.ID,
                Text = item.CompanyName,

                InvoiceType = item.Type,
                InvoiceTypeDesc = item.Type.GetDescription(),
                DeliveryType = item.DeliveryType,
                DeliveryTypeDesc = item.DeliveryType.GetDescription(),
                CompanyName = item.CompanyName,
                TaxCode = item.TaxperNumber,
                BankName = item.Bank,
                BankAccount = item.Account,
                Address = string.IsNullOrEmpty(item.RegAddress) ? item.BankAddress : item.RegAddress,
                Tel = item.CompanyTel,
                ReceipterCompany = item.CompanyName,
                ReceipterName = item.Name,
                ReceipterTel = item.Mobile,
                DetailAddress = item.Address,
                ClientID = clientId,
                IsPersonal = isPersonal,
            });

            var invoiceResultList = invoiceResult.ToList();

            //无发票信息，默认为个人
            if (invoiceResult.Count() < 1 || !isPersonal)
            {
                invoiceResultList.Add(new
                {
                    Value = "个人",
                    Text = "个人",

                    InvoiceType = InvoiceType.None,
                    InvoiceTypeDesc = InvoiceType.None.GetDescription(),
                    DeliveryType = InvoiceDeliveryType.HelpYourself,
                    DeliveryTypeDesc = InvoiceDeliveryType.HelpYourself.GetDescription(),
                    CompanyName = "个人",
                    TaxCode = "",
                    BankName = "",
                    BankAccount = "",
                    Address = "",
                    Tel = "",
                    ReceipterCompany = "",
                    ReceipterName = "",
                    ReceipterTel = "",
                    DetailAddress = "",
                    ClientID = clientId,
                    IsPersonal = true,
                });
            }

            this.Model.InvoiceData = new
            {
                Invoice = invoiceResultList,
            };
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            //商品信息
            var ids = Request.QueryString["IDs"];
            var idArr = ids.Split(',');
            //港币 含税金额， 人民币不含税金额（账单）
            var bills = new BillsAll().Where(b => idArr.Contains(b.ID)).ToArray();
            var billitems = new BillItemsAll().SearchByIDs(idArr).ToMyObject();
            var linq = bills.Select(s => new
            {
                ID = s.ID,
                BillID = s.ID,
                Amount = (billitems.Where(t => t.BillID == s.ID).Sum(t => t.LeftTotalPrice) * 1.06m).ToRound1(),
                ProductName = "*物流辅助服务*服务费",
                // Difference = 0,
                ProductModel = "",
                Quantity = 1,
                //单价不含税
                Price = billitems.Where(t => t.BillID == s.ID).Sum(t => t.LeftTotalPrice).ToRound1(),
                TotalPrice = billitems.Where(t => t.BillID == s.ID).Sum(t => t.LeftTotalPrice).ToRound1(),
                InvoiceTaxRate = 0.06m,
                //含税单价
                UnitPrice = (billitems.Where(t => t.BillID == s.ID).Sum(t => t.LeftTotalPrice) * 1.06m).ToRound1(),
                //税务名称
                TaxName = "*物流辅助服务*服务费",
                //税务编码
                TaxCode = "3040407040000000000"
            });

            return linq;
        }
        /// <summary>
        /// 提交开票申请
        /// </summary>
        protected void Submit()
        {
            #region 界面数据
            try
            {
                //发票信息  邮寄信息
                string invoiceType = Request.Form["InvoiceType"];
                string deliveryType = Request.Form["DeliveryType"];
                string title = Request.Form["Title"];
                string taxNumber = Request.Form["TaxCode"];
                string bankName = Request.Form["BankName"];
                string bankAccount = Request.Form["BankAccount"];
                string address = Request.Form["Address"];
                string tel = Request.Form["Tel"];
                string receipterCompany = Request.Form["ReceipterCompany"];
                string receipterName = Request.Form["ReceipterName"];
                string receipterTel = Request.Form["ReceipterTel"];
                string detailAddress = Request.Form["DetailAddress"];
                string clientid = Request.Form["ClientId"];
                string summary = Request.Form["Summary"];
                string ispersonal = Request.Form["IsPersonal"];
                //产品信息
                var products = Request.Form["products"].Replace("&quot;", "'").Replace("amp;", "");
                var productList = products.JsonTo<InvoiceNoticeItem[]>();
                #endregion

                #region 数据
                InvoiceNotice invoiceNotice = new InvoiceNotice();
                invoiceNotice.ClientID = clientid;
                invoiceNotice.IsPersonal = Convert.ToBoolean(ispersonal);
                invoiceNotice.FromType = Services.Enums.InvoiceFromType.HKStore;
                invoiceNotice.Type = string.IsNullOrEmpty(invoiceType) ? InvoiceType.None : (InvoiceType)int.Parse(invoiceType);
                invoiceNotice.Title = title;
                invoiceNotice.TaxNumber = taxNumber;
                invoiceNotice.RegAddress = address;
                invoiceNotice.Tel = tel;
                invoiceNotice.BankName = bankName;
                invoiceNotice.BankAccount = bankAccount;
                invoiceNotice.PostAddress = detailAddress;
                invoiceNotice.PostRecipient = receipterName;
                invoiceNotice.PostTel = receipterTel;
                if (deliveryType == "0")
                {
                    invoiceNotice.DeliveryType = InvoiceDeliveryType.SendByPost;//未知状态，导出开票文件会失败
                }
                else
                {
                    invoiceNotice.DeliveryType = (InvoiceDeliveryType)int.Parse(deliveryType);
                }
                invoiceNotice.AdminID = Erp.Current.ID;
                invoiceNotice.Summary = summary.Trim();

                invoiceNotice.InvoiceNoticeItem = productList;

                invoiceNotice.Enter();



                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
            }
            #endregion
        }
    }
}