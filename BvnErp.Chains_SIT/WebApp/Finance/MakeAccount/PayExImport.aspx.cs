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

namespace WebApp.Finance.MakeAccount
{
    public partial class PayExImport : Uc.PageBase
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
                view = view.SearchBySwapCreSta(false);
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

        protected void MakeAccount()
        {
            string BankName = Request.Form["BankName"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];
            string ContrNo = Request.Form["ContrNo"];
            string EntryId = Request.Form["EntryId"];

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime from = DateTime.Parse(StartDate);
                DateTime to = DateTime.Parse(EndDate).AddDays(1);
                TimeSpan day = to.Subtract(from);
                if (day.TotalDays > 31)
                {
                    Response.Write((new { success = false, msg = "不能一次生成超一个月的数据" }).Json());
                    return;
                }
            }
            else
            {
                Response.Write((new { success = false, msg = "必须勾选开始结束日期" }).Json());
                return;
            }

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            var model = Model.JsonTo<List<mk>>();

            List<string> swapNoticeIDs = new List<string>();
            foreach (var item in model)
            {
                swapNoticeIDs.Add(item.ID);
            }

            var result = new Needs.Ccs.Services.Models.SwapImport().Make(ContrNo, EntryId, BankName, StartDate, EndDate, swapNoticeIDs);

            Response.Write((new { success = result }).Json());

        }

        public class mk
        {
            public string ID { get; set; }
        }

        protected void MakeAccountAll()
        {
            string BankName = Request.Form["BankName"];
            string startDate = Request.Form["StartDate"];
            string endDate = Request.Form["EndDate"];
            string ContrNo = Request.Form["ContrNo"];
            string EntryId = Request.Form["EntryId"];

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime from = DateTime.Parse(startDate);
                DateTime to = DateTime.Parse(endDate).AddDays(1);
                TimeSpan day = to.Subtract(from);
                if (day.TotalDays > 31)
                {
                    Response.Write((new { success = false, msg = "不能一次生成超一个月的数据" }).Json());
                    return;
                }
            }
            else
            {
                Response.Write((new { success = false, msg = "必须勾选开始结束日期" }).Json());
                return;
            }

            List<string> swapNoticeIDs = new List<string>();
            var result = new Needs.Ccs.Services.Models.SwapImport().Make(ContrNo, EntryId, BankName, startDate, endDate, swapNoticeIDs);
            Response.Write((new { success = result }).Json());
        }
    }
}