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

namespace WebApp.ReportManage.BillSummary
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);  
            int.TryParse(Request.QueryString["rows"], out rows);

            string StartTime = Request.QueryString["StartTime"];
            string EndTime = Request.QueryString["EndTime"];    
            string OrderID = Request.QueryString["OrderID"];
            string ClientName = Request.QueryString["ClientName"];
            var type = Request.QueryString["ClientType"];

            using (var query = new Needs.Ccs.Services.Views.ReportManage.BillSummaryView())
            { 
                var view = query;

                if (!string.IsNullOrWhiteSpace(OrderID))
                {
                    OrderID = OrderID.Trim();
                    view = view.SearchByOrderID(OrderID);
                }

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

                if (!string.IsNullOrEmpty(type))
                {
                    int itype = Int32.Parse(type);
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
                string OrderID = Request.Form["OrderID"];
                string ClientName = Request.Form["ClientName"];
                var type = Request.Form["ClientType"];

                var billsummary = new Needs.Ccs.Services.Views.ReportManage.BillSummaryView().AsQueryable();

                if (!string.IsNullOrEmpty(StartTime))
                {
                    billsummary = billsummary.Where(item => item.DDate.Value.CompareTo(StartTime) >= 0);
                }
                if (!string.IsNullOrEmpty(EndTime))
                {
                    var endTime = DateTime.Parse(EndTime).AddDays(1);
                    billsummary = billsummary.Where(item => item.DDate.Value.CompareTo(endTime) < 0);
                }
                if (!string.IsNullOrEmpty(OrderID))
                {
                    billsummary = billsummary.Where(item => item.OrderID.Contains(OrderID.Trim()));
                }
                if (!string.IsNullOrEmpty(ClientName))
                {
                    billsummary = billsummary.Where(item => item.ClientName.Contains(ClientName.Trim()));
                }
                if (!string.IsNullOrEmpty(type))
                {
                    int itype = Int32.Parse(type);
                    billsummary = billsummary.Where(item => item.ClientType == (ClientType)itype);
                }

                Func<Needs.Ccs.Services.Models.BillSummary, object> convert = billSummsry => new
                {
                    MainOrderID = billSummsry.MainOrderID,
                    OrderID = billSummsry.ID,
                    ClientName = billSummsry.ClientName,
                    ContrNo = billSummsry.ContrNo,
                    DDate = billSummsry.DDate.Value.ToString("yyyy-MM-dd"),
                    DeclarePrice = billSummsry.DeclarePrice.ToRound(2),
                    Currency = billSummsry.Currency,
                    RealExchangeRate = billSummsry.RealExchangeRate,
                    CustomsExchangeRate = billSummsry.CustomsExchangeRate,
                    RMBDeclarePrice = (billSummsry.RealExchangeRate * billSummsry.DeclarePrice).Value.ToRound(2),
                    AgencyRate = billSummsry.AgencyRate,
                    //AddedValueTax = billSummsry.AddedValueTax,修改为增值税金额小于50，显示为0 by 2020-09-27 yeshuangshuang
                    AddedValueTax = (int)billSummsry.ClientType == 1 ? billSummsry.AddedValueTax.Value.CompareTo(50) == -1 ? 0 : billSummsry.AddedValueTax : billSummsry.AddedValueTax,
                    Incidental = billSummsry.Incidental,
                    // Tariff = billSummsry.Tariff,修改为关税金额小于50，显示为0 by 2020-09-27 yeshuangshuang
                    Tariff = (int)billSummsry.ClientType == 1 ? billSummsry.Tariff.Value.CompareTo(50) == -1 ? 0 : billSummsry.Tariff : billSummsry.Tariff,
                    AgencyFee = billSummsry.AgencyFee,
                    TotalTariff = (((int)billSummsry.ClientType == 1 ? billSummsry.AddedValueTax.Value.CompareTo(50) == -1 ? 0 : billSummsry.AddedValueTax : billSummsry.AddedValueTax) + billSummsry.Incidental + ((int)billSummsry.ClientType == 1 ? billSummsry.Tariff.Value.CompareTo(50) == -1 ? 0 : billSummsry.Tariff : billSummsry.Tariff) + billSummsry.AgencyFee).Value.ToRound(2),
                    TotalDeclare = ((billSummsry.RealExchangeRate * billSummsry.DeclarePrice) + (((int)billSummsry.ClientType == 1 ? billSummsry.AddedValueTax.Value.CompareTo(50) == -1 ? 0 : billSummsry.AddedValueTax : billSummsry.AddedValueTax) + billSummsry.Incidental + ((int)billSummsry.ClientType == 1 ? billSummsry.Tariff.Value.CompareTo(50) == -1 ? 0 : billSummsry.Tariff : billSummsry.Tariff) + billSummsry.AgencyFee)).Value.ToRound(2),
                    SupplierName = billSummsry.SupplierName,
                    InvoiceType = billSummsry.InvoiceType.GetDescription()

                };

                #region 设置导出格式

                DataTable dtBillSummary = NPOIHelper.JsonToDataTable(billsummary.OrderBy(b => b.MainOrderID).Select(convert).ToArray().Json());

                string fileName = "账单汇总" + DateTime.Now.Ticks + ".xls";
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                var excelconfig = new ExcelConfig();
                excelconfig.AutoMergedColumn = 0;//合并相同列,参数为0
                excelconfig.Title = "账单汇总";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MainOrderID", ExcelColumn = "主订单编号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单编号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientName", ExcelColumn = "客户名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContrNo", ExcelColumn = "报关合同号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DDate", ExcelColumn = "报关日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclarePrice", ExcelColumn = "货值", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "RMBDeclarePrice", ExcelColumn = "货值(RMB)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "RealExchangeRate", ExcelColumn = "实时汇率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CustomsExchangeRate", ExcelColumn = "海关汇率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AgencyRate", ExcelColumn = "代理费率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AgencyFee", ExcelColumn = "代理费", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AddedValueTax", ExcelColumn = "增值税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Tariff", ExcelColumn = "关税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Incidental", ExcelColumn = "杂费", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalTariff", ExcelColumn = "税费合计", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalDeclare", ExcelColumn = "报关总金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SupplierName", ExcelColumn = "交货供应商", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceType", ExcelColumn = "开票类型", Alignment = "center" });

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