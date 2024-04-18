using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
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

namespace WebApp.GoodsBill
{
    public partial class OutputDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string InDateFrom = Request.QueryString["InDateFrom"];
            string InDateTo = Request.QueryString["InDateTo"];
            string ClientName = Request.QueryString["ClientName"];
            string Model = Request.QueryString["Model"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            using (var query = new Needs.Ccs.Services.Views.OutputView())
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(InDateFrom))
                {
                    InDateFrom = InDateFrom.Trim();
                    DateTime dtFrom = Convert.ToDateTime(InDateFrom);
                    view = view.SearchByFrom(dtFrom);
                }

                if (!string.IsNullOrWhiteSpace(InDateTo))
                {
                    InDateTo = InDateTo.Trim();
                    DateTime dtTo = Convert.ToDateTime(InDateTo).AddDays(1);
                    view = view.SearchByTo(dtTo);
                }
                else
                {
                    DateTime dtTo = DateTime.Now.Date.AddDays(-1);
                    view = view.SearchByTo(dtTo);
                }

                if (!string.IsNullOrWhiteSpace(ClientName))
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByClientName(ClientName);
                }

                if (!string.IsNullOrWhiteSpace(Model))
                {
                    Model = Model.Trim();
                    view = view.SearchByModel(Model);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        protected void Download()
        {
            string InDateFrom = Request.Form["InDateFrom"];
            string InDateTo = Request.Form["InDateTo"];

            InDateFrom = InDateFrom.Trim();
            DateTime dtFrom = Convert.ToDateTime(InDateFrom);

            InDateTo = InDateTo.Trim();
            DateTime dtTo = Convert.ToDateTime(InDateTo).AddDays(1);

            TimeSpan timeSpan = dtTo - dtFrom;
            if (timeSpan.Days > 31)
            {
                Response.Write((new
                {
                    success = false,
                    message = "只能导出一个月之内的数据",

                }).Json());
            }
            else
            {
                //Stopwatch stopwatch1 = new Stopwatch();
                //stopwatch1.Start();               
                var InputView = new Needs.Ccs.Services.Views.OutputViewDownload().Where(t => t.OutStoreDate > dtFrom && t.OutStoreDate < dtTo).ToArray();
                //stopwatch1.Stop();
                //string s1 = stopwatch1.ElapsedMilliseconds.ToString();

                //Stopwatch stopwatch2 = new Stopwatch();
                //stopwatch2.Start();
                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("OutStoreDate");
                dt.Columns.Add("InvoiceDate");
                dt.Columns.Add("InvoiceType");
                dt.Columns.Add("ContrNo");
                dt.Columns.Add("GName");
                dt.Columns.Add("GoodsBrand");
                dt.Columns.Add("GoodsModel");
                dt.Columns.Add("InvoiceQty");
                dt.Columns.Add("GQty");
                dt.Columns.Add("Gunit");              
                dt.Columns.Add("PurchasingPrice");
                dt.Columns.Add("TaxedPrice");
                dt.Columns.Add("TradeCurr");               
                dt.Columns.Add("OwnerName");
                dt.Columns.Add("TaxName");
                dt.Columns.Add("TaxCode");
                dt.Columns.Add("OperatorID");
                dt.Columns.Add("SalesPrice");
                dt.Columns.Add("InvoicePrice");
                dt.Columns.Add("InvoiceNo");

                foreach (var item in InputView)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = item.ID;
                    dr["OutStoreDate"] = item.OutStoreDate;
                    dr["ContrNo"] = item.ContrNo;
                    dr["GName"] = item.GName;
                    dr["GoodsBrand"] = item.GoodsBrand;
                    dr["GoodsModel"] = item.GoodsModel;
                    dr["InvoiceQty"] = item.OutQty;
                    dr["GQty"] = item.OutQty;
                    dr["Gunit"] = item.Gunit;                  
                    dr["PurchasingPrice"] = item.PurchasingPrice;
                    dr["TaxedPrice"] =  Math.Round((item.OutQty.Value/item.GQty)*item.TaxedPrice.Value,2,MidpointRounding.AwayFromZero);
                    dr["TradeCurr"] = item.TradeCurr;                  
                    dr["OwnerName"] = item.OwnerName;
                    dr["TaxName"] = item.TaxName;
                    dr["TaxCode"] = item.TaxCode;
                    dr["OperatorID"] = item.OperatorID;
                   
                    if (item.InvoiceType == 1)
                    {
                        dr["InvoiceDate"] = "";
                        dr["InvoiceType"] = "服务费发票";
                        dr["SalesPrice"] = "";
                        dr["InvoicePrice"] = "";
                        dr["InvoiceNo"] = "";
                    }
                    else
                    {
                        dr["InvoiceDate"] = item.InvoiceDate;
                        dr["InvoiceType"] = "全额发票";
                        dr["SalesPrice"] = item.SalesPrice;
                        dr["InvoicePrice"] = Math.Round((item.OutQty.Value / item.GQty)*item.DeclTotal * item.RealExchangeRate.Value, 4);
                        dr["InvoiceNo"] = item.InvoiceNo;
                    }
                    dt.Rows.Add(dr);
                }

                //stopwatch2.Stop();
                //string s2 = stopwatch2.ElapsedMilliseconds.ToString();
                //int count = dt.Rows.Count;

                try
                {
                    string fileName = DateTime.Now.Ticks + ".xlsx";

                    //创建文件目录
                    FileDirectory fileDic = new FileDirectory(fileName);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                    fileDic.CreateDataDirectory();

                    //设置导出格式
                    var excelconfig = new ExcelConfig();
                    excelconfig.FilePath = fileDic.FilePath;
                    excelconfig.Title = "出库数据";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 16;
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ID", ExcelColumn = "编号", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OutStoreDate", ExcelColumn = "出库日期", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceDate", ExcelColumn = "开票日期", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceType", ExcelColumn = "发票类型", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContrNo", ExcelColumn = "合同号", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GName", ExcelColumn = "报关品名", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsBrand", ExcelColumn = "品牌", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsModel", ExcelColumn = "型号", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceQty", ExcelColumn = "发票数量", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GQty", ExcelColumn = "数量", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Gunit", ExcelColumn = "单位", Alignment = "center" });                   
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PurchasingPrice", ExcelColumn = "进价", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxedPrice", ExcelColumn = "完税价格", Alignment = "center" });
                    //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TradeCurr", ExcelColumn = "币制" });                  
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OwnerName", ExcelColumn = "客户名称", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxName", ExcelColumn = "税务名称", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxCode", ExcelColumn = "税务编码", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OperatorID", ExcelColumn = "出库人", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SalesPrice", ExcelColumn = "售价", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoicePrice", ExcelColumn = "开票金额", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceNo", ExcelColumn = "发票号", Alignment = "center" });
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
                    string xx = ex.ToString();
                }





            }

        }
    }
}