using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Linq;
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
    public partial class Approval : Uc.PageBase
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
            //付款人
            this.Model.PayerData = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles
                .Where(manager => manager.Role.Name == "集团财务出纳").Select(item => new { item.Admin.ID, item.Admin.ByName }).Json();

            this.Model.PayExchangeApplyData = "".Json();
            this.Model.ProxyFileData = "".Json();
            this.Model.ProductFeeLimitData = "".Json();
            this.Model.AvailableProductFee = "".Json();

            this.Model.CurrentName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;

            string ID = Request.QueryString["ApplyID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.UnApprovalPayExchangeApply
                .Where(item => item.ID == ID).FirstOrDefault();
            if (apply != null)
            {
                //#region 银行禁止/敏感

                //string supplierBankAddress = apply.BankAddress.Replace("'", "&#39");
                //var splittedStrs = Split(supplierBankAddress);

                //splittedStrs = splittedStrs.Distinct().ToArray();
                //splittedStrs = splittedStrs.Where(t => t.Length != 1).ToArray();

                //int pageSize = 1000;
                //int pageCount = (splittedStrs.Length / pageSize) + (splittedStrs.Length % pageSize > 0 ? 1 : 0);

                //bool isSensitive = false;
                //int bankSensitiveType = 0;

                //for (int i = 1; i <= pageCount; i++)
                //{
                //    var theSplittedStrs = splittedStrs.Skip(pageSize * (i - 1)).Take(pageSize);

                //    var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Views.PayExchangeSensitiveWordCheckModel>();
                //    predicate = predicate.And(t => t.WordContent == "qwertyuiopasdfg"); //这里是用来使得下面的 Or 生效的

                //    foreach (var splittedStr in theSplittedStrs)
                //    {
                //        predicate = predicate.Or(t => t.WordContent == splittedStr);
                //    }

                //    var payExchangeSensitiveWordCheckView = new Needs.Ccs.Services.Views.PayExchangeSensitiveWordCheckView(Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.All);
                //    payExchangeSensitiveWordCheckView.AllowPaging = false;
                //    payExchangeSensitiveWordCheckView.Predicate = predicate;
                //    var oneSensitiveArea = payExchangeSensitiveWordCheckView.FirstOrDefault();

                //    if (oneSensitiveArea == null)
                //    {
                //        isSensitive = false;
                //    }
                //    else
                //    {
                //        isSensitive = true;
                //        bankSensitiveType = oneSensitiveArea.AreaType.GetHashCode();
                //        break;
                //    }
                //}

                //#endregion
                //查出货款可用垫款
                var availableProductFeeView = new Needs.Ccs.Services.Views.AvailableProductFeeView(apply.Client.ID);
                // decimal availableProductFee = (availableProductFeeView.GetProductUpperLimit() - availableProductFeeView.GetProductPayable()).ToRound(2);
                decimal availableProductFee = availableProductFeeView.GetProductAdvanceMoneyApply();// - availableProductFeeView.GetProductPayable()).ToRound(2);  //by yess 2020-12-29
                this.Model.AvailableProductFee = availableProductFee;

                this.Model.PayExchangeApplyData = new
                {
                    SupplierName = apply.SupplierName.Replace("'", "&#39"),
                    SupplierAddress = apply.SupplierAddress,
                    SupplierEnglishName = apply.SupplierEnglishName.Replace("'", "&#39"),
                    BankName = apply.BankName,
                    BankAddress = apply.BankAddress.Replace("'", "&#39"),
                    BankAccount = apply.BankAccount,
                    SwiftCode = apply.SwiftCode,
                    PaymentType = apply.PaymentType.GetDescription(),
                    ExpectPayDate = apply.ExpectPayDate?.ToString("yyyy-MM-dd"),
                    OtherInfo = apply.OtherInfo,
                    Summary = apply.Summary,
                    CreateDate = apply.CreateDate.ToString(),
                    //SettlemenDate = apply.SettlemenDate.ToString(),
                    Currency = apply.Currency,
                    ClientID = apply.Client.ID,
                    ClientName = apply.Client.Company.Name,
                    ClientCode = apply.Client.ClientCode,
                    Merchandiser = apply.Client.Merchandiser.RealName,
                    ExchangeRateType = apply.ExchangeRateType.GetDescription(),
                    ExchangeRate = apply.ExchangeRate,
                    Price = ((decimal)apply.TotalAmount).ToRound(2),
                    RmbPrice = ((decimal)(apply.TotalAmount * apply.ExchangeRate)).ToRound(2),//人民币金额
                    ABA = apply.ABA ?? "",
                    IBAN = apply.IBAN ?? "",
                    IsAdvanceMoney = apply.IsAdvanceMoney,
                    FatherID = apply.FatherID
                    //BankIsSensitive = isSensitive,
                    //BankSensitiveType = bankSensitiveType,
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
                    // WebUrl = FileDirectory.Current.FileServerUrl + "/" + applyFile.Url.ToUrl(),//查看路径
                    WebUrl = (DateTime.Compare(applyFile.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0)
                    ? FileDirectory.Current.PvDataFileUrl + "/" + applyFile.Url.ToUrl()
                    : FileDirectory.Current.FileServerUrl + "/" + applyFile.Url.ToUrl(),
                }.Json();
            }
        }

        #region 切割字符串

        static string[] Split(string origin)
        {
            List<string> resultStrs = new List<string>();

            for (int i = 1; i <= origin.Length; i++)
            {
                var sdfsf = Split(origin, i);
                resultStrs.AddRange(sdfsf);
            }

            return resultStrs.ToArray();
        }

        static string[] Split(string origin, int len)
        {
            List<string> resultStrs = new List<string>();

            for (int i = 0; i <= origin.Length - len; i++)
            {
                resultStrs.Add(origin.Substring(i, len));
            }

            return resultStrs.ToArray();
        }

        #endregion

        protected void data()
        {
            string ID = Request.QueryString["ApplyID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.UnApprovalPayExchangeApply
                .Where(item => item.ID == ID).FirstOrDefault();
            Func<PayExchangeApplyItem, object> convert = item => new
            {
                OrderID = item.OrderID,
                Currency = apply.Currency,
                DeclarePrice = item.DeclarePrice,
                ReceivableAmount = item.ReceivableAmount,
                ReceivedAmount = item.ReceivedAmount,
                DyjIDs = item.DyjIDs,
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
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.UnApprovalPayExchangeApply
                .Where(item => item.ID == ID).FirstOrDefault();
            var data = apply.PayExchangeApplyFiles.Where(item => item.FileType == Needs.Ccs.Services.Enums.FileType.PIFiles);
            Func<PayExchangeApplyFile, object> convert = item => new
            {
                ID = item.ID,
                FileName = item.FileName,
                FileFormat = item.FileFormat,
                Url = item.Url,    //数据库相对路径
                //WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),//查看路径
                WebUrl = (DateTime.Compare(item.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0)
                    ? FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl()
                    : FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
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

        /// <summary>
        /// 检查银行地址是否有敏感地区
        /// </summary>
        protected void CheckIsSensitive()
        {
            string ApplyID = Request.QueryString["ApplyID"];

            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.UnApprovalPayExchangeApply
                .Where(item => item.ID == ID).FirstOrDefault();

            if (apply == null)
            {
                Response.Write((new { success = true, BankIsSensitive = false, BankSensitiveType = 0, }).Json());
                return;
            }

            #region 银行禁止/敏感

            string supplierBankAddress = apply.BankAddress.Replace("'", "&#39");
            var splittedStrs = Split(supplierBankAddress);

            splittedStrs = splittedStrs.Distinct().ToArray();
            splittedStrs = splittedStrs.Where(t => t.Length != 1).ToArray();

            int pageSize = 1000;
            int pageCount = (splittedStrs.Length / pageSize) + (splittedStrs.Length % pageSize > 0 ? 1 : 0);

            bool isSensitive = false;
            int bankSensitiveType = 0;

            for (int i = 1; i <= pageCount; i++)
            {
                var theSplittedStrs = splittedStrs.Skip(pageSize * (i - 1)).Take(pageSize);

                var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Views.PayExchangeSensitiveWordCheckModel>();
                predicate = predicate.And(t => t.WordContent == "qwertyuiopasdfg"); //这里是用来使得下面的 Or 生效的

                foreach (var splittedStr in theSplittedStrs)
                {
                    predicate = predicate.Or(t => t.WordContent == splittedStr);
                }

                var payExchangeSensitiveWordCheckView = new Needs.Ccs.Services.Views.PayExchangeSensitiveWordCheckView(Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.All);
                payExchangeSensitiveWordCheckView.AllowPaging = false;
                payExchangeSensitiveWordCheckView.Predicate = predicate;
                var oneSensitiveArea = payExchangeSensitiveWordCheckView.FirstOrDefault();

                if (oneSensitiveArea == null)
                {
                    isSensitive = false;
                }
                else
                {
                    isSensitive = true;
                    bankSensitiveType = oneSensitiveArea.AreaType.GetHashCode();
                    break;
                }
            }

            #endregion

            Response.Write((new { success = true, BankIsSensitive = isSensitive, BankSensitiveType = bankSensitiveType, }).Json());

        }


        protected void isCanDelivery()
        {

            var id = Request.Form["ID"];

            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.UnApprovalPayExchangeApply[id];
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

            var receiptAmount = new Needs.Ccs.Services.Views.OrderReceivedsView().Where(t => t.FeeSourceID == fatherID).Sum(t=> t.Amount);


            Response.Write((new { Amount = receiptAmount}).Json());
        }
    }
}
