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
    public partial class List : Uc.PageBase
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

            using (var query = new Needs.Ccs.Services.Views.FeeReportView())
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
                    int itype = Int32.Parse(FeeType);
                    view = view.SearchByType(itype);
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

                List<LambdaExpression> lamdas = new List<LambdaExpression>();
               
                if (!string.IsNullOrEmpty(StartTime))
                {
                    StartTime = StartTime.Trim();
                    DateTime dtStart = Convert.ToDateTime(StartTime);
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Models.FeeReport, bool>>)(t => t.Date >= dtStart));
                }

                if (!string.IsNullOrEmpty(EndTime))
                {
                    EndTime = EndTime.Trim();
                    DateTime dtEnd = Convert.ToDateTime(EndTime).AddDays(1);
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Models.FeeReport, bool>>)(t => t.Date <= dtEnd));
                }

                if (!string.IsNullOrEmpty(ClientName))
                {
                    ClientName = ClientName.Trim();
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Models.FeeReport, bool>>)(t => t.ClientName.Contains(ClientName)));
                }

                if (!string.IsNullOrEmpty(FeeType))
                {
                    FeeType = FeeType.Trim();
                    int itype = Int32.Parse(FeeType);
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Models.FeeReport, bool>>)(t => t.Type == (OrderPremiumType)itype));
                }


                var billsummary = new Needs.Ccs.Services.Views.FeeReportDownloadView().GetDownload(lamdas.ToArray()).OrderByDescending(t=>t.Date);

                Func<Needs.Ccs.Services.Models.FeeReport, object> convert = orderPremium => new
                {
                    ID = orderPremium.ID,
                    ClientName = orderPremium.ClientName,
                    Date = orderPremium.Date.ToString("yyyy-MM-dd"),
                    OrderID = orderPremium.OrderID,
                    Type = orderPremium.Type.GetDescription(),
                    ReceivableAmount = orderPremium.ReceivableAmount,
                    ReceivableCurrency = orderPremium.ReceivableCurrency,
                    PayableAmount = orderPremium.PayableAmount,
                    PayableCurrency = orderPremium.PayableCurrency,
                    PayableTaxedAmount = orderPremium.PayableTaxedAmount,                   
                    ReceiptsAmount = orderPremium.ReceiptsAmount,
                    OwedMoney = orderPremium.OwedMoney,
                    Discount = orderPremium.Discount,
                    FeeCreator = orderPremium.FeeCreator.RealName,
                    Salesman = orderPremium.Client.ServiceManager.RealName,
                    Mechandiser = orderPremium.Client.Merchandiser.RealName,
                };

                var billSummaryOrigin = billsummary.ToList();

                #region 设置导出格式

                DataTable dtBillSummary = new DataTable();
                dtBillSummary.Columns.Add("Date");
                dtBillSummary.Columns.Add("ClientName");
                dtBillSummary.Columns.Add("OrderID");
                dtBillSummary.Columns.Add("Type");
                dtBillSummary.Columns.Add("ReceivableAmount");
                dtBillSummary.Columns.Add("ReceivableCurrency");
                dtBillSummary.Columns.Add("PayableAmount");
                dtBillSummary.Columns.Add("PayableCurrency");
                dtBillSummary.Columns.Add("PayableTaxedAmount");
                dtBillSummary.Columns.Add("ReceiptsAmount");
                dtBillSummary.Columns.Add("OwedMoney");
                dtBillSummary.Columns.Add("Discount");
                dtBillSummary.Columns.Add("FeeCreator");
                dtBillSummary.Columns.Add("Mechandiser");
                dtBillSummary.Columns.Add("SalesMan");

                foreach (var orderPremium in billSummaryOrigin)
                {
                    DataRow dr = dtBillSummary.NewRow();
                    dr["Date"] = orderPremium.Date.ToString("yyyy-MM-dd");
                    dr["ClientName"] = orderPremium.ClientName;
                    dr["OrderID"] = orderPremium.OrderID;
                    dr["Type"] = orderPremium.Type.GetDescription();
                    dr["ReceivableAmount"] = orderPremium.ReceivableAmount;
                    dr["ReceivableCurrency"] = orderPremium.ReceivableCurrency;
                    dr["PayableAmount"] = orderPremium.PayableAmount;
                    dr["PayableCurrency"] = orderPremium.PayableCurrency;
                    dr["PayableTaxedAmount"] = orderPremium.PayableTaxedAmount;
                    dr["ReceiptsAmount"] = orderPremium.ReceiptsAmount;
                    dr["OwedMoney"] = orderPremium.OwedMoney;
                    dr["Discount"] = orderPremium.Discount;
                    dr["FeeCreator"] = orderPremium.FeeCreator.RealName;
                    dr["Mechandiser"] = orderPremium.Client.Merchandiser.RealName;
                    dr["SalesMan"] = orderPremium.Client.ServiceManager.RealName;
                    dtBillSummary.Rows.Add(dr);
                }

                //DataTable dtBillSummary = NPOIHelper.JsonToDataTable(billsummary.OrderByDescending(b => b.Date).Select(convert).ToArray().Json());

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
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Date", ExcelColumn = "日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientName", ExcelColumn = "客户名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Type", ExcelColumn = "费用科目明细", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ReceivableAmount", ExcelColumn = "应付未税金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ReceivableCurrency", ExcelColumn = "应付币制", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayableAmount", ExcelColumn = "应收未税金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayableCurrency", ExcelColumn = "应收币种", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayableTaxedAmount", ExcelColumn = "应收含税金额(CNY)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ReceiptsAmount", ExcelColumn = "实收金额(CNY)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OwedMoney", ExcelColumn = "欠款(CNY)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Discount", ExcelColumn = "费用优惠", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FeeCreator", ExcelColumn = "制单人", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Mechandiser", ExcelColumn = "跟单客服", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SalesMan", ExcelColumn = "业务员", Alignment = "center" });              

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