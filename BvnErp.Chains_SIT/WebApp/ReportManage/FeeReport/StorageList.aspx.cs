using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.ReportManage.FeeReport
{
    public partial class StorageList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.OrderPremiumType = Needs.Utils.Descriptions.EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderPremiumType>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
            }
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string StartTime = Request.QueryString["StartTime"];
            string EndTime = Request.QueryString["EndTime"];
            string ClientName = Request.QueryString["ClientName"];
            var FeeType = Request.QueryString["FeeType"];

            using (var query = new Needs.Ccs.Services.Views.StorageFeeReportView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(ClientName))
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByOwnerName(ClientName);
                }

                if (!string.IsNullOrEmpty(StartTime))
                {
                    DateTime start = Convert.ToDateTime(StartTime);
                    view = view.SearchByStartDate(start);
                }
                if (!string.IsNullOrEmpty(EndTime))
                {
                    DateTime end = Convert.ToDateTime(EndTime).AddDays(1);
                    view = view.SearchByEndDate(end);
                }

                if (!string.IsNullOrEmpty(FeeType))
                {
                    FeeType = FeeType.Trim();
                    view = view.SearchByType(FeeType);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        protected void ExportExcel()
        {
            try
            {
                string StartTime = Request.Form["StartTime"];
                string EndTime = Request.Form["EndTime"];
                string ClientName = Request.Form["ClientName"];
                var FeeType = Request.Form["FeeType"];

                var billsummary = new Needs.Ccs.Services.Views.StorageFeeReportView().AsQueryable();

                if (!string.IsNullOrEmpty(StartTime))
                {
                    billsummary = billsummary.Where(item => item.CreateTime.CompareTo(StartTime) >= 0);
                }
                if (!string.IsNullOrEmpty(EndTime))
                {
                    var endTime = DateTime.Parse(EndTime).AddDays(1);
                    billsummary = billsummary.Where(item => item.CreateTime.CompareTo(endTime) < 0);
                }

                if (!string.IsNullOrEmpty(ClientName))
                {
                    billsummary = billsummary.Where(item => item.payCompanyName.Contains(ClientName.Trim()));
                }
                if (!string.IsNullOrEmpty(FeeType))
                {                  
                    billsummary = billsummary.Where(item => item.Subject.Contains(FeeType.Trim()));
                }

                Func<Needs.Ccs.Services.Models.StorageFeeReport, object> convert = orderPremium => new
                {
                    ID = orderPremium.OrderID,
                    OrderID = orderPremium.OrderID,
                    Business = orderPremium.Business,
                    Catalog = orderPremium.Catalog,
                    Subject = orderPremium.Subject,
                    ReceivableAmount = orderPremium.ReceivableAmount,
                    ReceivableCNYAmount = orderPremium.ReceivableCNYAmount==null?"0": orderPremium.ReceivableCNYAmount.Value.ToString("0.00"),
                    PayableAmount = Math.Round(orderPremium.PayableAmount,2),
                    PayableTaxedAmount = orderPremium.PayableTaxedAmount,
                    CreateTime = orderPremium.CreateTime.ToString("yyyy-MM-dd"),
                    ReceiptsAmount = orderPremium.ReceiptsAmount,
                    Discount = orderPremium.Discount,
                    OwedMoney = orderPremium.OwedMoney,
                    AdminName = orderPremium.AdminName,
                    Currency = orderPremium.Currency,
                    payCompanyName = orderPremium.payCompanyName,
                    recCompanyName = orderPremium.recCompanyName,
                };

                var billSummaryOrigin = billsummary.OrderByDescending(t => t.CreateTime).ToList();

                #region 设置导出格式
                DataTable dtBillSummary = new DataTable();
                dtBillSummary.Columns.Add("CreateTime");
                dtBillSummary.Columns.Add("payCompanyName");
                dtBillSummary.Columns.Add("OrderID");
                dtBillSummary.Columns.Add("Subject");
                dtBillSummary.Columns.Add("ReceivableAmount");
                dtBillSummary.Columns.Add("ReceivableCNYAmount");
                dtBillSummary.Columns.Add("PayableAmount");
                dtBillSummary.Columns.Add("PayableTaxedAmount");
                dtBillSummary.Columns.Add("ReceiptsAmount");
                dtBillSummary.Columns.Add("OwedMoney");
                dtBillSummary.Columns.Add("Discount");
                dtBillSummary.Columns.Add("AdminName");

                foreach(var orderPremium in billSummaryOrigin)
                {
                    DataRow dr = dtBillSummary.NewRow();
                    dr["CreateTime"] = orderPremium.CreateTime.ToString("yyyy-MM-dd");
                    dr["payCompanyName"] = orderPremium.payCompanyName;
                    dr["OrderID"] = orderPremium.OrderID;
                    dr["Subject"] = orderPremium.Subject;
                    dr["ReceivableAmount"] = orderPremium.ReceivableAmount;
                    dr["ReceivableCNYAmount"] = orderPremium.ReceivableCNYAmount == null ? "0" : orderPremium.ReceivableCNYAmount.Value.ToString("0.00");
                    dr["PayableAmount"] = Math.Round(orderPremium.PayableAmount, 2);
                    dr["PayableTaxedAmount"] = orderPremium.PayableTaxedAmount;
                    dr["ReceiptsAmount"] = orderPremium.ReceiptsAmount==null?0:Math.Round(orderPremium.ReceiptsAmount.Value,2);
                    dr["OwedMoney"] = orderPremium.OwedMoney;
                    dr["Discount"] = orderPremium.Discount;
                    dr["AdminName"] = orderPremium.AdminName;
                    dtBillSummary.Rows.Add(dr);
                }

                //DataTable dtBillSummary = NPOIHelper.JsonToDataTable(billsummary.OrderByDescending(b => b.CreateTime).Select(convert).ToArray().Json());

                string fileName = "费用汇总" + DateTime.Now.Ticks + ".xls";
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                var excelconfig = new ExcelConfig();
                excelconfig.AutoMergedColumn = 0;//合并相同列,参数为0
                excelconfig.Title = "费用汇总";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CreateTime", ExcelColumn = "日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "payCompanyName", ExcelColumn = "客户名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Subject", ExcelColumn = "费用科目明细", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ReceivableAmount", ExcelColumn = "应付金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ReceivableCNYAmount", ExcelColumn = "应付金额(CNY)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayableAmount", ExcelColumn = "应收金额(CNY)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayableTaxedAmount", ExcelColumn = "应收含税金额(CNY)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ReceiptsAmount", ExcelColumn = "实收金额(CNY)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OwedMoney", ExcelColumn = "欠款(CNY)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Discount", ExcelColumn = "费用优惠(CNY)", Alignment = "center" });             
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AdminName", ExcelColumn = "制单人", Alignment = "center" });            

                #endregion

                //调用导出方法
                NPOIHelper.ExcelDownload(dtBillSummary, excelconfig);

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
    }
}