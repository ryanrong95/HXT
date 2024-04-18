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

namespace WebApp.Classify.Product
{
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
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string model = Request.QueryString["Model"];
            string ProductName = Request.QueryString["ProductName"];
            string HSCode = Request.QueryString["HSCode"];
            string StrLastClassifyTimeBegin = Request.QueryString["LastClassifyTimeBegin"];
            string StrLastClassifyTimeEnd = Request.QueryString["LastClassifyTimeEnd"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<ClassifyProduct, bool>> expression = item => item.ClassifyStatus == ClassifyStatus.Done;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(orderID))
            {
                Expression<Func<ClassifyProduct, bool>> lambda1 = item => item.OrderID.Contains(orderID.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(model))
            {
                var orderItemIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems.Where(item=>item.Model.Contains(model.Trim())).Select(item=>item.ID).ToArray();
                //var productIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Products.Where(c => c.Model.Contains(model.Trim())).Select(c => c.ID).ToArray();
                Expression<Func<ClassifyProduct, bool>> lambda1 = item => orderItemIDs.Contains(item.ID);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ProductName))
            {
                lamdas.Add((Expression<Func<ClassifyProduct, bool>>)(item => item.Category.Name.Contains(ProductName)));
            }
            if (!string.IsNullOrEmpty(HSCode))
            {
                lamdas.Add((Expression<Func<ClassifyProduct, bool>>)(item => item.Category.HSCode.Contains(HSCode)));
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeBegin))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeBegin, out dt))
                {
                    lamdas.Add((Expression<Func<ClassifyProduct, bool>>)(item => item.Category.UpdateDate >= dt));
                }
            }
            if (!string.IsNullOrEmpty(StrLastClassifyTimeEnd))
            {
                DateTime dt;
                if (DateTime.TryParse(StrLastClassifyTimeEnd, out dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<ClassifyProduct, bool>>)(item => item.Category.UpdateDate < dt));
                }
            }
            #endregion

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            List<LambdaExpression> lamdasOrderByDescDateTime = new List<LambdaExpression>();
            lamdasOrderByDescDateTime.Add((Expression<Func<ClassifyProduct, DateTime>>)(t => t.CreateDate));

            var products = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll.GetPageList(
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
                            item.OrderID,
                            item.Client.ClientCode,
                            ClientName = item.Client.Company.Name,
                            item.Category.HSCode,
                            Name = item.Category.Name,
                            item.Category.Elements,
                            item.Model,
                            item.Manufacturer,
                            item.Origin,
                            item.Quantity,
                            item.Unit,
                            item.Category.Unit1,
                            UnitPrice = item.UnitPrice.ToString("0.0000"),
                            item.Currency,
                            TariffRate = item.ImportTax.Rate.ToString("0.0000"),
                            ValueAddRate = item.AddedValueTax.Rate.ToString("0.0000"),
                            item.Category.CIQCode,
                            ClassifyStatus = item.ClassifyStatus.GetDescription(),
                            CreateDate = item.CreateDate.ToString().Replace("T", " ") ?? "--",
                            OrderStatus = item.OrderStatus.GetDescription(),
                            IsQuoted = item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted, //是否已报价

                            //产品归类锁定
                            LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                            Locker = item.Locker?.RealName ?? "--",
                            LockTime = item.LockDate?.ToString().Replace("T", " ") ?? "--",
                            IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                            IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID,

                            ClassifyFirstOperatorName = string.IsNullOrEmpty(item.ClassifyFirstOperatorName) ? "--" : item.ClassifyFirstOperatorName,
                            ClassifySecondOperatorName = string.IsNullOrEmpty(item.ClassifySecondOperatorName) ? "--" : item.ClassifySecondOperatorName,

                        }
                     ).ToArray(),
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
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll[id];
                if (classifyProduct.IsLocked && classifyProduct.Locker.ID != Needs.Wl.Admin.Plat.AdminPlat.Current.ID)
                {
                    Response.Write((new { success = false, message = "当前产品归类已被锁定，锁定人【" + classifyProduct.Locker.RealName + "】，锁定时间【" + classifyProduct.LockDate + "】" }).Json());
                }
                else if (from == ClassifyStep.DoneEdit.GetHashCode().ToString() && classifyProduct.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted)
                {
                    Response.Write((new { success = false, message = "该订单已报价！" }).Json());
                }
                else
                {
                    if (!classifyProduct.IsLocked)
                    {
                        classifyProduct.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                        var classify = ClassifyFactory.Create(ClassifyStep.DoneEdit, classifyProduct);
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
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll[id];
                classifyProduct.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var classify = ClassifyFactory.Create(ClassifyStep.DoneEdit, classifyProduct);
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
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string OrderID = Request.Form["OrderID"];
            string Model = Request.Form["Model"];
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

                Needs.Ccs.Services.Views.Alls.ClassifyDoneAll view = new Needs.Ccs.Services.Views.Alls.ClassifyDoneAll();
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
                    IsOrdered = item.IsOrdered ? "已下单" : "未下单",
                    OrderID = item.OrderID == null ? "-" : item.OrderID,
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