using Needs.Utils;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CostApiHandler
    {
        private string _billNo { get; set; } = string.Empty;

        private string _batchID { get; set; } = string.Empty;

        public CostApiHandler(string billNo, string batchID)
        {
            this._billNo = billNo;
            this._batchID = batchID;
        }

        /// <summary>
        /// 总公司审批不通过，被调用接口，更新状态
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool HeadOfficeApproveRefuse(string summary, out string errMsg)
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    //从 CostApply 表中根据单据号 billNo 查出 CostApply
                    var costApply = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplies>()
                        .Where(t => t.BillNo == this._billNo
                                 && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
                    if (costApply == null)
                    {
                        string err = "不存在单据号为 " + this._billNo + " 的费用申请";
                        new Needs.Ccs.Services.Models.CostApplyApiLog()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            BatchID = _batchID,
                            Status = Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = err,
                        }.Enter();

                        errMsg = err;
                        return false;
                    }

                    //更新 CostApply 状态为 "已经取消"
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplies>(new
                    {
                        CostStatus = (int)Enums.CostStatusEnum.Cancel,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == costApply.ID);

                    //新增一条 CostApplyLog 日志
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.CostApplyLogs>(new Layer.Data.Sqls.ScCustoms.CostApplyLogs
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        CostApplyID = costApply.ID,
                        CurrentCostStatus = (int)costApply.CostStatus,
                        NextCostStatus = (int)Enums.CostStatusEnum.Cancel,
                        CreateDate = DateTime.Now,
                        Summary = "总公司拒绝了费用申请." + summary,
                    });

                    //将对应的付款通知 PaymentNotice 更新为 PaymentNoticeStatus.Canceled "已取消"状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentNotices>(new
                    {
                        Status = (int)Enums.PaymentNoticeStatus.Canceled,
                        UpdateDate = DateTime.Now,
                    }, item => item.CostApplyID == costApply.ID && item.Status == (int)Enums.PaymentNoticeStatus.UnPay);
                }

                errMsg = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                new Needs.Ccs.Services.Models.CostApplyApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = _batchID,
                    Status = Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = ex.Message,
                }.Enter();

                errMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 总公司审批通过后，付款成功，被调用接口，更新状态，并获取付款凭证
        /// </summary>
        /// <returns></returns>
        public bool PaymentSuccessNotice(string seqNo, string bankAccount, string paymentVoucherUrl, out string errMsg)
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var costApply = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplies>().Where(t => t.BillNo == this._billNo).FirstOrDefault();
                    if (costApply == null)
                    {
                        string err = "不存在单据号为 " + this._billNo + " 的费用申请";
                        new Needs.Ccs.Services.Models.CostApplyApiLog()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            BatchID = _batchID,
                            Status = Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = err,
                        }.Enter();

                        errMsg = err;
                        return false;
                    }

                    //根据 CostApplyID 从 PaymentNotice 表中查出 PaymentNoticeID
                    var paymentNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>()
                        .Where(t => t.CostApplyID == costApply.ID && t.Status == (int)Enums.PaymentNoticeStatus.UnPay).FirstOrDefault();

                    var Notice = new Views.PaymentNoticesView()[paymentNotice.ID];  //Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyPaymentNotice[ID];
                    Notice.SeqNo = seqNo;
                    //Notice.ExchangeRate = decimal.Parse(ExchangeRate);
                    //Notice.PayType = (Needs.Ccs.Services.Enums.PaymentType)int.Parse(PayType);
                    //Notice.FinanceVault = new FinanceVault { ID = FinanceVault };
                    //Notice.FinanceAccount = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts.Where(t => t.ID == FinanceAccount).FirstOrDefault();
                    var payer = Needs.Underly.FkoFactory<Admin>.Create(paymentNotice.PayerID);
                    Notice.SetOperator(payer);

                    //账户、金库
                    var financeAccount = new Needs.Ccs.Services.Views.FinanceAccountsView()
                        .Where(t => t.BankAccount == bankAccount && t.Status == Enums.Status.Normal).FirstOrDefault();
                    if (financeAccount == null)
                    {
                        string err = "不存在账户 " + bankAccount;
                        new Needs.Ccs.Services.Models.CostApplyApiLog()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            BatchID = _batchID,
                            Status = Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = err,
                        }.Enter();

                        errMsg = err;
                        return false;
                    }

                    Notice.FinanceVault = new FinanceVault { ID = financeAccount.FinanceVaultID };
                    Notice.FinanceAccount = financeAccount;

                    Notice.Paid();

                    var costApplyApproval = new Needs.Ccs.Services.Models.CostApplyApproval(costApply.ID);
                    costApplyApproval.UpdatePayTime(DateTime.Now);

                    //后台下载付款凭证
                    DownloadPaymentVoucherBackground(paymentNotice.ID, paymentVoucherUrl, payer);
                }

                errMsg = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                new Needs.Ccs.Services.Models.CostApplyApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = _batchID,
                    Status = Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = ex.Message,
                }.Enter();

                errMsg = ex.Message;
                return false;
            }
        }

        //后台下载付款凭证
        public void DownloadPaymentVoucherBackground(string paymentNoticeID, string paymentVoucherUrl, Admin payer)
        {
            try
            {
                string originFileName = string.Empty;
                string[] urlArrs = paymentVoucherUrl.Split('/');
                if (urlArrs == null || urlArrs.Length <= 0)
                {
                    new Needs.Ccs.Services.Models.CostApplyApiLog()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        BatchID = _batchID,
                        Status = Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = "无法从url中获取到文件名",
                    }.Enter();

                    return;
                }
                else
                {
                    originFileName = urlArrs[urlArrs.Length - 1];
                }

                string fileFormat = string.Empty;
                if (!string.IsNullOrEmpty(originFileName))
                {
                    string[] arr = originFileName.Split('.');
                    if (arr != null && arr.Length > 0)
                    {
                        fileFormat = arr[arr.Length - 1];
                    }
                }

                Task.Run(() =>
                {
                    string fileName = originFileName.ReName();
                    string webAppFileSavePath = ConfigurationManager.AppSettings["WebAppFileSavePath"];

                    //创建文件夹
                    string dataPath = Needs.Ccs.Services.SysConfig.PayExchange + @"\" + DateTime.Now.ToString("yyyyMM") + @"\" + DateTime.Now.Day.ToString().PadLeft(2, '0') + @"\";
                    string filePath = webAppFileSavePath + @"\" + dataPath + fileName;
                    string virtualPath = dataPath + fileName;

                    System.IO.FileInfo last = new System.IO.FileInfo(filePath);
                    //确保所在文件夹存在
                    if (!last.Directory.Exists)
                    {
                        last.Directory.Create();
                    }

                    System.Net.WebClient wbClient = new System.Net.WebClient();
                    wbClient.DownloadFile(paymentVoucherUrl, filePath);

                    PaymentNoticeFile NoticeFile = new PaymentNoticeFile()
                    {
                        PaymentNoticeID = paymentNoticeID,
                        FileName = originFileName,
                        FileFormat = fileFormat,
                        Url = virtualPath,
                        FileType = Enums.FileType.PaymentVoucher,
                        AdminID = payer.ID,
                        CreateDate = DateTime.Now,
                        Status = Enums.Status.Normal,
                    };
                    NoticeFile.Enter();
                });
            }
            catch (Exception ex)
            {
                new Needs.Ccs.Services.Models.CostApplyApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = _batchID,
                    Status = Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = ex.Message,
                }.Enter();
            }
        }

    }
}
