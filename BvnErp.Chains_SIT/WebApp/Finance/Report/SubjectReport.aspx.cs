using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance.Report
{
    public partial class SubjectReport : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data1()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            string ContrNo = Request.QueryString["ContrNo"];
            string ClientName = Request.QueryString["ClientName"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(ContrNo))
            {
                ContrNo = ContrNo.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Models.SubjectReportItem, bool>>)(t => t.ContrNo == ContrNo));
            }

            if (!string.IsNullOrEmpty(ClientName))
            {
                ClientName = ClientName.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Models.SubjectReportItem, bool>>)(t => t.ClientName == ClientName));
            }

            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                DateTime dtStart = Convert.ToDateTime(StartDate);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Models.SubjectReportItem, bool>>)(t => t.DeclareDate >= dtStart));
            }

            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Models.SubjectReportItem, bool>>)(t => t.DeclareDate <= dtEnd));
            }


            int count = 0;
            var subjectList = new Needs.Ccs.Services.Views.SubjectReportView().GetResult(out count, page, rows, lamdas.ToArray()).OrderByDescending(t => t.DeclareDate);

            Func<Needs.Ccs.Services.Models.SubjectReportItem, object> convert = t => new
            {
                DeclareDate = t.DeclareDate?.ToString("yyyy-MM-dd"),
                ContrNo = t.ContrNo,
                DecForeignTotal = t.DecForeignTotal,
                DecAgentTotal = t.DecAgentTotal,
                DecYunBaoZaTotal = t.DecYunBaoZaTotal,
                DecTotalPriceRMB = t.DecTotalPriceRMB,
                ImportPrice = t.ImportPrice,
                SalePrice = t.SalePrice,
                Tariff = t.Tariff,
                ActualExciseTax = t.ActualExciseTax == null ? 0 : t.ActualExciseTax.Value,
                ActualAddedValueTax = t.ActualAddedValueTax == null ? 0 : t.ActualAddedValueTax.Value,
                ExchangeCustomer = t.ExchangeCustomer,
                ExchangeXDT = t.ExchangeXDT,
                RealExchangeRate = t.RealExchangeRate,
                DueCustomerFC = t.DueCustomerFC,
                DueCustomerRMB = t.DueCustomerRMB,
                DueXDTFC = t.DueXDTFC,
                DueXDTRMB = t.DueXDTRMB,
                ActualTariff = t.ActualTariff == null ? 0 : t.ActualTariff.Value,
                DutiablePrice = t.DutiablePrice,
                ClientName = t.ClientName,
                InvoiceTypeName = t.InvoiceTypeName,
                ConsignorCode = t.ConsignorCode,
                InvoiceNo = string.IsNullOrEmpty(t.InvoiceNo) ? "-" : t.InvoiceNo,
                InvoiceDate = t.InvoiceDate == null ? "-" : t.InvoiceDate?.ToString("yyyy-MM-dd")
            };

            Response.Write(new
            {
                rows = subjectList.Select(convert).ToArray(),
                total = count,
            }.Json());
        }

        protected void data()
        {

            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            string ContrNo = Request.QueryString["ContrNo"];
            string ClientName = Request.QueryString["ClientName"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            using (var query = new Needs.Ccs.Services.Views.SubjectReportViewNew())
            {
                var view = query;


                if (!string.IsNullOrEmpty(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    view = view.SearchByContrNo(ContrNo);
                }

                if (!string.IsNullOrEmpty(ClientName))
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByName(ClientName);
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    StartDate = StartDate.Trim();
                    DateTime dtStart = Convert.ToDateTime(StartDate);
                    view = view.SearchByFrom(dtStart);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    EndDate = EndDate.Trim();
                    DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByTo(dtEnd);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }

        }


        protected void Export()
        {
            string ContrNo = Request.Form["ContrNo"];
            string ClientName = Request.QueryString["ClientName"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];


            using (var query = new Needs.Ccs.Services.Views.SubjectReportViewNew())
            {
                var view = query;


                if (!string.IsNullOrEmpty(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    view = view.SearchByContrNo(ContrNo);
                }

                if (!string.IsNullOrEmpty(ClientName))
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByName(ClientName);
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    StartDate = StartDate.Trim();
                    DateTime dtStart = Convert.ToDateTime(StartDate);
                    view = view.SearchByFrom(dtStart);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    EndDate = EndDate.Trim();
                    DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByTo(dtEnd);
                }

                // Response.Write(view.ToMyPage(page, rows).Json());
                var result = view.ToMyPage(null, null);

                //if (result.Count() == 0)
                //{
                //    Response.Write((new
                //    {
                //        success = false,
                //        message = "暂无数据导出"
                //    }).Json());
                //    return;
                //}

                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(result.Json());
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
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientName", ExcelColumn = "客户名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceTypeName", ExcelColumn = "开票类型", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConsignorCode", ExcelColumn = "供应商", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclareDate", ExcelColumn = "报关日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContrNo", ExcelColumn = "合同号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DecForeignTotal", ExcelColumn = "报关外币", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DecAgentTotal", ExcelColumn = "委托外币", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DecYunBaoZaTotal", ExcelColumn = "运保杂外币", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DecTotalPriceRMB", ExcelColumn = "报关总价", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ImportPrice", ExcelColumn = "进口", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SalePrice", ExcelColumn = "运保杂", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Tariff", ExcelColumn = "应交关税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ActualExciseTax", ExcelColumn = "实缴消费税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ActualAddedValueTax", ExcelColumn = "实缴增值税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ExchangeCustomer", ExcelColumn = "汇兑-客户", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ExchangeXDT", ExcelColumn = "汇兑-华芯通", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "RealExchangeRate", ExcelColumn = "实时汇率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DueCustomerFC", ExcelColumn = "应付-客户外币", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DueCustomerRMB", ExcelColumn = "应付-客户RMB", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DueXDTFC", ExcelColumn = "应付-华芯通外币", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DueXDTRMB", ExcelColumn = "应付-华芯通RMB", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ActualTariff", ExcelColumn = "实交关税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DutiablePrice", ExcelColumn = "完税价格", Alignment = "center" });
                //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceNo", ExcelColumn = "发票号", Alignment = "center" });
                //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceDate", ExcelColumn = "开票日期", Alignment = "center" });
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
        }




        //ExportStatistics
        protected void ExportStatistics()
        {
            string ContrNo = Request.Form["ContrNo"];
            string ClientName = Request.QueryString["ClientName"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];


            using (var query = new Needs.Ccs.Services.Views.SubjectReportViewNew())
            {
                var view = query;


                if (!string.IsNullOrEmpty(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    view = view.SearchByContrNo(ContrNo);
                }

                if (!string.IsNullOrEmpty(ClientName))
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByName(ClientName);
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    StartDate = StartDate.Trim();
                    DateTime dtStart = Convert.ToDateTime(StartDate);
                    view = view.SearchByFrom(dtStart);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    EndDate = EndDate.Trim();
                    DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByTo(dtEnd);
                }

                // Response.Write(view.ToMyPage(page, rows).Json());
                var result = view.ToMyPage(null, null, true);

                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(result.Json());
                string fileName = DateTime.Now.Ticks + ".xlsx";
                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                #region 设置导出格式
                var excelconfig = new ExcelConfig();
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.Title = "科目汇总";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclareDate", ExcelColumn = "报关日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Tian", ExcelColumn = "天", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ImportPrice", ExcelColumn = "委托报关（RMB)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "YunBaoZa", ExcelColumn = "运保杂", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Tariff", ExcelColumn = "应交关税RMB", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ActualTariff", ExcelColumn = "实交关税RMB", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ExciseTax", ExcelColumn = "消费税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ActualExciseTax", ExcelColumn = "消费税实缴", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ActualAddedValueTax", ExcelColumn = "增值税RMB", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ExchangeCustomer", ExcelColumn = "委托金额-汇兑", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientName", ExcelColumn = "公司", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ExchangeXDT", ExcelColumn = "运保杂-汇兑", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "RealExchangeRate", ExcelColumn = "汇率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DecAgentTotal", ExcelColumn = "委托金额usd", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConsignorCode", ExcelColumn = "物流方公司", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DecYunBaoZaTotal", ExcelColumn = "运保杂-usd", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceTypeName", ExcelColumn = "开票类型", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Check", ExcelColumn = "检验", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Addition1", ExcelColumn = "留空", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Tian1", ExcelColumn = "天", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DecAgentTotal1", ExcelColumn = "委托金额usd", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ActualTariff1", ExcelColumn = "实交关税RMB", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Addition2", ExcelColumn = "留空", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ExchangeCustomerOpposite", ExcelColumn = "委托金额-汇兑", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ImportPrice1", ExcelColumn = "委托报关（RMB)", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Tariff1", ExcelColumn = "应交关税RMB", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Addition3", ExcelColumn = "留空", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientName1", ExcelColumn = "公司", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "RealExchangeRate1", ExcelColumn = "汇率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Check1", ExcelColumn = "检验", Alignment = "center" });
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
        }


        private string GetInvoiceTypeName(Needs.Ccs.Services.Enums.InvoiceType invoiceType)
        {
            string invoiceTypeName = "";

            switch (invoiceType)
            {
                case Needs.Ccs.Services.Enums.InvoiceType.Full:
                    invoiceTypeName = "单抬头";
                    break;
                case Needs.Ccs.Services.Enums.InvoiceType.Service:
                    invoiceTypeName = "双抬头";
                    break;
                default:
                    break;
            }

            return invoiceTypeName;
        }

    }
}