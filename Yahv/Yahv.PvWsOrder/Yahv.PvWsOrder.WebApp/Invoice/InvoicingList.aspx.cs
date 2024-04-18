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
    public partial class InvoicingList : ErpParticlePage
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
            //开票类型选项
            List<object> invoiceTypeOptionList = new List<object>();
            invoiceTypeOptionList.Add(new { value = InvoiceType.Normal, text = InvoiceType.Normal.GetDescription(), });
            invoiceTypeOptionList.Add(new { value = InvoiceType.VAT, text = InvoiceType.VAT.GetDescription(), });
            invoiceTypeOptionList.Add(new { value = InvoiceType.None, text = InvoiceType.None.GetDescription(), });
            this.Model.InvoiceTypeOption = invoiceTypeOptionList;

            //申请人选项
            this.Model.ApplyOption = new Yahv.PvWsOrder.Services.Views.AdminsAll().OrderBy(t => t.RealName)
                .Select(item => new { value = item.ID, text = item.RealName, }).ToArray();
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

            string ClientCode = Request.QueryString["ClientCode"];
            string CompanyName = Request.QueryString["CompanyName"];
            string InvoiceType = Request.QueryString["InvoiceType"];
            string AdminID = Request.QueryString["AdminID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            using (var query = new Yahv.PvWsOrder.Services.Views.InvoicingListView())
            {
                var view = query;

                view = view.SearchByInvoiceNoticeStatus(Yahv.PvWsOrder.Services.Enums.InvoiceEnum.Applied);

                if (!string.IsNullOrEmpty(ClientCode))
                {
                    ClientCode = ClientCode.Trim();
                    view = view.SearchByEnterCode(ClientCode);
                }

                if (!string.IsNullOrEmpty(CompanyName))
                {
                    CompanyName = CompanyName.Trim();
                    view = view.SearchByCompanyName(CompanyName);
                }

                if (!string.IsNullOrEmpty(InvoiceType))
                {
                    view = view.SearchByInvoiceType((Yahv.Underly.InvoiceType)int.Parse(InvoiceType));
                }

                if (!string.IsNullOrEmpty(AdminID))
                {
                    view = view.SearchByAdminID(AdminID);
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    DateTime begin = DateTime.Parse(StartDate);
                    view = view.SearchByInvoiceNoticeCreateDateBegin(begin);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime end = DateTime.Parse(EndDate);
                    end = end.AddDays(1);
                    view = view.SearchByInvoiceNoticeCreateDateEnd(end);
                }

                Func<Yahv.PvWsOrder.Services.Views.InvoicingListViewModel, InvoicingListDataModel> convert = item => new InvoicingListDataModel
                {
                    InvoiceNoticeID = item.InvoiceNoticeID,
                    EnterCode = item.EnterCode,
                    CompanyName = item.CompanyName,
                    InvoiceTypeDes = item.InvoiceType.GetDescription(),
                    Amount = item.Amount,
                    InvoiceDeliveryTypeDes = item.InvoiceDeliveryType.GetDescription(),
                    InvoiceNoticeStatusDes = item.InvoiceNoticeStatus.GetDescription(),
                    AdminID = item.AdminID,
                    AdminName = item.AdminName,
                    InvoiceNoticeCreateDateDes = item.InvoiceNoticeCreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                };

                var viewData = view.ToMyPage(convert, page, rows);

                Response.Write(new { total = viewData.Item2, rows = viewData.Item1, }.Json());
            }
        }

        /// <summary>
        /// 导出 Excel
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                var InvoiceNoticeIDs = Request["InvoiceNoticeIDs"].Replace("&quot;", "").Replace("\"", "").Trim().Split(',');
                InvoicingListDataModel[] data = new InvoicingListDataModel[0];

                using (var query = new Yahv.PvWsOrder.Services.Views.InvoicingListView())
                {
                    var view = query;

                    view = view.SearchByInvoiceNoticeIDs(InvoiceNoticeIDs);

                    Func<Yahv.PvWsOrder.Services.Views.InvoicingListViewModel, InvoicingListDataModel> convert = item => new InvoicingListDataModel
                    {
                        InvoiceNoticeID = item.InvoiceNoticeID,
                        EnterCode = item.EnterCode,
                        CompanyName = item.CompanyName,
                        InvoiceTypeDes = item.InvoiceType.GetDescription(),
                        Amount = item.Amount,
                        InvoiceDeliveryTypeInt = Convert.ToString((int)item.InvoiceDeliveryType),
                        InvoiceDeliveryTypeDes = item.InvoiceDeliveryType.GetDescription(),
                        InvoiceNoticeStatusDes = item.InvoiceNoticeStatus.GetDescription(),
                        AdminID = item.AdminID,
                        AdminName = item.AdminName,
                        InvoiceNoticeCreateDateDes = item.InvoiceNoticeCreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        TaxNumber = item.TaxNumber,
                        RegAddress = item.RegAddress,
                        Tel = item.Tel,
                        BankName = item.BankName,
                        BankAccount = item.BankAccount,
                        Summary = item.Summary,
                        PostAddress = item.PostAddress,
                        PostRecipient = item.PostRecipient,
                        PostTel = item.PostTel,
                    };

                    var viewData = view.ToMyPage(convert);

                    data = viewData.Item1;
                }

                InvoiceDetailDataModel[] allInvoiceItems = new InvoiceDetailDataModel[0];
                using (var query = new Yahv.PvWsOrder.Services.Views.InvoiceDetailView())
                {
                    var view = query;

                    view = view.SearchByInvoiceNoticeIDs(InvoiceNoticeIDs);

                    Func<Yahv.PvWsOrder.Services.Views.InvoiceDetailViewModel, InvoiceDetailDataModel> convert = item => new InvoiceDetailDataModel
                    {
                        InvoiceNoticeItemID = item.InvoiceNoticeItemID,
                        InvoiceNoticeID = item.InvoiceNoticeID,
                        BillID = item.BillID,
                        OrderIDs = item.OrderIDs,
                        DetailAmountDes = item.DetailAmount.ToRound1(2).ToString(),
                        CompanyName = item.CompanyName,
                        InvoiceNo = item.InvoiceNo,
                        InvoiceTimeDes = item.InvoiceTime?.ToString("yyyy/MM/dd"),
                        TaxAmountDes = (item.DetailSalesTotalPrice * (decimal)1.06).ToRound1(2).ToString(), //返回给页面时计算
                        DetailSalesUnitPriceDes = item.DetailSalesUnitPrice.ToString(),
                        DetailSalesTotalPriceDes = item.DetailSalesTotalPrice.ToString(),
                        DifferenceDes = item.Difference != null ? item.Difference.ToString() : Convert.ToString(0),
                        InvoiceNoticeItemCreateDate = item.InvoiceNoticeItemCreateDate,
                        AmountDes = item.Amount.ToString(),
                        Amount = item.Amount,
                        UnitPrice = item.UnitPrice,
                        FromType = item.FromType,
                    };

                    var viewData = view.ToMyPage(convert);

                    allInvoiceItems = viewData.Item1;
                }

                //深圳代仓储 获取期号 Begin

                string[] voucherIDs = allInvoiceItems.Where(t => t.FromType == InvoiceFromType.SZStore).Select(item => item.BillID).ToArray();
                if (voucherIDs != null && voucherIDs.Length > 0)
                {
                    try
                    {
                        var vourcherInfos = new VourcherInfoView(voucherIDs).GetVourcherInfo();
                        if (vourcherInfos.success)
                        {
                            for (int i = 0; i < allInvoiceItems.Length; i++)
                            {
                                if (allInvoiceItems[i].FromType == InvoiceFromType.SZStore)
                                {
                                    allInvoiceItems[i].CutDateIndex = vourcherInfos.voucherinfos.FirstOrDefault(t => t.VoucherID == allInvoiceItems[i].BillID)?.CutDateIndex;
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

                DataTable dtSheetName = createTbForName();
                for (int i = 0; i < data.Length; i++)
                {
                    DataRow dr = dtSheetName.NewRow();
                    dr["NO"] = i;//生成序号
                    dr["MemberCode"] = !string.IsNullOrEmpty(data[i].EnterCode) ? data[i].EnterCode : data[i].CompanyName;//生成sheet名称
                    dtSheetName.Rows.Add(dr);
                }

                List<DataTable> dts = new List<DataTable>();
                for (int i = 0; i < data.Length; i++)
                {
                    DataTable dt = createTable();
                    string sheetname = !string.IsNullOrEmpty(data[i].EnterCode) ? data[i].EnterCode : data[i].CompanyName;
                    sheetname = getSheetName(dtSheetName, sheetname, i);

                    var invoiceItems = allInvoiceItems.Where(t => t.InvoiceNoticeID == data[i].InvoiceNoticeID).ToArray();

                    if (invoiceItems != null && invoiceItems.Length > 0)
                    {
                        for (int j = 0; j < invoiceItems.Length; j++)
                        {
                            //开票类型为服务费发票
                            DataRow dr = dt.NewRow();
                            dr["SheetName"] = sheetname;
                            dr["Qty"] = 1;
                            dr["rowscount"] = invoiceItems.Length;
                            dr["NO"] = j + 1; //序号
                            dr["Name"] = "*物流辅助服务*服务费"; //品名
                            dr["UnitPriceTax"] = Convert.ToDouble(invoiceItems[j].UnitPrice); //【【【含税单价】】】
                            dr["TaxOffAmount"] = Convert.ToDouble((invoiceItems[j].Amount / (decimal)(1.06)).ToRound1(4)); //【【【不含税金额】】】
                            dr["TaxPoint"] = 0.06; //税率
                            dr["TaxoffUnitPrice"] = invoiceItems[j].Amount / ((decimal)1.06).ToRound1(4); //【【【不含税单价】】】
                            dr["AmountTax"] = Convert.ToDouble(invoiceItems[j].Amount); //【【【含税金额】】】
                            dr["OrderNo"] = invoiceItems[j].FromType == InvoiceFromType.SZStore ? invoiceItems[j].CutDateIndex : invoiceItems[j].OrderIDs; //订单号/合同号
                            dr["Difference"] = Convert.ToDouble(invoiceItems[j].DifferenceDes); //差额
                            dr["InvoiceNo"] = invoiceItems[j].InvoiceNo;
                            dr["ModelInfoClassificationValue"] = "3040407040000000000";
                            dr["ModelInfoClassification"] = "*物流辅助服务*服务费";
                            dr["rowscount"] = invoiceItems.Length;
                            if (data[i].InvoiceDeliveryTypeInt == Convert.ToString((int)InvoiceDeliveryType.SendByPost))
                            {
                                dr["ReceiptAddress"] = data[i].PostAddress;
                                dr["ReceiptPhone"] = data[i].PostTel;
                                dr["ReceiptName"] = data[i].PostRecipient;
                            }

                            if (j == 0)
                            {
                                dr["CompanyName"] = data[i].CompanyName;
                                dr["TaxNumber"] = data[i].TaxNumber;
                                dr["AddressPhone"] = data[i].RegAddress + "/" + data[i].Tel;
                                dr["BankInfo"] = data[i].BankName + "/" + data[i].BankAccount;
                                dr["InvoiceDeliveryMethod"] = Convert.ToInt32(data[i].InvoiceDeliveryTypeInt);
                                dr["TotalQuantity"] = invoiceItems.Length;
                                dr["TotalAmountTax"] = data[i].Amount.ToRound1(2);
                                dr["Remark"] = data[i].Summary;

                                dr["TotalTaxOffAmount"] = (data[i].Amount / (decimal)(1.06)).ToRound1(4); //【【【总不含税金额】】】
                                dr["TotalDifference"] = invoiceItems.Sum(t => Convert.ToDecimal(t.DifferenceDes)).ToRound1(2); //【【【总差额】】】
                            }

                            dt.Rows.Add(dr);
                        }

                        dts.Add(dt);
                    }
                }

                var fileName = DateTime.Now.Ticks + ".xlsx";

                //创建文件夹

                FileDirectory fileDir = new FileDirectory(fileName, FileType.Test);
                fileDir.CreateDirectory();
                string filePath = fileDir.DownLoadRoot + fileName;
                NPOIHelper.InvoiceInfoExcel(dts, filePath, data.Length);

                Response.Write((new { success = true, message = "导出成功", url = @"../Files/Download/" + fileName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message, }).Json());
            }
        }

        private DataTable createTbForName()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MemberCode");
            dt.Columns.Add("NO");
            return dt;
        }

        /// <summary>
        /// 创建table表头
        /// </summary>
        /// <returns></returns>
        private DataTable createTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("SheetName");
            dt.Columns.Add("NO");
            dt.Columns.Add("Name");
            dt.Columns.Add("Model");
            dt.Columns.Add("Unit");
            dt.Columns.Add("TaxoffUnitPrice");
            dt.Columns.Add("Qty");
            dt.Columns.Add("TaxOffAmount");
            dt.Columns.Add("TaxPoint");
            dt.Columns.Add("TaxNumber");
            dt.Columns.Add("UnitPriceTax");
            dt.Columns.Add("AmountTax");
            dt.Columns.Add("OrderNo");
            dt.Columns.Add("ModelInfoClassification");
            dt.Columns.Add("ModelInfoClassificationValue");

            dt.Columns.Add("CompanyName");
            dt.Columns.Add("AddressPhone");
            dt.Columns.Add("BankInfo");
            dt.Columns.Add("InvoiceDeliveryMethod");
            dt.Columns.Add("ReceiptAddress");
            dt.Columns.Add("ReceiptName");
            dt.Columns.Add("ReceiptPhone");
            dt.Columns.Add("rowscount");
            // dt.Columns.Add("Quantity");
            dt.Columns.Add("Remark");
            dt.Columns.Add("Difference");
            dt.Columns.Add("TotalDifference");
            dt.Columns.Add("InvoiceNo");

            dt.Columns.Add("TotalQuantity");// 合计总数量
            dt.Columns.Add("TotalAmountTax");//合计总金额(含税)
            dt.Columns.Add("TotalTaxOffAmount");//合计总金额(不含税)
            return dt;
        }

        private string getSheetName(DataTable dt, string currentname, int currentrow)
        {
            string sheetname = currentname;
            DataRow[] dr = dt.Select("MemberCode='" + currentname + "'");
            if (dr.Length > 1)
            {
                int istartrow = Convert.ToInt16(dr[0]["NO"].ToString());
                int no = currentrow - istartrow + 1;
                sheetname = currentname + "-" + no.ToString();
            }
            return sheetname;
        }

        public class InvoicingListDataModel
        {
            /// <summary>
            /// 开票编号
            /// </summary>
            public string InvoiceNoticeID { get; set; }

            /// <summary>
            /// 客户编号
            /// </summary>
            public string EnterCode { get; set; }

            /// <summary>
            /// 公司名称
            /// </summary>
            public string CompanyName { get; set; }

            /// <summary>
            /// 开票类型
            /// </summary>
            public string InvoiceTypeDes { get; set; }

            /// <summary>
            /// 含税金额
            /// </summary>
            public decimal Amount { get; set; }

            /// <summary>
            /// 发票交付方式 Int
            /// </summary>
            public string InvoiceDeliveryTypeInt { get; set; }

            /// <summary>
            /// 发票交付方式 Des
            /// </summary>
            public string InvoiceDeliveryTypeDes { get; set; }

            /// <summary>
            /// 开票状态
            /// </summary>
            public string InvoiceNoticeStatusDes { get; set; }

            /// <summary>
            /// 申请人ID
            /// </summary>
            public string AdminID { get; set; }

            /// <summary>
            /// 申请人姓名
            /// </summary>
            public string AdminName { get; set; }

            /// <summary>
            /// 申请日期
            /// </summary>
            public string InvoiceNoticeCreateDateDes { get; set; }

            /// <summary>
            /// 企业税号
            /// </summary>
            public string TaxNumber { get; set; }

            /// <summary>
            /// 注册地址
            /// </summary>
            public string RegAddress { get; set; }

            /// <summary>
            /// 电话
            /// </summary>
            public string Tel { get; set; }

            /// <summary>
            /// 开户行
            /// </summary>
            public string BankName { get; set; }

            /// <summary>
            /// 开户行账号
            /// </summary>
            public string BankAccount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Summary { get; set; }

            /// <summary>
            /// 收票地址
            /// </summary>
            public string PostAddress { get; set; }

            /// <summary>
            /// 收票人/公司
            /// </summary>
            public string PostRecipient { get; set; }

            /// <summary>
            /// 电话
            /// </summary>
            public string PostTel { get; set; }
        }

        public class InvoiceDetailDataModel
        {
            /// <summary>
            /// InvoiceNoticeItemID
            /// </summary>
            public string InvoiceNoticeItemID { get; set; }

            /// <summary>
            /// InvoiceNoticeID
            /// </summary>
            public string InvoiceNoticeID { get; set; }

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
            /// 
            /// </summary>
            public decimal Amount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public decimal UnitPrice { get; set; }

            /// <summary>
            /// 开票申请来源
            /// </summary>
            public InvoiceFromType FromType { get; set; }

            /// <summary>
            /// 期号(深圳代仓储)
            /// </summary>
            public string CutDateIndex { get; set; }
        }
    }
}