using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Notice
{
    /// <summary>
    /// 收款通知列表界面
    /// </summary>
    public partial class ReceivingList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ClientName = Server.UrlDecode(Request.QueryString["ClientName"]) ?? string.Empty;
        }

        protected void data1()
        {
            string ClientName = Request.QueryString["ClientName"];
            string QuerenStatus = Request.QueryString["QuerenStatus"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string LastReceiptDateStartDate = Request.QueryString["LastReceiptDateStartDate"];
            string LastReceiptDateEndDate = Request.QueryString["LastReceiptDateEndDate"];

            //var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices.Where(x => x.ClearAmount < x.Amount).AsQueryable();
            //Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ReceiptNotices.AsQueryable();
            var notices = new Needs.Ccs.Services.Views.ReceiptNoticesViewForReceivingList().AsQueryable();

            if (string.IsNullOrEmpty(ClientName) == false)
            {
                notices = notices.Where(item => item.Client.Company.Name.Contains(ClientName));
            }
            if (!string.IsNullOrEmpty(QuerenStatus) && QuerenStatus != "0")
            {
                //0 - 全部, 1 - 未确认, 2 - 已确认
                if (QuerenStatus == "1")
                {
                    notices = notices.Where(x => x.ClearAmount < x.Amount);
                }
                else if (QuerenStatus == "2")
                {
                    notices = notices.Where(x => x.ClearAmount >= x.Amount);
                }
            }
            if (string.IsNullOrEmpty(StartDate) == false)
            {
                DateTime start = Convert.ToDateTime(StartDate);
                notices = notices.Where(item => item.ReceiptDate >= start);
            }
            if (string.IsNullOrEmpty(EndDate) == false)
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                notices = notices.Where(item => item.ReceiptDate < end);
            }
            if (string.IsNullOrEmpty(LastReceiptDateStartDate) == false)
            {
                DateTime lastStart = Convert.ToDateTime(LastReceiptDateStartDate);
                notices = notices.Where(item => item.LastReceiptDate >= lastStart);
            }
            if (string.IsNullOrEmpty(LastReceiptDateEndDate) == false)
            {
                DateTime lastEnd = Convert.ToDateTime(LastReceiptDateEndDate).AddDays(1);
                notices = notices.Where(item => item.LastReceiptDate < lastEnd);
            }

            notices = notices.OrderByDescending(item => item.ReceiptDate).OrderByDescending(item => item.CreateDate);

            //前台显示
            Func<ReceiptNotice, object> convert = item => new
            {
                item.ID,
                ClientID = item.Client?.ID, //用于传递到下一个页面
                item?.SeqNo,
                VaultName = item.Vault.Name,
                item?.Account.AccountName,
                ClientName = item.Client?.Company.Name,
                FinanceReceiptFeeType = item.FeeType.GetDescription(),
                Amount = item?.Amount.ToString("#0.00"),
                ClearAmount = item.ClearAmount.ToString("#0.00"),
                QuerenStatus = item.ClearAmount < item.Amount ? "未确认" : "已确认",
                ReceiptDate = item?.ReceiptDate.ToString("yyyy-MM-dd"),

                LastReceiptDate = item.LastReceiptDate?.ToString("yyyy-MM-dd"),
            };

            this.Paging(notices, convert);
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientName = Request.QueryString["ClientName"];
            string QuerenStatus = Request.QueryString["QuerenStatus"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string LastReceiptDateStartDate = Request.QueryString["LastReceiptDateStartDate"];
            string LastReceiptDateEndDate = Request.QueryString["LastReceiptDateEndDate"];

            using (var query = new Needs.Ccs.Services.Views.ReceiptNoticesViewForReceivingListNew())
            {
                var view = query;

                if (string.IsNullOrEmpty(ClientName) == false)
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByClientName(ClientName);
                }
                if (!string.IsNullOrEmpty(QuerenStatus) && QuerenStatus != "0")
                {
                    //0 - 全部, 1 - 未确认, 2 - 已确认
                    if (QuerenStatus == "1" || QuerenStatus == "2")
                    {
                        view = view.SearchByQuerenStatus(QuerenStatus);
                    }
                }
                if (string.IsNullOrEmpty(StartDate) == false)
                {
                    DateTime start = Convert.ToDateTime(StartDate);
                    view = view.SearchByReceiptDateBegin(start);
                }
                if (string.IsNullOrEmpty(EndDate) == false)
                {
                    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByReceiptDateEnd(end);
                }
                if (string.IsNullOrEmpty(LastReceiptDateStartDate) == false)
                {
                    DateTime lastStart = Convert.ToDateTime(LastReceiptDateStartDate);
                    view = view.SearchByLastReceiptDateBegin(lastStart);
                }
                if (string.IsNullOrEmpty(LastReceiptDateEndDate) == false)
                {
                    DateTime lastEnd = Convert.ToDateTime(LastReceiptDateEndDate).AddDays(1);
                    view = view.SearchByLastReceiptDateEnd(lastEnd);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        protected void dataDetail2()
        {
            string financeReceiptId = Request.QueryString["FinanceReceiptId"];

            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            lamdas.Add((Expression<Func<Needs.Ccs.Services.Models.ReceiveDetail, bool>>)(t => t.FinanceReceiptID == financeReceiptId));
            lamdas.Add((Expression<Func<Needs.Ccs.Services.Models.ReceiveDetail, bool>>)(t => t.Type == OrderReceiptType.Received));
            int count = 0;
            var subjectList = new Needs.Ccs.Services.Views.ReceiveDetailReportView().GetResult(out count, page, rows, lamdas.ToArray());

            Func<Needs.Ccs.Services.Models.ReceiveDetail, object> convertReceiptRecordFinance = item => new
            {
                OrderID = item.OrderID,
                ContrNo = item.ContrNo,
                AddedValueTax = item.AddedValueTax,
                Tariff = -item.Tariff,
                ShowAgencyFee = item.ShowAgencyFee,
                GoodsAmount = item.GoodsAmount,
                PaymentExchangeRate = item.PaymentExchangeRate,
                FCAmount = item.FCAmount,
                RealExchangeRate = item.RealExchangeRate,
                DueGoods = item.DueGoods,
                Gains = item.Gains
            };

            this.Paging(subjectList, convertReceiptRecordFinance);
        }

        protected void dataDetail()
        {
            string financeReceiptId = Request.QueryString["FinanceReceiptId"];

            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            using (var query = new Needs.Ccs.Services.Views.ReceiveDetailReportViewNew())
            {
                var view = query;

                view = view.SearchByFinanceReceiptIDAndReceived(financeReceiptId);

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        /// <summary>
        /// 导出对账单
        /// </summary>
        protected void ExportDetail()
        {
            string ClientName = Request.Form["ClientName"];
            string QuerenStatus = Request.Form["QuerenStatus"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];
            string LastReceiptDateStartDate = Request.Form["LastReceiptDateStartDate"];
            string LastReceiptDateEndDate = Request.Form["LastReceiptDateEndDate"];

            try
            {
                ReceiveDetailDownload download = new ReceiveDetailDownload();
                var notices = new Needs.Ccs.Services.Views.OrderReceiptDetailView().AsQueryable();

                if (string.IsNullOrEmpty(ClientName) == false)
                {
                    notices = notices.Where(item => item.Client.Company.Name.Contains(ClientName));
                }
                if (!string.IsNullOrEmpty(QuerenStatus) && QuerenStatus != "0")
                {
                    //0 - 全部, 1 - 未确认, 2 - 已确认
                    if (QuerenStatus == "1")
                    {
                        notices = notices.Where(x => x.ClearAmount < x.ClearAmount);
                    }
                    else if (QuerenStatus == "2")
                    {
                        notices = notices.Where(x => x.ClearAmount >= x.ClearAmount);
                    }
                }
                if (string.IsNullOrEmpty(StartDate) == false)
                {
                    DateTime start = Convert.ToDateTime(StartDate);
                    notices = notices.Where(item => item.ReceiveDate >= start);
                }
                if (string.IsNullOrEmpty(EndDate) == false)
                {
                    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                    notices = notices.Where(item => item.ReceiveDate < end);
                }
                download.Items = notices.ToList();

                #region 赋值 客户类型(单抬头、双抬头)

                for (int i = 0; i < download.Items.Count; i++)
                {
                    download.Items[i].InvoiceTypeName = "";

                    if (download.Items[i].InvoiceTypeInt != null)
                    {
                        switch (download.Items[i].InvoiceTypeInt)
                        {
                            case (int)InvoiceType.Full:
                                download.Items[i].InvoiceTypeName = "单抬头";
                                break;
                            case (int)InvoiceType.Service:
                                download.Items[i].InvoiceTypeName = "双抬头";
                                break;
                            default:
                                break;
                        }
                    }
                }

                #endregion

                var fileName = DateTime.Now.Ticks + ".xlsx";

                //创建文件夹
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                download.toExcel(file.FilePath, download.ToDataTable());

                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出做账明细报表
        /// </summary>
        protected void ExportFinanceReport()
        {
            string ClientName = Request.Form["ClientName"];
            string QuerenStatus = Request.Form["QuerenStatus"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"]; 
            string LastReceiptDateStartDate = Request.Form["LastReceiptDateStartDate"];
            string LastReceiptDateEndDate = Request.Form["LastReceiptDateEndDate"];


            try
            {
                ReceiptExportExcel download = new ReceiptExportExcel();
                var url = download.Export(ClientName, QuerenStatus, StartDate, EndDate, LastReceiptDateStartDate, LastReceiptDateEndDate);

                Response.Write((new { success = true, message = "导出成功", url = url }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

    }
}