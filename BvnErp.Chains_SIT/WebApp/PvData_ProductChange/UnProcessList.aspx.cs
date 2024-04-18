using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views.Alls;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Classify;

namespace WebApp.PvData_ProductChange
{
    public partial class UnProcessList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int page, rows;
            int.TryParse(Request.QueryString["PageNumber"], out page);
            int.TryParse(Request.QueryString["PageSize"], out rows);

            var currentSc = new CurrentSc()
            {
                InitUrl = Request.QueryString["InitUrl"] ?? string.Empty,
                PageNumber = page,
                PageSize = rows,
                OrderID = Convert.ToString(Request.QueryString["OrderID"]) ?? string.Empty,
                ClientCode = Convert.ToString(Request.QueryString["ClientCode"]) ?? string.Empty,
                ProductChangeAddTimeBegin = Convert.ToString(Request.QueryString["ProductChangeAddTimeBegin"]) ?? string.Empty,
                ProductChangeAddTimeEnd = Convert.ToString(Request.QueryString["ProductChangeAddTimeEnd"]) ?? string.Empty,
            };

            this.Model.CurrentSc = currentSc.Json();

            var admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
            var byName = new Needs.Ccs.Services.Views.AdminsTopView().FirstOrDefault(item => item.ID == admin.ID).ByName;//获取别名
            var adminRole = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRolesAll.FirstOrDefault(item => item.ID == admin.ID);
            this.Model.Admin = new
            {
                ID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                UserName = Needs.Wl.Admin.Plat.AdminPlat.Current.UserName,
                RealName = byName,
                Role = adminRole.DeclarantRole.GetHashCode()
            }.Json();

            var pvdataApi = new PvDataApiSetting();
            var wladminApi = new WlAdminApiSetting();

            this.Model.DomainUrls = new
            {
                PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                CallBackUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.SubmitClassified,
                NextUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.GetNext,
            }.Json();
        }

        protected void data()
        {
            string ClientCode = Request.QueryString["ClientCode"];
            string OrderId = Request.QueryString["OrderID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var productList = new PD_OrderItemChangeNoticesAll();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> expression = t => t.ProcessState == ProcessState.UnProcess;

            if (!string.IsNullOrEmpty(ClientCode))
            {
                var orderids = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders.Where(item => item.Client.ClientCode == ClientCode)
                    .Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => orderids.Contains(t.OrderID);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(OrderId))
            {
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.OrderID == OrderId.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var from = DateTime.Parse(StartDate);
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.CreateDate >= from;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var to = DateTime.Parse(EndDate).AddDays(1);
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.CreateDate < to;
                lamdas.Add(lambda1);
            }

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var products = productList.GetPageList(page, rows, expression, lamdas.ToArray());

            Response.Write(new
            {
                rows = products.Select(
                    item => new
                    {
                        OrderID = item.OrderID,
                        OrderItemID = item.OrderItemID,
                        ClientCode = item.ClientCode,
                        CompanyName = item.CompanyName,
                        ProductName = item.ProductName,
                        ProductModel = item.ProductModel,
                        Type = item.Type.GetDescription(),
                        TypeValue = item.Type,
                        Date = item.CreateDate.ToString().Replace("T", ""),
                        ProcessState = item.ProcessState.GetDescription(),
                        LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                        Locker = item.Locker?.ByName ?? "--",
                        LockTime = item.LockDate?.ToString().Replace("T", " ") ?? "--",
                        IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                        IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID
                    }
                ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 获取归类信息
        /// </summary>
        /// <returns></returns>
        protected object GetClassified()
        {
            try
            {
                string itemId = Request.Form["itemId"];
                OrderItemChangeType changeType = (OrderItemChangeType)Convert.ToInt16(Request.Form["changeType"]);
                var item = new PD_ClassifyProductsAll()[itemId];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var adminRole = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRolesAll.FirstOrDefault(ar => ar.ID == admin.ID);
                var byName = new Needs.Ccs.Services.Views.AdminsTopView().FirstOrDefault(x => x.ID == admin.ID).ByName;//获取别名

                #region 锁定

                item.Admin = admin;
                item.ClassifyStep = ClassifyStep.ReClassify;
                var result = item.Lock();
                if (result.code == 300)
                {
                    //锁定失败，抛出异常
                    throw new Exception(result.data);
                }

                #endregion

                #region 合同发票

                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[item.OrderID];
                var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
                // var t2 = order.CreateDate;
                var pis = order.MainOrderFiles.Where(file => file.FileType == FileType.OriginalInvoice)
                    .ToList().Select(file => new
                    {
                        file.ID,
                        FileName = file.Name,
                        file.FileFormat,
                        Url = DateTime.Compare(file.CreateDate, t1) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + file.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + file.Url.ToUrl(),
                        // Url = FileDirectory.Current.FileServerUrl + "/" + file.Url.ToUrl(),
                    }).Json();

                #endregion

                #region 特殊类型

                var specialTypes = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == item.OrderID).ToList();
                StringBuilder sb = new StringBuilder();
                foreach (var st in specialTypes)
                {
                    sb.Append(st.Type.GetDescription() + "|");
                }

                #endregion

                #region 产品变更信息

                var changeLogs = item.GetChangeLogs();
                var mfrChangeLogs = changeLogs.Where(n => n.Type == OrderItemChangeType.BrandChange).ToArray();
                var modelChangeLogs = changeLogs.Where(n => n.Type == OrderItemChangeType.ProductModelChange).ToArray();
                var originChangeLogs = changeLogs.Where(n => n.Type == OrderItemChangeType.OriginChange).ToArray();
                var hsCodeChangeLogs = changeLogs.Where(n => n.Type == OrderItemChangeType.HSCodeChange).ToArray();
                var tariffNameChangeLogs = changeLogs.Where(n => n.Type == OrderItemChangeType.TariffNameChange).ToArray();

                var productChange = new
                {
                    Manufacturer = (mfrChangeLogs != null && mfrChangeLogs.Length > 0) ? FormatLogs(mfrChangeLogs) : string.Empty,
                    PartNumber = (modelChangeLogs != null && modelChangeLogs.Length > 0) ? FormatLogs(modelChangeLogs) : string.Empty,
                    Origin = (originChangeLogs != null && originChangeLogs.Length > 0) ? FormatLogs(originChangeLogs) : string.Empty,
                    HsCode = (hsCodeChangeLogs != null && hsCodeChangeLogs.Length > 0) ? FormatLogs(hsCodeChangeLogs) : string.Empty,
                    TariffName = (tariffNameChangeLogs != null && tariffNameChangeLogs.Length > 0) ? FormatLogs(tariffNameChangeLogs) : string.Empty,
                }.Json();

                #endregion

                #region 归类信息

                var pvdataApi = new PvDataApiSetting();
                var wladminApi = new WlAdminApiSetting();
                var data = new
                {
                    ItemID = item.ID,
                    MainID = item.OrderID,
                    OrderedDate = item.OrderedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ClientCode = item.Client.ClientCode,
                    ClientName = item.Client.Company.Name,

                    PartNumber = item.Model,
                    Manufacturer = item.Manufacturer,
                    CustomName = item.Name,
                    Origin = item.Origin,
                    UnitPrice = item.UnitPrice.ToString("0.0000"),
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    Currency = item.Currency,
                    TotalPrice = item.TotalPrice.ToString("0.0000"),
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                    HSCode = item.Category?.HSCode,
                    TariffName = item.Category?.Name,
                    ImportPreferentialTaxRate = item.ImportTax?.Rate.ToString("0.0000"),//芯达通没有区分优惠税率和加征税率, 将进口关税赋给优惠税率
                    VATRate = item.AddedValueTax?.Rate.ToString("0.0000"),
                    ExciseTaxRate = item.ExciseTax?.Rate.ToString("0.0000"),
                    TaxCode = item.Category?.TaxCode,
                    TaxName = item.Category?.TaxName,
                    LegalUnit1 = item.Category?.Unit1,
                    LegalUnit2 = item.Category?.Unit2,
                    CIQCode = item.Category?.CIQCode,
                    Elements = item.Category?.Elements,

                    OriginATRate = 0,//芯达通没有区分优惠税率和加征税率, 进口关税已经赋给优惠税率, 加征税率为0
                    CIQ = (item.Category.Type & ItemCategoryType.Inspection) > 0,
                    CIQprice = item.InspectionFee.GetValueOrDefault(),
                    Ccc = (item.Category.Type & ItemCategoryType.CCC) > 0,
                    Embargo = (item.Category.Type & ItemCategoryType.Forbid) > 0,
                    HkControl = (item.Category.Type & ItemCategoryType.HKForbid) > 0,
                    IsHighPrice = (item.Category.Type & ItemCategoryType.HighValue) > 0,
                    Coo = (item.Category.Type & ItemCategoryType.OriginProof) > 0,

                    PIs = pis,
                    ProductChange = productChange,
                    SpecialType = sb.ToString().TrimEnd('|'),
                    PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                    CallBackUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.SubmitClassified,
                    NextUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.GetNext,
                    Step = ClassifyStep.ReClassify.GetHashCode(),
                    CreatorID = admin.ID,
                    CreatorName = byName,
                    Role = adminRole.DeclarantRole.GetHashCode()
                };

                #endregion

                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = item.OrderID;

                switch (changeType)
                {
                    case OrderItemChangeType.BrandChange:
                        noticeLog.NoticeType = SendNoticeType.ManufactureChange;
                        break;

                    case OrderItemChangeType.ProductModelChange:
                        noticeLog.NoticeType = SendNoticeType.ModelChange;
                        break;

                    case OrderItemChangeType.OriginChange:
                        noticeLog.NoticeType = SendNoticeType.OriginChange;
                        break;
                }

                noticeLog.SendNotice();

                return new { code = 200, success = true, data = data };
            }
            catch (Exception ex)
            {
                return new { code = 300, success = false, data = ex.Message };
            }
        }

        private string FormatLogs(OrderItemChangeLog[] orderItemChangeLogs)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < orderItemChangeLogs.Length; i++)
            {
                sb.Append("● " + orderItemChangeLogs[i].CreateDate + "，" + orderItemChangeLogs[i].Summary + "。<br>");
            }

            return sb.ToString().Trim();
        }
    }
}
