using Needs.Utils;
using Needs.Utils.Serializers;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using WebApp.Ccs.Utils;
using Needs.Ccs.Services.Enums;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Descriptions;
using Needs.Ccs.Services;
using System.Linq.Expressions;
namespace WebApp.Finance.Report
{
    public partial class PayExDetailList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string ClientName = Request.QueryString["ClientName"];
            string OrderID = Request.QueryString["OrderID"];
            string ContrNo = Request.QueryString["ContrNo"];

            using (var query = new Needs.Ccs.Services.Views.PayExchangeDetailView())
            {
                var view = query;              
                if (!string.IsNullOrEmpty(ClientName))
                {
                    view = view.SearchByClientName(ClientName);
                }

                if (!string.IsNullOrEmpty(OrderID))
                {
                    view = view.SearchByOrderID(OrderID);
                }

                if (!string.IsNullOrEmpty(ContrNo))
                {
                    view = view.SearchByContrNo(ContrNo);
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    StartDate = StartDate.Trim();
                    var from = DateTime.Parse(StartDate);
                    view = view.SearchByFrom(from);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    EndDate = EndDate.Trim();
                    var to = DateTime.Parse(EndDate).AddDays(1);
                    view = view.SearchByTo(to);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        protected void Export()
        {
            try
            {
                string StartDate = Request.QueryString["StartDate"];
                string EndDate = Request.QueryString["EndDate"];
                string ClientName = Request.QueryString["ClientName"];
                string OrderID = Request.QueryString["OrderID"];
                string ContrNo = Request.QueryString["ContrNo"];

                List<Needs.Ccs.Services.Models.PayExchangeDetail> datas = new List<Needs.Ccs.Services.Models.PayExchangeDetail>();

                using (var query = new Needs.Ccs.Services.Views.PayExchangeDetailView())
                {
                    var view = query;
                    if (!string.IsNullOrEmpty(ClientName))
                    {
                        view = view.SearchByClientName(ClientName);
                    }

                    if (!string.IsNullOrEmpty(OrderID))
                    {
                        view = view.SearchByOrderID(OrderID);
                    }

                    if (!string.IsNullOrEmpty(ContrNo))
                    {
                        view = view.SearchByContrNo(ContrNo);
                    }

                    if (!string.IsNullOrEmpty(StartDate))
                    {
                        StartDate = StartDate.Trim();
                        var from = DateTime.Parse(StartDate);
                        view = view.SearchByFrom(from);
                    }

                    if (!string.IsNullOrEmpty(EndDate))
                    {
                        EndDate = EndDate.Trim();
                        var to = DateTime.Parse(EndDate).AddDays(1);
                        view = view.SearchByTo(to);
                    }

                    datas = view.ToList();
                }

 
                #region 设置导出格式

                DataTable dtBillSummary = new DataTable();
                dtBillSummary.Columns.Add("PayExchangeApplyID");
                dtBillSummary.Columns.Add("ClientName");
                dtBillSummary.Columns.Add("OrderID");
                dtBillSummary.Columns.Add("ContrNo");               
                dtBillSummary.Columns.Add("Amount");
                dtBillSummary.Columns.Add("PayExchangeRate");
                dtBillSummary.Columns.Add("AmountRMB");
                dtBillSummary.Columns.Add("Currency");
                dtBillSummary.Columns.Add("CreateDate");
                dtBillSummary.Columns.Add("SupplierName");


                foreach (var orderPremium in datas)
                {
                    DataRow dr = dtBillSummary.NewRow();
                    dr["PayExchangeApplyID"] = orderPremium.PayExchangeApplyID;
                    dr["ClientName"] = orderPremium.ClientName;
                    dr["OrderID"] = orderPremium.OrderID;
                    dr["ContrNo"] = orderPremium.ContrNo;
                    dr["Amount"] = orderPremium.Amount;
                    dr["PayExchangeRate"] = orderPremium.PayExchangeRate;
                    dr["AmountRMB"] = orderPremium.AmountRMB;
                    dr["Currency"] = orderPremium.Currency;
                    dr["CreateDate"] = orderPremium.CreateDate.ToString("yyyy-MM-dd");
                    dr["SupplierName"] = orderPremium.SupplierName;
                    dtBillSummary.Rows.Add(dr);
                }

                string fileName = "付汇数据" + ClientName+ "_" + StartDate + "_" + EndDate + "_" + ".xls";
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                var excelconfig = new ExcelConfig();
                excelconfig.AutoMergedColumn = 0;//合并相同列,参数为0
                excelconfig.Title = "付汇数据" +  ClientName + "_" + StartDate + "_" + EndDate;
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayExchangeApplyID", ExcelColumn = "付汇编号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientName", ExcelColumn = "客户名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContrNo", ExcelColumn = "合同号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Amount", ExcelColumn = "付汇金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayExchangeRate", ExcelColumn = "付汇汇率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AmountRMB", ExcelColumn = "付汇RMB", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });                        
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CreateDate", ExcelColumn = "申请时间", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SupplierName", ExcelColumn = "供应商", Alignment = "center" });
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