using Needs.Utils;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance.ReceivableBill
{
    public partial class DualServiceList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string DDateStartDate = Request.QueryString["DDateStartDate"];
            string DDateEndDate = Request.QueryString["DDateEndDate"];
            string OwnerName = Request.QueryString["OwnerName"];
            string OrderID = Request.QueryString["OrderID"];
            string InvoiceNo = Request.QueryString["InvoiceNo"];

            using (var query = new Needs.Ccs.Services.Views.DualServiceListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(DDateStartDate))
                {
                    DateTime begin = DateTime.Parse(DDateStartDate);
                    view = view.SearchByDDateBegin(begin);
                }
                if (!string.IsNullOrEmpty(DDateEndDate))
                {
                    DateTime end = DateTime.Parse(DDateEndDate);
                    end = end.AddDays(1);
                    view = view.SearchByDDateEnd(end);
                }
                if (!string.IsNullOrEmpty(OwnerName))
                {
                    OwnerName = OwnerName.Trim();
                    view = view.SearchByOwnerName(OwnerName);
                }
                if (!string.IsNullOrEmpty(OrderID))
                {
                    OrderID = OrderID.Trim();
                    view = view.SearchByOrderID(OrderID);
                }
                if (!string.IsNullOrEmpty(InvoiceNo))
                {
                    InvoiceNo = InvoiceNo.Trim();
                    view = view.SearchByInvoiceNo(InvoiceNo);
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
                string DDateStartDate = Request.Form["DDateStartDate"];
                string DDateEndDate = Request.Form["DDateEndDate"];
                string OwnerName = Request.Form["OwnerName"];
                string OrderID = Request.Form["OrderID"];
                string InvoiceNo = Request.Form["InvoiceNo"];

                using (var query = new Needs.Ccs.Services.Views.DualServiceListView())
                {
                    var view = query;

                    if (!string.IsNullOrEmpty(DDateStartDate))
                    {
                        DateTime begin = DateTime.Parse(DDateStartDate);
                        view = view.SearchByDDateBegin(begin);
                    }
                    if (!string.IsNullOrEmpty(DDateEndDate))
                    {
                        DateTime end = DateTime.Parse(DDateEndDate);
                        end = end.AddDays(1);
                        view = view.SearchByDDateEnd(end);
                    }
                    if (!string.IsNullOrEmpty(OwnerName))
                    {
                        OwnerName = OwnerName.Trim();
                        view = view.SearchByOwnerName(OwnerName);
                    }
                    if (!string.IsNullOrEmpty(OrderID))
                    {
                        OrderID = OrderID.Trim();
                        view = view.SearchByOrderID(OrderID);
                    }
                    if (!string.IsNullOrEmpty(InvoiceNo))
                    {
                        InvoiceNo = InvoiceNo.Trim();
                        view = view.SearchByInvoiceNo(InvoiceNo);
                    }

                    var dataListJson = view.ToMyPage().Json();

                    //写入数据
                    DataTable dt = NPOIHelper.JsonToDataTable(dataListJson);

                    string fileName = DateTime.Now.Ticks + ".xls";

                    //创建文件目录
                    FileDirectory fileDic = new FileDirectory(fileName);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                    fileDic.CreateDataDirectory();

                    #region 设置导出格式

                    var excelconfig = new ExcelConfig();
                    excelconfig.FilePath = fileDic.FilePath;
                    excelconfig.Title = "应收账款-双抬头服务费";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 16;
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;

                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OwnerName", ExcelColumn = "客户名称", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单编号", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DDate", ExcelColumn = "报关日期", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ServiceAmount", ExcelColumn = "服务费金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UnInvoiceAmount", ExcelColumn = "未开票金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceAmount", ExcelColumn = "发票金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceNo", ExcelColumn = "发票号", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceTime", ExcelColumn = "开票日期", Alignment = "center" });

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