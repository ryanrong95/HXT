using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Finance.Payment.Notice
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Data();
        }

        protected void Load_Data()
        {
            this.Model.PaymentType = Needs.Utils.Descriptions.EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PaymentType>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal).Select(item => new { Value = item.ID, Text = item.Name }).Json();

            this.Model.FinanceAccountData = "".Json();
            this.Model.Notice = "".Json();
            this.Model.RealExchangeRate = "".Json();

            string ID = Request.QueryString["ID"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyPaymentNotice[ID];

            if (entity != null)
            {
                string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];

                string applyDate = string.Empty;
                string payDate = string.Empty;
                string bankAutoDisplay = string.Empty;
                bool isCash = false;
                if (!string.IsNullOrEmpty(entity.CostApplyID))
                {
                    var costApply = new Needs.Ccs.Services.Views.CostApplyDetailView().GetResult(entity.CostApplyID);
                    if (costApply != null)
                    {
                        applyDate = costApply.CreateDate.ToShortDateString();
                        if (costApply.MoneyType == MoneyTypeEnum.BankAutoApply)
                        {
                            bankAutoDisplay = "这比款项是银行自动扣款的费用，已自动扣款，无需实际打款";
                        }

                        isCash = costApply.CashType == CashTypeEnum.Cash;
                    }

                    payDate = string.Empty;
                }
                else
                {
                    applyDate = entity.PayExchangeApply != null ? entity.PayExchangeApply.CreateDate.ToShortDateString() : string.Empty;
                    payDate = entity.PayDate.ToShortDateString();
                }

                DateTime dt = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);

                PayExchangeApplyFile file = null;
                //付汇委托书
                if (entity.PayExchangeApply != null)
                {
                    file = entity.PayExchangeApply.PayExchangeApplyFiles.Where(t => t.FileType == Needs.Ccs.Services.Enums.FileType.PayExchange).FirstOrDefault();
                    file.Url = (DateTime.Compare(file.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0)
                            ? FileDirectory.Current.PvDataFileUrl + "/" + file.Url.ToUrl()
                            : FileDirectory.Current.FileServerUrl + "/" + file.Url.ToUrl();
                }

                this.Model.Notice = new
                {
                    ID = entity.ID,
                    SeqNo = entity.SeqNo,
                    AdminID = entity.Admin.ID,
                    Payer = entity.Payer,
                    PaymentApply = entity.PaymentApply,
                    PayFeeType = entity.PayFeeType,
                    FeeTypeInt = entity.FeeTypeInt,
                    //PayFeeTypeName = entity.CostApplyID!=null?"":entity.FeeTypeInt > 10000 ? ((FeeTypeEnum)entity.FeeTypeInt).ToString() : ((FeeType)entity.FeeTypeInt).GetDescription(),   
                    PayFeeTypeName = entity.CostApplyID != "" ? "" : ((FinanceFeeType)entity.FeeTypeInt).GetDescription(),
                    FinanceVault = entity.FinanceVault,
                    FinanceAccount = entity.FinanceAccount,

                    Amount = entity.Amount,
                    Currency = entity.Currency,
                    PayDate = payDate,
                    PayType = (int)entity.PayType,
                    ApplyPayType = entity.PayType.GetDescription(),
                    ExchangeRate = entity.ExchangeRate,
                    Status = entity.Status,
                    CostApplyID = entity.CostApplyID,
                    RefundApplyID = entity.RefundApplyID,
                    //收款人
                    PayeeName = entity.PayeeName,
                    BankName = entity.BankName,
                    BankAccount = entity.BankAccount,
                    BankAddress = entity.PayExchangeApply?.BankAddress,
                    SwiftCode = entity.PayExchangeApply != null ? entity.PayExchangeApply.SwiftCode : string.Empty,
                    OtherInfo = entity.PayExchangeApply?.OtherInfo,
                    Summary = entity.PayExchangeApply?.Summary,
                    ApplyDate = applyDate,

                    //手续费及流水号 2020-09-29 by yeshuangshuang
                    Poundage = entity.Poundage,
                    SeqNoPoundage = entity.SeqNoPoundage,

                    //付汇委托书
                    PayExchangeApplyFile = file,
                    //支付凭证
                    PayNoticeFile = new { FileName = entity.File?.FileName, Url = FileServerUrl + @"/" + entity.File?.Url.ToUrl() },
                    BankAutoDisplay = bankAutoDisplay,
                    IsCash = isCash,
                    USDAmount = entity.USDAmount,
                }.Json().Replace("'", "&#39"); //防止前台转json失败

                var RealExchangeRate = Needs.Wl.Admin.Plat.AdminPlat.RealTimeRates.Where(item => item.Code == entity.Currency).FirstOrDefault()?.Rate;
                if (RealExchangeRate != null)
                {
                    this.Model.RealExchangeRate = new
                    {
                        Rate = RealExchangeRate,
                    }.Json();
                }
                if (entity.FinanceVault != null)
                {
                    this.Model.FinanceAccountData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(item => item.FinanceVaultID == entity.FinanceVault.ID)
                    .Select(item => new { Value = item.ID, Text = item.AccountName }).Json();
                }
            }
        }

        protected void getAccounts()
        {
            string VaultID = Request.Form["VaultID"];
            string Currency = Request.Form["Currency"];
            string IsCash = Request.Form["IsCash"];

            var result = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Where(t => t.FinanceVaultID == VaultID && (t.Currency == Currency || t.Currency == "USD"));

            if (!string.IsNullOrEmpty(IsCash) && IsCash == "true")
            {
                result = result.Where(t => t.IsCash == true);
            }
            else
            {
                result = result.Where(t => t.IsCash == false);
            }

            if (result != null)
            {
                Response.Write(result.Select(item => new { Value = item.ID, Text = item.AccountName }).Json());
            }
        }

        protected void SavePaymentNotice()
        {
            try
            {
                var ID = Request.Form["ID"];
                var SeqNo = Request.Form["SeqNo"];
                var ExchangeRate = Request.Form["ExchangeRate"];
                var PayType = Request.Form["PayType"];
                var FinanceVault = Request.Form["FinanceVault"];
                var FinanceAccount = Request.Form["FinanceAccount"];

                string FileID = Request.Form["FileID"];
                string FileName = Request.Form["FileName"];
                string FileFormat = Request.Form["FileFormat"];
                string FileUrl = Request.Form["FileUrl"];

                string FeeTypeInt = Request.Form["FeeTypeInt"];
                string CostApplyID = Request.Form["CostApplyID"];
                string IsCash = Request.Form["IsCash"];

                string Poundage = Request.Form["Poundage"];//手续费
                string SeqNoPoundage = Request.Form["SeqNoPoundage"];//手续费流水号  

                string USDAmount = Request.Form["USDAmount"];//美金金额
                string RefundApplyID = Request.Form["RefundApplyID"];//退款申请ID

                string ActualPayDate = Request.Form["ActualPayDate"];//实际付款日期

                if (string.IsNullOrEmpty(ActualPayDate))
                {
                    Response.Write((new { success = false, message = "请选择实际付款日期" }).Json());
                    return;
                }
                DateTime actualPayTime = Convert.ToDateTime(ActualPayDate);


                var Notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyPaymentNotice[ID];
                if (!string.IsNullOrEmpty(IsCash) && IsCash == "true")
                {
                    Notice.SeqNo = "0";
                }
                else
                {
                    Notice.SeqNo = SeqNo;
                }
                if (!string.IsNullOrEmpty(Poundage))
                {
                    Notice.Poundage = decimal.Parse(Poundage);
                    if (!string.IsNullOrEmpty(SeqNoPoundage))
                    {
                        Notice.SeqNoPoundage = SeqNoPoundage;
                    }
                    else
                    {
                        Notice.SeqNoPoundage = "Pou" + SeqNo;
                    }
                }
                else
                {
                    Notice.Poundage = null;
                    Notice.SeqNoPoundage = null;
                }

                if (!string.IsNullOrEmpty(USDAmount))
                {
                    Notice.USDAmount = decimal.Parse(USDAmount);
                }
                Notice.PayDate = actualPayTime;
                Notice.ExchangeRate = decimal.Parse(ExchangeRate);
                Notice.PayType = (Needs.Ccs.Services.Enums.PaymentType)int.Parse(PayType);
                Notice.FinanceVault = new FinanceVault { ID = FinanceVault };
                Notice.FinanceAccount = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(t => t.ID == FinanceAccount).FirstOrDefault();
                Notice.SetOperator(Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID));
                Notice.Paid();

                if (!string.IsNullOrEmpty(CostApplyID) && CostApplyID != "null")
                {
                    var costApplyApproval = new Needs.Ccs.Services.Models.CostApplyApproval(CostApplyID);
                    costApplyApproval.UpdatePayTime(DateTime.Now);
                }

                if (!string.IsNullOrEmpty(RefundApplyID) && RefundApplyID != "null")
                {
                    var apply = new Needs.Ccs.Services.Views.RefundApplyView().Where(t => t.ID == RefundApplyID).FirstOrDefault();
                    apply.Approve(RefundApplyStatus.Paid);

                    var currentAdmin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                    Logs log = new Logs();
                    log.Name = "退款申请";
                    log.MainID = apply.ID;
                    log.AdminID = apply.Applicant.ID;
                    log.Json = apply.Json();
                    log.Summary = "财务【" + currentAdmin.ByName + "】完成退款申请付款";
                    log.Enter();
                }

                #region 处理附件
                if (string.IsNullOrEmpty(FileName) == false)
                {
                    PaymentNoticeFile NoticeFile = new PaymentNoticeFile()
                    {
                        ID = FileID,
                        PaymentNoticeID = Notice.ID,
                        FileName = FileName,
                        FileFormat = FileFormat,
                        Url = FileUrl,
                        FileType = FileType.PaymentVoucher,
                        AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                        CreateDate = DateTime.Now,
                        Status = Status.Normal,
                    };
                    NoticeFile.Enter();
                }
                #endregion

                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = ID;
                noticeLog.NoticeType = SendNoticeType.PayPayExchange;
                noticeLog.Readed = true;
                noticeLog.SendNotice();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失敗：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传付款凭证
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                var file = Request.Files["uploadFile"];

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

        //付汇日志记录
        protected void LoadLogs()
        {
            string ApplyID = Request.Form["ApplyID"];
            var applyLogs = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.PayExchangeLogs.Where(item => item.PayExchangeApplyID == ApplyID);
            Func<Needs.Ccs.Services.Models.PayExchangeLog, object> convert = item => new
            {
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
            applyLogs = applyLogs.OrderByDescending(t => t.CreateDate);
            Response.Write(new { rows = applyLogs.Select(convert).ToArray(), }.Json());
        }


        protected void CostApplyFiles()
        {
            string CostApplyID = Request.QueryString["CostApplyID"];
            string[] CostApplyIDs = CostApplyID.Split(',');
            if (CostApplyIDs.Length > 1)
            {
                CostApplyID = CostApplyIDs[0];
            }

            var files = new Needs.Ccs.Services.Views.CostApplyFilesView().GetResults(CostApplyID);

            Func<Needs.Ccs.Services.Views.CostApplyFilesViewModel, object> convert = item => new
            {
                CostApplyFileID = item.CostApplyFileID,
                FileName = item.FileName,
                FileFormat = item.FileFormat,
                Url = item.Url,
                WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
            };

            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
            }.Json());
        }

        protected void CostApplyLogs()
        {
            string CostApplyID = Request.Form["CostApplyID"];

            var logs = new Needs.Ccs.Services.Views.CostApplyLogsView().GetResults(CostApplyID);

            Func<Needs.Ccs.Services.Views.CostApplyLogsViewModel, object> convert = item => new
            {
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };

            Response.Write(new { rows = logs.Select(convert).ToArray(), }.Json());
        }

        protected void RefundApplyLogs()
        {
            string RefundApplyID = Request.Form["RefundApplyID"];
            var logs = new Needs.Ccs.Services.Views.LogsView().Where(t => t.MainID == RefundApplyID).OrderByDescending(t => t.CreateDate);

            Func<Needs.Ccs.Services.Models.Logs, object> convert = item => new
            {
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };

            Response.Write(new { rows = logs.Select(convert).ToArray(), }.Json());
        }

        /// <summary>
        /// 加载申请项
        /// </summary>
        /// <returns></returns>
        protected void Feedata()
        {
            var id = Request.QueryString["CostApplyID"];

            var view = new Needs.Ccs.Services.Views.CostApplyItemsView().Where(t => t.CostApplyID == id).ToArray();

            Response.Write(new
            {
                rows = view.Select(
                        item => new
                        {
                            item.ID,
                            FeeName = item.FeeType.GetDescription().Replace("付款-", ""),
                            Price = item.Amount,
                            FeeDesc = item.FeeDesc,
                        }
                    ).ToArray(),
                total = view.Count(),
            }.Json());



        }

        protected void SavePoundage()
        {
            try
            {
                string ID = Request.Form["ID"];
                string Poundage = Request.Form["Poundage"];
                string SeqNoPoundage = Request.Form["SeqNoPoundage"];

                var Notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyPaymentNotice[ID];
                if (!string.IsNullOrEmpty(Poundage))
                {
                    string oldSeqPoun = Notice.SeqNoPoundage;
                    decimal newPoundage = decimal.Parse(Poundage);
                    decimal diffPoundage = Notice.Poundage == null ? newPoundage : newPoundage - Notice.Poundage.Value;

                    Notice.Poundage = newPoundage;
                    Notice.SeqNoPoundage = SeqNoPoundage;

                    Notice.UpdatePoundage(diffPoundage, oldSeqPoun);
                }
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.ToString() }).Json());
            }

        }
    }
}
