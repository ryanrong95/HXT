using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance.Swap
{
    public partial class SwapList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            this.Model.BankData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks.Select(item => new { value = item.ID, text = item.Name }).Json();
        }

        /// <summary>
        /// 加载换汇通知
        /// </summary>
        protected void data1()
        {
            string BankName = Request.QueryString["BankName"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string ContrNo = Request.QueryString["ContrNo"];
            string EntryId = Request.QueryString["EntryId"];

            var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice.AsQueryable()
                .Where(item => item.SwapStatus == SwapStatus.Audited);
            //查询条件
            if (!string.IsNullOrEmpty(BankName))
            {
                BankName = BankName.Trim();
                notices = notices.Where(t => t.BankName == BankName);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                notices = notices.Where(t => t.CreateDate >= start);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                notices = notices.Where(t => t.CreateDate < end);
            }
            if (!string.IsNullOrEmpty(ContrNo))
            {
                ContrNo = ContrNo.Trim();
                notices = notices.Where(t => t.Items.Where(x => x.SwapDecHead.ContrNo == ContrNo).Count() > 0);
            }
            if (!string.IsNullOrEmpty(EntryId))
            {
                EntryId = EntryId.Trim();
                notices = notices.Where(t => t.Items.Where(x => x.SwapDecHead.EntryId == EntryId).Count() > 0);
            }

            notices = notices.OrderByDescending(t => t.CreateDate);

            Func<SwapNotice, object> convert = item => new
            {
                ID = item.ID,
                Creator = item.Admin.RealName,
                item.Currency,
                item.TotalAmount,
                item.BankName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd"),
                SwapStatus = item.SwapStatus.GetDescription(),
                item.ConsignorCode,
            };
            notices = notices.OrderByDescending(t => t.UpdateDate);
            this.Paging(notices, convert);
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string BankName = Request.QueryString["BankName"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string ContrNo = Request.QueryString["ContrNo"];
            string EntryId = Request.QueryString["EntryId"];

            using (var query = new Needs.Ccs.Services.Views.SwapedListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(BankName))
                {
                    BankName = BankName.Trim();
                    view = view.SearchByBankName(BankName);
                }
                if (!string.IsNullOrEmpty(StartDate))
                {
                    DateTime start = Convert.ToDateTime(StartDate);
                    view = view.SearchByStartDate(start);
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByEndDate(end);
                }
                if (!string.IsNullOrEmpty(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    view = view.SearchByContrNo(ContrNo);
                }
                if (!string.IsNullOrEmpty(EntryId))
                {
                    EntryId = EntryId.Trim();
                    view = view.SearchByEntryId(EntryId);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        /// <summary>
        /// 导出换汇文件
        /// </summary>
        protected void ExportSwapFiles()
        {
            try
            {
                //1.创建文件夹(文件压缩后存放的地址)
                FileDirectory file = new FileDirectory();
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.ZipSwapFile);
                file.CreateDataDirectory();

                string ID = Request.Form["ID"];
                var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice[ID];
                var zipFileName = "SwapDownload.zip";

                var files = new List<string>();
                //2.返回PDF文件
                if (apply.BankName.Contains("星展"))
                {
                    files = apply.ToXingzhanPDf();

                }
                if (apply.BankName.Contains("汇丰"))
                {
                    files = apply.ToHuiFengPDf();
                }
                if (apply.BankName.Contains("渣打"))
                {
                    files = apply.ToSCBPDf();
                }
                else
                {
                    files = apply.ToNongYePDf();
                }
                //3.压缩文件并下载
                ZipFile zip = new ZipFile(zipFileName);
                zip.SetFilePath(file.FilePath);
                zip.Files = files;
                zip.ZipFiles();
                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl + zipFileName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出 Excel
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                string BankName = Request.Form["BankName"];
                string StartDate = Request.Form["StartDate"];
                string EndDate = Request.Form["EndDate"];
                string ContrNo = Request.Form["ContrNo"];
                string EntryId = Request.Form["EntryId"];

                var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Views.ExportSwapedDecHeadListModel>();

                //查询条件
                if (!string.IsNullOrEmpty(BankName))
                {
                    BankName = BankName.Trim();
                    predicate = predicate.And(t => t.BankName == BankName);
                }
                if (!string.IsNullOrEmpty(StartDate))
                {
                    DateTime start = Convert.ToDateTime(StartDate);
                    predicate = predicate.And(t => t.SwapNoticeCreateDate >= start);
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                    predicate = predicate.And(t => t.SwapNoticeCreateDate < end);
                }
                if (!string.IsNullOrEmpty(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    predicate = predicate.And(t => t.ContrNo == ContrNo);
                }
                if (!string.IsNullOrEmpty(EntryId))
                {
                    EntryId = EntryId.Trim();
                    predicate = predicate.And(t => t.EntryId == EntryId);
                }

                Needs.Ccs.Services.Views.ExportSwapedDecHeadListView view = new Needs.Ccs.Services.Views.ExportSwapedDecHeadListView();
                view.AllowPaging = false;
                view.Predicate = predicate;

                var dataList = view.ToList();

                Func<Needs.Ccs.Services.Views.ExportSwapedDecHeadListModel, object> convert = swapedDecHead => new
                {
                    DecHeadID = swapedDecHead.DecHeadID,
                    DDate = swapedDecHead.DDate?.ToString("yyyy-MM-dd"), //报关日期
                    OwnerName = swapedDecHead.OwnerName, //客户名称
                    ConsignorCode = swapedDecHead.ConsignorCode,//境外发货人
                    ContrNo = swapedDecHead.ContrNo, //合同号
                    OrderID = swapedDecHead.OrderID, //订单号
                    EntryId = swapedDecHead.EntryId, //海关编号
                    Currency = swapedDecHead.Currency, //币种
                    DeclTotalSum = swapedDecHead.DeclTotalSum?.ToRound(2), //报关金额
                    DeclarePrice = swapedDecHead.DeclarePrice.ToRound(2), //委托金额
                    SwapedAmount = swapedDecHead.SwapedAmount?.ToRound(2), //换汇金额
                    SwapNoticeCreateDate = swapedDecHead.SwapNoticeCreateDate.ToString("yyyy-MM-dd"), //换汇日期
                    BankName = swapedDecHead.BankName, //换汇银行
                };

                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(dataList.Select(convert).ToArray().Json());

                string fileName = DateTime.Now.Ticks + ".xls";

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                #region 设置导出格式

                var excelconfig = new ExcelConfig();
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.Title = "已换汇";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DDate", ExcelColumn = "报关日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OwnerName", ExcelColumn = "客户名称", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConsignorCode", ExcelColumn = "境外发货人", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContrNo", ExcelColumn = "合同号", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单号", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "EntryId", ExcelColumn = "海关编号", Alignment = "left" });

                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclTotalSum", ExcelColumn = "报关金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclarePrice", ExcelColumn = "委托金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SwapedAmount", ExcelColumn = "换汇金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SwapNoticeCreateDate", ExcelColumn = "换汇日期", Alignment = "center" });

                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "BankName", ExcelColumn = "换汇银行", Alignment = "center" });

                #endregion

                //调用导出方法
                NPOIHelper.ExcelDownload(dt, excelconfig);

                Response.Write((new
                {
                    success = true,
                    message = "导出成功",
                    url = fileDic.FileUrl
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "导出失败：" + ex.Message,
                }).Json());
            }
        }

        protected void DownloadBillDetail()
        {
            string SwapNoticeID = Request.Form["SwapNoticeID"];
            var items = new Needs.Ccs.Services.Views.SwapDetailView().Where(item => item.SwapNoticeID == SwapNoticeID).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("ContrNo");
            dt.Columns.Add("OrderID");
            dt.Columns.Add("ClientName");
            dt.Columns.Add("Currency");
            dt.Columns.Add("RealExchangeRate");
            dt.Columns.Add("TransPremiumInsuranceAmount");
            dt.Columns.Add("TransPremiumInsuranceAmountRMB");
            dt.Columns.Add("AcceptanceCustomer");
            dt.Columns.Add("AcceptanceXDT");

            string currentDate = items[0].DDate.Value.ToString("yyyy-MM-dd");
            decimal? sumTransPremiumInsuranceAmount = 0, sumTransPremiumInsuranceAmountRMB = 0;
            decimal? sumAcceptanceCustomer = 0, sumAcceptanceXDT = 0;

            foreach (var t in items)
            {
                if (currentDate == t.DDate.Value.ToString("yyyy-MM-dd"))
                {
                    DataRow dr = dt.NewRow();
                    dr["ContrNo"] = t.ContrNo;
                    dr["OrderID"] = t.OrderID;
                    dr["ClientName"] = t.ClientName;
                    dr["Currency"] = t.Currency;
                    dr["RealExchangeRate"] = t.RealExchangeRate;
                    dr["TransPremiumInsuranceAmount"] = Math.Round(t.TransPremiumInsuranceAmount == null ? 0 : t.TransPremiumInsuranceAmount.Value, 2, MidpointRounding.AwayFromZero);
                    dr["TransPremiumInsuranceAmountRMB"] = Math.Round(t.TransPremiumInsuranceAmountRMB == null ? 0 : t.TransPremiumInsuranceAmountRMB.Value, 2, MidpointRounding.AwayFromZero);
                    dr["AcceptanceCustomer"] = Math.Round(t.AcceptanceCustomer == null ? 0 : t.AcceptanceCustomer.Value, 2, MidpointRounding.AwayFromZero);
                    dr["AcceptanceXDT"] = Math.Round(t.AcceptanceXDT == null ? 0 : t.AcceptanceXDT.Value, 2, MidpointRounding.AwayFromZero);
                    dt.Rows.Add(dr);


                    sumTransPremiumInsuranceAmount += t.TransPremiumInsuranceAmount;
                    sumTransPremiumInsuranceAmountRMB += t.TransPremiumInsuranceAmountRMB;
                    sumAcceptanceCustomer += t.AcceptanceCustomer;
                    sumAcceptanceXDT += t.AcceptanceXDT;
                }
                else
                {
                    DataRow drSum = dt.NewRow();
                    drSum["ContrNo"] = "合计";
                    drSum["OrderID"] = "";
                    drSum["ClientName"] = "";
                    drSum["Currency"] = "";
                    drSum["RealExchangeRate"] = "";
                    drSum["TransPremiumInsuranceAmount"] = Math.Round(sumTransPremiumInsuranceAmount == null ? 0 : sumTransPremiumInsuranceAmount.Value, 2, MidpointRounding.AwayFromZero);
                    drSum["TransPremiumInsuranceAmountRMB"] = Math.Round(sumTransPremiumInsuranceAmountRMB == null ? 0 : sumTransPremiumInsuranceAmountRMB.Value, 2, MidpointRounding.AwayFromZero);
                    drSum["AcceptanceCustomer"] = Math.Round(sumAcceptanceCustomer == null ? 0 : sumAcceptanceCustomer.Value, 2, MidpointRounding.AwayFromZero);
                    drSum["AcceptanceXDT"] = Math.Round(sumAcceptanceXDT == null ? 0 : sumAcceptanceXDT.Value, 2, MidpointRounding.AwayFromZero);
                    dt.Rows.Add(drSum);

                    DataRow dr = dt.NewRow();
                    dr["ContrNo"] = t.ContrNo;
                    dr["OrderID"] = t.OrderID;
                    dr["ClientName"] = t.ClientName;
                    dr["Currency"] = t.Currency;
                    dr["RealExchangeRate"] = t.RealExchangeRate;
                    dr["TransPremiumInsuranceAmount"] = Math.Round(t.TransPremiumInsuranceAmount == null ? 0 : t.TransPremiumInsuranceAmount.Value, 2, MidpointRounding.AwayFromZero);
                    dr["TransPremiumInsuranceAmountRMB"] = Math.Round(t.TransPremiumInsuranceAmountRMB == null ? 0 : t.TransPremiumInsuranceAmountRMB.Value, 2, MidpointRounding.AwayFromZero);
                    dr["AcceptanceCustomer"] = Math.Round(t.AcceptanceCustomer == null ? 0 : t.AcceptanceCustomer.Value, 2, MidpointRounding.AwayFromZero);
                    dr["AcceptanceXDT"] = Math.Round(t.AcceptanceXDT == null ? 0 : t.AcceptanceXDT.Value, 2, MidpointRounding.AwayFromZero);
                    dt.Rows.Add(dr);


                    sumTransPremiumInsuranceAmount = t.TransPremiumInsuranceAmount;
                    sumTransPremiumInsuranceAmountRMB = t.TransPremiumInsuranceAmountRMB;
                    sumAcceptanceCustomer = t.AcceptanceCustomer;
                    sumAcceptanceXDT = t.AcceptanceXDT;

                    currentDate = t.DDate.Value.ToString("yyyy-MM-dd");
                }


            }

            DataRow drLastSum = dt.NewRow();
            drLastSum["ContrNo"] = "合计";
            drLastSum["OrderID"] = "";
            drLastSum["ClientName"] = "";
            drLastSum["Currency"] = "";
            drLastSum["RealExchangeRate"] = "";
            drLastSum["TransPremiumInsuranceAmount"] = Math.Round(sumTransPremiumInsuranceAmount == null ? 0 : sumTransPremiumInsuranceAmount.Value, 2, MidpointRounding.AwayFromZero);
            drLastSum["TransPremiumInsuranceAmountRMB"] = Math.Round(sumTransPremiumInsuranceAmountRMB == null ? 0 : sumTransPremiumInsuranceAmountRMB.Value, 2, MidpointRounding.AwayFromZero);
            drLastSum["AcceptanceCustomer"] = Math.Round(sumAcceptanceCustomer == null ? 0 : sumAcceptanceCustomer.Value, 2, MidpointRounding.AwayFromZero);
            drLastSum["AcceptanceXDT"] = Math.Round(sumAcceptanceXDT == null ? 0 : sumAcceptanceXDT.Value, 2, MidpointRounding.AwayFromZero);
            dt.Rows.Add(drLastSum);

            string fileName = DateTime.Now.Ticks + ".xlsx";
            //创建文件目录
            FileDirectory fileDic = new FileDirectory(fileName);
            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
            fileDic.CreateDataDirectory();

            #region 设置导出格式
            var excelconfig = new ExcelConfig();
            excelconfig.FilePath = fileDic.FilePath;
            excelconfig.Title = "科目明细";
            excelconfig.TitleFont = "微软雅黑";
            excelconfig.TitlePoint = 16;
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
            excelconfig.ColumnEntity = listColumnEntity;
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContrNo", ExcelColumn = "合同号", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单编号", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientName", ExcelColumn = "客户名称", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "RealExchangeRate", ExcelColumn = "实时汇率", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TransPremiumInsuranceAmount", ExcelColumn = "运保杂", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TransPremiumInsuranceAmountRMB", ExcelColumn = "运保杂RMB", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AcceptanceCustomer", ExcelColumn = "汇兑-客户(借)", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AcceptanceXDT", ExcelColumn = "汇兑-华芯通(借)", Alignment = "center" });
            #endregion

            //调用导出方法
            NPOIHelper.ExcelDownload(dt, excelconfig);
            Response.Write((new
            {
                success = true,
                message = "导出成功",
                url = fileDic.FileUrl
            }).Json());
        }

        /// <summary>
        /// 导出财务做账报表
        /// </summary>
        protected void ExportSwapExcel()
        {

            string BankName = Request.Form["BankName"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];
            string ContrNo = Request.Form["ContrNo"];
            string EntryId = Request.Form["EntryId"];

            try
            {
                SwapDetailReport download = new SwapDetailReport();
                var url = download.Export(ContrNo, EntryId, BankName, StartDate, EndDate);

                Response.Write((new { success = true, message = "导出成功", url = url }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }
    }
}