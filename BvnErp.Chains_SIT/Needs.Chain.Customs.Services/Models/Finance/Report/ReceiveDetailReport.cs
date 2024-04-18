using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Ccs.Services;
using Needs.Utils;
using Needs.Utils.Npoi;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 导出财务做账-收款统计表
    /// </summary>
    public class ReceiptExportExcel
    {

        public string Export(string ClientName, string QuerenStatus, string StartDate, string EndDate, string ReceiptStartDate, string ReceiptEndDate)
        {
            //货款明细表视图
            var view = new Views.OrderReceiptReportView();

            var viewDetail = view.GetProductReceiptDetail(StartDate, EndDate, ReceiptStartDate, ReceiptEndDate);

            //统计表视图
            var viewMain = view.GetFinanceReceiptMain(StartDate, EndDate, ReceiptStartDate, ReceiptEndDate);

            //查询条件过滤
            if (!string.IsNullOrEmpty(QuerenStatus) && QuerenStatus != "0")
            {
                //0 - 全部, 1 - 未确认, 2 - 已确认
                if (QuerenStatus == "1")
                {
                    viewMain = viewMain.Where(x => x.ClearAmount < x.Amount).ToList();
                }
                else if (QuerenStatus == "2")
                {
                    viewMain = viewMain.Where(x => x.ClearAmount == x.Amount).ToList();
                }
            }
            if (string.IsNullOrEmpty(ClientName) == false)
            {
                viewMain = viewMain.Where(item => item.ClientName == ClientName).ToList();
            }

            viewMain = viewMain.OrderBy(t => t.ReceiptDate).ToList();


            //主表执行查询
            var mainList = viewMain;


            //之前没有完全过滤收款时间，所以需要按照统计表有的进行汇差统计
            //财务收款ID
            var receiptIDs = mainList.Select(t => t.ID).Distinct().ToArray();
            //货款明细执行查询
            var productdetail = viewDetail.Where(t => receiptIDs.Contains(t.FinanceReceiptID)).ToArray();
            //货款明细补全汇率
            var detailList = view.GetProductReceipt(productdetail);


            ////明细表执行查询
            //var productdetail = viewDetail.ToArray();
            ////货款明细补全汇率
            //var detailList = view.GetProductReceipt(productdetail);


            #region 进行数据修正

            foreach (var m in mainList)
            {
                //1、统计总汇差
                m.TotalExchangeSpread = detailList.Where(t => t.FinanceReceiptID == m.ID).Sum(t => t.exDiff);

                //2、K3账户名词修改
                switch (m.AccountName)
                {
                    case ("芯达通-星展银行账户"):
                        m.AccountName = "星展银行（中国）有限公司深圳分行";
                        break;
                    case ("芯达通-中国银行深圳罗岗支行"):
                        m.AccountName = "中国银行深圳罗岗支行";
                        break;
                    case ("芯达通-农业银行人民币账户"):
                        m.AccountName = "中国农业银行股份有限公司深圳免税大厦支行";
                        break;
                    case ("芯达通-宁波银行人民币户"):
                        m.AccountName = "宁波银行深圳分行";
                        break;
                    case ("芯达通-兴业银行"):
                        m.AccountName = "兴业银行股份有限公司深圳罗湖支行";
                        break;
                    default:
                        break;
                }

                //3、重新统计 已确认金额 累积到ReceiptEndDate，不用当前实际的
                //

            }

            #endregion

            #region 收款统计
            var mainData = mainList.Select(r => new
            {
                收款ID = r.ID,
                流水号 = r.SeqNo,
                收款账户名称 = r.AccountName,
                收款日期 = r.ReceiptDate.ToString("yyyy-MM-dd"),
                客户名称 = r.ClientName,
                客户类型 = r.InvoiceType == (int)Enums.InvoiceType.Full ? "单抬头" : "双抬头",
                收款金额RMB = r.Amount,
                已确认金额 = r.ClearAmount,
                未确认金额 = r.UnClearAmount,
                货款 = r.TotalProduct,
                增值税 = r.TotalAddTax,
                关税 = r.TotalTariffTax,
                消费税 = r.TotalExciseTax,
                代理费 = r.TotalAgency,
                汇差 = r.TotalExchangeSpread
            });
            #endregion

            #region 货款明细
            var detailData = detailList.OrderBy(t => t.ReceiptDate).Select(d => new
            {
                收款ID = d.FinanceReceiptID,
                流水号 = d.SeqNo,
                客户名称 = d.ClientName,
                合同号 = d.ContractNo,
                发票类型 = d.InvoiceType == (int)Enums.InvoiceType.Full ? "单抬头" : "双抬头",
                收款日期 = d.ReceiptDate.ToString("yyyy-MM-dd"),
                付汇ID = d.PayExchangeApplyID,
                美金货值 = d.DeclPrice,
                付汇汇率 = d.DeclRate,
                人民币货款 = d.DeclPriceRMB,
                实收货款 = d.ProductReceiptAmount,
                报关日期 = d.DDate.ToString("yyyy-MM-dd"),
                报关_开票汇率 = d.DDateRate,
                报关_开票金额 = d.DeclarePrice,
                汇差 = d.ExchangeSpread,
                币别 = d.Currency,
                美金 = d.USD,
                人民币 = d.RMB,
                实收汇差 = d.exDiff,
                差 = d.Diff
            });
            #endregion

            //文件
            string filename = "收款统计表" + DateTime.Now.Ticks + ".xlsx";
            FileDirectory fileDic5 = new FileDirectory(filename);
            fileDic5.SetChildFolder(Needs.Ccs.Services.SysConfig.Export);
            fileDic5.CreateDataDirectory();

            var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Needs.Ccs.Services.SysConfig.ExportReceiptStatistics);
            using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook workbook = new XSSFWorkbook(file);
                NPOIHelper npoi = new NPOIHelper(workbook);

                npoi.SetSheet("货款明细");
                npoi.GenerateExcelByTemplate(detailData, 1);

                npoi.SetSheet("收款统计");
                npoi.GenerateExcelByTemplate(mainData, 1);

                npoi.SaveAs(fileDic5.FilePath);
            }

            return fileDic5.FileUrl;

        }

        public string ExportForCredential(string ClientName, string QuerenStatus, string StartDate, string EndDate, string ReceiptStartDate, string ReceiptEndDate)
        {
            //货款明细表视图
            var view = new Views.OrderReceiptReportView();

            var viewDetail = view.GetProductReceiptDetailForCredential(StartDate, EndDate, ReceiptStartDate, ReceiptEndDate);

            //统计表视图
            var viewMain = view.GetFinanceReceiptMainForCredential(StartDate, EndDate, ReceiptStartDate, ReceiptEndDate);

            //查询条件过滤
            if (!string.IsNullOrEmpty(QuerenStatus) && QuerenStatus != "0")
            {
                //0 - 全部, 1 - 未确认, 2 - 已确认
                if (QuerenStatus == "1")
                {
                    viewMain = viewMain.Where(x => x.ClearAmount < x.Amount).ToList();
                }
                else if (QuerenStatus == "2")
                {
                    viewMain = viewMain.Where(x => x.ClearAmount == x.Amount).ToList();
                }
            }
            if (string.IsNullOrEmpty(ClientName) == false)
            {
                viewMain = viewMain.Where(item => item.ClientName == ClientName).ToList();
            }

            viewMain = viewMain.OrderBy(t => t.ReceiptDate).ToList();


            //主表执行查询
            var mainList = viewMain.AsQueryable();


            //之前没有完全过滤收款时间，所以需要按照统计表有的进行汇差统计
            //财务收款ID
            var receiptIDs = mainList.Select(t => t.ID).Distinct().ToArray();           
            //货款明细执行查询
            var productdetail = viewDetail.Where(t => receiptIDs.Contains(t.FinanceReceiptID)).ToArray();
            //货款明细补全汇率
            var detailList = view.GetProductReceipt(productdetail);


            ////明细表执行查询
            //var productdetail = viewDetail.ToArray();
            ////货款明细补全汇率
            //var detailList = view.GetProductReceipt(productdetail);


            #region 进行数据修正

            foreach (var m in mainList)
            {
                //1、统计总汇差
                m.TotalExchangeSpread = detailList.Where(t => t.FinanceReceiptID == m.ID).Sum(t => t.exDiff);

                //2、K3账户名词修改
                switch (m.AccountName)
                {
                    case ("芯达通-星展银行账户"):
                        m.AccountName = "星展银行（中国）有限公司深圳分行";
                        break;
                    case ("芯达通-中国银行深圳罗岗支行"):
                        m.AccountName = "中国银行深圳罗岗支行";
                        break;
                    case ("芯达通-农业银行人民币账户"):
                        m.AccountName = "中国农业银行股份有限公司深圳免税大厦支行";
                        break;
                    case ("芯达通-宁波银行人民币户"):
                        m.AccountName = "宁波银行深圳分行";
                        break;
                    case ("芯达通-兴业银行"):
                        m.AccountName = "兴业银行股份有限公司深圳罗湖支行";
                        break;
                    default:
                        break;
                }

                //3、重新统计 已确认金额 累积到ReceiptEndDate，不用当前实际的
                //

            }

            #endregion

            #region 收款统计
            var mainData = mainList.Select(r => new
            {
                收款ID = r.ID,
                流水号 = r.SeqNo,
                收款账户名称 = r.AccountName,
                收款日期 = r.ReceiptDate.ToString("yyyy-MM-dd"),
                客户名称 = r.ClientName,
                客户类型 = r.InvoiceType == (int)Enums.InvoiceType.Full ? "单抬头" : "双抬头",
                收款金额RMB = r.Amount,
                已确认金额 = r.ClearAmount,
                未确认金额 = r.UnClearAmount,
                货款 = r.TotalProduct,
                增值税 = r.TotalAddTax,
                关税 = r.TotalTariffTax,
                消费税 = r.TotalExciseTax,
                代理费 = r.TotalAgency,
                汇差 = r.TotalExchangeSpread
            });
            #endregion

            #region 货款明细
            var detailData = detailList.OrderBy(t => t.ReceiptDate).Select(d => new
            {
                收款ID = d.FinanceReceiptID,
                流水号 = d.SeqNo,
                客户名称 = d.ClientName,
                合同号 = d.ContractNo,
                发票类型 = d.InvoiceType == (int)Enums.InvoiceType.Full ? "单抬头" : "双抬头",
                收款日期 = d.ReceiptDate.ToString("yyyy-MM-dd"),
                付汇ID = d.PayExchangeApplyID,
                美金货值 = d.DeclPrice,
                付汇汇率 = d.DeclRate,
                人民币货款 = d.DeclPriceRMB,
                实收货款 = d.ProductReceiptAmount,
                报关日期 = d.DDate.ToString("yyyy-MM-dd"),
                报关_开票汇率 = d.DDateRate,
                报关_开票金额 = d.DeclarePrice,
                汇差 = d.ExchangeSpread,
                币别 = d.Currency,
                美金 = d.USD,
                人民币 = d.RMB,
                实收汇差 = d.exDiff,
                差 = d.Diff
            });
            #endregion

            //文件
            string filename = "收款统计表" + DateTime.Now.Ticks + ".xlsx";
            FileDirectory fileDic5 = new FileDirectory(filename);
            fileDic5.SetChildFolder(Needs.Ccs.Services.SysConfig.Export);
            fileDic5.CreateDataDirectory();

            var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Needs.Ccs.Services.SysConfig.ExportReceiptStatistics);
            using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook workbook = new XSSFWorkbook(file);
                NPOIHelper npoi = new NPOIHelper(workbook);

                npoi.SetSheet("货款明细");
                npoi.GenerateExcelByTemplate(detailData, 1);

                npoi.SetSheet("收款统计");
                npoi.GenerateExcelByTemplate(mainData, 1);

                npoi.SaveAs(fileDic5.FilePath);
            }

            return fileDic5.FileUrl;

        }

        public List<ReceiveDetailReportMain> ImportData(string ClientName, string QuerenStatus, string StartDate, string EndDate, string ReceiptStartDate, string ReceiptEndDate)
        {
            //货款明细表视图
            var view = new Views.OrderReceiptReportView();

            var viewDetail = view.GetProductReceiptDetailForCredential(StartDate, EndDate, ReceiptStartDate, ReceiptEndDate);

            //统计表视图
            var viewMain = view.GetFinanceReceiptMainForCredential(StartDate, EndDate, ReceiptStartDate, ReceiptEndDate);

            //查询条件过滤
            if (!string.IsNullOrEmpty(QuerenStatus) && QuerenStatus != "0")
            {
                //0 - 全部, 1 - 未确认, 2 - 已确认
                if (QuerenStatus == "1")
                {
                    viewMain = viewMain.Where(x => x.ClearAmount < x.Amount).ToList();
                }
                else if (QuerenStatus == "2")
                {
                    viewMain = viewMain.Where(x => x.ClearAmount == x.Amount).ToList();
                }
            }
            if (string.IsNullOrEmpty(ClientName) == false)
            {
                viewMain = viewMain.Where(item => item.ClientName == ClientName).ToList();
            }

            viewMain = viewMain.OrderBy(t => t.ReceiptDate).ToList();


            //主表执行查询
            var mainList = viewMain;


            //之前没有完全过滤收款时间，所以需要按照统计表有的进行汇差统计
            //财务收款ID
            var receiptIDs = mainList.Select(t => t.ID).Distinct().ToArray();        
            //货款明细执行查询
            var productdetail = viewDetail.Where(t => receiptIDs.Contains(t.FinanceReceiptID)).ToArray();
            //货款明细补全汇率
            var detailList = view.GetProductReceipt(productdetail);


            #region 进行数据修正

            foreach (var m in mainList)
            {
                //1、统计总汇差
                m.TotalExchangeSpread = detailList.Where(t => t.FinanceReceiptID == m.ID).Sum(t => t.exDiff);

                //2、K3账户名词修改
                switch (m.AccountName)
                {
                    case ("芯达通-星展银行账户"):
                        m.AccountName = "星展银行（中国）有限公司深圳分行";
                        break;
                    case ("芯达通-中国银行深圳罗岗支行"):
                        m.AccountName = "中国银行深圳罗岗支行";
                        break;
                    case ("芯达通-农业银行人民币账户"):
                        m.AccountName = "中国农业银行股份有限公司深圳免税大厦支行";
                        break;
                    case ("芯达通-宁波银行人民币户"):
                        m.AccountName = "宁波银行深圳分行";
                        break;
                    case ("芯达通-兴业银行"):
                        m.AccountName = "兴业银行股份有限公司深圳罗湖支行";
                        break;
                    default:
                        break;
                }
            }

            #endregion

            return mainList;


        }
    }

    /// <summary>
    /// 导出Excel主表 
    /// </summary>
    public class ReceiveDetailReportMain : IUnique
    {

        /// <summary>
        /// FinanceReceiptID以收款单位
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 收款流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public int InvoiceType { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 收款账户名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// 收款总金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 已核销金额
        /// </summary>
        public decimal ClearAmount { get; set; }

        /// <summary>
        /// 未核销金额
        /// </summary>
        public decimal UnClearAmount
        {

            get
            {
                return this.Amount - this.ClearAmount;
            }
        }

        /// <summary>
        /// 总货款
        /// </summary>
        public decimal? TotalProduct { get; set; }

        /// <summary>
        /// 总增值税
        /// </summary>
        public decimal? TotalAddTax { get; set; }

        /// <summary>
        /// 总关税
        /// </summary>
        public decimal? TotalTariffTax { get; set; }

        /// <summary>
        /// 总消费税
        /// </summary>
        public decimal? TotalExciseTax { get; set; }

        /// <summary>
        /// 总代理费
        /// </summary>
        public decimal? TotalAgency { get; set; }


        /// <summary>
        /// 总汇差
        /// </summary>
        public decimal TotalExchangeSpread
        {
            get; set;
        }

        public string OrderRecepitID { get; set; }

    }

    public class ReceiveDetailReport : IUnique
    {

        /// <summary>
        /// PayExchangeItemID以单个付汇申请项为单位
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 财务收款ID
        /// </summary>
        public string FinanceReceiptID { get; set; }

        /// <summary>
        /// 收款流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int InvoiceType { get; set; }

        public decimal OrderRealRate { get; set; }

        public string PayExchangeApplyID { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// 货款实收，根据 收款ID、付汇、订单汇总
        /// </summary>
        public decimal ProductReceiptAmount { get; set; }

        /// <summary>
        /// 付汇货值美元
        /// </summary>
        public decimal DeclPrice { get; set; }

        /// <summary>
        /// 付汇申请汇率
        /// </summary>
        public decimal DeclRate { get; set; }

        /// <summary>
        /// 收款当日汇率
        /// </summary>
        public decimal ReceiptRate { get; set; }

        /// <summary>
        /// RMB收款（付汇申请 应收款）
        /// </summary>
        public decimal DeclPriceRMB
        {
            get
            {
                //return (this.DeclPrice * ReceiptRate).ToRound(2);
                return (this.DeclPrice * DeclRate).ToRound(2);
            }
        }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime DDate { get; set; }

        /// <summary>
        /// 报关币种
        /// </summary>
        public string DeclareCurrency { get; set; }

        /// <summary>
        /// 报关当日汇率
        /// </summary>
        public decimal DDateRate { get; set; }

        /// <summary>
        /// 报关金额RMB = Round (DeclPrice * 报关当日汇率， 2）
        /// </summary>
        public decimal DeclarePrice
        {
            //get
            //{
            //    return (this.DeclPrice * DDateRate).ToRound(2);
            //}
            get; set;
        }

        private decimal? exchangeSpread;
        /// <summary>
        /// 汇差 = 报关金额RMB - RMB收款
        /// </summary>
        public decimal ExchangeSpread
        {
            get
            {
                if (exchangeSpread.HasValue)
                {
                    return exchangeSpread.Value;
                }
                else
                {
                    return this.DeclarePrice - ProductReceiptAmount;
                }
            }

            set
            {
                exchangeSpread = value;
            }
        }

        public decimal? uSD { get; set; }

        public decimal USD
        {
            get
            {
                if (this.uSD == null)
                {
                    return Math.Round(this.ProductReceiptAmount / this.DeclRate, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    return this.uSD.Value;
                }
                
            }
            set { }
        }

        public decimal? rMB { get; set; }
        public decimal RMB
        {
            get
            {
                if (this.rMB == null)
                {
                    return Math.Round(this.USD * this.DDateRate, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    return this.rMB.Value;
                }               
            }
            set { }
        }
        public decimal exDiff
        {
            get
            {
                return this.RMB-this.ProductReceiptAmount;
            }
        }
        public decimal Diff
        {
            get
            {
                return this.exDiff-this.ExchangeSpread;
            }
        }

        public string Currency { get; set; }


    }
}
