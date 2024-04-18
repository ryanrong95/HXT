using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Classify.PreProduct
{
    /// <summary>
    /// 已完成查询界面
    /// </summary>
    public partial class DoneList : Uc.PageBase
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
                IsShowLocked = Convert.ToBoolean(Request.QueryString["IsShowLocked"]),
                Model = Convert.ToString(Request.QueryString["Model"]) ?? string.Empty,
                OrderID = Convert.ToString(Request.QueryString["OrderID"]) ?? string.Empty,
                ProductName = Convert.ToString(Request.QueryString["ProductName"]) ?? string.Empty,
                HSCode = Convert.ToString(Request.QueryString["HSCode"]) ?? string.Empty,
                LastClassifyTimeBegin = Convert.ToString(Request.QueryString["LastClassifyTimeBegin"]) ?? string.Empty,
                LastClassifyTimeEnd = Convert.ToString(Request.QueryString["LastClassifyTimeEnd"]) ?? string.Empty,
            };

            this.Model.CurrentSc = currentSc.Json();
        }

        /// <summary>
        /// 初始化预归类产品数据
        /// </summary>
        protected void data()
        {
            string Model = Request.QueryString["Model"];
            string ProductName = Request.QueryString["ProductName"];
            string HSCode = Request.QueryString["HSCode"];
            string StrLastClassifyTimeBegin = Request.QueryString["LastClassifyTimeBegin"];
            string StrLastClassifyTimeEnd = Request.QueryString["LastClassifyTimeEnd"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<PreClassifyProduct, bool>> expression = item => item.ClassifyStatus == ClassifyStatus.Done;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(Model))
            {
                Expression<Func<PreClassifyProduct, bool>> lambda1 = item => item.Model.Contains(Model);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ProductName))
            {
                lamdas.Add((Expression<Func<PreClassifyProduct, bool>>)(item => item.ProductName.Contains(ProductName)));
            }
            if (!string.IsNullOrEmpty(HSCode))
            {
                lamdas.Add((Expression<Func<PreClassifyProduct, bool>>)(item => item.HSCode.Contains(HSCode)));
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeBegin))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeBegin, out dt))
                {
                    lamdas.Add((Expression<Func<PreClassifyProduct, bool>>)(item => item.UpdateDate >= dt));
                }
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeEnd))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeEnd, out dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<PreClassifyProduct, bool>>)(item => item.UpdateDate < dt));
                }
            }
            #endregion

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            List<LambdaExpression> lamdasOrderByDescDateTime = new List<LambdaExpression>();
            lamdasOrderByDescDateTime.Add((Expression<Func<PreClassifyProduct, DateTime>>)(t => t.CreateDate));

            var products = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PreClassifyProductsAll.GetPageList(
                page,
                rows,
                expression,
                null,
                lamdasOrderByDescDateTime.ToArray(),
                "",
                true,
                lamdas.ToArray());

            var resultRows = products.Select(
                       item => new
                       {
                           item.ID,
                           item.ProductName,
                           item.Model,
                           item.Manufacturer,
                           item.HSCode,
                           PreProductID = item.PreProductID,

                           item.Elements,
                           item.TariffRate,
                           UnitPrice = item.PreProduct.Price,
                           item.PreProduct.Currency,
                           item.Unit1,
                           item.Unit2,
                           item.AddedValueRate,
                           item.CIQCode,
                           ClassifyStatus = item.ClassifyStatus.GetDescription(),
                           ClientCode = item.PreProduct.Client.ClientCode,
                           ClientName = item.PreProduct.Client.Company.Name,
                           item.TaxCode,
                           item.TaxName,
                           CompanyType = item.PreProduct.CompanyType.GetDescription(),
                           CreateDate = item.CreateDate.ToString().Replace("T", " ") ?? "--",
                           ClassifyFirstOperatorName = string.IsNullOrEmpty(item.ClassifyFirstOperatorName) ? "--" : item.ClassifyFirstOperatorName,
                           ClassifySecondOperatorName = string.IsNullOrEmpty(item.ClassifySecondOperatorName) ? "--" : item.ClassifySecondOperatorName,
                           //PreOrderStatus = string.IsNullOrEmpty(item.ProductUnionCode) ? "未下单" : "已下单", //预归类产品下单状态
                           //IsOrdered = string.IsNullOrEmpty(item.ProductUnionCode) ? false : true, //接口是否已下单
                           ProductUnionCode = item.ProductUnionCode,

                           //产品归类锁定
                           LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                           Locker = item.Locker?.RealName ?? "--",
                           LockTime = item.LockDate?.ToString().Replace("T", " ") ?? "--",
                           IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                           IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
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
                                 resultRow.ProductName,
                                 resultRow.Model,
                                 resultRow.Manufacturer,
                                 resultRow.HSCode,
                                 resultRow.PreProductID,

                                 resultRow.Elements,
                                 resultRow.TariffRate,
                                 resultRow.UnitPrice,
                                 resultRow.Currency,
                                 resultRow.Unit1,
                                 resultRow.Unit2,
                                 resultRow.AddedValueRate,
                                 resultRow.CIQCode,
                                 resultRow.ClassifyStatus,
                                 resultRow.ClientCode,
                                 resultRow.ClientName,
                                 resultRow.TaxCode,
                                 resultRow.TaxName,
                                 resultRow.CompanyType,
                                 resultRow.CreateDate,
                                 resultRow.ClassifyFirstOperatorName,
                                 resultRow.ClassifySecondOperatorName,
                                 PreOrderStatus = productUniqueCodeExistInOrderItem == null ? "未下单" : "已下单", //预归类产品下单状态
                                 IsOrdered = productUniqueCodeExistInOrderItem == null ? false : true, //接口是否已下单
                                 resultRow.ProductUnionCode,

                                 //产品归类锁定
                                 resultRow.LockStatus,
                                 resultRow.Locker,
                                 resultRow.LockTime,
                                 resultRow.IsCanClassify,
                                 resultRow.IsCanUnlock,
                             };

            Response.Write(new
            {
                rows = rowDisplay.ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 归类锁定
        /// </summary>
        protected void Lock()
        {
            try
            {
                var id = Request.Form["ID"];
                var from = Request.Form["From"];
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PreClassifyProductsAll[id];

                var orderItemsOrigin = new Needs.Ccs.Services.Views.Origins.OrderItemsOrigin();
                int countUnionCodeCountInOrderItems = orderItemsOrigin
                    .Where(t => t.ProductUniqueCode == classifyProduct.ProductUnionCode
                             && t.Status == Needs.Ccs.Services.Enums.Status.Normal)
                    .Count();

                if (classifyProduct.IsLocked && classifyProduct.Locker.ID != Needs.Wl.Admin.Plat.AdminPlat.Current.ID)
                {
                    Response.Write((new { success = false, message = "当前产品归类已被锁定，锁定人【" + classifyProduct.Locker.RealName + "】，锁定时间【" + classifyProduct.LockDate + "】" }).Json());
                }
                else if (from == ClassifyStep.PreDoneEdit.GetHashCode().ToString() && countUnionCodeCountInOrderItems > 0)  //&& !string.IsNullOrEmpty(classifyProduct.ProductUnionCode)
                {
                    Response.Write((new { success = false, message = "接口已经下单！" }).Json());
                }
                else
                {
                    if (!classifyProduct.IsLocked)
                    {
                        classifyProduct.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                        var classify = ClassifyFactory.Create(ClassifyStep.PreDoneEdit, classifyProduct);
                        classify.Lock();
                    }
                    Response.Write((new { success = true, message = "" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 解除归类锁定
        /// </summary>
        protected void UnLock()
        {
            try
            {
                var id = Request.Form["ID"];
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.PreClassifyProductsAll[id];
                classifyProduct.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var classify = ClassifyFactory.Create(ClassifyStep.PreDoneEdit, classifyProduct);
                classify.UnLock();

                Response.Write((new { success = true, message = "已解除产品归类锁定" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出已报关数据excel
        /// </summary>
        protected void Export()
        {
            int page, rows;
            int.TryParse(Request.Form["page"], out page);
            int.TryParse(Request.Form["rows"], out rows);

            string Model = Request.Form["Model"];
            string ProductName = Request.Form["ProductName"];
            string HSCode = Request.Form["HSCode"];
            string StrLastClassifyTimeBegin = Request.Form["LastClassifyTimeBegin"];
            string StrLastClassifyTimeEnd = Request.Form["LastClassifyTimeEnd"];

            try
            {
                var predicate = PredicateBuilder.Create<ClassifyDoneAllModels>();

                if (!string.IsNullOrEmpty(Model))
                {
                    Model = Model.Trim();
                    predicate = predicate.And(item => item.Model.Contains(Model));
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
                    if (DateTime.TryParse(StrLastClassifyTimeBegin, out DateTime dt1))
                    {
                        predicate = predicate.And(item => item.CompleteTime >= dt1);
                    }
                }
                if (!string.IsNullOrEmpty(StrLastClassifyTimeEnd))
                {
                    if (DateTime.TryParse(StrLastClassifyTimeEnd, out DateTime dt2))
                    {
                        dt2 = dt2.AddDays(1);
                        predicate = predicate.And(item => item.CompleteTime < dt2);
                    }
                }

                Needs.Ccs.Services.Views.Alls.PreClassifyDoneAll view = new Needs.Ccs.Services.Views.Alls.PreClassifyDoneAll();
                view.AllowPaging = false;
                view.PageIndex = page;
                view.PageSize = rows;
                view.Predicate = predicate;

                int recordCount = view.RecordCount;
                var doneLists = view.ToList();

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
                    IsOrdered = item.IsOrdered? "已下单" : "未下单",
                    ClassifyLog = string.Join(",", item.ClassifyLog)
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

    }
}