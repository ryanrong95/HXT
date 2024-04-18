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
    public partial class OutGoodsSummary : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string currentDate = Request.QueryString["currentDate"];
            string previousDate = Request.QueryString["previousDate"];
            string OwnerName = Request.QueryString["OwnerName"];


            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            using (var query = new Needs.Ccs.Services.Views.OutGoodsDetailView())
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(currentDate))
                {
                    currentDate = currentDate.Trim() + "-01";
                    DateTime dtFrom = Convert.ToDateTime(currentDate);
                    view = view.SearchByCurrent(dtFrom);
                }

                if (!string.IsNullOrWhiteSpace(previousDate))
                {
                    previousDate = previousDate.Trim() + "-01"; ;
                    DateTime dtTo = Convert.ToDateTime(previousDate);
                    view = view.SearchByPrevious(dtTo);
                }

                if (!string.IsNullOrEmpty(OwnerName))
                {
                    OwnerName = OwnerName.Trim();
                    view = view.SearchByOwnerName(OwnerName);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        protected void Download()
        {
            string currentDate = Request.Form["currentDate"];
            string previousDate = Request.Form["previousDate"];

            if (string.IsNullOrEmpty(currentDate) && string.IsNullOrEmpty(previousDate))
            {
                Response.Write((new
                {
                    success = false,
                    message = "请选择一个时间范围",

                }).Json());
                return;
            }

            Func<Needs.Ccs.Services.Models.OutStoreViewModel, object> convert = item => new
            {
                item.ID,
                item.OutStoreDate,
                item.InvoiceDate,
                item.GName,
                item.GoodsBrand,
                item.GoodsModel,
                item.GQty,
                item.Gunit,
                item.DeclPrice,
                item.PurchasingPrice,
                item.TaxedPrice,
                item.TradeCurr,
                item.OwnerName,
                item.TaxCode,
                item.TaxName,
                item.OperatorID,
                item.SalesPrice,
                item.InvoicePrice,
                item.InvoiceNo,
            };


            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            //dt.Columns.Add("OutStoreDate");
            dt.Columns.Add("InvoiceDate");
            dt.Columns.Add("GName");
            dt.Columns.Add("GoodsBrand");
            dt.Columns.Add("GoodsModel");
            dt.Columns.Add("InvoiceQty");
            dt.Columns.Add("GQty");
            dt.Columns.Add("Gunit");
            dt.Columns.Add("PurchasingPrice");
            dt.Columns.Add("TaxedPrice");
            //dt.Columns.Add("TradeCurr");
            dt.Columns.Add("OwnerName");
            dt.Columns.Add("TaxName");
            dt.Columns.Add("TaxCode");
            //dt.Columns.Add("OperatorID");
            dt.Columns.Add("SalesPrice");
            dt.Columns.Add("InvoicePrice");
            dt.Columns.Add("InvoiceNo");

            if (!string.IsNullOrEmpty(currentDate))
            {
                currentDate = currentDate.Trim() + "-01"; ;
                DateTime dtFrom = Convert.ToDateTime(currentDate);
                var InputView = new Needs.Ccs.Services.Views.OutGoodsDownload().Where(item => item.StorageDate >= dtFrom).ToArray();

                foreach (var item in InputView)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = item.ID;
                    //dr["OutStoreDate"] = item.OutStoreDate;
                    dr["GName"] = item.GName;
                    dr["GoodsBrand"] = item.GoodsBrand;
                    dr["GoodsModel"] = item.GoodsModel;
                    dr["GQty"] = item.GQty;
                    dr["Gunit"] = item.Gunit;
                    dr["PurchasingPrice"] = item.PurchasingPrice;
                    dr["TaxedPrice"] = item.TaxedPrice;
                    //dr["TradeCurr"] = item.TradeCurr;
                    dr["OwnerName"] = item.OwnerName;
                    dr["TaxName"] = item.TradeCurr;
                    dr["TaxCode"] = item.OwnerName;
                    //dr["OperatorID"] = item.OperatorID;

                    if (item.InvoiceType == 1)
                    {
                        dr["InvoiceDate"] = "服务费发票";
                        dr["SalesPrice"] = "服务费发票";
                        dr["InvoicePrice"] = "服务费发票";
                        dr["InvoiceNo"] = "服务费发票";
                    }
                    else
                    {
                        dr["InvoiceDate"] = item.InvoiceDate;
                        dr["SalesPrice"] = item.SalesPrice;
                        dr["InvoicePrice"] = item.InvoicePrice;
                        dr["InvoiceNo"] = item.InvoiceNo;
                    }
                    dt.Rows.Add(dr);
                }
            }

            if (!string.IsNullOrEmpty(previousDate))
            {
                previousDate = previousDate.Trim() + "-01"; ;
                DateTime dtTo = Convert.ToDateTime(previousDate).AddDays(1);
                var InputView = new Needs.Ccs.Services.Views.OutGoodsDownload().Where(item => item.StorageDate < dtTo).ToArray();

                foreach (var item in InputView)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = item.ID;
                    //dr["OutStoreDate"] = item.OutStoreDate;
                    dr["GName"] = item.GName;
                    dr["GoodsBrand"] = item.GoodsBrand;
                    dr["GoodsModel"] = item.GoodsModel;
                    dr["GQty"] = item.GQty;
                    dr["Gunit"] = item.Gunit;
                    dr["PurchasingPrice"] = item.PurchasingPrice;
                    dr["TaxedPrice"] = item.TaxedPrice;
                    //dr["TradeCurr"] = item.TradeCurr;
                    dr["OwnerName"] = item.OwnerName;
                    dr["TaxName"] = item.TradeCurr;
                    dr["TaxCode"] = item.OwnerName;
                    //dr["OperatorID"] = item.OperatorID;

                    if (item.InvoiceType == 1)
                    {
                        dr["InvoiceDate"] = "服务费发票";
                        dr["SalesPrice"] = "服务费发票";
                        dr["InvoicePrice"] = "服务费发票";
                        dr["InvoiceNo"] = "服务费发票";
                    }
                    else
                    {
                        dr["InvoiceDate"] = item.InvoiceDate;
                        dr["SalesPrice"] = item.SalesPrice;
                        dr["InvoicePrice"] = item.InvoicePrice;
                        dr["InvoiceNo"] = item.InvoiceNo;
                    }
                    dt.Rows.Add(dr);
                }
            }

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
                //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OutStoreDate", ExcelColumn = "出库日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceDate", ExcelColumn = "开票日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GName", ExcelColumn = "报关品名", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsBrand", ExcelColumn = "品牌", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsModel", ExcelColumn = "型号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceQty", ExcelColumn = "发票数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GQty", ExcelColumn = "数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Gunit", ExcelColumn = "单位", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PurchasingPrice", ExcelColumn = "进价", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxedPrice", ExcelColumn = "完税价格", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OwnerName", ExcelColumn = "客户名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxName", ExcelColumn = "税务名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxCode", ExcelColumn = "税务编码", Alignment = "center" });
                //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OperatorID", ExcelColumn = "出库人", Alignment = "center" });
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