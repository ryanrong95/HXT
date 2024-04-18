using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Invoice
{
    public partial class InvoiceDetail : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {

        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string OrderID = Request.QueryString["OrderID"];
            string ComanyName = Request.QueryString["ComanyName"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            using (var query = new Yahv.PvWsOrder.Services.Views.InvoiceDetailView())
            {
                var view = query;

                view = view.SearchByInvoiceNoticeStatusInvoiced();

                if (!string.IsNullOrEmpty(OrderID))
                {
                    OrderID = OrderID.Trim();
                    view = view.SearchByOrderID(OrderID);
                }

                if (!string.IsNullOrEmpty(ComanyName))
                {
                    ComanyName = ComanyName.Trim();
                    view = view.SearchByCompanyName(ComanyName);
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    DateTime begin = DateTime.Parse(StartDate);
                    view = view.SearchByInvoiceTimeBegin(begin);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime end = DateTime.Parse(EndDate);
                    end = end.AddDays(1);
                    view = view.SearchByInvoiceTimeEnd(end);
                }

                Func<Yahv.PvWsOrder.Services.Views.InvoiceDetailViewModel, InvoiceDetailDataModel> convert = item => new InvoiceDetailDataModel
                {
                    InvoiceNoticeItemID = item.InvoiceNoticeItemID,
                    BillID = item.BillID,
                    OrderIDs = item.OrderIDs,
                    DetailAmountDes = item.DetailAmount.ToRound1(2).ToString(),
                    CompanyName = item.CompanyName,
                    InvoiceNo = item.InvoiceNo,
                    InvoiceTimeDes = item.InvoiceTime?.ToString("yyyy/MM/dd"),
                    TaxAmountDes = (item.DetailSalesTotalPrice * (decimal)0.06).ToRound1(2).ToString(), //返回给页面时计算
                    DetailSalesUnitPriceDes = item.DetailSalesUnitPrice.ToString(),
                    DetailSalesTotalPriceDes = item.DetailSalesTotalPrice.ToString(),
                    DifferenceDes = item.Difference?.ToString(),
                    InvoiceNoticeItemCreateDate = item.InvoiceNoticeItemCreateDate,
                    AmountDes = item.Amount.ToString(),
                    FromType = item.FromType,
                };

                var viewData = view.ToMyPage(convert, page, rows);

                //深圳代仓储 获取期号 Begin

                string[] voucherIDs = viewData.Item1.Where(t => t.FromType == InvoiceFromType.SZStore).Select(item => item.BillID).ToArray();
                if (voucherIDs != null && voucherIDs.Length > 0)
                {
                    try
                    {
                        var vourcherInfos = new VourcherInfoView(voucherIDs).GetVourcherInfo();
                        if (vourcherInfos.success)
                        {
                            for (int i = 0; i < viewData.Item1.Length; i++)
                            {
                                if (viewData.Item1[i].FromType == InvoiceFromType.SZStore)
                                {
                                    viewData.Item1[i].OrderIDs = vourcherInfos.voucherinfos.FirstOrDefault(t => t.VoucherID == viewData.Item1[i].BillID)?.CutDateIndex;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //如果接口出错，不要影响这个导出
                    }
                }

                //深圳代仓储 获取期号 End

                Response.Write(new { total = viewData.Item2, rows = viewData.Item1, }.Json());
            }
        }

        /// <summary>
        /// 导出发票信息
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                string OrderID = Request.QueryString["OrderID"];
                string ComanyName = Request.QueryString["ComanyName"];
                string StartDate = Request.QueryString["StartDate"];
                string EndDate = Request.QueryString["EndDate"];

                InvoiceDetailDataModel[] noticeitem = new InvoiceDetailDataModel[0];
                using (var query = new Yahv.PvWsOrder.Services.Views.InvoiceDetailView())
                {
                    var view = query;

                    view = view.SearchByInvoiceNoticeStatusInvoiced();

                    if (!string.IsNullOrEmpty(OrderID))
                    {
                        OrderID = OrderID.Trim();
                        view = view.SearchByOrderID(OrderID);
                    }

                    if (!string.IsNullOrEmpty(ComanyName))
                    {
                        ComanyName = ComanyName.Trim();
                        view = view.SearchByCompanyName(ComanyName);
                    }

                    if (!string.IsNullOrEmpty(StartDate))
                    {
                        DateTime begin = DateTime.Parse(StartDate);
                        view = view.SearchByInvoiceTimeBegin(begin);
                    }

                    if (!string.IsNullOrEmpty(EndDate))
                    {
                        DateTime end = DateTime.Parse(EndDate);
                        end = end.AddDays(1);
                        view = view.SearchByInvoiceTimeEnd(end);
                    }

                    Func<Yahv.PvWsOrder.Services.Views.InvoiceDetailViewModel, InvoiceDetailDataModel> convert = item => new InvoiceDetailDataModel
                    {
                        InvoiceNoticeItemID = item.InvoiceNoticeItemID,
                        BillID = item.BillID,
                        OrderIDs = item.OrderIDs,
                        DetailAmountDes = item.DetailAmount.ToRound1(2).ToString(),
                        CompanyName = item.CompanyName,
                        InvoiceNo = item.InvoiceNo,
                        InvoiceTimeDes = item.InvoiceTime?.ToString("yyyy/MM/dd"),
                        TaxAmountDes = (item.DetailSalesTotalPrice * (decimal)0.06).ToRound1(2).ToString(), //返回给页面时计算
                        DetailSalesUnitPriceDes = item.DetailSalesUnitPrice.ToString(),
                        DetailSalesTotalPriceDes = item.DetailSalesTotalPrice.ToString(),
                        DifferenceDes = item.Difference?.ToString(),
                        InvoiceNoticeItemCreateDate = item.InvoiceNoticeItemCreateDate,
                        AmountDes = item.Amount.ToString(),
                        FromType = item.FromType,
                    };

                    var viewData = view.ToMyPage(convert);

                    noticeitem = viewData.Item1;
                }

                //深圳代仓储 获取期号 Begin

                string[] voucherIDs = noticeitem.Where(t => t.FromType == InvoiceFromType.SZStore).Select(item => item.BillID).ToArray();
                if (voucherIDs != null && voucherIDs.Length > 0)
                {
                    try
                    {
                        var vourcherInfos = new VourcherInfoView(voucherIDs).GetVourcherInfo();
                        if (vourcherInfos.success)
                        {
                            for (int i = 0; i < noticeitem.Length; i++)
                            {
                                if (noticeitem[i].FromType == InvoiceFromType.SZStore)
                                {
                                    noticeitem[i].OrderIDs = vourcherInfos.voucherinfos.FirstOrDefault(t => t.VoucherID == noticeitem[i].BillID)?.CutDateIndex;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //如果接口出错，不要影响这个导出
                    }
                }

                //深圳代仓储 获取期号 End

                Func<InvoiceDetailDataModel, object> convertToExcel = item => new
                {
                    TimeNow = DateTime.Now.ToString("yyyy/MM/dd"),
                    //ID = item.ID,
                    OrderID = item.OrderIDs,
                    ProductName = "*物流辅助服务*服务费",  //产品名称
                    ProductModel = "",//型号
                    //Unit = item.OrderItem?.Unit == null ? "" : item.OrderItem?.Unit,//计量单位
                    Unit = "",//单位
                    SalesUnitPrice = item.DetailSalesUnitPriceDes, //不含税价格
                    Quantity = 1,//数量
                    SalesTotalPrice = item.DetailSalesTotalPriceDes, //不含税金额
                    UnitPrice = item.DetailAmountDes, //含税单价
                    Amount = item.DetailAmountDes,//含税总额
                    Name = item.CompanyName,
                    InvoiceNo = item.InvoiceNo == null ? "" : item.InvoiceNo,
                    UpdateDate = item.InvoiceTimeDes,
                    TaxAmount = item.TaxAmountDes,
                };

                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(noticeitem.Select(convertToExcel).ToArray().Json());

                var fileName = DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDir = new FileDirectory(fileName, FileType.Test);
                fileDir.CreateDirectory();
                string filePath = fileDir.DownLoadRoot + fileName;

                NPOIHelper.NPOIExcel(dt, filePath);

                Response.Write((new { success = true, message = "导出成功", url = @"../Files/Download/" + fileName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message, }).Json());
            }
        }

        public class InvoiceDetailDataModel
        {
            /// <summary>
            /// InvoiceNoticeItemID
            /// </summary>
            public string InvoiceNoticeItemID { get; set; }

            /// <summary>
            /// 账单ID
            /// </summary>
            public string BillID { get; set; }

            /// <summary>
            /// 订单号 IDs（逗号间隔）
            /// </summary>
            public string OrderIDs { get; set; }

            /// <summary>
            /// 含税单价
            /// </summary>
            public string DetailUnitPriceDes { get; set; }

            /// <summary>
            /// 含税金额
            /// </summary>
            public string DetailAmountDes { get; set; }

            /// <summary>
            /// 开票公司
            /// </summary>
            public string CompanyName { get; set; }

            /// <summary>
            /// 发票号码
            /// </summary>
            public string InvoiceNo { get; set; }

            /// <summary>
            /// 开票日期
            /// </summary>
            public string InvoiceTimeDes { get; set; }

            /// <summary>
            /// 税额
            /// </summary>
            public string TaxAmountDes { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string DetailSalesUnitPriceDes { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string DetailSalesTotalPriceDes { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string DifferenceDes { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime InvoiceNoticeItemCreateDate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string AmountDes { get; set; }

            /// <summary>
            /// 开票申请来源
            /// </summary>
            public InvoiceFromType FromType { get; set; }

            ///// <summary>
            ///// 期号(深圳代仓储)
            ///// </summary>
            //public string CutDateIndex { get; set; }
        }
    }
}