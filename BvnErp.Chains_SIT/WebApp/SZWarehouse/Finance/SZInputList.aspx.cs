using Needs.Ccs.Services.Enums;
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

namespace WebApp.SZWarehouse.Finance
{
    public partial class SZInputList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            var invoiceType = EnumUtils.ToDictionary<InvoiceType>().Select(item => new { item.Key, item.Value });
            this.Model.InvoiceType = invoiceType.Json();
        }

        protected void data()
        {
            string GName = Request.QueryString["GName"];
            string GoodsModel = Request.QueryString["GoodsModel"];
            string ContrNo = Request.QueryString["ContrNo"];
            string EntryId = Request.QueryString["EntryId"];
            string InvoiceCompany = Request.QueryString["InvoiceCompany"];
            string StartTime = Request.QueryString["StartTime"];
            string EndTime = Request.QueryString["EndTime"];
            string InvoiceType = Request.QueryString["InvoiceType"];

            var SZInputList = new Needs.Ccs.Services.Views.SZInput1View();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> expression = item => true;

            #region 页面查询条件

            if (!string.IsNullOrEmpty(GName))
            {
                Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.GName == GName;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(GoodsModel))
            {
                Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.GoodsModel == GoodsModel;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ContrNo))
            {
                Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.ContrNo == ContrNo;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(EntryId))
            {
                Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.EntryId == EntryId;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(InvoiceCompany))
            {
                Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.InvoiceCompany.Contains(InvoiceCompany);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(InvoiceType))
            {
                int invoiceType = Int32.Parse(InvoiceType);
                Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.InvoiceType == (Needs.Ccs.Services.Enums.InvoiceType)invoiceType;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.CreateDate.Value.CompareTo(StartTime) >= 0;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                var endTime = DateTime.Parse(EndTime).AddDays(1);
                Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.CreateDate.Value.CompareTo(endTime) < 0;
                lamdas.Add(lambda1);
            }

            #endregion

            #region 页面需要数据

            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var lists = SZInputList.GetPageList(page, rows, expression, lamdas.ToArray());

            Response.Write(new
            {
                rows = lists.Select(
                        item => new
                        {
                            ID = item.ID,
                            CodeTS = item.CodeTS,
                            GNo = item.GNo,
                            GName = item.GName,
                            GModel = item.GModel,
                            GoodsBrand = item.GoodsBrand,
                            GoodsModel = item.GoodsModel,
                            OriginCountry = item.OriginCountryName,
                            GQty = item.GQty,
                            NetWt = item.NetWt,
                            DeclPrice = item.DeclPrice,
                            DeclTotal = item.DeclTotal,
                            TradeCurr = item.TradeCurr,

                            ContrNo = item.ContrNo,
                            CustomsRate = item.CustomsRate,
                            TariffRate = item.TariffRate.ToString("0.0000"),
                            DeclTotalRMB = item.DeclTotalRMB.ToString("0.00"),
                            TariffPay = item.TariffPay,
                            TariffPayed = item.TariffPayed,
                            ExciseTax = item.ExciseTax,
                            ExciseTaxPayed = item.ExciseTaxPayed,
                            ValueVat = item.ValueVatPayed,
                            CustomsValue = item.CustomsValue,
                            CustomsValueVat = item.CustomsValueVat,
                            InvoiceCompany = item.InvoiceCompany,
                            EntryId = item.EntryId,
                            CreateDate = item.CreateDate?.ToString("yyyy-MM-dd"),

                            TaxName = item.TaxName,
                            TaxCode = item.TaxCode,
                            ConsignorCode = item.ConsignorCode,
                            InvoiceType = item.InvoiceType.GetDescription(),

                            FillinDate = item.FillinDate?.ToString("yyyy-MM-dd")
                        }
                     ).ToArray(),
                total = lists.Total,
            }.Json());

            #endregion

        }

        protected void Export()
        {
            try
            {
                string GName = Request.Form["GName"];
                string GoodsModel = Request.Form["GoodsModel"];
                string ContrNo = Request.Form["ContrNo"];
                string EntryId = Request.Form["EntryId"];
                string InvoiceCompany = Request.Form["InvoiceCompany"];
                string StartTime = Request.Form["StartTime"];
                string EndTime = Request.Form["EndTime"];
                string InvoiceType = Request.Form["InvoiceType"];

                #region 新写法 待完善

                //var SZInputList = new Needs.Ccs.Services.Views.SZInput1View();
                //List<LambdaExpression> lamdas = new List<LambdaExpression>();
                //Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> expression = item => true;

                //#region 页面查询条件

                //if (!string.IsNullOrEmpty(GName))
                //{
                //    Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.GName == GName;
                //    lamdas.Add(lambda1);
                //}
                //if (!string.IsNullOrEmpty(GoodsModel))
                //{
                //    Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.GoodsModel == GoodsModel;
                //    lamdas.Add(lambda1);
                //}
                //if (!string.IsNullOrEmpty(ContrNo))
                //{
                //    Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.ContrNo == ContrNo;
                //    lamdas.Add(lambda1);
                //}
                //if (!string.IsNullOrEmpty(EntryId))
                //{
                //    Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.EntryId == EntryId;
                //    lamdas.Add(lambda1);
                //}
                //if (!string.IsNullOrEmpty(InvoiceCompany))
                //{
                //    Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.InvoiceCompany.Contains(InvoiceCompany);
                //    lamdas.Add(lambda1);
                //}
                //if (!string.IsNullOrEmpty(StartTime))
                //{
                //    Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.CreateDate.Value.CompareTo(StartTime) >= 0;
                //    lamdas.Add(lambda1);
                //}
                //if (!string.IsNullOrEmpty(EndTime))
                //{
                //    var endTime = DateTime.Parse(EndTime).AddDays(1);
                //    Expression<Func<Needs.Ccs.Services.Models.SZInput, bool>> lambda1 = item => item.CreateDate.Value.CompareTo(endTime) < 0;
                //    lamdas.Add(lambda1);
                //}

                //#endregion

                //#region 页面需要数据

                //var lists = SZInputList.GetAlls(expression, lamdas.ToArray());

                //var results = lists.Select(
                //        item => new
                //        {
                //            ID = item.ID,
                //            CodeTS = item.CodeTS,
                //            GName = item.GName,
                //            GModel = item.GModel,
                //            GoodsBrand = item.GoodsBrand,
                //            GoodsModel = item.GoodsModel,
                //            OriginCountry = item.OriginCountryName,
                //            GQty = item.GQty,
                //            NetWt = item.NetWt,
                //            DeclPrice = item.DeclPrice,
                //            DeclTotal = item.DeclTotal,
                //            TradeCurr = item.TradeCurr,

                //            ContrNo = item.ContrNo,
                //            CustomsRate = item.CustomsRate.ToString("0.0000"),
                //            TariffRate = item.TariffRate.ToString("0.0000"),
                //            DeclTotalRMB = item.DeclTotalRMB.ToString("0.00"),
                //            TariffPay = item.TariffPay,
                //            TariffPayed = item.TariffPayed,
                //            ValueVat = item.ValueVatPayed,
                //            CustomsValue = item.CustomsValue,
                //            CustomsValueVat = item.CustomsValueVat,
                //            InvoiceCompany = item.InvoiceCompany,
                //            EntryId = item.EntryId,
                //            CreateDate = item.CreateDate?.ToString("yyyy-MM-dd"),

                //            TaxName = item.TaxName,
                //            TaxCode = item.TaxCode,
                //        }
                //     ).ToArray();

                //#endregion

                //#region 组装excel

                ////写入数据
                //DataTable dt = NPOIHelper.JsonToDataTable(results.Json());

                //string fileName = DateTime.Now.Ticks + ".xlsx";

                #endregion

                var SZInput = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZInput.AsQueryable();
                if (!string.IsNullOrEmpty(GName))
                {
                    SZInput = SZInput.Where(item => item.GName == GName);
                }
                if (!string.IsNullOrEmpty(GoodsModel))
                {
                    SZInput = SZInput.Where(item => item.GoodsModel == GoodsModel);
                }
                if (!string.IsNullOrEmpty(ContrNo))
                {
                    SZInput = SZInput.Where(item => item.ContrNo == ContrNo);
                }
                if (!string.IsNullOrEmpty(EntryId))
                {
                    SZInput = SZInput.Where(item => item.EntryId == EntryId);
                }
                if (!string.IsNullOrEmpty(InvoiceCompany))
                {
                    SZInput = SZInput.Where(item => item.InvoiceCompany.Contains(InvoiceCompany));
                }
                if (!string.IsNullOrEmpty(InvoiceType))
                {
                    int invoiceType = Int32.Parse(InvoiceType);
                    SZInput = SZInput.Where(item => item.InvoiceType == (Needs.Ccs.Services.Enums.InvoiceType)invoiceType);
                }
                if (!string.IsNullOrEmpty(StartTime))
                {
                    SZInput = SZInput.Where(item => item.CreateDate.Value.CompareTo(StartTime) >= 0);
                }
                if (!string.IsNullOrEmpty(EndTime))
                {
                    var endTime = DateTime.Parse(EndTime).AddDays(1);
                    SZInput = SZInput.Where(item => item.CreateDate.Value.CompareTo(endTime) < 0);
                }
                Func<Needs.Ccs.Services.Models.SZInput, object> convert = item => new
                {
                    CreateDate = item.CreateDate?.ToString("yyyy-MM-dd"),
                    FillinDate = item.FillinDate?.ToString("yyyy-MM-dd"),
                    ContrNo = item.ContrNo,
                    GoodsModel = item.GoodsModel,
                    GoodsBrand = item.GoodsBrand,
                    GQty = item.GQty,
                    DeclPrice = item.DeclPrice,
                    DeclTotal = item.DeclTotal,
                    NetWt = item.NetWt,
                    TradeCurr = item.TradeCurr,
                    OriginCountry = item.OriginCountryName,
                    CodeTS = item.CodeTS,
                    GName = item.GName,
                    GModel = item.GModel,
                    CustomsRate = item.CustomsRate.ToString("0.000000"),
                    TariffRate = item.TariffRate.ToString("0.0000"),
                    DeclTotalRMB = item.DeclTotalRMB.ToString("0.00"),
                    TariffPay = item.TariffPay,
                    ValueVat = item.ValueVatPayed,
                    ExciseTax = item.ExciseTax,
                    ExciseTaxPayed = item.ExciseTaxPayed,
                    TariffPayed = item.TariffPayed,
                    CustomsValue = item.CustomsValue,
                    CustomsValueVat = item.CustomsValueVat,
                    InvoiceType = item.InvoiceType.GetDescription(),
                    InvoiceCompany = item.InvoiceCompany,
                    ConsignorCode = item.ConsignorCode,
                    EntryId = item.EntryId,
                    TaxName = item.TaxName,
                    TaxCode = item.TaxCode,
                    DYJOrderID = item.DYJOrderID,

                };

                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(SZInput.Select(convert).ToArray().Json());

                string fileName = DateTime.Now.Ticks + ".xlsx";

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                //设置导出格式
                var excelconfig = new ExcelConfig();
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.Title = "已报关产品数据";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CreateDate", ExcelColumn = "报关日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FillinDate", ExcelColumn = "填发日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContrNo", ExcelColumn = "合同号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsModel", ExcelColumn = "规格型号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsBrand", ExcelColumn = "品牌", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GQty", ExcelColumn = "成交数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclPrice", ExcelColumn = "单价", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclTotal", ExcelColumn = "总价", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "NetWt", ExcelColumn = "净重", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TradeCurr", ExcelColumn = "币制", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OriginCountry", ExcelColumn = "原产国", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CodeTS", ExcelColumn = "商品编号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GName", ExcelColumn = "品名" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GModel", ExcelColumn = "功能" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CustomsRate", ExcelColumn = "海关汇率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TariffRate", ExcelColumn = "关税率", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclTotalRMB", ExcelColumn = "报关总价", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TariffPay", ExcelColumn = "应交关税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ValueVat", ExcelColumn = "实交增值税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ExciseTax", ExcelColumn = "应交消费税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ExciseTaxPayed", ExcelColumn = "实交消费税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TariffPayed", ExcelColumn = "实交关税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CustomsValue", ExcelColumn = "完税价格", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CustomsValueVat", ExcelColumn = "完税价格增值税", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceType", ExcelColumn = "开票类型", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceCompany", ExcelColumn = "开票公司" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConsignorCode", ExcelColumn = "供应商", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "EntryId", ExcelColumn = "报关单号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxName", ExcelColumn = "税务名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxCode", ExcelColumn = "税务编码", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DYJOrderID", ExcelColumn = "大赢家订单编号", Alignment = "center" });
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
