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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;
using WebApp.Classify;

namespace WebApp.PvData_ConsultClassify
{
    /// <summary>
    /// 已完成查询界面
    /// </summary>
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
                CallBackUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreSubmitClassified,
                NextUrl = ConfigurationManager.AppSettings[wladminApi.ApiName] + wladminApi.PreGetNext,
            }.Json();
        }

        /// <summary>
        /// 初始化预归类产品数据
        /// </summary>
        protected void data()
        {
            string Model = Request.QueryString["Model"];
            string manufacturer = Request.QueryString["Manufacturer"];
            string ProductName = Request.QueryString["ProductName"];
            string HSCode = Request.QueryString["HSCode"];
            string StrLastClassifyTimeBegin = Request.QueryString["LastClassifyTimeBegin"];
            string StrLastClassifyTimeEnd = Request.QueryString["LastClassifyTimeEnd"];
            string IsCCC = Request.QueryString["IsCCC"];
            string IsForbidden = Request.QueryString["IsForbidden"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<PD_PreClassifyProduct, bool>> expression = item => (item.ClassifyStatus == ClassifyStatus.Done || item.ClassifyStatus == ClassifyStatus.Anomaly) &&
                                                                                item.PreProduct.UseType == PreProductUserType.Consult;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(Model))
            {
                Expression<Func<PD_PreClassifyProduct, bool>> lambda1 = item => item.Model.Contains(Model.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(manufacturer))
            {
                Expression<Func<PD_PreClassifyProduct, bool>> lambda1 = item => item.Manufacturer.Contains(manufacturer.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ProductName))
            {
                lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.ProductName.Contains(ProductName)));
            }
            if (!string.IsNullOrEmpty(HSCode))
            {
                lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.HSCode.Contains(HSCode)));
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeBegin))
            {
                if (DateTime.TryParse(StrLastClassifyTimeBegin, out DateTime dt))
                {
                    lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.UpdateDate >= dt));
                }
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeEnd))
            {
                if (DateTime.TryParse(StrLastClassifyTimeEnd, out DateTime dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => item.UpdateDate < dt));
                }
            }
            if (!string.IsNullOrEmpty(IsCCC))
            {
                bool ccc = false;
                bool.TryParse(IsCCC, out ccc);
                if (ccc)
                {
                    lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => (item.Type & ItemCategoryType.CCC) > 0));
                }
            }
            if (!string.IsNullOrEmpty(IsForbidden))
            {
                bool forbidden = false;
                bool.TryParse(IsForbidden, out forbidden);
                if (forbidden)
                {
                    lamdas.Add((Expression<Func<PD_PreClassifyProduct, bool>>)(item => ((item.Type & ItemCategoryType.Forbid) | (item.Type & ItemCategoryType.HKForbid)) > 0));
                }
            }
            #endregion

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var products = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PD_PreClassifyProductsDone.GetPageList(
                page,
                rows,
                expression,
                lamdas.ToArray());

            var resultRows = products.Select(
                       item => new
                       {
                           item.ID,
                           MainID = item.ID,
                           ClientCode = item.PreProduct.Client.ClientCode,
                           ClientName = item.PreProduct.Client.Company.Name,
                           ProductUnionCode = item.ProductUnionCode,

                           PartNumber = item.Model,
                           Manufacturer = item.Manufacturer,
                           Origin = string.Empty,
                           UnitPrice = item.PreProduct.Price.ToString("0.0000"),
                           Quantity = item.PreProduct.Qty,
                           Currency = item.PreProduct.Currency,
                           ClassifyStatusVal = item.ClassifyStatus,
                           ClassifyStatus = item.ClassifyStatus.GetDescription(),
                           ClassifyFirstOperatorName = item.ClassifyFirstOperatorName,
                           ClassifySecondOperatorName = item.ClassifySecondOperatorName,
                           CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                           CompleteDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                           CompanyType = item.PreProduct.CompanyType.GetDescription(),

                           HSCode = item.HSCode,
                           TariffName = item.ProductName,
                           ImportPreferentialTaxRate = item.TariffRate?.ToString("0.0000"),
                           VATRate = item.AddedValueRate?.ToString("0.0000"),
                           ExciseTaxRate = item.ExciseTaxRate?.ToString("0.0000") ?? "0.0000",
                           TaxCode = item.TaxCode,
                           TaxName = item.TaxName,
                           LegalUnit1 = item.Unit1,
                           LegalUnit2 = item.Unit2,
                           CIQCode = item.CIQCode,
                           Elements = item.Elements,

                           OriginATRate = "",
                           CIQ = (item.Type & ItemCategoryType.Inspection) > 0,
                           CIQprice = item.InspectionFee.GetValueOrDefault(),
                           Ccc = (item.Type & ItemCategoryType.CCC) > 0,
                           Embargo = (item.Type & ItemCategoryType.Forbid) > 0,
                           HkControl = (item.Type & ItemCategoryType.HKForbid) > 0,
                           IsHighPrice = (item.Type & ItemCategoryType.HighValue) > 0,
                           Coo = (item.Type & ItemCategoryType.OriginProof) > 0,

                           IsPushStatusWarning = item.IsPushStatusWarning,
                           //特殊类型
                           SpecialType = item.GetSpecialType(),

                           //产品归类锁定
                           LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                           Locker = item.Locker?.ByName ?? "--",
                           LockTime = item.LockDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "--",
                           IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                           IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                           item.RegisterName,
                           item.IcgooAdminName
                       }
                    ).ToList();

            var orderItemsOrigin = new Needs.Ccs.Services.Views.Origins.OrderItemsOrigin();
            string[] productUnionCodesInResultRows = resultRows.Select(t => t.ProductUnionCode).ToArray();
            var productUniqueCodeExistInOrderItems = (from orderItemsOrigin_Item in orderItemsOrigin
                                                      where productUnionCodesInResultRows.Contains(orderItemsOrigin_Item.ProductUniqueCode)
                                                         && orderItemsOrigin_Item.Status == Needs.Ccs.Services.Enums.Status.Normal
                                                      group orderItemsOrigin_Item by new { orderItemsOrigin_Item.ProductUniqueCode } into g
                                                      select new
                                                      {
                                                          ProductUniqueCode = g.Key.ProductUniqueCode,
                                                          //ProductUniqueCodeCount = g.Count(),
                                                      }).ToList();

            var rowDisplay = from resultRow in resultRows
                             join productUniqueCodeExistInOrderItem in productUniqueCodeExistInOrderItems
                                    on new { ProductUniqueCode = resultRow.ProductUnionCode, }
                                    equals new { ProductUniqueCode = productUniqueCodeExistInOrderItem.ProductUniqueCode, }
                                    into productUniqueCodeExistInOrderItems2
                             from productUniqueCodeExistInOrderItem in productUniqueCodeExistInOrderItems2.DefaultIfEmpty()
                             select new
                             {
                                 resultRow.ID,
                                 resultRow.MainID,
                                 resultRow.ClientCode,
                                 resultRow.ClientName,

                                 resultRow.PartNumber,
                                 resultRow.Manufacturer,
                                 resultRow.Origin,
                                 resultRow.UnitPrice,
                                 resultRow.Quantity,
                                 resultRow.Currency,
                                 resultRow.ClassifyStatusVal,
                                 resultRow.ClassifyStatus,
                                 resultRow.ClassifyFirstOperatorName,
                                 resultRow.ClassifySecondOperatorName,
                                 resultRow.CreateDate,
                                 resultRow.CompleteDate,
                                 resultRow.CompanyType,
                                 PreOrderStatus = productUniqueCodeExistInOrderItem == null ? "未下单" : "已下单", //预归类产品下单状态
                                 IsOrdered = productUniqueCodeExistInOrderItem == null ? false : true, //接口是否已下单
                                 resultRow.ProductUnionCode,

                                 resultRow.HSCode,
                                 resultRow.TariffName,
                                 resultRow.ImportPreferentialTaxRate,
                                 resultRow.VATRate,
                                 resultRow.ExciseTaxRate,
                                 resultRow.TaxCode,
                                 resultRow.TaxName,
                                 resultRow.LegalUnit1,
                                 resultRow.LegalUnit2,
                                 resultRow.CIQCode,
                                 resultRow.Elements,

                                 resultRow.OriginATRate,
                                 resultRow.CIQ,
                                 resultRow.CIQprice,
                                 resultRow.Ccc,
                                 resultRow.Embargo,
                                 resultRow.HkControl,
                                 resultRow.IsHighPrice,
                                 resultRow.Coo,

                                 resultRow.IsPushStatusWarning,
                                 resultRow.SpecialType,

                                 //产品归类锁定
                                 resultRow.LockStatus,
                                 resultRow.Locker,
                                 resultRow.LockTime,
                                 resultRow.IsCanClassify,
                                 resultRow.IsCanUnlock,
                                 resultRow.RegisterName,
                                 resultRow.IcgooAdminName
                             };

            Response.Write(new
            {
                rows = rowDisplay.ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 退回
        /// </summary>
        /// <returns></returns>
        protected void Return()
        {
            try
            {
                string id = Request.Form["mainId"];
                string reason = Request.Form["Reason"];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var product = new PD_PreClassifyProduct()
                {
                    ID = id,
                    Admin = admin,
                    ClassifyStep = ClassifyStep.PreDoneEdit,
                    Summary = reason,
                };
                var classify = PD_ClassifyFactory.Create(ClassifyStep.PreDoneEdit, product);
                classify.Return();

                Response.Write((new { success = true, message = "退回成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = true, message = "退回失败: " + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 获取订单信息：合同发票、供应商
        /// </summary>
        /// <returns></returns>
        protected object GetOrderInfos()
        {
            string code = Request.Form["productUnionCode"];

            #region 合同发票
            string pis = "";
            var orderFiles = new Needs.Ccs.Services.Views.PreProductInvoicesView(code);
            if (orderFiles != null && orderFiles.Count() > 0)
            {
                pis = orderFiles.ToList().Select(item => new
                {
                    item.ID,
                    FileName = item.Name,
                    item.FileFormat,
                    Url = FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl(),
                }).Json();
            }
            #endregion

            #region 供应商
            string supplier = "";
            var supplierMap = new Needs.Ccs.Services.Views.PreProduct.ProductSupplierMapView().FirstOrDefault(map => map.ID == code);
            if (supplierMap != null)
            {
                var clientSupplier = new Needs.Ccs.Services.Views.ClientSuppliersView().FirstOrDefault(cs => cs.ID == supplierMap.SupplierID);
                supplier = new { clientSupplier.ID, clientSupplier.Name, clientSupplier.ChineseName }.Json();
            }
            #endregion

            return new
            {
                PIs = pis,
                Supplier = supplier,
            };
        }

        /// <summary>
        /// 导出预归类产品数据excel
        /// </summary>
        protected void Export()
        {
            int page, rows;
            int.TryParse(Request.Form["page"], out page);
            int.TryParse(Request.Form["rows"], out rows);

            string Model = Request.Form["Model"];
            string Manufacturer = Request.Form["Manufacturer"];
            string ProductName = Request.Form["ProductName"];
            string HSCode = Request.Form["HSCode"];
            string StrLastClassifyTimeBegin = Request.Form["LastClassifyTimeBegin"];
            string StrLastClassifyTimeEnd = Request.Form["LastClassifyTimeEnd"];

            try
            {
                var predicate = PredicateBuilder.Create<ClassifyDoneAllModels>();
                predicate = predicate.And(item => item.UseType == PreProductUserType.Consult);

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

                var view = new Needs.Ccs.Services.Views.Alls.PD_PreClassifyProductsExcel();
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
                    ClassifyStatus = item.ClassifyStatus.GetDescription(),
                    item.ClientName,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    CompleteTime = item.CompleteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.ClassifyFirstOperatorName,
                    item.ClassifySecondOperatorName,
                    item.TaxCode,
                    item.TaxName,
                    IsOrdered = item.IsOrdered ? "已下单" : "未下单",
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
                excelconfig.Title = "预归类已完成产品";
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
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClassifyStatus", ExcelColumn = "归类状态", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientName", ExcelColumn = "客户名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CreateDate", ExcelColumn = "创建时间", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CompleteTime", ExcelColumn = "归类完成时间", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClassifyFirstOperatorName", ExcelColumn = "预处理一人员", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClassifySecondOperatorName", ExcelColumn = "预处理二人员", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxCode", ExcelColumn = "税务编码", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxName", ExcelColumn = "税务名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "IsOrdered", ExcelColumn = "是否下单", Alignment = "center" });
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

        protected void Delete()
        {
            try
            {
                string id = Request.Form["mainId"];
                var product = new PD_PreClassifyProduct()
                {
                    ID = id,
                    ClassifyStep = ClassifyStep.PreDoneEdit,
                    PreProduct = new PreProduct
                    {
                        ProductUnionCode = id
                    }
                };
                var classify = PD_ClassifyFactory.Create(ClassifyStep.PreDoneEdit, product);
                classify.Delete();

                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = true, message = "删除失败: " + ex.Message }).Json());
            }
        }

        protected void DeleteCheck()
        {
            string id = Request.Form["mainId"];
            var product = new Needs.Ccs.Services.Views.OrderItemsView().Where(t => t.ProductUniqueCode == id).FirstOrDefault();
            if (product != null)
            {
                Response.Write((new { success = false, message = "该产品已生成订单，不能删除!" }).Json());
            }
            else
            {
                Response.Write((new { success = true, message = "可以删除" }).Json());
            }
        }
    }
}
