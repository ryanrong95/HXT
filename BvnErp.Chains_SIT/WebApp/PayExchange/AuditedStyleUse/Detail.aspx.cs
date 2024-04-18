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

namespace WebApp.PayExchange.AuditedStyleUse
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
            this.Model.PayExchangeApplyData = "".Json();

            string ID = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyPayExchangeApply.
                Where(item => item.ID == ID).FirstOrDefault();
            if (apply != null)
            {
                if (apply.SupplierAddress == null)
                {
                    apply.SupplierAddress = "";
                }
                if (apply.OtherInfo == null)
                {
                    apply.OtherInfo = "";
                }
                if (apply.Summary == null)
                {
                    apply.Summary = "";
                }

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
                    ExpectPayDate = apply.ExpectPayDate == null ? "" : ((DateTime)apply.ExpectPayDate).ToString("yyyy-MM-dd"),
                    OtherInfo = apply.OtherInfo,
                    Summary = apply.Summary,
                    CreateDate = apply.CreateDate.ToString(),
                    SettlemenDate = apply.SettlemenDate.ToString(),
                    Currency = apply.Currency,
                    ClientName = apply.Client.Company.Name,
                    ClientCode = apply.Client.ClientCode,
                    ExchangeRateType = apply.ExchangeRateType.GetDescription(),
                    ExchangeRate = apply.ExchangeRate,
                    Price = ((decimal)apply.TotalAmount).ToRound(4),
                    RmbPrice = ((decimal)(apply.TotalAmount * apply.ExchangeRate)).ToRound(4),//人民币金额
                }.Json();
                var clientAgreement = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements
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
                    WebUrl = FileDirectory.Current.FileServerUrl + "/" + applyFile.Url.ToUrl(),//查看路径
                }.Json();
            }
        }

        protected void data()
        {
            string ID = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyPayExchangeApply
                .Where(item => item.ID == ID).FirstOrDefault();

            Func<PayExchangeApplyItem, object> convert = item => new
            {
                OrderID = item.OrderID,
                Currency = apply.Currency,
                DeclarePrice = item.DeclarePrice,
                PaidPrice = item.PaidExchangeAmount - item.Amount,
                Amount = item.Amount,
                CreateDate = item.CreateDate,
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
            string ID = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyPayExchangeApply
                .Where(item => item.ID == ID).FirstOrDefault();
            var data = apply.PayExchangeApplyFiles.Where(item => item.FileType == Needs.Ccs.Services.Enums.FileType.PIFiles);
            Func<PayExchangeApplyFile, object> convert = item => new
            {
                ID = item.ID,
                FileName = item.FileName,
                FileFormat = item.FileFormat,
                Url = item.Url,    //数据库相对路径
                WebUrl = DateTime.Compare(item.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + item.Url?.ToUrl()
            };
            //this.Paging(data, linq);
            Response.Write(new
            {
                rows = data.Select(convert).ToArray()
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
            applyLogs = applyLogs.OrderBy(t => t.CreateDate);
            Response.Write(new { rows = applyLogs.Select(convert).ToArray(), }.Json());
        }

        protected void TestTableJson()
        {
            List<TestTableModel> listTestTableModel = new List<TestTableModel>();

            listTestTableModel.Add(new TestTableModel()
            {
                OrderID = "WL13420190222001",
                CreateDate = "2019-04-01",
                Currency = "USD",
                DeclarePrice = "9170.19",
                PaidPrice = "100",
                Amount = "9000"
            });
            listTestTableModel.Add(new TestTableModel()
            {
                OrderID = "WL13420190222001",
                CreateDate = "2019-04-01",
                Currency = "USD",
                DeclarePrice = "9170.19",
                PaidPrice = "100",
                Amount = "9000"
            });
            listTestTableModel.Add(new TestTableModel()
            {
                OrderID = "WL13420190222001",
                CreateDate = "2019-04-01",
                Currency = "USD",
                DeclarePrice = "9170.19",
                PaidPrice = "100",
                Amount = "9000"
            });

            Response.Write(listTestTableModel.Json());
        }
    }
}

public class TestTableModel
{
    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderID { get; set; }

    /// <summary>
    /// 申请时间
    /// </summary>
    public string CreateDate { get; set; }

    /// <summary>
    /// 币种
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// 报关总价
    /// </summary>
    public string DeclarePrice { get; set; }

    /// <summary>
    /// 已付汇金额
    /// </summary>
    public string PaidPrice { get; set; }

    /// <summary>
    /// 本次申请金额
    /// </summary>
    public string Amount { get; set; }
}