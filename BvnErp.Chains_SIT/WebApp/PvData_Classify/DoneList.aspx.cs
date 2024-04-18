using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;
using WebApp.Classify;

namespace WebApp.PvData_Classify
{
    public partial class DoneList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
            var byName = new Needs.Ccs.Services.Views.AdminsTopView().FirstOrDefault(item => item.ID == admin.ID).ByName;//获取别名
            var adminRole = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRolesAll.FirstOrDefault(item => item.ID == admin.ID);
            this.Model.Admin = new
            {
                ID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                UserName = Needs.Wl.Admin.Plat.AdminPlat.Current.UserName,
                RealName = byName,
                Role = adminRole == null ? DeclarantRole.SeniorDeclarant.GetHashCode() : adminRole.DeclarantRole.GetHashCode()
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

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string model = Request.QueryString["Model"];
            string manufacturer = Request.QueryString["Manufacturer"];
            string ProductName = Request.QueryString["ProductName"];
            string HSCode = Request.QueryString["HSCode"];
            string StrLastClassifyTimeBegin = Request.QueryString["LastClassifyTimeBegin"];
            string StrLastClassifyTimeEnd = Request.QueryString["LastClassifyTimeEnd"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<PD_ClassifyProduct, bool>> expression = item => item.ClassifyStatus == ClassifyStatus.Done;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(orderID))
            {
                Expression<Func<PD_ClassifyProduct, bool>> lambda1 = item => item.OrderID.Contains(orderID.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(model))
            {
                Expression<Func<PD_ClassifyProduct, bool>> lambda1 = item => item.Model.Contains(model.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(manufacturer))
            {
                Expression<Func<PD_ClassifyProduct, bool>> lambda1 = item => item.Manufacturer.Contains(manufacturer.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ProductName))
            {
                lamdas.Add((Expression<Func<PD_ClassifyProduct, bool>>)(item => item.Category.Name.Contains(ProductName)));
            }
            if (!string.IsNullOrEmpty(HSCode))
            {
                lamdas.Add((Expression<Func<PD_ClassifyProduct, bool>>)(item => item.Category.HSCode.Contains(HSCode)));
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeBegin))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeBegin, out dt))
                {
                    lamdas.Add((Expression<Func<PD_ClassifyProduct, bool>>)(item => item.Category.UpdateDate >= dt));
                }
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeEnd))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeEnd, out dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<PD_ClassifyProduct, bool>>)(item => item.Category.UpdateDate < dt));
                }
            }
            #endregion

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            List<LambdaExpression> lamdasOrderByDescDateTime = new List<LambdaExpression>();
            lamdasOrderByDescDateTime.Add((Expression<Func<PD_ClassifyProduct, DateTime>>)(t => t.CreateDate));

            var products = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PD_ClassifyProductsAll.GetPageList(
                page,
                rows,
                expression,
                null,
                lamdasOrderByDescDateTime.ToArray(),
                "",
                true,
                lamdas.ToArray());

            Response.Write(new
            {
                rows = products.Select(
                        item => new
                        {
                            item.ID,
                            ItemID = item.ID,
                            item.OrderID,
                            MainID = item.OrderID,
                            item.MainOrderID,
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
                            ClassifyStatus = item.ClassifyStatus.GetDescription(),
                            ClassifyFirstOperatorName = item.Category.ClassifyFirstOperator?.RealName ?? "--",
                            ClassifySecondOperatorName = item.Category.ClassifySecondOperator?.RealName ?? "--",
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            CompleteDate = item.Category.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            OrderStatus = item.OrderStatus.GetDescription(),
                            IsQuoted = item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted, //是否已报价

                            HSCode = item.Category.HSCode,
                            TariffName = item.Category.Name,
                            ImportPreferentialTaxRate = item.ImportTax.Rate.ToString("0.0000"),
                            VATRate = item.AddedValueTax.Rate.ToString("0.0000"),
                            ExciseTaxRate = item.ExciseTax?.Rate.ToString("0.0000") ?? "0.0000",
                            TaxCode = item.Category.TaxCode,
                            TaxName = item.Category.TaxName,
                            LegalUnit1 = item.Category.Unit1,
                            LegalUnit2 = item.Category.Unit2,
                            CIQCode = item.Category.CIQCode,
                            Elements = item.Category.Elements,

                            OriginATRate = "",
                            CIQ = (item.Category.Type & ItemCategoryType.Inspection) > 0,
                            CIQprice = item.InspectionFee.GetValueOrDefault(),
                            Ccc = (item.Category.Type & ItemCategoryType.CCC) > 0,
                            Embargo = (item.Category.Type & ItemCategoryType.Forbid) > 0,
                            HkControl = (item.Category.Type & ItemCategoryType.HKForbid) > 0,
                            IsHighPrice = (item.Category.Type & ItemCategoryType.HighValue) > 0,
                            Coo = (item.Category.Type & ItemCategoryType.OriginProof) > 0,

                            //产品归类锁定
                            LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                            Locker = item.Locker?.ByName ?? "--",
                            LockTime = item.LockDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "--",
                            IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                            IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID
                        }
                     ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 获取订单信息：合同发票、特殊类型
        /// </summary>
        /// <returns></returns>
        protected object GetOrderInfos()
        {
            string orderId = Request.Form["orderId"];

            #region 合同发票
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[orderId];
            var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
            var pis = order.MainOrderFiles.Where(item => item.FileType == FileType.OriginalInvoice)
                .ToList().Select(item => new
                {
                    item.ID,
                    FileName = item.Name,
                    item.FileFormat,
                    //Url = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
                    Url = DateTime.Compare(item.CreateDate, t1) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
                }).Json();
            #endregion

            #region 特殊类型
            var specialTypes = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.OrderVoyageNo.Where(t => t.Order.ID == orderId).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var st in specialTypes)
            {
                sb.Append(st.Type.GetDescription() + "|");
            }
            #endregion

            return new
            {
                PIs = pis,
                SpecialType = sb.Length > 0 ? sb.ToString().TrimEnd('|') : "--",
            };
        }

        /// <summary>
        /// 导出归类产品数据excel
        /// </summary>
        protected void Export()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string OrderID = Request.Form["OrderID"];
            string Model = Request.Form["Model"];
            string Manufacturer = Request.Form["Manufacturer"];
            string ProductName = Request.Form["ProductName"];
            string HSCode = Request.Form["HSCode"];
            string StrLastClassifyTimeBegin = Request.Form["LastClassifyTimeBegin"];
            string StrLastClassifyTimeEnd = Request.Form["LastClassifyTimeEnd"];

            try
            {
                var predicate = PredicateBuilder.Create<ClassifyDoneAllModels>();

                if (!string.IsNullOrEmpty(OrderID))
                {
                    OrderID = OrderID.Trim();
                    predicate = predicate.And(item => item.OrderID.Contains(OrderID));
                }
                if (!string.IsNullOrEmpty(Model))
                {
                    Model = Model.Trim();
                    predicate = predicate.And(item => item.Model.Contains(Model));
                }
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    Manufacturer = Manufacturer.Trim();
                    predicate = predicate.And(item => item.Manufacturer.Contains(Manufacturer));
                }
                if (!string.IsNullOrEmpty(ProductName))
                {
                    ProductName = ProductName.Trim();
                    predicate = predicate.And(item => item.ProductName.Contains(ProductName));
                }
                if (!string.IsNullOrEmpty(HSCode))
                {
                    HSCode = HSCode.Trim();
                    predicate = predicate.And(item => item.HSCode.Contains(HSCode));
                }
                if (!string.IsNullOrEmpty(StrLastClassifyTimeBegin))
                {
                    DateTime dt1 = DateTime.Parse(StrLastClassifyTimeBegin);
                    predicate = predicate.And(item => item.CompleteTime >= dt1);
                }
                if (!string.IsNullOrEmpty(StrLastClassifyTimeEnd))
                {
                    DateTime dt2 = DateTime.Parse(StrLastClassifyTimeEnd);
                    dt2 = dt2.AddDays(1);
                    predicate = predicate.And(item => item.CompleteTime < dt2);
                }

                var view = new Needs.Ccs.Services.Views.Alls.PD_ClassifyProductsExcel();
                view.AllowPaging = false;
                view.PageIndex = page;
                view.PageSize = rows;
                view.Predicate = predicate;

                int recordCount = view.RecordCount;
                var doneLists = view.ToList();

                var logsView = new Needs.Ccs.Services.Views.Alls.Logs_ClassifyModifiedAll();
                var logs = (from log in logsView
                            where log.Summary.Contains("海关编码")
                            select new { log.PartNumber, log.Manufacturer, log.Summary }).ToList();

                Func<ClassifyDoneAllModels, object> convert = item => new
                {
                    item.Manufacturer,
                    item.Model,
                    item.ProductName,
                    item.HSCode,
                    item.Elements,
                    item.TariffRate,
                    item.UnitPrice,
                    item.Quantity,
                    ClassifyStatus = item.ClassifyStatus.GetDescription(),
                    item.ClientName,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    CompleteTime = item.CompleteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.ClassifyFirstOperatorName,
                    item.ClassifySecondOperatorName,
                    item.TaxCode,
                    item.TaxName,
                    IsOrdered = item.IsOrdered ? "已下单" : "未下单",
                    OrderID = item.OrderID == null ? "-" : item.OrderID,
                    ClassifyLog = string.Join(",", logs.Where(log => log.PartNumber == item.Model && log.Manufacturer == item.Manufacturer).Select(log => log.Summary))
                };

                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(doneLists.Select(convert).ToArray().Json());

                string fileName = DateTime.Now.Ticks + ".xlsx";

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                #region 设置导出格式

                var excelconfig = new ExcelConfig();
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.Title = "归类已完成产品";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Manufacturer", ExcelColumn = "品牌", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Model", ExcelColumn = "产品型号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ProductName", ExcelColumn = "报关品名", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "HSCode", ExcelColumn = "HS编码", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Elements", ExcelColumn = "申报要素", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TariffRate", ExcelColumn = "关税率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UnitPrice", ExcelColumn = "单价", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Quantity", ExcelColumn = "数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClassifyStatus", ExcelColumn = "归类状态", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientName", ExcelColumn = "客户名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CreateDate", ExcelColumn = "创建时间", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CompleteTime", ExcelColumn = "归类完成时间", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClassifyFirstOperatorName", ExcelColumn = "预处理一人员", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClassifySecondOperatorName", ExcelColumn = "预处理二人员", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxCode", ExcelColumn = "税务编码", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxName", ExcelColumn = "税务名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "IsOrdered", ExcelColumn = "是否下单", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单编号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClassifyLog", ExcelColumn = "归类日志", Alignment = "center" });

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
