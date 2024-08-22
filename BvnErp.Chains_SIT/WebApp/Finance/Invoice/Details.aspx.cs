using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Invoice
{
    public partial class Details : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNotice[ID];
            //开票信息
            this.Model.InvoiceData = new
            {
                InvoiceType = notice.InvoiceType.GetDescription(),
                DeliveryType = notice.DeliveryType.GetDescription(),
                CompanyName = notice.Client.Company.Name,
                TaxCode = notice.Client.Invoice.TaxCode,
                BankInfo = notice.BankName + " " + notice.BankAccount,
                AddressTel = notice.Address + " " + notice.Tel
            }.Json();
            //邮寄信息
            this.Model.MaileDate = new
            {
                ReceipCompany = notice.Client.Company.Name,
                ReceiterName = notice.MailName,
                ReceiterTel = notice.MailMobile,
                DetailAddres = notice.MailAddress,
                WaybillCode = notice.WaybillCode,
            }.Json();
            //其它信息
            this.Model.OtherData = new
            {
                WaybillCode= notice.WaybillCode,
                Amount = notice.Amount.ToRound(2),
                Difference = notice.Difference,
                Summary = notice.Summary,
            }.Json();
            this.Model.InvoiceLog = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeLog.Where(x => x.InvoiceNoticeID == ID).Json();


        }

        /// <summary>
        /// 开票通知明细
        /// </summary>
        protected void ProductData()
        {
            string id = Request["ID"];
            //1.根据ID获取通知项内容
            var noticeitem = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeItem.Where(x => x.InvoiceNoticeID == id);


            List<InvoiceNoticeItem> listInvoiceNoticeItem = noticeitem.ToList();
            decimal totalQuantity = 0;
            foreach (var item in listInvoiceNoticeItem)
            {
                totalQuantity += item.OrderItem == null ? 1 : item.OrderItem.Quantity;
            }

            var totaldata = new
            {
                Amount = listInvoiceNoticeItem.Sum(t => t.Amount).ToRound(2), //含税金额
                Difference = listInvoiceNoticeItem.Sum(t => t.Difference), //开票差额
                Quantity = totalQuantity, //数量
                SalesTotalPrice = listInvoiceNoticeItem.Sum(t => t.SalesTotalPrice).ToRound(2), //金额
            };


            //前台显示
            Func<InvoiceNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                item.OrderID,
                ProductName = item.OrderItem == null ? "*经纪代理服务*经纪代理" : item.OrderItem?.Category.Name,  //产品名称
                //ProductModel = item.OrderItem?.Product.Model,//型号
                ProductModel = item.OrderItem?.Model,
                item.OrderItem?.Unit,//单位
                Quantity = item.OrderItem == null ? 1 : item.OrderItem.Quantity,//数量
                item.SalesUnitPrice, //单价
                SalesTotalPrice = item.SalesTotalPrice.ToRound(2), //金额
                item.UnitPrice, //含税单价
                item.InvoiceTaxRate, //税率
                Amount=item.Amount.ToRound(2),//含税总额
                //为了与开票软件一致，这里先算出不含税金额，再算出含税金额
                //Amount = (((item.Amount + item.Difference) / (1 + item.InvoiceTaxRate)).ToRound(2) * (1 + item.InvoiceTaxRate)).ToRound(2),
                TaxName = item.OrderItem == null ? "*经纪代理服务*经纪代理" : item.TaxName,//税务名称
                TaxCode = item.OrderItem == null ? "3040407040000000000" : item.TaxCode,
                item.Difference,
                item.InvoiceCode,
                item.InvoiceNo,
                InvoiceDate = item.InvoiceDate?.ToString("yyyy-MM-dd"),
                item.UnitName,
            };

            Response.Write(new { rows = listInvoiceNoticeItem.Select(convert).ToArray(), totaldata = totaldata }.Json());
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        protected void LoadInvoiceLogs()
        {
            string id = Request.Form["ID"];
            var noticeLog = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeLog.Where(x => x.InvoiceNoticeID == id);
            noticeLog = noticeLog.OrderByDescending(x => x.CreateDate);
            Func<InvoiceNoticeLog, object> convert = item => new
            {
                ID = item.ID,
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
            Response.Write(new { rows = noticeLog.Select(convert).ToArray(), }.Json());
        }

        /// <summary>
        /// 发票文件
        /// </summary>
        protected void InvoiceFiles()
        {
            string invoiceNoticeID = Request.QueryString["InvoiceNoticeID"];

            if (!string.IsNullOrEmpty(invoiceNoticeID))
            {
                Needs.Ccs.Services.Views.InvoiceNoticeFileView view = new Needs.Ccs.Services.Views.InvoiceNoticeFileView();
                var files = view.Where(t => t.InvoiceNoticeID == invoiceNoticeID && t.FileType == InvoiceNoticeFileType.Invoice);
                Func<Needs.Ccs.Services.Models.InvoiceNoticeFile, object> convert = file => new
                {
                    file.ID,
                    file.Name,
                    FileType = file.FileType.GetDescription(),
                    file.FileFormat,
                    Url = FileDirectory.Current.PvDataFileUrl + "/" + file.Url.ToUrl(),
                };

                Response.Write(new
                {
                    rows = files.Select(convert).ToList(),
                    total = files.Count()
                }.Json());
            }
            else
            {
                Response.Write(new
                {
                    rows = new List<string>(),
                    total = 0
                }.Json());
            }
        }
    }
}