using Needs.Ccs.Services;
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
    public partial class MonoList : Uc.PageBase
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

            using (var query = new Needs.Ccs.Services.Views.MonoListView())
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
                //if (!string.IsNullOrEmpty(InvoiceNo))
                //{
                //    InvoiceNo = InvoiceNo.Trim();
                //    view = view.SearchByInvoiceNo(InvoiceNo);
                //}

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
        
        protected void GetBillAmounts()
        {
            string OrderIDs = HttpUtility.UrlDecode(Request.Form["OrderIDs[]"]).Replace("&quot;", "\'").Replace("amp;", "");

            string[] orderID_Array = OrderIDs.Split(',');

            var billAmounts = new Needs.Ccs.Services.Views.BillAmountsView().GetBillAmounts(orderID_Array);

            var theRealFields = billAmounts.Select(item => new
            {
                OrderID = item.OrderID,
                Bill = (item.totalCNYPrice + (item.totalTraiff ?? 0) + (item.totalAddedValueTax ?? 0) + item.totalAgencyFee + item.totalIncidentalFee).ToRound(2),
            }).ToArray();

            Response.Write(new
            {
                rows = theRealFields.ToArray(),
            }.Json());
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

                using (var query = new Needs.Ccs.Services.Views.MonoListView())
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
                    //if (!string.IsNullOrEmpty(InvoiceNo))
                    //{
                    //    InvoiceNo = InvoiceNo.Trim();
                    //    view = view.SearchByInvoiceNo(InvoiceNo);
                    //}

                    var dataListJson = view.ToMyPage().Json();

                    //写入数据
                    DataTable dt = NPOIHelper.JsonToDataTable(dataListJson);

                    #region 增加"账单金额"数据

                    List<string> orderID_List = new List<string>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        orderID_List.Add((string)dt.Rows[i]["OrderID"]);
                    }

                    var billAmounts = new Needs.Ccs.Services.Views.BillAmountsView().GetBillAmounts(orderID_List.ToArray());
                    var theRealFields = billAmounts.Select(item => new
                    {
                        OrderID = item.OrderID,
                        Bill = (item.totalCNYPrice + (item.totalTraiff ?? 0) + (item.totalAddedValueTax ?? 0) + item.totalAgencyFee + item.totalIncidentalFee).ToRound(2),
                    }).ToArray();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string theOrderID = (string)dt.Rows[i]["OrderID"];
                        var theBill = theRealFields.Where(t => t.OrderID == theOrderID).FirstOrDefault();
                        if (theBill != null)
                        {
                            dt.Rows[i]["BillAmount"] = theBill.Bill;
                        }
                    }

                    #endregion

                    string fileName = DateTime.Now.Ticks + ".xls";

                    //创建文件目录
                    FileDirectory fileDic = new FileDirectory(fileName);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                    fileDic.CreateDataDirectory();

                    #region 设置导出格式

                    var excelconfig = new ExcelConfig();
                    excelconfig.FilePath = fileDic.FilePath;
                    excelconfig.Title = "应收账款-单抬头";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 16;
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;

                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OwnerName", ExcelColumn = "客户名称", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单编号", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DDate", ExcelColumn = "报关日期", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclarationAmount", ExcelColumn = "报关金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AttorneyAmount", ExcelColumn = "委托金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "BillAmount", ExcelColumn = "账单金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceAmount", ExcelColumn = "发票金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UnInvoiceAmount", ExcelColumn = "未开票金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceNo", ExcelColumn = "发票号码", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceTime", ExcelColumn = "开票日期", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ThatDayExchangeRate", ExcelColumn = "当天汇率", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CustomsExchangeRate", ExcelColumn = "海关汇率", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContractNo", ExcelColumn = "合同号", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConsignorCode", ExcelColumn = "供应商", Alignment = "left" });
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