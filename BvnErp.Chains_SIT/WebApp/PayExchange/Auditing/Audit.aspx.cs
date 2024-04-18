using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
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

namespace WebApp.PayExchange.Auditing
{
    public partial class Audit : Uc.PageBase
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
            else
            {
                this.Model.ID = ID;
            }

            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyUnAuditedPayExchangeApply
                .Where(item => item.ID == ID).FirstOrDefault();

            if (apply != null)
            {
                //取当前实时汇率
                var rate = new Needs.Ccs.Services.Views.RealTimeExchangeRatesView(apply.Currency).ToRate().Rate;

                // 查询此客户是否存在已审批生效的垫资申请  by 2020 - 12 - 23 yess
                var advanceMoneyApply = new Needs.Ccs.Services.Views.AdvanceMoneyApplyView().FirstOrDefault(x => x.ClientID == apply.ClientID && x.Status == AdvanceMoneyStatus.Effective);
                var ReceivableAmount = apply.PayExchangeApplyItems.Sum(t => t.ReceivableAmount);//应收金额
                if (advanceMoneyApply != null)
                {
                    if ((advanceMoneyApply.Amount - advanceMoneyApply.AmountUsed) >= ReceivableAmount)
                    {
                        this.Model.IsadvanceMoney = 0;//有值就是存在垫资申请
                    }
                    else
                    {
                        this.Model.IsadvanceMoney = "";
                    }
                }
                else
                {
                    this.Model.IsadvanceMoney = "";
                }
                //查询可用垫资额度  by  2020-12-29  yess

                var availableProductFeeView = new Needs.Ccs.Services.Views.AvailableProductFeeView(apply.ClientID);
                decimal availableProductFee = availableProductFeeView.GetProductAdvanceMoneyApply();
                this.Model.AvailableProductFee = availableProductFee;

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
                    SettlemenDate = apply.SettlemenDate.ToString(),
                    Currency = apply.Currency,
                    ClientName = apply.Client.Company.Name,
                    ClientCode = apply.Client.ClientCode,
                    Merchandiser = apply.Client.Merchandiser.RealName,
                    ExchangeRateType = apply.ExchangeRateType.GetDescription(),
                    ExchangeRate = rate,
                    Price = ((decimal)apply.TotalAmount).ToRound(2),
                    RmbPrice = ((decimal)(apply.TotalAmount * rate)).ToRound(2),//人民币金额
                    ABA = apply.ABA ?? "",
                    IBAN = apply.IBAN ?? "",

                    BankIsSensitive = isSensitive,
                    BankSensitiveType = bankSensitiveType,
                    FatherID = apply.FatherID,
                    IsadvanceMoney = apply.IsAdvanceMoney,
                    HandlingFeePayerType = apply.HandlingFeePayerType,
                    HandlingFee = apply.HandlingFee,
                    USDRate = apply.USDRate
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
                            .Sum(item => (decimal?)(item.Amount * item.Rate)).GetValueOrDefault();
                        this.Model.ProductFeeLimitData = new
                        {
                            PeriodType = PeriodType.GetDescription(),
                            PeriodTypeValue = PeriodType,
                            RemainAdvances = productFeeLimit - unpaidFees,
                        }.Json();
                    }
                }

                //是否使用十点汇率
                this.Model.IsTen = clientAgreement.IsTen;
            }
            //付汇委托书
            var applyFile = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.PayExchangeApplyFile.
                Where(item => item.PayExchangeApplyID == ID && item.FileType == Needs.Ccs.Services.Enums.FileType.PayExchange).FirstOrDefault();
            if (applyFile != null)
            {
                this.Model.ProxyFileData = new
                {
                    ID = applyFile.ID,
                    FileName = applyFile.FileName,
                    FileFormat = applyFile.FileFormat,
                    Url = applyFile.Url.Replace(@"\", @"\\"),
                    WebUrl = (DateTime.Compare(applyFile.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0)
                    ? FileDirectory.Current.PvDataFileUrl + "/" + applyFile.Url.ToUrl()
                    : FileDirectory.Current.FileServerUrl + "/" + applyFile.Url.ToUrl(),
                    // WebUrl = FileDirectory.Current.FileServerUrl + "/" + applyFile.Url.ToUrl(),//查看路径
                }.Json();
            }
        }



        protected void isCanDelivery()
        {

            var id = Request.Form["ID"];

            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyUnAuditedPayExchangeApply[id];
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
            string ID = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyUnAuditedPayExchangeApply
                .Where(item => item.ID == ID).FirstOrDefault();
            Func<PayExchangeApplyItem, object> convert = item => new
            {
                OrderID = item.OrderID,
                Currency = apply.Currency,
                DeclarePrice = item.DeclarePrice.ToRound(4),
                PaidPrice = (item.PaidExchangeAmount - item.Amount).ToRound(4),
                Amount = item.Amount.ToRound(4),
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
            string ID = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyUnAuditedPayExchangeApply.Where(item => item.ID == ID).FirstOrDefault();
            var data = apply.PayExchangeApplyFiles.Where(item => item.FileType == FileType.PIFiles);
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
                total = data.Count(),
            }.Json());
        }

        /// <summary>
        /// 上传PI图片文件
        /// </summary>
        protected void UploadFiles()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                var TemporaryID = Request.Form["ID"];
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = file.FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);
                            fileList.Add(new
                            {
                                FileName = file.FileName,
                                FileFormat = file.ContentType,
                                WebUrl = fileDic.FileUrl,
                                Url = fileDic.VirtualPath,
                            });
                        }
                    }
                }
                Response.Write((new { success = true, data = fileList }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, data = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传付汇委托书
        /// </summary>
        protected void UploadProxyFile()
        {
            string ID = Request.Form["ID"];
            try
            {
                var file = Request.Files["uploadProxyFile"];

                //处理附件
                if (file.ContentLength != 0)
                {
                    //文件保存
                    string fileName = file.FileName.ReName();

                    //创建文件目录
                    FileDirectory fileDic = new FileDirectory(fileName);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.PayExchange);
                    fileDic.CreateDataDirectory();
                    file.SaveAs(fileDic.FilePath);
                    //付汇委托书
                    PayExchangeApplyFile ProxyFile = new PayExchangeApplyFile()
                    {
                        ID = "",
                        PayExchangeApplyID = ID,
                        FileName = file.FileName,
                        FileFormat = file.ContentType,
                        Url = fileDic.VirtualPath,
                        FileType = FileType.PayExchange,
                        AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                        ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID,
                        CreateDate = DateTime.Now,
                        Status = Status.Normal,
                    };

                    //操作人
                    var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                    var payExchangeApply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyUnAuditedPayExchangeApply[ID];
                    payExchangeApply.ID = ID;
                    payExchangeApply.ProxyFile = ProxyFile;
                    payExchangeApply.SetOperator(admin);
                    payExchangeApply.Admin = admin;
                    payExchangeApply.UploadProxyFile();

                    var data = new
                    {
                        FileName = file.FileName,
                        FileFormat = file.ContentType,
                        WebUrl = fileDic.FileUrl,
                        Url = fileDic.VirtualPath,
                    };
                    Response.Write((new { success = true, data = data }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, data = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出付汇委托书
        /// </summary>
        protected void ExportProxyFile()
        {
            try
            {
                string ID = Request.Form["ID"];
                var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyUnAuditedPayExchangeApply
                    .Where(item => item.ID == ID).FirstOrDefault();

                //创建文件目录
                string fileName = DateTime.Now.Ticks + ".pdf";
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.PayExchange);
                fileDic.CreateDataDirectory();

                apply.PayExchangeAgentProxy.SaveAs(fileDic.FilePath);

                string fileUrl = fileDic.FileUrl;
                Response.Write((new { success = true, message = "导出成功", url = fileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 提交付汇申请
        /// </summary>
        protected void Submit()
        {
            try
            {
                string ID = Request.Form["ID"];
                string FileID = Request.Form["FileID"];
                string FileName = Request.Form["FileName"];
                string FileFormat = Request.Form["FileFormat"];
                string ExchangeRate = Request.Form["ExchangeRate"];
                string FileUrl = Request.Form["fileUrl"];
                int AdvanceMoney = Convert.ToInt32(Request.Form["AdvanceMoney"].ToString());

                if (string.IsNullOrEmpty(ID))
                {
                    Response.Write((new { success = false, message = "提交失败" }).Json());
                    return;
                }
                string fileData = Request.Form["FileData"].Replace("&quot;", "'");
                //Pi文件
                IEnumerable<PayExchangeApplyFile> files = fileData.JsonTo<IEnumerable<PayExchangeApplyFile>>();

                ////付汇委托书
                //PayExchangeApplyFile ProxyFile = new PayExchangeApplyFile()
                //{
                //    ID = FileID,
                //    PayExchangeApplyID = ID,
                //    FileName = FileName,
                //    FileFormat = FileFormat,
                //    Url = FileUrl,
                //    FileType = FileType.PayExchange,
                //    AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                //    ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID,
                //    CreateDate = DateTime.Now,
                //    Status = Status.Normal,
                //};

                //操作人
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var payExchangeApply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyUnAuditedPayExchangeApply[ID];
                payExchangeApply.ID = ID;
                payExchangeApply.PayExchangeApplyFiles = new PayExchangeApplyFiles(files);
                //payExchangeApply.ProxyFile = ProxyFile;
                payExchangeApply.SetOperator(admin);
                payExchangeApply.Admin = admin;
                payExchangeApply.ExchangeRate = decimal.Parse(ExchangeRate);
                payExchangeApply.IsAdvanceMoney = AdvanceMoney;
                payExchangeApply.Audit();

                //NoticeLog noticeLog = new NoticeLog();
                //noticeLog.MainID = ID;
                //noticeLog.OrderID = payExchangeApply.PayExchangeApplyItems.FirstOrDefault().OrderID;
                //noticeLog.NoticeType = SendNoticeType.PayExchangeAudit;
                //noticeLog.Readed = true;
                //noticeLog.SendNotice();

                //noticeLog.NoticeType = SendNoticeType.PayExChangeApprove;
                //noticeLog.Readed = false;               
                //noticeLog.SendNotice();

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                ex.CcsLog("跟单员付汇审核报错");
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        protected void Cancel()
        {
            try
            {
                string ID = Request.Form["ID"];
                if (string.IsNullOrEmpty(ID))
                {
                    Response.Write((new { success = false, message = "取消失败" }).Json());
                    return;
                }
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyUnAuditedPayExchangeApply[ID];
                apply.SetOperator(admin);
                apply.Cancel();

                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = ID;
                noticeLog.OrderID = apply.PayExchangeApplyItems.FirstOrDefault().OrderID;
                noticeLog.NoticeType = SendNoticeType.PayExchangeAudit;
                noticeLog.SendNotice();

                Response.Write((new { success = true, message = "取消成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "取消失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 日志记录
        /// </summary>
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
    }
}
