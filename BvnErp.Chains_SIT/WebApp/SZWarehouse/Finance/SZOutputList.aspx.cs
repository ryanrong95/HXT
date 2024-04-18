using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.SZWarehouse.Finance
{
    public partial class SZOutputList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void data()
        {
            string Name = Request.QueryString["Name"];
            string Model = Request.QueryString["Model"];
            string InvoiceCompany = Request.QueryString["InvoiceCompany"];
            string StartTime = Request.QueryString["StartTime"];
            string EndTime = Request.QueryString["EndTime"];

            var SZOutput = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOutput.AsQueryable();
            if (!string.IsNullOrEmpty(Name))
            {
                SZOutput = SZOutput.Where(item => item.Sorting.OrderItem.Name.Contains(Name));
            }
            if (!string.IsNullOrEmpty(Model))
            {
                SZOutput = SZOutput.Where(item => item.Sorting.OrderItem.Model.Contains(Model));
            }
            if (!string.IsNullOrEmpty(InvoiceCompany))
            {
                SZOutput = SZOutput.Where(item => item.InvoiceCompany.Contains(InvoiceCompany));
            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                SZOutput = SZOutput.Where(item => item.CreateDate.CompareTo(StartTime) >= 0);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                var endTime = DateTime.Parse(EndTime).AddDays(1);
                SZOutput = SZOutput.Where(item => item.CreateDate.CompareTo(endTime) < 0);
            }

            Func<Needs.Ccs.Services.Models.SZOutput, object> convert = item => new
            {
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd"),
                Name = item.Sorting.OrderItem.Name,
                Model = item.Sorting.OrderItem.Model,
                Brand = item.Sorting.OrderItem.Manufacturer,
                TotalQuantity = item.TotalQuantity,
                Quantity = item.Quantity,
                InUnitPrice = item.InUnitPrice,
                CostAmount = item.CostAmount,
                OutUnitPrice = item.OutUnitPrice,
                SalesAmount = item.SalesAmount,
                InterestRate = item.InterestRate,
                TaxRate = item.TaxRate,
                TaxAmount = item.TaxAmount,
                TotalAmount = item.TotalAmount,
                TotalInterestRate = item.TotalInterestRate,
                InvoiceCompany = item.InvoiceCompany,
            };
            this.Paging(SZOutput, convert);
        }

        protected void Export()
        {
            try
            {
                string Name = Request.QueryString["Name"];
                string Model = Request.QueryString["Model"];
                string InvoiceCompany = Request.QueryString["InvoiceCompany"];
                string StartTime = Request.QueryString["StartTime"];
                string EndTime = Request.QueryString["EndTime"];

                var SZOutput = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOutput.AsQueryable();
                if (!string.IsNullOrEmpty(Name))
                {
                    SZOutput = SZOutput.Where(item => item.Sorting.OrderItem.Name.Contains(Name));
                }
                if (!string.IsNullOrEmpty(Model))
                {
                    SZOutput = SZOutput.Where(item => item.Sorting.OrderItem.Model.Contains(Model));
                }
                if (!string.IsNullOrEmpty(InvoiceCompany))
                {
                    SZOutput = SZOutput.Where(item => item.InvoiceCompany.Contains(InvoiceCompany));
                }
                if (!string.IsNullOrEmpty(StartTime))
                {
                    SZOutput = SZOutput.Where(item => item.CreateDate.CompareTo(StartTime) >= 0);
                }
                if (!string.IsNullOrEmpty(EndTime))
                {
                    var endTime = DateTime.Parse(EndTime).AddDays(1);
                    SZOutput = SZOutput.Where(item => item.CreateDate.CompareTo(endTime) < 0);
                }
                Func<Needs.Ccs.Services.Models.SZOutput, object> convert = item => new
                {
                    UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd"),
                    Name = item.Sorting.OrderItem.Name,
                    Model = item.Sorting.OrderItem.Model,
                    Brand = item.Sorting.OrderItem.Manufacturer,
                    TotalQuantity = item.TotalQuantity,
                    Quantity = item.Quantity,
                    InUnitPrice = item.InUnitPrice,
                    CostAmount = item.CostAmount,
                    OutUnitPrice = item.OutUnitPrice,
                    SalesAmount = item.SalesAmount,
                    InterestRate = item.InterestRate,
                    TaxRate = item.TaxRate,
                    TaxAmount = item.TaxAmount,
                    TotalAmount = item.TotalAmount,
                    TotalInterestRate = item.TotalInterestRate,
                    InvoiceCompany = item.InvoiceCompany,
                };

                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(SZOutput.Select(convert).ToArray().Json());

                string fileName = DateTime.Now.Ticks + ".xlsx";
                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                //设置导出格式
                var excelconfig = new ExcelConfig();
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.Title = "已出库产品数据";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UpdateDate", ExcelColumn = "出库日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Name", ExcelColumn = "产品名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Model", ExcelColumn = "规格型号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Brand", ExcelColumn = "品牌", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalQuantity", ExcelColumn = "订单数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Quantity", ExcelColumn = "出库数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InUnitPrice", ExcelColumn = "进价", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CostAmount", ExcelColumn = "成本", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OutUnitPrice", ExcelColumn = "销价", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SalesAmount", ExcelColumn = "销售金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InterestRate", ExcelColumn = "利率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxRate", ExcelColumn = "税率" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxAmount", ExcelColumn = "税额" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalAmount", ExcelColumn = "总额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalInterestRate", ExcelColumn = "总利率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceCompany", ExcelColumn = "客户", Alignment = "center" });
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