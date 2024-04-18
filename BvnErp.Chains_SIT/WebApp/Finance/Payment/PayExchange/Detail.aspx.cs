using Needs.Ccs.Services;
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

namespace WebApp.Finance.Payment.PayExchange
{
    public partial class Detail : Uc.PageBase
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
            this.Model.PayExchangeApplyData = "".Json();
            this.Model.ProxyFileData = "".Json();
            this.Model.NoticeData = "".Json();
            this.Model.ProductFeeLimitData = "".Json();

            string ID = Request.QueryString["ApplyID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.AdminPayExchangeApply.
                Where(item => item.ID == ID).FirstOrDefault();
            if (apply != null)
            {
                this.Model.PayExchangeApplyData = new
                {
                    SupplierName = apply.SupplierName,
                    SupplierAddress = apply.SupplierAddress,
                    SupplierEnglishName = apply.SupplierEnglishName,
                    BankName = apply.BankName,
                    BankAddress = apply.BankAddress.Replace("'", "&#39"),
                    BankAccount = apply.BankAccount,
                    SwiftCode = apply.SwiftCode,
                    PaymentType = apply.PaymentType.GetDescription(),
                    ExpectPayDate = apply.ExpectPayDate?.ToString("yyyy-MM-dd"),
                    OtherInfo = apply.OtherInfo,
                    Summary = apply.Summary,
                    CreateDate = apply.CreateDate.ToShortDateString(),
                    Currency = apply.Currency,
                    ClientName = apply.Client.Company.Name,
                    ClientCode = apply.Client.ClientCode,
                    Merchandiser = apply.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ExchangeRateType = apply.ExchangeRateType.GetDescription(),
                    ExchangeRate = apply.ExchangeRate,
                    Price = ((decimal)apply.TotalAmount).ToRound(2),
                    RmbPrice = ((decimal)(apply.TotalAmount * apply.ExchangeRate)).ToRound(2),//人民币金额
                    ABA = apply.ABA ?? "",
                    IBAN = apply.IBAN ?? "",
                    FatherID = apply.FatherID
                }.Json();

                var clientAgreement = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements
                    .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                    .Where(item => item.ClientID == apply.ClientID).FirstOrDefault();
                if (clientAgreement != null)
                {
                    //结算方式
                    var PeriodType = clientAgreement.ProductFeeClause.PeriodType;
                    if (PeriodType == Needs.Ccs.Services.Enums.PeriodType.PrePaid)
                    {
                        this.Model.ProductFeeLimitData = new
                        {
                            PeriodType = PeriodType.GetDescription(),
                            PeriodTypeValue = PeriodType,
                            RemainAdvances = "",
                        }.Json();
                    }
                    else
                    {
                        //客户货款垫款上线
                        var productFeeLimit = clientAgreement.ProductFeeClause.UpperLimit;
                        //客户未付货款总额
                        var unpaidFees = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceipts
                            .Where(item => item.ClientID == apply.ClientID && item.FeeType == Needs.Ccs.Services.Enums.OrderFeeType.Product)
                            .Sum(item => item.Amount * item.Rate);
                        this.Model.ProductFeeLimitData = new
                        {
                            PeriodType = PeriodType.GetDescription(),
                            PeriodTypeValue = PeriodType,
                            RemainAdvances = productFeeLimit - unpaidFees,
                        }.Json();
                    }
                }
            }
            var applyFile = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.PayExchangeApplyFile.
                Where(item => item.PayExchangeApplyID == ID && item.FileType == Needs.Ccs.Services.Enums.FileType.PayExchange).FirstOrDefault();
            if (applyFile != null)
            {
                this.Model.ProxyFileData = new
                {
                    ID = applyFile.ID,
                    FileName = applyFile.FileName,
                    FileFormat = applyFile.FileFormat,
                    Url = applyFile.Url,
                    WebUrl = DateTime.Compare(applyFile.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0 ?
                      FileDirectory.Current.PvDataFileUrl + "/" + applyFile.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + applyFile.Url?.ToUrl(),
                }.Json();
            }

            var Notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.PaymentNotice.Where(item => item.PayExchangeApply.ID == ID).FirstOrDefault();
            if (Notice != null)
            {
                this.Model.NoticeData = new
                {
                    Payer = Notice.Payer?.ByName,
                    Summary = Notice.Summary,
                }.Json();
            }
        }

        protected void data()
        {
            string ID = Request.QueryString["ApplyID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.AdminPayExchangeApply.
                Where(item => item.ID == ID).FirstOrDefault();

            Func<PayExchangeApplyItem, object> convert = item => new
            {
                OrderID = item.OrderID,
                Currency = apply.Currency,
                DeclarePrice = item.DeclarePrice,
                ReceivableAmount = item.ReceivableAmount,
                ReceivedAmount = item.ReceivedAmount,
                PaidPrice = item.PaidExchangeAmount - item.Amount,
                Amount = item.Amount,
                CreateDate = item.CreateDate.ToShortDateString(),
            };

            Response.Write(new
            {
                rows = apply.PayExchangeApplyItems.Select(convert).ToArray()
            }.Json());
        }

        /// <summary>
        /// 加载PI文件
        /// </summary>
        protected void filedata()
        {
            string ID = Request.QueryString["ApplyID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.AdminPayExchangeApply.
                Where(item => item.ID == ID).FirstOrDefault();
            var data = apply.PayExchangeApplyFiles.Where(item => item.FileType == Needs.Ccs.Services.Enums.FileType.PIFiles);
            Func<PayExchangeApplyFile, object> convert = item => new
            {
                ID = item.ID,
                FileName = item.FileName,
                FileFormat = item.FileFormat,
                Url = item.Url,    //数据库相对路径
                                   // WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),//查看路径
                WebUrl = DateTime.Compare(item.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + item.Url?.ToUrl(),
            };
            Response.Write(new
            {
                rows = data.Select(convert).ToArray(),
                total = data.Count()
            }.Json());
        }

        //付汇日志记录
        protected void LoadLogs()
        {
            string ApplyID = Request.Form["ApplyID"];
            var applyLogs = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.PayExchangeLogs.Where(item => item.PayExchangeApplyID == ApplyID);
            Func<PayExchangeLog, object> convert = item => new
            {
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
            applyLogs = applyLogs.OrderByDescending(t => t.CreateDate);
            Response.Write(new { rows = applyLogs.Select(convert).ToArray(), }.Json());
        }

        protected void isCanDelivery()
        {

            var id = Request.Form["ID"];

            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.AdminPayExchangeApply[id];
            string ClientID = apply.ClientID;
            string getOrderId = "";
            bool isOverDuePayment = false;
            UnHangUpCheck unHangUpCheck = new UnHangUpCheck();
            bool isExceedLimit = unHangUpCheck.IsExceedLimit(ClientID);
            isOverDuePayment = unHangUpCheck.isOverDuePayment(ClientID);
            if (isOverDuePayment)
            {
                getOrderId = unHangUpCheck.GetOrderID(ClientID);
            }
            Response.Write((new { success = isExceedLimit, OverDuePayment = isOverDuePayment, GetOrderId = getOrderId }).Json());
        }

        /// <summary>
        /// 拆分的付汇，显示已收款RMB金额
        /// </summary>
        protected void FatherReceipts()
        {

            var fatherID = Request.Form["FatherID"];

            var receiptAmount = new Needs.Ccs.Services.Views.OrderReceivedsView().Where(t => t.FeeSourceID == fatherID).Sum(t => t.Amount);


            Response.Write((new { success = true, Amount = receiptAmount }).Json());
        }

    }
}
