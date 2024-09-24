using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using Needs.Wl.Models;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 发票运单
    /// </summary>
    public class SwapNotice : IUnique, IFulError, IFulSuccess
    {
        Purchaser purchaser = PurchaserContext.Current;

        #region 数据库属性

        public string ID { get; set; }

        public Admin Admin { get; set; }

        /// <summary>
        /// 换汇银行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 境外发货人
        /// </summary>
        public string ConsignorCode { get; set; }
        /// <summary>
        /// 换汇总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 换汇汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 换汇所用RMB
        /// </summary>
        public decimal TotalAmountCNY { get; set; }

        public Enums.SwapStatus SwapStatus { get; set; }

        public int SwapStatusInt { get; set; }
        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 换出账户(RMB)
        /// </summary>
        public FinanceAccount OutAccount { get; set; }

        /// <summary>
        /// 中间账户
        /// </summary>
        public FinanceAccount MidAccount { get; set; }

        /// <summary>
        /// 换人账户
        /// </summary>
        public FinanceAccount InAccount { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Poundage { get; set; }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal RealTimeExchangeRate { get; set; }

        #endregion

        /// <summary>
        /// 换出金库
        /// </summary>
        public FinanceVault OutVault { get; set; }

        /// <summary>
        /// 中间金库
        /// </summary>
        public FinanceVault MidVault { get; set; }

        /// <summary>
        /// 换人金库
        /// </summary>
        public FinanceVault InVault { get; set; }

        /// <summary>
        /// 换出账户付款流水号
        /// </summary>
        public string SeqNoOut { get; set; }
        /// <summary>
        /// 中间账户收款流水号
        /// </summary>
        public string SeqNoMidR { get; set; }
        /// <summary>
        /// 中间账户付款流水号
        /// </summary>
        public string SeqNoMidP { get; set; }
        /// <summary>
        /// 换入账户收款流水号
        /// </summary>
        public string SeqNoIn { get; set; }

        /// <summary>
        /// 手续费流水号
        /// </summary>
        public string SeqNoPoundage { get; set; }

        /// <summary>
        /// 合同日期
        /// </summary>
        public string ContrDate { get; set; }

        /// <summary>
        /// 发票日期
        /// </summary>
        public string InvoiceDate { get; set; }

        private string documentNo;
        /// <summary>
        /// 换汇编号
        /// </summary>
        public string DocumentNo
        {
            get
            {
                if (string.IsNullOrEmpty(documentNo))
                {
                    var list = new List<string>();
                    //var ht = purchaser.ContractNoPrefix.Length;
                    //this.Items.ToList().OrderBy(t => t.SwapDecHead.ContrNo.Substring(ht, ht + 7)).ToList().ForEach(t =>
                    //{
                    //    var no = t.SwapDecHead.ContrNo.Substring(ht, ht + 7).Replace("-", string.Empty);
                    //    if (!list.Contains(no))
                    //    {
                    //        list.Add(no);
                    //    }
                    //});

                    this.Items.ToList().OrderBy(t => Regex.Replace(t.SwapDecHead.ContrNo, "[A-Z]", "", RegexOptions.IgnoreCase).Substring(0, 9)).ToList().ForEach(t =>
                      {
                          var no = Regex.Replace(t.SwapDecHead.ContrNo, "[A-Z]", "", RegexOptions.IgnoreCase).Substring(0, 9).Replace("-", string.Empty);
                          if (!list.Contains(no))
                          {
                              list.Add(no);
                          }
                      });

                    //设置日期
                    this.ContrDate = DateTime.ParseExact(list.First(), "yyyyMMdd", new System.Globalization.CultureInfo("zh-CN"), System.Globalization.DateTimeStyles.AllowWhiteSpaces).AddDays(-1).ToString("yyyy-MM-dd");
                    this.InvoiceDate = DateTime.ParseExact(list.Last(), "yyyyMMdd", new System.Globalization.CultureInfo("zh-CN"), System.Globalization.DateTimeStyles.AllowWhiteSpaces).AddDays(1).ToString("yyyy-MM-dd");

                    if (list.Count > 1)
                    {
                        //多天报关单
                        this.documentNo = "PS" + list.First() + "-" + list.Last().Substring(6);
                    }
                    else
                    {
                        //单天报关单
                        this.documentNo = "PS" + list.FirstOrDefault();
                    }
                }
                return this.documentNo;
            }
        }

        private Admin Operator { get; set; }

        public void SetOperator(Admin admin)
        {
            this.Operator = admin;
        }

        public IEnumerable<SwapNoticeItem> Items { get; set; }
        public string uid { get; set; }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public event SwapNoticeCompletedHanlder Completed;       

        private void SwapNotice_Completed(object sender, SwapNoticeCompletedEventArgs e)
        {
            var notice = e.SwapNotice;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //换出账户数据维护
                FinancePayment payOut = new FinancePayment();
                payOut.SeqNo = notice.SeqNoOut;
                if (notice.MidAccount != null)
                {
                    payOut.PayeeName = notice.MidAccount.AccountName;
                    payOut.BankName = notice.MidAccount.BankName;
                    payOut.BankAccount = notice.MidAccount.BankAccount;
                }
                else
                {
                    payOut.PayeeName = notice.InAccount.AccountName;
                    payOut.BankName = notice.InAccount.BankName;
                    payOut.BankAccount = notice.InAccount.BankAccount;
                }
                payOut.Payer = notice.Operator;
                //payOut.PayFeeType = FinanceFeeType.Product;//相同公司是资金调拨，不同公司是货款  2020-09-28 by yeshuangshuang
                payOut.PayFeeType = FinanceFeeType.FundTransfer;
                payOut.FinanceVault = notice.OutVault;
                payOut.FinanceAccount = notice.OutAccount;
                payOut.Amount = notice.TotalAmountCNY;
                payOut.Currency = notice.OutAccount.Currency;
                payOut.ExchangeRate = 1.0M;
                payOut.PayType = PaymentType.TransferAccount;
                payOut.PayDate = DateTime.Now;
                payOut.Enter();

                notice.OutAccount.Balance -= payOut.Amount;
                //换汇手续费
                if (notice.Poundage != 0)
                {
                    FinancePayment Poundage = new FinancePayment();
                    Poundage.SeqNo = notice.SeqNoPoundage;
                    Poundage.PayeeName = notice.OutAccount.AccountName;
                    Poundage.Payer = notice.Operator;
                    Poundage.PayFeeType = FinanceFeeType.Poundage;
                    Poundage.FinanceVault = notice.OutVault;
                    Poundage.FinanceAccount = notice.OutAccount;
                    Poundage.BankName = notice.OutAccount.BankName;
                    Poundage.BankAccount = notice.OutAccount.BankAccount;
                    Poundage.Amount = notice.Poundage;
                    Poundage.Currency = notice.OutAccount.Currency;
                    Poundage.ExchangeRate = 1.0M;
                    Poundage.PayType = PaymentType.TransferAccount;
                    Poundage.PayDate = DateTime.Now;
                    Poundage.Enter();
                }

                //中间账户数据维护
                if (notice.MidVault != null && notice.MidAccount != null)
                {
                    FinanceReceipt recMid = new FinanceReceipt();
                    recMid.SeqNo = notice.SeqNoMidR;
                    recMid.Payer = notice.OutAccount.AccountName;
                    recMid.FeeType = FinanceFeeType.FundTransfer;
                    //recMid.FeeType = FinanceFeeType.Product;//相同公司是资金调拨，不同公司是货款  2020-09-28 by yeshuangshuang
                    recMid.ReceiptType = PaymentType.TransferAccount;
                    recMid.ReceiptDate = DateTime.Now;
                    recMid.Currency = notice.MidAccount.Currency;
                    recMid.Rate = notice.ExchangeRate;
                    recMid.Amount = notice.TotalAmount;
                    recMid.Vault = notice.MidVault;
                    recMid.Account = notice.MidAccount;
                    recMid.Admin = notice.Operator;
                    recMid.Enter();
                    notice.MidAccount.Balance += recMid.Amount;

                    FinancePayment payMid = new FinancePayment();
                    payMid.SeqNo = notice.SeqNoMidP; 
                    payMid.PayeeName = notice.InAccount.AccountName;
                    payMid.Payer = notice.Operator;
                    //payMid.PayFeeType = FinanceFeeType.FundTransfer;//相同公司是资金调拨，不同公司是货款  2020-09-28 by yeshuangshuang
                    payMid.PayFeeType = FinanceFeeType.Product;
                    payMid.FinanceVault = notice.MidVault;
                    payMid.FinanceAccount = notice.MidAccount;
                    payMid.BankName = notice.InAccount.BankName;
                    payMid.BankAccount = notice.InAccount.BankAccount;
                    payMid.Amount = notice.TotalAmount;
                    payMid.Currency = notice.MidAccount.Currency; 
                    payMid.ExchangeRate = notice.ExchangeRate;
                    payMid.PayType = PaymentType.TransferAccount;
                    payMid.PayDate = DateTime.Now;
                    payMid.Enter();
                }
                //换人账户数据维护
                FinanceReceipt recIn = new FinanceReceipt();
                recIn.SeqNo = notice.SeqNoIn;
                if (notice.MidAccount != null)
                {
                    recIn.Payer = notice.MidAccount.AccountName;
                }
                else
                {
                    recIn.Payer = notice.OutAccount.AccountName;
                }
                recIn.FeeType = FinanceFeeType.Product;
                recIn.ReceiptType = PaymentType.TransferAccount;
                recIn.ReceiptDate = DateTime.Now;
                recIn.Currency = notice.InAccount.Currency;
                recIn.Rate = notice.ExchangeRate;
                recIn.Amount = notice.TotalAmount;
                recIn.Vault = notice.InVault;
                recIn.Account = notice.InAccount;
                recIn.Admin = notice.Operator;
                recIn.Enter();

                ////改变报关单换汇状态
                //foreach (var item in notice.Items)
                //{
                //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(
                //    new
                //    {
                //        SwapStatus = (int)Enums.SwapStatus.Audited,
                //    }, t => t.ID == item.SwapDecHead.ID);
                //}
            }

            //改变报关单换汇状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                string[] decHeadIDs = notice.Items.Select(t => t.SwapDecHead.ID).ToArray();

                //var decHeadSwapedAmounts = (from swapNoticeItem in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                //                            where decHeadIDs.Contains(swapNoticeItem.DecHeadID) && swapNoticeItem.Status == (int)Enums.Status.Normal
                //                            group swapNoticeItem by new { swapNoticeItem.DecHeadID, } into g
                //                            select new
                //                            {
                //                                DecHeadID = g.Key.DecHeadID,
                //                                SwapedAmount = g.Sum(t => t.Amount),
                //                            }).ToList();

                var decHeadSwapedAmounts = (from swapNoticeItem in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                                                                    .Where(t => decHeadIDs.Contains(t.DecHeadID) && t.Status == (int)Enums.Status.Normal)
                                            join swapNotice in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>()
                                                on new
                                                {
                                                    SwapNoticeID = swapNoticeItem.SwapNoticeID,
                                                    SwapNoticeStatus = (int)Enums.SwapStatus.Audited,
                                                }
                                                equals new
                                                {
                                                    SwapNoticeID = swapNotice.ID,
                                                    SwapNoticeStatus = swapNotice.Status,
                                                }
                                            group swapNoticeItem by new { swapNoticeItem.DecHeadID, } into g
                                            select new
                                            {
                                                DecHeadID = g.Key.DecHeadID,
                                                SwapedAmount = g.Sum(t => t.Amount),
                                            }).ToList();

                foreach (var item in notice.Items)
                {
                    string decHeadID = item.SwapDecHead.ID;
                    decimal totalSwapAmount = item.SwapDecHead.Lists.Sum(t => t.DeclTotal);

                    var oneSwapedAmount = decHeadSwapedAmounts.Where(t => t.DecHeadID == decHeadID).FirstOrDefault();
                    if (oneSwapedAmount != null && totalSwapAmount <= oneSwapedAmount.SwapedAmount)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new
                        {
                            SwapStatus = (int)Enums.SwapStatus.Audited,
                        }, t => t.ID == decHeadID);

                    }
                    
                }
            }
         
            //通给中心
            CenterFinancePayment centerFee = new CenterFinancePayment(notice);

            SendStrcut sendStrcut = new SendStrcut();
            sendStrcut.sender = "FSender001";
            sendStrcut.option = CenterConstant.Enter;
            sendStrcut.model = centerFee;
            //提交中心
            string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
            string requestUrl = URL + FinanceApiSetting.SwapUrl;
            string apiclient = JsonConvert.SerializeObject(sendStrcut);

            Logs log = new Logs();
            log.Name = "换汇同步";
            log.MainID = notice.ID;
            log.AdminID = "";
            log.Json = apiclient;
            log.Summary = "";
            log.Enter();

            HttpResponseMessage response = new HttpResponseMessage();
            response = new HttpUtility.HttpClientHelp().HttpClient("POST", requestUrl, apiclient);
        }

        public SwapNotice()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.SwapStatus = Enums.SwapStatus.Auditing;

            this.Completed += SwapNotice_Completed;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.SwapNotice);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new Linq.ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //查询这次取消有哪些 SwapNoticeItems
                var swapNoticeItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                    .Where(t => t.SwapNoticeID == this.ID
                             && t.Status == (int)Enums.Status.Normal).ToList();

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        Status = Enums.SwapStatus.Cancel,
                    }, item => item.ID == this.ID);

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>(
                    new
                    {
                        Status = Enums.Status.Delete,
                    }, item => item.SwapNoticeID == this.ID);

                ////改变报关单换汇状态
                //foreach (var item in Items)
                //{
                //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(
                //    new
                //    {
                //        SwapStatus = (int)Enums.SwapStatus.UnAuditing,
                //    }, t => t.ID == item.SwapDecHead.ID);
                //}

                if (swapNoticeItems != null && swapNoticeItems.Any())
                {
                    string[] swapNoticeItemIDs = swapNoticeItems.Select(t => t.ID).ToArray();

                    var payApplySwapNoticeItemRelations = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayApplySwapNoticeItemRelation>()
                        .Where(t => swapNoticeItemIDs.Contains(t.SwapNoticeItemID) && t.Status == (int)Enums.Status.Normal).ToList();

                    if (payApplySwapNoticeItemRelations != null && payApplySwapNoticeItemRelations.Any())
                    {
                        string[] payApplySwapNoticeItemRelationIDs = payApplySwapNoticeItemRelations.Select(t => t.ID).ToArray();
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayApplySwapNoticeItemRelation>(new
                        {
                            Status = (int)Enums.Status.Delete,
                            UpdateDate = DateTime.Now,
                        }, item => payApplySwapNoticeItemRelationIDs.Contains(item.ID));


                        string[] payExchangeApplyItemIDs = payApplySwapNoticeItemRelations.Select(t => t.PayExchangeApplyItemID).Distinct().ToArray();
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new
                        {
                            ApplyStatus = (int)Enums.ApplyItemStatus.Appling,
                        }, item => payExchangeApplyItemIDs.Contains(item.ID));
                    }

                }
            }

            //改变报关单换汇状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                string[] decHeadIDs = Items.Select(t => t.SwapDecHead.ID).ToArray();

                var decHeadSwapedAmounts = (from swapNoticeItem in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                                            where decHeadIDs.Contains(swapNoticeItem.DecHeadID) && swapNoticeItem.Status == (int)Enums.Status.Normal
                                            group swapNoticeItem by new { swapNoticeItem.DecHeadID, } into g
                                            select new
                                            {
                                                DecHeadID = g.Key.DecHeadID,
                                                SwapedAmount = g.Sum(t => t.Amount),
                                            }).ToList();

                foreach (var item in Items)
                {
                    string decHeadID = item.SwapDecHead.ID;
                    decimal totalSwapAmount = item.SwapDecHead.Lists.Sum(t => t.DeclTotal);


                    Enums.SwapStatus targetDecHeadSwapStatus;

                    var oneSwapedAmount = decHeadSwapedAmounts.Where(t => t.DecHeadID == decHeadID).FirstOrDefault();
                    if (oneSwapedAmount == null || oneSwapedAmount.SwapedAmount == 0)
                    {
                        targetDecHeadSwapStatus = Enums.SwapStatus.UnAuditing;
                    }
                    else
                    {
                        if (totalSwapAmount <= oneSwapedAmount.SwapedAmount)
                        {
                            targetDecHeadSwapStatus = Enums.SwapStatus.Auditing;
                        }
                        else
                        {
                            targetDecHeadSwapStatus = Enums.SwapStatus.PartAudit;
                        }
                    }

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new
                    {
                        SwapStatus = (int)targetDecHeadSwapStatus,
                    }, t => t.ID == decHeadID);
                }
            }

        }

        /// <summary>
        /// 完成换汇 
        /// </summary>
        public void Complete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(
                    new
                    {
                        BankName = this.BankName,
                        ExchangeRate = this.ExchangeRate,
                        TotalAmountCNY = this.TotalAmountCNY,
                        Status = SwapStatus.Audited,
                        //UpdateDate = DateTime.Now,
                        OutFinanceAccountID = this.OutAccount?.ID,
                        InFinanceAccountID = this.InAccount?.ID,
                        MidFinanceAccountID = this.MidAccount?.ID,
                        Poundage = this.Poundage,
                        RealTimeExchangeRate = this.RealTimeExchangeRate,
                    }, item => item.ID == this.ID);
            }
            this.OnComplete();
        }

        /// <summary>
        /// 审批通过 
        /// </summary>
        public void Approve()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(
                    new
                    {
                        Status = Needs.Ccs.Services.Enums.SwapStatus.Auditing,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
            }
            this.OnEnterSuccess();
        }
        /// <summary>
        /// 审批不通过 
        /// </summary>
        public void unApprove()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(
                    new
                    {
                        Status = Needs.Ccs.Services.Enums.SwapStatus.RefuseAudit,
                        Summary = this.Summary,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
            }
            this.OnEnterSuccess();
        }
        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        virtual public void OnComplete()
        {
            if (this != null && this.Completed != null)
            {
                //成功后触发事件
                this.Completed(this, new SwapNoticeCompletedEventArgs(this));
            }
        }

        /// <summary>
        /// 导出农业银行换汇文件
        /// </summary>
        /// <returns></returns>
        public List<string> ToNongYePDf()
        {
            //将要压缩的文件添加到files 集合中
            List<string> files = new List<string>();
            BaseSaleContractPDF baseSaleContractPDF = new OtherSaleContractPDF(this);
            PdfDocument pdf = baseSaleContractPDF.SaleContractPdf();
            string fileName = this.ID + "销售合同" + ".pdf";
            var filedoc = CreateFileDic(fileName, pdf);
            files.Add(filedoc.FilePath);

            PdfDocument pdf2 = ToCommerceInvoicePdf();
            string fileName2 = ID + "商业发票" + ".pdf";
            var filedoc2 = CreateFileDic(fileName2, pdf2);
            files.Add(filedoc2.FilePath);

            PdfDocument pdf3 = ToDeclarPdf();
            string fileName3 = ID + "清单" + ".pdf";
            var filedoc3 = CreateFileDic(fileName3, pdf3);
            files.Add(filedoc3.FilePath);

            //生成-进口付汇报关单清单.xls
            #region  开始

            //try
            //{
            var linq = this.Items.Select(t => new
            {
                报关单企业代码 = t.SwapDecHead.AgentScc.Substring(t.SwapDecHead.AgentScc.Length - 10, 9),
                报关单号 = t.SwapDecHead.EntryId,
                币种 = t.SwapDecHead.Currency,
                核验金额 = t.Amount.ToRound(2).ToString("0.##"), //t.SwapDecHead.SwapAmount?.ToRound(2).ToString("0.##"),
                进口金额 = t.Amount.ToRound(2).ToString("0.##"), //t.SwapDecHead.SwapAmount?.ToRound(2).ToString("0.##"),
                进口日期 = t.SwapDecHead.DDate?.ToString("yyyy-MM-dd"),
            });
            //底部汇总DataTable

            string filename = "进口付汇报关单清单" + DateTime.Now.Ticks + ".xlsx";
            FileDirectory fileDic5 = new FileDirectory(filename);
            fileDic5.SetChildFolder(SysConfig.Export);
            fileDic5.CreateDataDirectory();

            var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SysConfig.ExportUrl);
            using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                DataTable sumTable = new DataTable("SumTable");
                sumTable.Columns.Add("column1");
                sumTable.Columns.Add("column2");
                sumTable.Columns.Add("column3");
                sumTable.Columns.Add("column4");

                DataRow row = sumTable.NewRow();
                row["column3"] = "合计";
                row["column4"] = this.TotalAmount.ToString("0.##");
                sumTable.Rows.Add(row);

                XSSFWorkbook workbookTemp = new XSSFWorkbook(file);

                NPOIHelper npoi = new NPOIHelper(workbookTemp);

                npoi.GenerateExcelByTemplate(linq, 2, sumTable);
                npoi.SaveAs(fileDic5.FilePath);
                files.Add(fileDic5.FilePath);
            }
            #endregion ---结束
            //将要压缩的报关单文件添加到files 集合中
            foreach (var item in this.Items)
            {
                files.Add(new FileDirectory().FilePath + @"\" + item.SwapDecHead.decheadFile.Url);
            }
            return files;
        }

        /// <summary>
        /// 导出渣打银行换汇文件
        /// </summary>
        /// <returns></returns>
        public List<string> ToSCBPDf()
        {
            //将要压缩的文件添加到files 集合中
            List<string> files = new List<string>();
            BaseSaleContractPDF baseSaleContractPDF = new SCBSaleContractPDF(this);
            PdfDocument pdf = baseSaleContractPDF.SaleContractPdf();
            string fileName = this.ID + "销售合同" + ".pdf";
            var filedoc = CreateFileDic(fileName, pdf);
            files.Add(filedoc.FilePath);

            PdfDocument pdf2 = ToCommerceInvoicePdf();
            string fileName2 = ID + "商业发票" + ".pdf";
            var filedoc2 = CreateFileDic(fileName2, pdf2);
            files.Add(filedoc2.FilePath);

            PdfDocument pdf3 = ToDeclarPdf();
            string fileName3 = ID + "清单" + ".pdf";
            var filedoc3 = CreateFileDic(fileName3, pdf3);
            files.Add(filedoc3.FilePath);

            //生成-进口付汇报关单清单.xls
            #region  开始

            //try
            //{
            var linq = this.Items.Select(t => new
            {
                报关单企业代码 = t.SwapDecHead.AgentScc.Substring(t.SwapDecHead.AgentScc.Length - 10, 9),
                报关单号 = t.SwapDecHead.EntryId,
                币种 = t.SwapDecHead.Currency,
                核验金额 = t.Amount.ToRound(2).ToString("0.##"), //t.SwapDecHead.SwapAmount?.ToRound(2).ToString("0.##"),
                进口金额 = t.Amount.ToRound(2).ToString("0.##"), //t.SwapDecHead.SwapAmount?.ToRound(2).ToString("0.##"),
                进口日期 = t.SwapDecHead.DDate?.ToString("yyyy-MM-dd"),
            });
            //底部汇总DataTable

            string filename = "进口付汇报关单清单" + DateTime.Now.Ticks + ".xlsx";
            FileDirectory fileDic5 = new FileDirectory(filename);
            fileDic5.SetChildFolder(SysConfig.Export);
            fileDic5.CreateDataDirectory();

            var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SysConfig.ExportUrl);
            using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                DataTable sumTable = new DataTable("SumTable");
                sumTable.Columns.Add("column1");
                sumTable.Columns.Add("column2");
                sumTable.Columns.Add("column3");
                sumTable.Columns.Add("column4");

                DataRow row = sumTable.NewRow();
                row["column3"] = "合计";
                row["column4"] = this.TotalAmount.ToString("0.##");
                sumTable.Rows.Add(row);

                XSSFWorkbook workbookTemp = new XSSFWorkbook(file);

                NPOIHelper npoi = new NPOIHelper(workbookTemp);

                npoi.GenerateExcelByTemplate(linq, 2, sumTable);
                npoi.SaveAs(fileDic5.FilePath);
                files.Add(fileDic5.FilePath);
            }
            #endregion ---结束
            //将要压缩的报关单文件添加到files 集合中
            foreach (var item in this.Items)
            {
                files.Add(new FileDirectory().FilePath + @"\" + item.SwapDecHead.decheadFile.Url);
            }
            return files;
        }

        public List<string> ToXingzhanPDf()
        {
            //将要压缩的文件添加到files 集合中
            List<string> files = new List<string>();
            #region  // 1.开始生成-进口付汇报关单清单.xls
            var linq = this.Items.Select(t => new
            {
                报关单企业代码 = t.SwapDecHead.AgentScc.Substring(t.SwapDecHead.AgentScc.Length - 10, 9),
                报关单号 = t.SwapDecHead.EntryId,
                币种 = t.SwapDecHead.Currency,
                核验金额 = t.SwapDecHead.SwapAmount?.ToRound(2).ToString("0.##"),
                进口金额 = t.SwapDecHead.SwapAmount?.ToRound(2).ToString("0.##"),
                进口日期 = t.SwapDecHead.DDate?.ToString("yyyy-MM-dd"),
            });
            //底部汇总DataTable


            string filename = "进口付汇报关单清单" + DateTime.Now.Ticks + ".xlsx";
            FileDirectory fileDic5 = new FileDirectory(filename);
            fileDic5.SetChildFolder(SysConfig.Export);
            fileDic5.CreateDataDirectory();

            var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SysConfig.ExportXingZhanUrl);
            using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                DataTable sumTable = new DataTable("SumTable");
                sumTable.Columns.Add("column1");
                sumTable.Columns.Add("column2");
                sumTable.Columns.Add("column3");
                sumTable.Columns.Add("column4");

                DataRow row = sumTable.NewRow();
                row["column3"] = "合计";
                row["column4"] = this.TotalAmount;
                sumTable.Rows.Add(row);

                XSSFWorkbook workbookTemp = new XSSFWorkbook(file);

                NPOIHelper npoi = new NPOIHelper(workbookTemp);

                npoi.GenerateExcelByTemplate(linq, 2, sumTable);
                npoi.SaveAs(fileDic5.FilePath);
                //添加到list集合
                files.Add(fileDic5.FilePath);
            }
            #endregion ---结束

            //2.报关单
            foreach (var item in this.Items)
            {
                //添加报关单到list 集合
                files.Add(new FileDirectory().FilePath + @"\" + item.SwapDecHead.decheadFile.Url);
            }
            return files;
        }

        /// <summary>
        /// 导出汇丰银行换汇
        /// </summary>
        /// <returns></returns>
        public List<string> ToHuiFengPDf()
        {
            //将要压缩的文件添加到files 集合中
            List<string> files = new List<string>();
            PdfDocument pdf = ToHuifengSaleContractPDf();
            string fileName = this.ID + "销售合同" + ".pdf";
            var filedoc = CreateFileDic(fileName, pdf);
            files.Add(filedoc.FilePath);

            PdfDocument pdf2 = ToHuifengCommerceInvoicePdf();
            string fileName2 = ID + "商业发票" + ".pdf";
            var filedoc2 = CreateFileDic(fileName2, pdf2);
            files.Add(filedoc2.FilePath);

            PdfDocument pdf3 = ToHuifengDeclarPdf();
            string fileName3 = ID + "清单" + ".pdf";
            var filedoc3 = CreateFileDic(fileName3, pdf3);
            files.Add(filedoc3.FilePath);



            //生成-进口付汇报关单清单.xls
            #region  开始

            //try
            //{
            var linq = this.Items.Select(t => new
            {
                报关单企业代码 = t.SwapDecHead.AgentScc.Substring(t.SwapDecHead.AgentScc.Length - 10, 9),
                报关单号 = t.SwapDecHead.EntryId,
                币种 = t.SwapDecHead.Currency,
                核验金额 = t.Amount.ToRound(2).ToString("0.##"), //t.SwapDecHead.SwapAmount?.ToRound(2).ToString("0.##"),
                进口金额 = t.Amount.ToRound(2).ToString("0.##"), //t.SwapDecHead.SwapAmount?.ToRound(2).ToString("0.##"),
                进口日期 = t.SwapDecHead.DDate?.ToString("yyyy-MM-dd"),
            });
            //底部汇总DataTable

            string filename = "进口付汇报关单清单" + DateTime.Now.Ticks + ".xlsx";
            FileDirectory fileDic5 = new FileDirectory(filename);
            fileDic5.SetChildFolder(SysConfig.Export);
            fileDic5.CreateDataDirectory();

            var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SysConfig.ExportUrl);
            using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                DataTable sumTable = new DataTable("SumTable");
                sumTable.Columns.Add("column1");
                sumTable.Columns.Add("column2");
                sumTable.Columns.Add("column3");
                sumTable.Columns.Add("column4");

                DataRow row = sumTable.NewRow();
                row["column3"] = "合计";
                row["column4"] = this.TotalAmount.ToString("0.##");
                sumTable.Rows.Add(row);

                XSSFWorkbook workbookTemp = new XSSFWorkbook(file);

                NPOIHelper npoi = new NPOIHelper(workbookTemp);

                npoi.GenerateExcelByTemplate(linq, 2, sumTable);
                npoi.SaveAs(fileDic5.FilePath);
                files.Add(fileDic5.FilePath);
            }
            #endregion ---结束



            foreach (var item in this.Items)
            {

                files.Add(new FileDirectory().FilePath + @"\" + item.SwapDecHead.decheadFile.Url);
            }

            return files;
        }

        /// <summary>
        /// 创建并保存PDf文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="pdf"></param>
        /// <returns></returns>
        public FileDirectory CreateFileDic(string filename, PdfDocument pdf)
        {
            FileDirectory fileDic = new FileDirectory(filename);
            fileDic.SetChildFolder(SysConfig.SwapFile);
            fileDic.CreateDataDirectory();
            pdf.SaveToFile(fileDic.FilePath);
            pdf.Close();
            return fileDic;
        }

        /// <summary>
        /// 导出商业发票
        /// </summary>
        public PdfDocument ToCommerceInvoicePdf()
        {
            var vendor = new VendorContext(VendorContextInitParam.SwapNoticeID, this.ID, "CaiWu").Current1;

            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();

            //设置margin
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(0.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(1.57f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4, margin);

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            PdfTrueTypeFont font4 = new PdfTrueTypeFont(new Font("SimSun", 16f, FontStyle.Bold), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头

            float y = 5;
            float width = page.Canvas.ClientSize.Width;
            var contractNo = this.DocumentNo; //"PS" + DateTime.Now.ToString("yyyyMMdd");
            var dateTime = this.InvoiceDate; //DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");

            string message = vendor.CompanyName;

            page.Canvas.DrawString(message, font4, brush, 0, y, formatLeft);
            y += font4.MeasureString(message, formatLeft).Height + 8;

            message = vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            //message = "Unit B2,2/F.,Houtex Ind.Bldg.,16Hung To Rd.,Kwun Tong,Kowloon,HK 电话:(852)31019258";
            //message = "Unit B2, 2 / F.,Houtex Ind. Bldg., 16 Hung To Rd., Kwun Tong, Kowloon，HK 电话：（852）31019258";
            message = vendor.AddressEN + ".电话：" + vendor.Tel;
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 6;

            message = "商业发票";
            page.Canvas.DrawString(message, font4, brush, width / 2, y, formatCenter);
            y += font4.MeasureString(message, formatCenter).Height + 8;

            message = "COMMERCIAL INVOICE";
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;


            message = "致:" + purchaser.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "日期:" + dateTime;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 15;

            message = "地址:" + purchaser.Address;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "发票编号:" + contractNo;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 15;
            #endregion

            #region 

            ////创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 8f), true);

            //设置列宽
            grid.Columns.Add(6);
            grid.Columns[0].Width = width * 0.2f;
            grid.Columns[1].Width = width * 0.2f;
            grid.Columns[2].Width = width * 0.2f;
            grid.Columns[3].Width = width * 0.1f;
            grid.Columns[4].Width = width * 0.2f;
            grid.Columns[5].Width = width * 0.1f; ;

            //产品信息
            PdfGridRow row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = "单号";
            row.Cells[1].Value = "货物名称";
            row.Cells[2].Value = "净重(KGS)";
            row.Cells[3].Value = "数量";
            row.Cells[4].Value = "总价(USD)";
            row.Cells[5].Value = "备注";

            foreach (PdfGridCell cell in row.Cells)
            {
                cell.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }
            decimal totalQty = 0;
            decimal? netWt = 0;
            row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = contractNo;
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "电子元器件";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            foreach (var item in this.Items)
            {
                netWt += item.SwapDecHead.Lists.Sum(x => x.NetWt)?.ToRound(2);
                totalQty += item.SwapDecHead.Lists.Sum(x => x.GQty).ToRound(0);
            }
            row.Cells[2].Value = netWt.ToString();
            row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[3].Value = totalQty.ToString();
            row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = this.TotalAmount.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[5].Value = this.Summary;
            row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            for (int i = 0; i < 8; i++)
            {
                row = grid.Rows.Add();
                row.Height = 20;
            }
            //合计行
            row = grid.Rows.Add();
            row.Height = 16;
            row.Cells[0].ColumnSpan = 3;
            row.Cells[0].Value = "总计:" + ConvertZh(this.TotalAmount.ToString("0.00"));
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = this.TotalAmount.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }

            PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
            y += result.Bounds.Height + 20;

            #endregion

            #region 尾
            font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);

            message =
                      "总净重：" + netWt + "KGS" + "\r\n" +
                      "\r\n" +
                      "总数量：" + totalQty + "PCS" + "\r\n" +
                      "\r\n" +
                      "总金额：USD" + this.TotalAmount.ToString("0.00");
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            //印章图片
            PdfImage HTimage = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.SignSealUrl));
            page.Canvas.DrawImage(HTimage, 100, y - 25);

            //建设银行要求额外盖一个章
            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.ContactStamp));
            page.Canvas.DrawImage(image, 330, y - 70);

            #endregion

            return pdf;
        }

        /// <summary>
        /// 导出销售合同
        /// </summary>
        /// <returns></returns>
        public PdfDocument ToSaleContractPDf()
        {
            var vendor = new VendorContext(VendorContextInitParam.SwapNoticeID, this.ID, "CaiWu").Current1;

            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();

            //设置margin
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(0.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(1.57f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4, margin);

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            PdfTrueTypeFont font4 = new PdfTrueTypeFont(new Font("SimSun", 16f, FontStyle.Bold), true);

            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头

            float y = 5;
            float width = page.Canvas.ClientSize.Width;
            var contractNo = this.DocumentNo;//"PS" + DateTime.Now.ToString("yyyyMMdd");
            var dateTime = this.ContrDate; //DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
            string message = vendor.CompanyName;

            //page.Canvas.DrawString(message, font4, brush, 0, y, formatLeft);
            page.Canvas.DrawString(message, font4, brush, page.Canvas.ClientSize.Width / 2, y, formatCenter);
            y += font4.MeasureString(message, formatCenter).Height + 8;

            message = vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            //message = "Unit B2,2/F.,Houtex Ind.Bldg.,16Hung To Rd.,Kwun Tong,Kowloon,HK 电话:(852)31019258";
            //message = "Unit B2, 2 / F.,Houtex Ind. Bldg., 16 Hung To Rd., Kwun Tong, Kowloon，HK 电话：（852）31019258";
            message = vendor.AddressEN + ".电话：" + vendor.Tel;

            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            message = "销售合同";
            page.Canvas.DrawString(message, font4, brush, width / 2, y, formatCenter);
            y += font4.MeasureString(message, formatCenter).Height + 8;

            message = "SALES CONTRACT";
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            message = "买方:" + purchaser.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            y += font3.MeasureString(message, formatLeft).Height + 15;

            message = "卖方:" + vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "日期:" + dateTime;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 15;

            message = "经买卖双方同意,按以下条款成交: ";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "合同编号:" + contractNo;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 15;
            #endregion

            #region 

            message = "1、";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            y += font3.MeasureString(message, formatLeft).Height + 5;

            ////创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 8f), true);

            //设置列宽
            grid.Columns.Add(6);
            grid.Columns[0].Width = width * 0.2f;
            grid.Columns[1].Width = width * 0.2f;
            grid.Columns[2].Width = width * 0.2f;
            grid.Columns[3].Width = width * 0.1f;
            grid.Columns[4].Width = width * 0.2f;
            grid.Columns[5].Width = width * 0.1f; ;

            //产品信息
            PdfGridRow row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = "单号";
            row.Cells[1].Value = "货物名称";
            row.Cells[2].Value = "净重(KGS)";
            row.Cells[3].Value = "数量";
            row.Cells[4].Value = "总价(USD)";
            row.Cells[5].Value = "备注";

            foreach (PdfGridCell cell in row.Cells)
            {
                cell.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }
            decimal totalQty = 0;
            decimal? netWt = 0;
            var DespPortCode = "";
            row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = contractNo;
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "电子元器件";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            foreach (var item in this.Items)
            {
                netWt += item.SwapDecHead.Lists.Sum(x => x.NetWt)?.ToRound(2);
                totalQty += item.SwapDecHead.Lists.Sum(x => x.GQty).ToRound(0);
                DespPortCode = item.SwapDecHead.Contract.DespPortCode;
            }
            row.Cells[2].Value = netWt.ToString();
            row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[3].Value = totalQty.ToString();
            row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = this.TotalAmount.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[5].Value = this.Summary;
            row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            for (int i = 0; i < 8; i++)
            {
                row = grid.Rows.Add();
                row.Height = 20;
            }
            //合计行
            row = grid.Rows.Add();
            row.Height = 16;
            row.Cells[0].ColumnSpan = 3;
            row.Cells[0].Value = "总计:" + ConvertZh(this.TotalAmount.ToString("0.00"));
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = this.TotalAmount.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }

            PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
            y += result.Bounds.Height + 20;

            #endregion

            //取币制的中文
            string CurrencyName = "";
            CurrencyName = new Needs.Ccs.Services.Views.BaseCurrenciesView().Where(item => item.Code == this.Currency).FirstOrDefault()?.Name;

            #region 尾
            font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            message =
                      "2、合同金额：USD" + this.TotalAmount.ToString("0.00") + "     " + ConvertZh(this.TotalAmount.ToString("0.00")) + "(" + CurrencyName + "）" + "   \r\n" +
                      "\r\n" +
                      "3、成交方式：CIF深圳  \r\n" +
                        "\r\n" +
                      "4、包装方式：纸箱   \r\n" +
                        "\r\n" +
                      "5、装运口岸和目的地：香港、深圳  \r\n" +
                        "\r\n" +
                      "6、结算方式：TT";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            y += font3.MeasureString(message, formatLeft).Height + 20;


            message = "卖方:" + vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "买方:" + purchaser.CompanyName;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 20;

            message = "签名盖章:";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            PdfImage HTimage = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.SignSealUrl));
            page.Canvas.DrawImage(HTimage, 50, y - 35);

            message = "签名盖章:";
            page.Canvas.DrawString(message, font3, brush, 375, y, formatRight);
            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.ContactStamp));
            page.Canvas.DrawImage(image, 330, y - 70);
            y += font3.MeasureString(message, formatLeft).Height + 5;
            #endregion

            return pdf;

        }

        /// <summary>
        ///导出报关单清单
        /// </summary>
        /// <returns></returns>
        public PdfDocument ToDeclarPdf()
        {
            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();

            //设置margin
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(0.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(1.57f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4, margin);

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 12f, FontStyle.Bold), true);

            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头
            float y = 5;
            float width = page.Canvas.ClientSize.Width;
            #endregion

            #region   
            ////创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 8f), true);

            //设置列宽
            grid.Columns.Add(4);
            grid.Columns[0].Width = width * 0.2f;
            grid.Columns[1].Width = width * 0.2f;
            grid.Columns[2].Width = width * 0.3f;
            grid.Columns[3].Width = width * 0.3f;

            PdfGridRow row = grid.Rows.Add();
            row.Height = 20;
            row.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 14f, FontStyle.Bold), true); ;
            row.Cells[0].ColumnSpan = 4;
            row.Cells[0].Value = "清单";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //产品信息
            //row = grid.Rows.Add();
            //row.Height = 20;
            //row.Cells[0].Value = "";
            //row.Cells[1].Value = "";
            //row.Cells[2].Value = "";
            //row.Cells[3].Value = "";

            foreach (PdfGridCell cell in row.Cells)
            {
                cell.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }

            // decimal totalAmount = 0;
            foreach (var item in this.Items)
            {
                row = grid.Rows.Add();
                row.Height = 20;
                row.Cells[0].Value = item.SwapDecHead.DDate?.ToString("yyyy-MM-dd");
                row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[1].Value = item.SwapDecHead.ContrNo;
                row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[2].Value = item.SwapDecHead.EntryId;
                row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                //row.Cells[3].Value = item.SwapDecHead.Lists.Sum(x => x.DeclTotal).ToRound(2).ToString("0.00");  // 确认总价取值是否正确 
                row.Cells[3].Value = item.Amount.ToRound(2).ToString("0.00");
                row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }





            //合计行
            row = grid.Rows.Add();
            row.Height = 16;
            row.Cells[0].ColumnSpan = 3;
            row.Cells[0].Value = "总计:";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[3].Value = this.TotalAmount.ToString("0.00");
            row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }
            PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
            y += result.Bounds.Height + 20;
            #endregion
            return pdf;
        }



        #region  ---汇丰银行换汇文件
        public PdfDocument ToHuifengSaleContractPDf()
        {
            var vendor = new VendorContext(VendorContextInitParam.SwapNoticeID, this.ID, "CaiWu").Current1;

            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();

            //设置margin
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(0.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(1.57f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4, margin);

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            PdfTrueTypeFont font4 = new PdfTrueTypeFont(new Font("SimSun", 16f, FontStyle.Bold), true);

            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头

            float y = 5;
            float width = page.Canvas.ClientSize.Width;
            var contractNo = this.DocumentNo; //"PS" + DateTime.Now.ToString("yyyyMMdd");
            var dateTime = this.ContrDate;//DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
            string message = vendor.CompanyName;

            //page.Canvas.DrawString(message, font4, brush, 0, y, formatLeft);
            page.Canvas.DrawString(message, font4, brush, page.Canvas.ClientSize.Width / 2, y, formatCenter);
            y += font4.MeasureString(message, formatCenter).Height + 8;

            message = vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            //message = "Unit B2,2/F.,Houtex Ind.Bldg.,16Hung To Rd.,Kwun Tong,Kowloon,HK 电话:(852)31019258";
            //message = "Unit B2, 2 / F.,Houtex Ind. Bldg., 16 Hung To Rd., Kwun Tong, Kowloon，HK 电话：（852）31019258";
            message = vendor.AddressEN + ".电话：" + vendor.Tel;

            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            message = "销售合同";
            page.Canvas.DrawString(message, font4, brush, width / 2, y, formatCenter);
            y += font4.MeasureString(message, formatCenter).Height + 8;

            message = "SALES CONTRACT";
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            message = "买方:" + purchaser.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            y += font3.MeasureString(message, formatLeft).Height + 15;

            message = "卖方:" + vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "日期:" + dateTime;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 15;

            message = "经买卖双方同意,按以下条款成交: ";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "合同编号:" + contractNo;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 15;
            #endregion

            #region 

            message = "1、";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            y += font3.MeasureString(message, formatLeft).Height + 5;

            ////创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 8f), true);

            //设置列宽
            grid.Columns.Add(6);
            grid.Columns[0].Width = width * 0.2f;
            grid.Columns[1].Width = width * 0.2f;
            grid.Columns[2].Width = width * 0.2f;
            grid.Columns[3].Width = width * 0.1f;
            grid.Columns[4].Width = width * 0.2f;
            grid.Columns[5].Width = width * 0.1f; ;

            //产品信息
            PdfGridRow row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = "单号";
            row.Cells[1].Value = "货物名称";
            row.Cells[2].Value = "净重(KGS)";
            row.Cells[3].Value = "数量";
            row.Cells[4].Value = "总价(USD)";
            row.Cells[5].Value = "备注";

            foreach (PdfGridCell cell in row.Cells)
            {
                cell.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }
            decimal totalQty = 0;
            decimal? netWt = 0;
            var DespPortCode = "";
            row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = contractNo;
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "电子元器件";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            foreach (var item in this.Items)
            {
                netWt += item.SwapDecHead.Lists.Sum(x => x.NetWt)?.ToRound(2);
                totalQty += item.SwapDecHead.Lists.Sum(x => x.GQty).ToRound(0);
                DespPortCode = item.SwapDecHead.Contract.DespPortCode;
            }
            row.Cells[2].Value = netWt.ToString();
            row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[3].Value = totalQty.ToString();
            row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = this.TotalAmount.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[5].Value = this.Summary;
            row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            for (int i = 0; i < 8; i++)
            {
                row = grid.Rows.Add();
                row.Height = 20;
            }
            //Add新加印章
            page.Canvas.DrawString("", font3, brush, 375, y, formatCenter);
            PdfImage image1 = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.SealUrl));
            page.Canvas.DrawImage(image1, width / 2 - 60, y + 50);


            //合计行
            row = grid.Rows.Add();
            row.Height = 16;
            row.Cells[0].ColumnSpan = 3;
            row.Cells[0].Value = "总计:" + ConvertZh(this.TotalAmount.ToString("0.00"));
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = this.TotalAmount.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }

            PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
            y += result.Bounds.Height + 20;

            #endregion

            string CurrencyName = "";
            CurrencyName = new Needs.Ccs.Services.Views.BaseCurrenciesView().Where(item => item.Code == this.Currency).FirstOrDefault()?.Name;

            #region 尾
            font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            message =
                      "2、合同金额：USD" + this.TotalAmount.ToString("0.00") + "     " + ConvertZh(this.TotalAmount.ToString("0.00")) + "(" + CurrencyName + "）" + "   \r\n" +
                      "\r\n" +
                      "3、成交方式：CIF深圳  \r\n" +
                        "\r\n" +
                      "4、包装方式：纸箱   \r\n" +
                        "\r\n" +
                      "5、装运口岸和目的地：香港、深圳  \r\n" +
                        "\r\n" +
                      "6、结算方式：TT";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            y += font3.MeasureString(message, formatLeft).Height + 20;


            message = "卖方:" + vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "买方:" + purchaser.CompanyName;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 20;

            message = "签名盖章:";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            PdfImage HTimage = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.SignSealUrl));
            page.Canvas.DrawImage(HTimage, 50, y - 35);

            message = "签名盖章:";
            page.Canvas.DrawString(message, font3, brush, 375, y, formatRight);
            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.ContactStamp));
            page.Canvas.DrawImage(image, 340, y - 60);
            y += font3.MeasureString(message, formatLeft).Height + 5;
            #endregion

            return pdf;

        }

        public PdfDocument ToHuifengCommerceInvoicePdf()
        {
            var vendor = new VendorContext(VendorContextInitParam.SwapNoticeID, this.ID, "CaiWu").Current1;

            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();

            //设置margin
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(0.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(1.57f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4, margin);

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            PdfTrueTypeFont font4 = new PdfTrueTypeFont(new Font("SimSun", 16f, FontStyle.Bold), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头

            float y = 5;
            float width = page.Canvas.ClientSize.Width;
            var contractNo = this.DocumentNo;//"PS" + DateTime.Now.ToString("yyyyMMdd");
            var dateTime = this.InvoiceDate; DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");

            string message = vendor.CompanyName;

            page.Canvas.DrawString(message, font4, brush, 0, y, formatLeft);
            y += font4.MeasureString(message, formatLeft).Height + 8;

            message = vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            //message = "Unit B2,2/F.,Houtex Ind.Bldg.,16Hung To Rd.,Kwun Tong,Kowloon,HK 电话:(852)31019258";
            //message = "Unit B2, 2 / F.,Houtex Ind. Bldg., 16 Hung To Rd., Kwun Tong, Kowloon，HK 电话：（852）31019258";
            message = vendor.AddressEN + ".电话：" + vendor.Tel;

            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 6;

            message = "商业发票";
            page.Canvas.DrawString(message, font4, brush, width / 2, y, formatCenter);
            y += font4.MeasureString(message, formatCenter).Height + 8;

            message = "COMMERCIAL INVOICE";
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;


            message = "致:" + purchaser.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "日期:" + dateTime;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 15;

            message = "地址:" + purchaser.Address;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "发票编号:" + contractNo;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 15;
            #endregion

            #region 

            ////创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 8f), true);

            //设置列宽
            grid.Columns.Add(6);
            grid.Columns[0].Width = width * 0.2f;
            grid.Columns[1].Width = width * 0.2f;
            grid.Columns[2].Width = width * 0.2f;
            grid.Columns[3].Width = width * 0.1f;
            grid.Columns[4].Width = width * 0.2f;
            grid.Columns[5].Width = width * 0.1f; ;

            //产品信息
            PdfGridRow row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = "单号";
            row.Cells[1].Value = "货物名称";
            row.Cells[2].Value = "净重(KGS)";
            row.Cells[3].Value = "数量";
            row.Cells[4].Value = "总价(USD)";
            row.Cells[5].Value = "备注";

            foreach (PdfGridCell cell in row.Cells)
            {
                cell.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }
            decimal totalQty = 0;
            decimal? netWt = 0;
            row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = contractNo;
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "电子元器件";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            foreach (var item in this.Items)
            {
                netWt += item.SwapDecHead.Lists.Sum(x => x.NetWt)?.ToRound(2);
                totalQty += item.SwapDecHead.Lists.Sum(x => x.GQty).ToRound(0);
            }
            row.Cells[2].Value = netWt.ToString();
            row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[3].Value = totalQty.ToString();
            row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = this.TotalAmount.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[5].Value = this.Summary;
            row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            for (int i = 0; i < 8; i++)
            {
                row = grid.Rows.Add();
                row.Height = 20;
            }

            //Add新加印章
            page.Canvas.DrawString("", font3, brush, 375, y, formatCenter);
            PdfImage image1 = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.SealUrl));
            page.Canvas.DrawImage(image1, width / 2 - 60, y + 50);
            //合计行
            row = grid.Rows.Add();
            row.Height = 16;
            row.Cells[0].ColumnSpan = 3;
            row.Cells[0].Value = "总计:" + ConvertZh(this.TotalAmount.ToString("0.00"));
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = this.TotalAmount.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }

            PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
            y += result.Bounds.Height + 20;

            #endregion

            #region 尾
            font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);

            message =
                      "总净重：" + netWt + "KGS" + "\r\n" +
                      "\r\n" +
                      "总数量：" + totalQty + "PCS" + "\r\n" +
                      "\r\n" +
                      "总金额：USD" + this.TotalAmount.ToString("0.00");
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            //印章图片
            PdfImage HTimage = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.SignSealUrl));
            page.Canvas.DrawImage(HTimage, 80, y - 25);

            #endregion

            return pdf;
        }

        public PdfDocument ToHuifengDeclarPdf()
        {
            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();

            //设置margin
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(0.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(1.57f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4, margin);

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font = new PdfTrueTypeFont(new Font("宋体", 14f, FontStyle.Bold), true);
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("宋体", 8f, FontStyle.Regular), true);
            // PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 12f, FontStyle.Bold), true);

            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头
            float y = 5;
            float width = page.Canvas.ClientSize.Width;
            #endregion

            #region   
            ////创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();

            string message = "报关单信息";
            page.Canvas.DrawString(message, font, brush, width / 2, y, formatCenter);
            y += font.MeasureString(message, formatCenter).Height + 8;

            //设置列宽
            grid.Columns.Add(6);
            grid.Columns[0].Width = width * 0.2f;
            grid.Columns[1].Width = width * 0.3f;
            grid.Columns[2].Width = width * 0.1f;
            grid.Columns[3].Width = width * 0.2f;
            grid.Columns[4].Width = width * 0.1f;
            grid.Columns[5].Width = width * 0.1f;

            //产品信息
            PdfGridRow row = grid.Rows.Add();
            row.Height = 25;
            row.Style.Font = new PdfTrueTypeFont(new Font("宋体", 10f, FontStyle.Bold), true);
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Bottom);
            row.Cells[0].Value = "报关单企业代码";
            row.Cells[1].Value = "报关单号";
            row.Cells[2].Value = "币种";
            row.Cells[3].Value = "核验金额";
            row.Cells[4].Value = "超额原因";
            row.Cells[5].Value = "超额备注";

            foreach (PdfGridCell cell in row.Cells)
            {
                cell.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }

            // decimal totalAmount = 0;
            foreach (var item in this.Items)
            {
                row = grid.Rows.Add();
                row.Height = 25;
                row.Cells[0].Value = item.SwapDecHead.AgentScc.Substring(item.SwapDecHead.AgentScc.Length - 10, 9);
                row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[1].Value = item.SwapDecHead.EntryId;
                row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[2].Value = item.SwapDecHead.Currency;
                row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                row.Cells[3].Value = item.Amount.ToRound(2).ToString("0.##"); //item.SwapDecHead.SwapAmount?.ToRound(2).ToString("0.##");  // 确认总价取值是否正确 
                row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[4].Value = "";
                row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[5].Value = "";
                row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }

            //合计行
            row = grid.Rows.Add();
            row.Height = 25;
            row.Style.Font = new PdfTrueTypeFont(new Font("宋体", 10f, FontStyle.Regular), true);
            row.Cells[2].Value = "合计";
            row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[3].Value = this.TotalAmount.ToString("0.##");
            row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //Add新加印章
            page.Canvas.DrawString("", font, brush, 0, y, formatLeft);
            PdfImage image1 = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.SealUrl));
            page.Canvas.DrawImage(image1, 0, y - 10);

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }
            PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
            y += result.Bounds.Height + 20;
            #endregion
            return pdf;
        }

        #endregion
        /// <summary>
        /// 将金额转化为中文大写
        /// </summary>
        /// <param name="money">金额 </param>
        /// <returns></returns>
        public string ConvertZh(string money)
        {
            //ryan 20201204 修改正确的转换大写方法
            decimal m = decimal.Parse(money);
            return CmycurD(m);

            #region
            //将小写金额转换成大写金额           
            //double MyNumber = Convert.ToDouble(money);
            //String[] MyScale = { "分", "角", "元", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "兆", "拾", "佰", "仟" };
            //String[] MyBase = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            //String M = "";
            //bool isPoint = false;
            //if (money.IndexOf(".") != -1)
            //{
            //    money = money.Remove(money.IndexOf("."), 1);
            //    isPoint = true;
            //}
            //for (int i = money.Length; i > 0; i--)
            //{
            //    int MyData = Convert.ToInt16(money[money.Length - i].ToString());
            //    M += MyBase[MyData];
            //    if (isPoint == true)
            //    {
            //        M += MyScale[i - 1];
            //    }
            //    else
            //    {
            //        M += MyScale[i + 1];
            //    }
            //}
            //return M;
            #endregion
        }

        /// <summary>
        /// 数字转大写
        /// </summary>
        /// <param name="Num">数字</param>/// <returns></returns>
        public string CmycurD(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字 
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字 
            string str3 = "";    //从原num值中取出的值 
            string str4 = "";    //数字的字符串形式 
            string str5 = "";  //人民币大写金额形式 
            int i;    //循环变量 
            int j;    //num的值乘以100的字符串长度 
            string ch1 = "";    //数字的汉语读法 
            string ch2 = "";    //数字位的汉字读法 
            int nzero = 0;  //用来计算连续的零值是几个 
            int temp;            //从原num值中取出的值 

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数 
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式 
            j = str4.Length;      //找出最高位 
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分 

            //循环取出每一位需要转换的值 
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值 
                temp = Convert.ToInt32(str3);      //转换为数字 
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时 
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位 
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位，则必须写上 
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整” 
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }
    }

    public class SwapContext
    {
        //换汇报关单
        public IEnumerable<SwapDecHead> SwapDecHeads { get; set; }

        /// <summary>
        /// 换汇通知创建人
        /// </summary>
        public Admin Creater { get; set; }

        public string BankName { get; set; }

        /// <summary>
        /// 提交换汇
        /// </summary>
        public void SubmitApply()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //创建换汇通知
                var swapNotice = new Layer.Data.Sqls.ScCustoms.SwapNotices();
                swapNotice.ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNotice);
                swapNotice.AdminID = this.Creater.ID;
                swapNotice.Currency = this.SwapDecHeads.First().Currency;
                swapNotice.TotalAmount = (decimal)this.SwapDecHeads.Sum(t => t.SwapAmount);
                swapNotice.BankName = this.BankName;
                swapNotice.CreateDate = DateTime.Now;
                swapNotice.UpdateDate = DateTime.Now;
                swapNotice.Status = (int)Enums.SwapStatus.Auditing;
                reponsitory.Insert(swapNotice);
                //创建换汇通知项明细
                foreach (var dechead in this.SwapDecHeads)
                {
                    var item = new Layer.Data.Sqls.ScCustoms.SwapNoticeItems(); ;
                    item.ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeItem);
                    item.SwapNoticeID = swapNotice.ID;
                    item.DecHeadID = dechead.ID;
                    item.CreateDate = DateTime.Now;
                    item.Amount = dechead.SwapAmount;
                    item.Status = (int)Enums.Status.Normal;
                    reponsitory.Insert(item);

                    //更新报关单的状态
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecHeads
                    {
                        SwapStatus = (int)Enums.SwapStatus.Auditing
                    }, t => t.ID == dechead.ID);
                }
            }
        }
    }
}
