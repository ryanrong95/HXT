using Needs.Linq;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services;
using Needs.Utils.Descriptions;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Converters;
using Needs.Utils;
using Needs.Ccs.Services.Models;

namespace WebApp.Finance.Declare
{
    public partial class CustomsInvoiceMerchandiser : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            //开票类型
            this.Model.InvoiceTypeData = EnumUtils.ToDictionary<InvoiceType>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ContrNo = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];
            string InvoiceType = Request.QueryString["InvoiceType"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string OwnerName = Request.QueryString["OwnerName"];

            using (var query = new Needs.Ccs.Services.Views.DecTaxListMerchandiserView())
            {

                var view = query;
                if (!Needs.Wl.Admin.Plat.AdminPlat.Current.IsSa) {
                    view = view.SearchByAdminID(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);//过滤人员
                }

                if (!string.IsNullOrWhiteSpace(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    view = view.SearchByContrNo(ContrNo);
                }

                if (!string.IsNullOrWhiteSpace(OrderID))
                {
                    OrderID = OrderID.Trim();
                    view = view.SearchByOrderID(OrderID);
                }

                if (!string.IsNullOrWhiteSpace(InvoiceType))
                {
                    var type = Enum.Parse(typeof(Needs.Ccs.Services.Enums.InvoiceType), InvoiceType);
                    view = view.SearchByInvoiceType((Needs.Ccs.Services.Enums.InvoiceType)type);
                }

                if (!string.IsNullOrWhiteSpace(OwnerName))
                {
                    OwnerName = OwnerName.Trim();
                    view = view.SearchByOwnerName(OwnerName);
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


        protected void ExportSingleTypeFiles()
        {
            try
            {


                #region 查询
     

                string DecheadIDs = Request.Form["DecheadIDs"];
                string ClientID = Request.Form["ClientID"];
                string UnPayExchangeAmount = Request.Form["UnPayExchangeAmount"];
                string DeclareAmount = Request.Form["DeclareAmount"];
                string PayExchangeAmount = Request.Form["PayExchangeAmount"];
                string DeclareAmountMonth = Request.Form["DeclareAmountMonth"];
                string PayExchangeAmountMonth = Request.Form["PayExchangeAmountMonth"];
                string FileType = Request.Form["FileType"];
                string FileTypeName = Request.Form["FileTypeName"];


                //查看客户限制允许下载
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.FirstOrDefault(t => t.ID == ClientID);
                if (client == null)
                {
                    Response.Write((new { success = false, message = "导出失败 ClientID异常" }).Json());
                    return;
                }

                var peFlag = true;
                if (client != null && !client.IsDownloadDecTax.Value)
                {
                    if (!string.IsNullOrEmpty(client.DecTaxExtendDate))
                    {
                        //有宽限日期
                        var extend = DateTime.Parse(client.DecTaxExtendDate);
                        peFlag = DateTime.Compare(extend, DateTime.Now) > 0;
                    }
                    else
                    {
                        //没有宽限日期
                        peFlag = false;
                    }

                    if (!peFlag)
                    {
                        Response.Write((new { success = peFlag, message = "此客户因付汇情况被设置为不允许下载海关票！请联系风控人员" }).Json());
                        return;
                    }
                }




                var DecTax = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecTax.AsQueryable();

                var Arr_IDs = DecheadIDs.Trim(',').Split(',');

                var orderIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead.Where(t => Arr_IDs.Contains(t.ID)).Select(t => t.OrderID).ToArray();

                var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders2.Where(t => orderIDs.Contains(t.ID)).ToArray();

                DecTax = DecTax.Where(t => Arr_IDs.Contains(t.ID));

                #endregion

                //记录跟单下载了 多少金额的海关发票
                var downLoadAmount = 0M;
                var Summary = "";

                //1.创建文件夹(文件压缩后存放的地址)
                FileDirectory file = new FileDirectory();
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.ZipFiles);
                file.CreateDataDirectory();
                string filePrefix = file.RootDirectory;

                List<string> files = new List<string>();

                var fileType = (Needs.Ccs.Services.Enums.FileType)Convert.ToInt16(FileType);

                foreach (var item in DecTax)
                {
                    if (fileType == Needs.Ccs.Services.Enums.FileType.DecHeadTariffFile)
                    {
                        var fileTariff = item.TariffFile;
                        if (fileTariff != null)
                        {
                            downLoadAmount += item.TariffValue.Value;
                            Summary += item.ContrNo + ",";
                            files.Add((filePrefix + @"\" + fileTariff.Url).ToUrl());
                        }
                    }
                    else if (fileType == Needs.Ccs.Services.Enums.FileType.DecHeadVatFile)
                    {
                        var fileVat = item.VatFile;
                        if (fileVat != null)
                        {
                            downLoadAmount += item.AddedValue.Value;
                            Summary += item.ContrNo + ",";
                            files.Add((filePrefix + @"\" + fileVat.Url).ToUrl());
                        }
                    }
                    else if (fileType == Needs.Ccs.Services.Enums.FileType.DecHeadFile)
                    {
                        var fileDec = item.DecFile;
                        if (fileDec != null)
                        {
                            downLoadAmount += item.OrderAgentAmount.Value;
                            Summary += item.ContrNo + ",";
                            files.Add((filePrefix + @"\" + fileDec.Url).ToUrl());
                        }
                    }
                }

                string zipFileName = FileTypeName + DateTime.Now.ToString("yyyyMMddHHmmss") + Needs.Ccs.Services.SysConfig.Postfix;
                ZipFile zip = new ZipFile(zipFileName);
                zip.SetFilePath(file.FilePath);
                zip.Files = files;
                zip.ZipFiles();

                if (fileType != Needs.Ccs.Services.Enums.FileType.DecHeadFile)
                {
                    //插入记录表
                    var record = new ClientUnPayExchangeRecord();
                    record.ClientID = ClientID;
                    record.Type = 2;
                    record.UnPayExchangeAmount = decimal.Parse(UnPayExchangeAmount);
                    record.DeclareAmount = decimal.Parse(DeclareAmount);
                    record.PayExchangeAmount = decimal.Parse(PayExchangeAmount);
                    record.DeclareAmountMonth = decimal.Parse(DeclareAmountMonth);
                    record.PayExchangeAmountMonth = decimal.Parse(PayExchangeAmountMonth);
                    record.Amount = orders.Sum(t => t.DeclarePrice); //downLoadAmount;
                    record.Currency = orders.FirstOrDefault().Currency;
                    record.Summary = Summary;
                    record.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                    record.Enter();
                }
               

                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl + zipFileName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }

        }
    }
}