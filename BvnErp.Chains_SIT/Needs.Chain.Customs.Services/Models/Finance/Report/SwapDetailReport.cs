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
using Needs.Ccs.Services.Views;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 导出财务做账-付汇统计表
    /// </summary>
    public class SwapDetailReport
    {

        public string Export(string ContractNo, string EntryID, string BankName, string StartDate, string EndDate)
        {
            var view = new Views.SwapNoticeReportView();

            var viewMain = view.GetSwapNoticeMain();

            var viewDetail = view.AsQueryable();

            //查询条件
            if (!string.IsNullOrEmpty(BankName))
            {
                BankName = BankName.Trim();
                viewMain = viewMain.Where(t => t.BankName == BankName);
                viewDetail = viewDetail.Where(t => t.SwapBank == BankName);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                viewMain = viewMain.Where(t => t.SwapDate >= start);
                viewDetail = viewDetail.Where(t => t.SwapDate >= start);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                viewMain = viewMain.Where(t => t.SwapDate < end);
                viewDetail = viewDetail.Where(t => t.SwapDate < end);
            }
            if (!string.IsNullOrEmpty(ContractNo))
            {
                ContractNo = ContractNo.Trim();
                viewDetail = viewDetail.Where(t => t.ContractNo == ContractNo);
            }
            if (!string.IsNullOrEmpty(EntryID))
            {
                EntryID = EntryID.Trim();
                viewDetail = viewDetail.Where(t => t.EntryID == EntryID);
            }

            //主表执行查询
            var mainList = viewMain.ToArray();
          
            //明细表执行查询
            var detailList = view.GetNoticeDetailWithDecPrice(viewDetail.ToArray());

            //换汇统计表: 明细表的分类汇总
            var detailSum = (from detail in detailList
                            group detail by new { detail.SwapNoticeID, detail.DDate, detail.DDateRate, detail.ClientName, detail.SupplierName, detail.Currency, detail.SwapDate, detail.SwapBank, detail.SwapBankMiddle, detail.SwapRate } into detail_group
                            select new SwapNoticeCollect
                            {
                                SwapNoticeID = detail_group.Key.SwapNoticeID,
                                DDate = detail_group.Key.DDate,
                                DDateRate = detail_group.Key.DDateRate,
                                ClientName = detail_group.Key.ClientName,
                                SupplierName = detail_group.Key.SupplierName,
                                Currency = detail_group.Key.Currency,
                                DeclPrice = detail_group.Sum(t => t.DeclPrice),
                                DeclarePrice = detail_group.Sum(t => t.DeclarePrice),
                                SwapPrice = detail_group.Sum(t => t.SwapPrice),
                                SwapDate = detail_group.Key.SwapDate,
                                SwapBank = detail_group.Key.SwapBank,
                                SwapBankMiddle = detail_group.Key.SwapBankMiddle,
                                SwapRate = detail_group.Key.SwapRate,

                                DeclarePriceRMB = detail_group.Sum(t => t.DeclarePrice) * detail_group.Key.DDateRate,
                                DeclPriceRMB = detail_group.Sum(t => t.DeclPrice) * detail_group.Key.DDateRate,
                                ScalePrice = detail_group.Sum(t => t.ScalePrice),
                                OtherPrice = detail_group.Sum(t => t.OtherPrice),
                                ScaleOtherPrice = detail_group.Sum(t => t.ScaleOtherPrice),
                                DeclExchangeSpread = detail_group.Sum(t => t.DeclExchangeSpread),
                                OtherExchangeSpread = detail_group.Sum(t => t.OtherExchangeSpread)
                            }).ToList();

            //K3做账银行名称转换
            //K3账户名词修改
            foreach (var m in mainList)
            {
                //K3账户名词修改
                switch (m.BankName)
                {
                    case ("星展银行"):
                        m.BankName = "星展银行（中国）有限公司深圳分行";
                        break;
                    case ("农业银行"):
                        m.BankName = "中国农业银行股份有限公司深圳免税大厦支行";
                        m.SwapBankMiddle = "中国农业银行深圳免税大厦支行美金账户";
                        break;
                    case ("宁波银行"):
                        m.BankName = "宁波银行深圳分行";
                        m.SwapBankMiddle = "宁波银行深圳分行美金账户";
                        break;
                    default:
                        break;
                }
            }
            
            #region 换汇主表
            var mainData = mainList.Select(r => new
            {
                换汇ID = r.SwapNoticeID,
                境外发货人 = r.ConsignorCode,
                换汇美金 = r.SwapAmount,
                换汇汇率 = r.SwapRate,
                换汇金额RMB = r.SwapAmountRMB,
                换汇银行 = r.BankName,
                过渡银行 = r.SwapBankMiddle,
                换汇日期 = r.SwapDate.ToString("yyyy-MM-dd"),
                
            });
            #endregion

            #region 换汇统计表

            //解决误差问题
            foreach (var main in mainData)
            {
                var sumData = detailSum.Where(t => t.SwapNoticeID == main.换汇ID).ToList();
                var diff = main.换汇金额RMB - sumData.Sum(t => t.DeclExchangeSpread + t.OtherExchangeSpread + t.DDateRateScalePrice + t.DDateRateScaleOtherPrice);
                detailSum.Where(t => t.SwapNoticeID == main.换汇ID).FirstOrDefault().OtherExchangeSpread = detailSum.Where(t => t.SwapNoticeID == main.换汇ID).FirstOrDefault().OtherExchangeSpread + diff;
            }

            var detailSumData = detailSum.Select(d => new
            {
                换汇ID = d.SwapNoticeID,
                报关日期 = d.DDate.ToString("yyyy-MM-dd"),
                客户名称 = d.ClientName,
                境外发货人 = d.SupplierName,
                币种 = d.Currency,
                报关金额 = d.DeclarePrice,
                报关金额RMB = Math.Round(d.DeclarePriceRMB,2,MidpointRounding.AwayFromZero),
                委托金额 = d.DeclPrice,
                委托金额RMB = Math.Round(d.DeclPriceRMB,2,MidpointRounding.AwayFromZero),
                换汇金额 = d.SwapPrice,
                对应比例的委托金额 = d.ScalePrice,
                换汇日期 = d.SwapDate.ToString("yyyy-MM-dd"),
                换汇银行 = d.SwapBank,
                换汇银行过渡银行 = d.SwapBankMiddle,
                运保杂美金 = d.OtherPrice,
                对应比例的运保杂 = d.ScaleOtherPrice,
                换汇汇率 = d.SwapRate,
                报关当天汇率 = d.DDateRate,
                客户委托金额汇差 = d.DeclExchangeSpread,
                芯达通运保杂汇差 = d.OtherExchangeSpread,
                对应比例的委托金额RMB报关 = d.DDateRateScalePrice,
                对应比例的运保杂金额RMB报关 = d.DDateRateScaleOtherPrice
            });

            #endregion

            #region 换汇明细表
            var detailData = detailList.Select(d => new
            {
                报关日期 = d.DDate.ToString("yyyy-MM-dd"),
                客户名称 = d.ClientName,
                境外发货人 = d.SupplierName,
                合同号 = d.ContractNo,
                订单号 = d.OrderID,
                海关编号 = d.EntryID,
                币种 = d.Currency,
                报关金额 = d.DeclarePrice,
                报关金额RMB =d.DeclarePriceRMB,
                委托金额 = d.DeclPrice,
                委托金额RMB = d.DeclPriceRMB,
                换汇金额 = d.SwapPrice,
                对应比例的委托金额 = d.ScalePrice,
                换汇日期 = d.SwapDate.ToString("yyyy-MM-dd"),
                换汇银行 = d.SwapBank,
                换汇银行过渡银行 = d.SwapBankMiddle,
                运保杂美金 = d.OtherPrice,
                对应比例的运保杂 = d.ScaleOtherPrice,
                换汇汇率 = d.SwapRate,
                报关当天汇率 = d.DDateRate,
                客户委托金额汇差 = d.DeclExchangeSpread,
                芯达通运保杂汇差 = d.OtherExchangeSpread
            });
            #endregion

            //文件
            string filename = "换汇统计表" + DateTime.Now.Ticks + ".xlsx";
            FileDirectory fileDic5 = new FileDirectory(filename);
            fileDic5.SetChildFolder(Needs.Ccs.Services.SysConfig.Export);
            fileDic5.CreateDataDirectory();

            var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Needs.Ccs.Services.SysConfig.ExportSwapStatistics);
            using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook workbook = new XSSFWorkbook(file);
                NPOIHelper npoi = new NPOIHelper(workbook);

                npoi.SetSheet("换汇明细表");
                npoi.GenerateExcelByTemplate(detailData, 1);

                npoi.SetSheet("换汇统计表");
                npoi.GenerateExcelByTemplate(detailSumData, 1);

                npoi.SetSheet("换汇主表");
                npoi.GenerateExcelByTemplate(mainData, 1);

                npoi.SaveAs(fileDic5.FilePath);
            }

            return fileDic5.FileUrl;
        }
    }





    /// <summary>
    /// 换汇明细
    /// </summary>
    public class SwapNoticeDetail : IUnique
    {
        /// <summary>
        /// SwapNoticeItemID  换汇itemID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 报关单ID
        /// </summary>
        public string DecHeadID { get; set; }

        /// <summary>
        /// 换汇ID
        /// </summary>
        public string SwapNoticeID { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime DDate { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 境外发货人
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNo { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 海关编号
        /// </summary>
        public string EntryID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 报关金额
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 报关当天汇率
        /// </summary>
        public decimal DDateRate { get; set; }

        /// <summary>
        /// 报关金额RMB = Round(报关金额*报关当天汇率,2)
        /// </summary>
        public decimal DeclarePriceRMB
        {
            get
            {
                return (this.DeclarePrice * this.DDateRate).ToRound(2);
            }
        }

        /// <summary>
        /// 委托金额
        /// </summary>
        public decimal DeclPrice { get; set; }

        /// <summary>
        /// 委托金额RMB  Round(委托金额*报关当天汇率,2)
        /// </summary>
        public decimal DeclPriceRMB
        {
            get
            {
                return (this.DeclPrice * this.DDateRate).ToRound(2);
            }
        }

        /// <summary>
        /// 委托金额
        /// </summary>
        public decimal SwapPrice { get; set; }

        public decimal? scalePrice { get; set; }
        /// <summary>
        /// 对应比例的委托金额
        /// Round(换汇金额/报关金额*委托金额美金,2)
        /// 若是最后一次换汇，使用减法（委托金额 - Sum（之前的对应比例委托金额））
        /// </summary>
        public decimal ScalePrice
        {
            get
            {
                if (!this.scalePrice.HasValue)
                {
                    if (this.SwapPrice != this.DeclarePrice)
                    {
                        //多次换汇
                        var swaps = new SwapNoticeReportView().GetLastSwap(this.DecHeadID);
                        var hadSwaped = swaps.Sum(t => t.Amount);
                        if (this.DeclarePrice == swaps.Sum(t => t.Amount))
                        {
                            //已经全部换汇，必定存在最后一次
                            var last = swaps.FirstOrDefault();
                            if (last.ID == this.ID)
                            {
                                //当前是最后一条
                                this.scalePrice = this.DeclPrice - swaps.Where(t => t.ID != this.ID).Sum(t => (t.Amount / this.DeclarePrice * this.DeclPrice).ToRound(2));
                                return scalePrice.Value;
                            }
                        }
                    }

                    //不是多次||不是最后一次，均使用比例正常返回
                    this.scalePrice = (this.SwapPrice / this.DeclarePrice * this.DeclPrice).ToRound(2);
                    return this.scalePrice.Value;
                }
                else
                {
                    return this.scalePrice.Value;
                }


            }
        }

        /// <summary>
        /// 换汇日期
        /// </summary>
        public DateTime SwapDate { get; set; }

        /// <summary>
        /// 换汇银行
        /// </summary>
        public string SwapBank { get; set; }

        /// <summary>
        /// 换汇中间账户
        /// </summary>
        public string SwapBankMiddle { get; set; }

        /// <summary>
        /// 运保杂美金
        /// </summary>
        public decimal OtherPrice
        {
            get
            {
                return this.DeclarePrice - this.DeclPrice;
            }
        }

        /// <summary>
        /// 对应比例的运保杂美金
        /// Round(换汇金额/报关金额*运保杂美金,2)
        /// </summary>
        public decimal ScaleOtherPrice
        {
            get
            {
                return (this.SwapPrice / this.DeclarePrice * this.OtherPrice).ToRound(2);
            }
        }

        /// <summary>
        /// 换汇汇率
        /// </summary>
        public decimal SwapRate { get; set; }

        /// <summary>
        /// 委托金额汇差
        /// Round(（换汇汇率 - 报关当天汇率） * 对应比例的委托金额,2)
        /// </summary>
        public decimal DeclExchangeSpread
        {
            get
            {
                return ((this.SwapRate - this.DDateRate) * this.ScalePrice).ToRound(2);
            }
        }

        /// <summary>
        /// 运保杂汇差
        /// Round(（换汇汇率 - 报关当天汇率） * 对应比例的运保杂,2)
        /// </summary>
        public decimal OtherExchangeSpread
        {
            get
            {
                return ((this.SwapRate - this.DDateRate) * this.ScaleOtherPrice).ToRound(2);
            }
        }

    }

    /// <summary>
    /// 换汇汇总-根据客户与报关日期
    /// </summary>
    public class SwapNoticeCollect
    {
        public string SwapNoticeID { get; set; }
        public DateTime DDate { get; set; }
        public decimal DDateRate { get; set; }
        public string ClientName { get; set; }
        public string SupplierName { get; set; }
        public string Currency { get; set; }
        public decimal DeclPrice { get; set; }
        public decimal DeclarePrice { get; set; }
        public decimal SwapPrice { get; set; }
        public DateTime SwapDate { get; set; }
        public string SwapBank { get; set; }
        public string SwapBankMiddle { get; set; }
        public decimal SwapRate { get; set; }
        public decimal DeclarePriceRMB { get; set; }
        public decimal DeclPriceRMB { get; set; }
        public decimal ScalePrice { get; set; }
        public decimal OtherPrice { get; set; }
        public decimal ScaleOtherPrice { get; set; }
        public decimal DeclExchangeSpread { get; set; }
        public decimal OtherExchangeSpread { get; set; }

        /// <summary>
        /// 对应比例的委托金额RMB-报关
        /// </summary>
        public decimal DDateRateScalePrice
        {
            get
            {
                return (this.ScalePrice * this.DDateRate).ToRound(2);
            }
        }

        /// <summary>
        /// 对应比例的运保杂金额RMB-报关
        /// </summary>
        public decimal DDateRateScaleOtherPrice
        {
            get
            {
                return (this.ScaleOtherPrice * this.DDateRate).ToRound(2);
            }
        }

    }

    /// <summary>
    /// 换汇
    /// </summary>
    public class SwapNoticeMain
    {
        /// <summary>
        /// 换汇ID 
        /// </summary>
        public string SwapNoticeID { get; set; }

        /// <summary>
        /// 境外发货人
        /// </summary>
        public string ConsignorCode { get; set; }

        /// <summary>
        /// 换汇美金
        /// </summary>
        public decimal SwapAmount { get; set; }

        /// <summary>
        /// 换汇汇率
        /// </summary>
        public decimal SwapRate { get; set; }

        /// <summary>
        /// 换汇金额RMB
        /// </summary>
        public decimal SwapAmountRMB { get; set; }

        /// <summary>
        /// 换汇银行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 过渡银行
        /// </summary>
        public string SwapBankMiddle { get; set; }

        /// <summary>
        /// 换汇日期
        /// </summary>
        public DateTime SwapDate { get; set; }


    }

    public class SwapNoticeItemOrigin
    {
        public string ID { get; set; }
        public string SwapNoticeID { get; set; }
        public string DecHeadID { get; set; }
        public DateTime CreateDate { get; set; }

        public decimal Amount { get; set; }

    }

}
