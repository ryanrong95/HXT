using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Invoice
{
    public partial class Confirm : ErpParticlePage
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
            string InvoiceNoticeID = Request.QueryString["InvoiceNoticeID"];

            InvoicingConfirmDataModel theInvoiceNotice = new InvoicingConfirmDataModel();

            using (var query = new Yahv.PvWsOrder.Services.Views.InvoicingListView())
            {
                var view = query;

                view = view.SearchByInvoiceNoticeID(InvoiceNoticeID);

                Func<Yahv.PvWsOrder.Services.Views.InvoicingListViewModel, InvoicingConfirmDataModel> convert = item => new InvoicingConfirmDataModel
                {
                    InvoiceNoticeID = item.InvoiceNoticeID,
                    EnterCode = item.EnterCode,
                    CompanyName = item.CompanyName,
                    InvoiceTypeDes = item.InvoiceType.GetDescription(),
                    Amount = item.Amount,
                    Difference = item.Difference,
                    InvoiceDeliveryTypeInt = Convert.ToString((int)item.InvoiceDeliveryType),
                    InvoiceDeliveryTypeDes = item.InvoiceDeliveryType.GetDescription(),
                    InvoiceNoticeStatusDes = item.InvoiceNoticeStatus.GetDescription(),
                    AdminID = item.AdminID,
                    AdminName = item.AdminName,
                    InvoiceNoticeCreateDateDes = item.InvoiceNoticeCreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    TaxNumber = item.TaxNumber,
                    RegAddress = item.RegAddress,
                    Tel = item.Tel,
                    BankName = item.BankName,
                    BankAccount = item.BankAccount,
                    Summary = item.Summary,
                    PostAddress = item.PostAddress,
                    PostRecipient = item.PostRecipient,
                    PostTel = item.PostTel,
                    WaybillCode = item.WaybillCode,
                };

                var viewData = view.ToMyPage(convert);

                theInvoiceNotice = viewData.Item1.FirstOrDefault();
            }


            //开票信息
            this.Model.InvoiceData = new
            {
                InvoiceType = theInvoiceNotice.InvoiceTypeDes,
                DeliveryType = theInvoiceNotice.InvoiceDeliveryTypeDes,
                CompanyName = theInvoiceNotice.CompanyName,
                TaxCode = theInvoiceNotice.TaxNumber,
                BankInfo = theInvoiceNotice.BankName + " " + theInvoiceNotice.BankAccount,
                AddressTel = theInvoiceNotice.RegAddress + " " + theInvoiceNotice.Tel
            };

            //邮寄信息
            this.Model.MaileDate = new
            {
                ReceipCompany = theInvoiceNotice.CompanyName,
                //ReceiterAndTel = notice.MailName + " " + notice.MailMobile,
                ReceiterName = theInvoiceNotice.PostRecipient,
                ReceiterTel = theInvoiceNotice.PostTel,
                DetailAddres = theInvoiceNotice.PostAddress,
                WaybillCode = theInvoiceNotice.WaybillCode,
            };

            //其它信息
            this.Model.OtherData = new
            {
                Amount = theInvoiceNotice.Amount.ToRound1(2),
                Difference = theInvoiceNotice.Difference,
                Summary = theInvoiceNotice.Summary,
            };
        }

        /// <summary>
        /// 加载产品信息
        /// </summary>
        protected void ProductData()
        {
            string InvoiceNoticeID = Request.QueryString["InvoiceNoticeID"];

            InvoiceDetailDataModel[] allInvoiceItems = new InvoiceDetailDataModel[0];
            using (var query = new Yahv.PvWsOrder.Services.Views.InvoiceDetailView())
            {
                var view = query;

                view = view.SearchByInvoiceNoticeIDs(new string[] { InvoiceNoticeID, });

                Func<Yahv.PvWsOrder.Services.Views.InvoiceDetailViewModel, InvoiceDetailDataModel> convert = item => new InvoiceDetailDataModel
                {
                    InvoiceNoticeItemID = item.InvoiceNoticeItemID,
                    InvoiceNoticeID = item.InvoiceNoticeID,
                    BillID = item.BillID,
                    OrderIDs = item.OrderIDs,
                    DetailAmountDes = item.DetailAmount.ToRound1(2).ToString(),
                    CompanyName = item.CompanyName,
                    InvoiceNo = item.InvoiceNo,
                    InvoiceTimeDes = item.InvoiceTime?.ToString("yyyy/MM/dd"),
                    TaxAmountDes = (item.DetailSalesTotalPrice * (decimal)1.06).ToRound1(2).ToString(), //返回给页面时计算
                    DetailSalesUnitPriceDes = item.DetailSalesUnitPrice.ToString(),
                    DetailSalesTotalPriceDes = item.DetailSalesTotalPrice.ToString(),
                    DifferenceDes = item.Difference != null ? item.Difference.ToString() : Convert.ToString(0),
                    Difference = item.Difference ?? 0,
                    InvoiceNoticeItemCreateDate = item.InvoiceNoticeItemCreateDate,
                    AmountDes = item.Amount.ToString(),
                    Amount = item.Amount,
                    UnitPrice = item.UnitPrice,
                };

                var viewData = view.ToMyPage(convert);

                allInvoiceItems = viewData.Item1;
            }

            var totaldata = new
            {
                Amount = allInvoiceItems.Sum(t => t.Amount).ToRound1(2), //含税金额
                Difference = allInvoiceItems.Sum(t => t.Difference), //开票差额
                Quantity = allInvoiceItems.Length, //数量
                SalesTotalPrice = allInvoiceItems.Sum(t => t.Amount / (decimal)1.06).ToRound1(2), //金额
            };


            //前台显示
            Func<InvoiceDetailDataModel, object> convertToPage = item => new
            {
                ID = item.InvoiceNoticeItemID,
                OrderID = item.OrderIDs,
                ProductName = "*物流辅助服务*服务费",  //产品名称
                //ProductModel = item.OrderItem?.Product.Model,//型号
                ProductModel = "",
                Unit = "",//单位
                Quantity = 1,//数量
                SalesUnitPrice = (item.Amount / (decimal)1.06).ToRound1(2), //单价
                SalesTotalPrice = (item.Amount / (decimal)1.06).ToRound1(2), //金额
                item.UnitPrice, //含税单价
                InvoiceTaxRate = 0.06, //税率
                Amount = item.Amount.ToRound1(2),//含税总额
                //为了与开票软件一致，这里先算出不含税金额，再算出含税金额
                //Amount = (((item.Amount + item.Difference) / (1 + item.InvoiceTaxRate)).ToRound(2)* (1 + item.InvoiceTaxRate)).ToRound(2),
                TaxName = "*物流辅助服务*服务费",//税务名称
                TaxCode = "3040407040000000000",
                item.Difference,
                item.InvoiceNo,
                UnitName = "",
            };

            Response.Write(new { rows = allInvoiceItems.Select(convertToPage).ToArray(), totaldata = totaldata }.Json());
        }

        /// <summary>
        /// 确认开票
        /// </summary>
        protected void ConfirmInvoice()
        {
            try
            {
                string InvoiceNoticeID = Request.Form["InvoiceNoticeID"];
                var changeProductData = Request.Form["Data"].Replace("&quot;", "'");
                var InvoiceModelList = changeProductData.JsonTo<List<Yahv.PvWsOrder.Services.Views.InvoiceSubmitModel>>();
                var count = InvoiceModelList.Where(x => x.InvoiceNo == null).Count();
                if (count > 0)
                {
                    Response.Write((new { success = false, message = "发票号码不允许为空！" }).Json());
                    return;
                }

                Yahv.PvWsOrder.Services.Views.InvoiceDetailView.ConfirmInvoice(InvoiceModelList, InvoiceNoticeID, Erp.Current.ID);

                Response.Write((new { success = true, message = "开票成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "申请失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 更新发票号
        /// </summary>
        protected void UpdateInvoiceNo()
        {
            try
            {
                string InvoiceNoticeID = Request.Form["InvoiceNoticeID"];

                var changeProductData = Request.Form["Data"].Replace("&quot;", "'");
                var InvoiceModelList = changeProductData.JsonTo<List<Yahv.PvWsOrder.Services.Views.InvoiceSubmitModel>>();
                foreach (var item in InvoiceModelList)
                {
                    Yahv.PvWsOrder.Services.Views.InvoiceDetailView.UpdateInvoiceNo(item.InvoiceNo, item.ID);
                }

                Response.Write((new { success = true, message = "更新成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "更新失败：" + ex.Message }).Json());
            }
        }

        public class InvoicingConfirmDataModel
        {
            /// <summary>
            /// 开票编号
            /// </summary>
            public string InvoiceNoticeID { get; set; }

            /// <summary>
            /// 客户编号
            /// </summary>
            public string EnterCode { get; set; }

            /// <summary>
            /// 公司名称
            /// </summary>
            public string CompanyName { get; set; }

            /// <summary>
            /// 开票类型
            /// </summary>
            public string InvoiceTypeDes { get; set; }

            /// <summary>
            /// 含税金额
            /// </summary>
            public decimal Amount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public decimal Difference { get; set; }

            /// <summary>
            /// 发票交付方式 Int
            /// </summary>
            public string InvoiceDeliveryTypeInt { get; set; }

            /// <summary>
            /// 发票交付方式 Des
            /// </summary>
            public string InvoiceDeliveryTypeDes { get; set; }

            /// <summary>
            /// 开票状态
            /// </summary>
            public string InvoiceNoticeStatusDes { get; set; }

            /// <summary>
            /// 申请人ID
            /// </summary>
            public string AdminID { get; set; }

            /// <summary>
            /// 申请人姓名
            /// </summary>
            public string AdminName { get; set; }

            /// <summary>
            /// 申请日期
            /// </summary>
            public string InvoiceNoticeCreateDateDes { get; set; }

            /// <summary>
            /// 企业税号
            /// </summary>
            public string TaxNumber { get; set; }

            /// <summary>
            /// 注册地址
            /// </summary>
            public string RegAddress { get; set; }

            /// <summary>
            /// 电话
            /// </summary>
            public string Tel { get; set; }

            /// <summary>
            /// 开户行
            /// </summary>
            public string BankName { get; set; }

            /// <summary>
            /// 开户行账号
            /// </summary>
            public string BankAccount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Summary { get; set; }

            /// <summary>
            /// 收票地址
            /// </summary>
            public string PostAddress { get; set; }

            /// <summary>
            /// 收票人/公司
            /// </summary>
            public string PostRecipient { get; set; }

            /// <summary>
            /// 电话
            /// </summary>
            public string PostTel { get; set; }

            /// <summary>
            /// 运单号
            /// </summary>
            public string WaybillCode { get; set; }
        }

        public class InvoiceDetailDataModel
        {
            /// <summary>
            /// InvoiceNoticeItemID
            /// </summary>
            public string InvoiceNoticeItemID { get; set; }

            /// <summary>
            /// InvoiceNoticeID
            /// </summary>
            public string InvoiceNoticeID { get; set; }

            /// <summary>
            /// 账单ID
            /// </summary>
            public string BillID { get; set; }

            /// <summary>
            /// 订单号 IDs（逗号间隔）
            /// </summary>
            public string OrderIDs { get; set; }

            /// <summary>
            /// 含税单价
            /// </summary>
            public string DetailUnitPriceDes { get; set; }

            /// <summary>
            /// 含税金额
            /// </summary>
            public string DetailAmountDes { get; set; }

            /// <summary>
            /// 开票公司
            /// </summary>
            public string CompanyName { get; set; }

            /// <summary>
            /// 发票号码
            /// </summary>
            public string InvoiceNo { get; set; }

            /// <summary>
            /// 开票日期
            /// </summary>
            public string InvoiceTimeDes { get; set; }

            /// <summary>
            /// 税额
            /// </summary>
            public string TaxAmountDes { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string DetailSalesUnitPriceDes { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string DetailSalesTotalPriceDes { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string DifferenceDes { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public decimal Difference { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime InvoiceNoticeItemCreateDate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string AmountDes { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public decimal Amount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public decimal UnitPrice { get; set; }
        }

    }
}