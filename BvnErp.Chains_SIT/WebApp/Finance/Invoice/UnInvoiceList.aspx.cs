using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance
{
    public partial class UnInvoiceList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            //申请人
            this.Model.ApplyData = Needs.Wl.Admin.Plat.AdminPlat.Admins.Select(item => new { Value = item.ID, Text = item.RealName }).Json();
            //开票类型
            this.Model.InvoiceTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.InvoiceType>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        /// <summary>
        /// 加载开票通知
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string applyID = Request.QueryString["Apply"];
            string invoiceType = Request.QueryString["InvoiceType"];
            string companyName = Request.QueryString["CompanyName"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];

            using (var query = new Needs.Ccs.Services.Views.InvoiceNoticeListView())
            {
                var view = query;

                view = view.SearchByInvoiceNoticeStatus(Needs.Ccs.Services.Enums.InvoiceNoticeStatus.UnAudit);

                if (!string.IsNullOrEmpty(applyID))
                {
                    applyID = applyID.Trim();
                    view = view.SearchByApplyID(applyID);
                }
                if (!string.IsNullOrEmpty(invoiceType))
                {
                    int type = int.Parse(invoiceType);
                    view = view.SearchByInvoiceType(type);
                }
                if (!string.IsNullOrEmpty(companyName))
                {
                    companyName = companyName.Trim();
                    view = view.SearchByCompanyName(companyName);
                }
                if (!string.IsNullOrEmpty(clientCode))
                {
                    clientCode = clientCode.Trim();
                    view = view.SearchByClientCode(clientCode);
                }
                if (!string.IsNullOrEmpty(startDate))
                {
                    var from = DateTime.Parse(startDate);
                    view = view.SearchByStartDate(from);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    var to = DateTime.Parse(endDate);
                    view = view.SearchByEndDate(to);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        /// <summary>
        /// 导出XML开票前，再次验证是否所有订单都已经报关完成
        /// </summary>
        protected void CheckOrderDeclare()
        {

            var invoiceIDs = Request.Form["IDs"].Split(',');
            var flag = true;
            var msg = "";

            try {

                var invoiceNoticeItems = new Needs.Ccs.Services.Views.InvoiceNoticeItemOriginView().Where(t => invoiceIDs.Contains(t.InvoiceNoticeID));

                var orderIDs = string.Join(",", invoiceNoticeItems.Select(t => t.OrderID).ToArray()).Split(',');

                var decheads = new Needs.Ccs.Services.Views.DecHeadsView().Where(t => t.IsSuccess).Select(t => t.OrderID).ToList();

                foreach (var id in orderIDs)
                {
                    if (!decheads.Any(t =>t == id))
                    {
                        flag = false;
                        msg += id + " ";
                    }
                }

                Response.Write((new { success = flag, message = "订单未报关：" + msg }).Json());

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "订单验证错误：" + ex.Message }).Json());
            }
        }

        protected void calculateXmlQty()
        {
            try
            {
                var IDs = Request["IDs"].Replace("&quot;", "").Trim().Split(',');
                var xmls = new InvoiceXmlSplitShowView().Where(t => IDs.Contains(t.InvoiceNoticeID)).ToList();
                Response.Write((new { success = true, message = "计算发票数量成功", data = xmls.Count() }).Json());
            }
            catch(Exception EX)
            {
                Response.Write((new { success = false, message = "计算发票数量失败" }).Json());
            }            
        }

        #region 导出Excel文件

        /// <summary>
        /// 导出发票信息
        /// </summary>
        protected void Export()
        {
            var IDs = Request["IDs"].Replace("&quot;", "").Trim().Split(',');
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNotice.Where(item => IDs.Contains(item.ID)).OrderBy(item => item.Client.Company.Name).ToList();
            try
            {
                DataTable dtSheetName = createTbForName();
                for (int i = 0; i < data.Count; i++)
                {
                    DataRow dr = dtSheetName.NewRow();
                    dr["NO"] = i;//生成序号
                    dr["MemberCode"] = data[i].Client.ClientCode;//生成sheet名称
                    dtSheetName.Rows.Add(dr);
                }

                List<DataTable> dts = new List<DataTable>();
                for (int i = 0; i < data.Count; i++)
                {
                    DataTable dt = createTable();
                    string sheetname = data[i].Client.ClientCode;
                    sheetname = getSheetName(dtSheetName, sheetname, i);

                    int detailcount = data[i].InvoiceItems.Count;
                    var detail = data[i].InvoiceItems[0].OrderItem;
                    if (detailcount != 0 && detail != null)
                    {
                        //开票类型为全额发票，包含订单项
                        #region 有明细信息
                        for (int j = 0; j < detailcount; j++)
                        {
                            //var TaxOffAmountPrice = data[i].InvoiceItems[j].SalesTotalPrice; ;
                            DataRow dr = dt.NewRow();
                            dr["SheetName"] = sheetname;
                            dr["NO"] = j + 1;
                            dr["Name"] = data[i].InvoiceItems[j].OrderItem.Category.Name;
                            dr["Model"] = data[i].InvoiceItems[j].OrderItem.Model;
                            dr["Unit"] = data[i].InvoiceItems[j].UnitName;
                            dr["TaxoffUnitPrice"] = data[i].InvoiceItems[j].SalesUnitPrice;
                            dr["Qty"] = decimal.Round(data[i].InvoiceItems[j].OrderItem.Quantity);
                            dr["TaxOffAmount"] = Convert.ToDouble(data[i].InvoiceItems[j].SalesTotalPrice);
                            dr["TaxPoint"] = data[i].InvoiceItems[j].InvoiceTaxRate;
                            dr["UnitPriceTax"] = Convert.ToDouble(data[i].InvoiceItems[j].UnitPrice);
                            dr["AmountTax"] = Convert.ToDouble(data[i].InvoiceItems[j].Amount);
                            dr["OrderNo"] = data[i].InvoiceItems[j].OrderID;
                            dr["Difference"] = decimal.Round(data[i].InvoiceItems[j].Difference, 2);
                            dr["TotalDifference"] = decimal.Round(data[i].TotalDif, 2);
                            dr["InvoiceNo"] = data[i].InvoiceItems[j].InvoiceNo;
                            dr["ModelInfoClassificationValue"] = data[i].InvoiceItems[j].TaxCode;
                            dr["ModelInfoClassification"] = data[i].InvoiceItems[j].TaxName;

                            dr["rowscount"] = detailcount;

                            if (j == 0)
                            {
                                //开票资料，或寄票信息，合计等
                                dr["CompanyName"] = data[i].Client.Company.Name;
                                dr["TaxNumber"] = data[i].ClientInvoice.TaxCode;
                                dr["AddressPhone"] = data[i].Address + "/" + data[i].Tel;
                                dr["BankInfo"] = data[i].BankName + "/" + data[i].BankAccount;
                                dr["InvoiceDeliveryMethod"] = (int)data[i].DeliveryType;
                                dr["TotalQuantity"] = decimal.Round(data[i].TotalQty);//总数量
                                dr["TotalAmountTax"] = decimal.Round(data[i].Amount, 2);
                                dr["Remark"] = data[i].Summary;
                                dr["TotalTaxOffAmount"] = (data[i].Amount / (1 + data[i].InvoiceTaxRate)).ToRound(4);
                                if (data[i].DeliveryType == InvoiceDeliveryType.SendByPost)
                                {
                                    dr["ReceiptAddress"] = data[i].MailAddress;
                                    dr["ReceiptPhone"] = data[i].MailMobile;
                                    dr["ReceiptName"] = data[i].MailName;
                                }

                            }
                            dt.Rows.Add(dr);
                        }
                        #endregion
                    }
                    else
                    {
                        //开票类型为服务费发票
                        DataRow dr = dt.NewRow();
                        dr["SheetName"] = sheetname;
                        dr["CompanyName"] = data[i].Client.Company.Name;
                        dr["TaxNumber"] = data[i].ClientInvoice.TaxCode;
                        dr["AddressPhone"] = data[i].Address + "/" + data[i].Tel;
                        dr["BankInfo"] = data[i].BankName + "/" + data[i].BankAccount;
                        dr["InvoiceDeliveryMethod"] = (int)data[i].DeliveryType;
                        dr["Qty"] = 1;
                        dr["TotalQuantity"] = 1;
                        dr["TotalAmountTax"] = data[i].Amount;
                        dr["rowscount"] = detailcount;
                        dr["Remark"] = data[i].Summary;

                        dr["NO"] = 1;
                        dr["Name"] = "*物流辅助服务*服务费";
                        dr["UnitPriceTax"] = Convert.ToDouble(data[i].InvoiceItems[0].UnitPrice);
                        dr["TaxOffAmount"] = Convert.ToDouble(data[i].InvoiceItems[0].SalesTotalPrice);
                        dr["TotalTaxOffAmount"] = (data[i].Amount / (1 + data[i].InvoiceTaxRate)).ToRound(4);
                        dr["TaxPoint"] = data[i].InvoiceTaxRate;
                        dr["TaxoffUnitPrice"] = data[i].Amount / (1 + data[i].InvoiceTaxRate).ToRound(4);
                        dr["AmountTax"] = Convert.ToDouble(data[i].Amount);
                        dr["OrderNo"] = data[i].InvoiceItems[0].OrderID;
                        dr["Difference"] = data[i].InvoiceItems[0].Difference;
                        dr["TotalDifference"] = data[i].InvoiceItems[0].Difference;
                        dr["InvoiceNo"] = data[i].InvoiceItems[0].InvoiceNo;
                        dr["ModelInfoClassificationValue"] = "3040407040000000000";
                        dr["ModelInfoClassification"] = "*物流辅助服务*服务费";
                        dr["rowscount"] = detailcount;
                        if (data[i].DeliveryType == InvoiceDeliveryType.SendByPost)
                        {
                            dr["ReceiptAddress"] = data[i].MailAddress;
                            dr["ReceiptPhone"] = data[i].MailMobile;
                            dr["ReceiptName"] = data[i].MailName;
                        }

                        dt.Rows.Add(dr);
                    }
                    dts.Add(dt);
                }

                var fileName = DateTime.Now.Ticks + ".xlsx";

                //创建文件夹
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                NPOIHelper.InvoiceInfoExcel(dts, file.FilePath, data.Count);

                ////更新开票通知状态为开票中
                //foreach(var item in data)
                //{
                //    item.InvoiceNoticeStatus = InvoiceNoticeStatus.Auditing;
                //    item.UpdateAuditing();
                //}
                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());
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

        private DataTable createTbForName()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MemberCode");
            dt.Columns.Add("NO");
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

        #endregion

        #region 导出XML文件新

        /// <summary>
        /// 开票操作
        /// 1、对于总金额小于10W的通知，可能会合并的
        /// 2、通知中，小于10W的Items
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        protected void ExportXml()
        {
            try
            {
               

                var IDs = Request["IDs"].Replace("&quot;", "").Trim().Split(',');
                var invoiceNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNotice.Where(item => IDs.Contains(item.ID)).ToList();

                var help = new InvoiceXmlHelp(invoiceNotices);
                var zipUrl = help.ExportXml();

                //更新开票通知状态为开票中
                foreach (var item in invoiceNotices)
                {
                    item.InvoiceNoticeStatus = InvoiceNoticeStatus.Auditing;
                    item.UpdateAuditing();
                }

                #region 插入发出商品
                Task.Run(() =>
                {
                    IOutGoodsAdd outGoodsAdd = new InvoiceOut();
                    outGoodsAdd.Client = invoiceNotices.FirstOrDefault().Client;
                    outGoodsAdd.OrderItems = new List<string>();
                    foreach (var invoiceNotice in invoiceNotices)
                    {
                        foreach(var item in invoiceNotice.InvoiceItems)
                        {
                            outGoodsAdd.OrderItems.Add(item.OrderItem.ID);
                        }                        
                    }

                    OutGoodsContext outGoodsContext = new OutGoodsContext(outGoodsAdd);
                    outGoodsContext.context();
                });
                #endregion


                Response.Write((new { success = true, message = "导出成功", url = zipUrl }).Json());
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

       
        #endregion

    }

}