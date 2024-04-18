using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.GoodsBill
{
    public partial class Storage : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string InDateFrom = Request.QueryString["InDateFrom"];
            string InDateTo = Request.QueryString["InDateTo"];
            string DeadLineFrom = Request.QueryString["DeadLineFrom"];
            //string DeadLineTo = Request.QueryString["DeadLineTo"];
            string Model = Request.QueryString["Model"];
            string ClientName = Request.QueryString["ClientName"]; 

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            using (var query = new Needs.Ccs.Services.Views.GoodsBillStorageView())
            {
                var view = query;


                if (!string.IsNullOrWhiteSpace(InDateFrom))
                {
                    InDateFrom = InDateFrom.Trim();
                    DateTime dtFrom = Convert.ToDateTime(InDateFrom);
                    view = view.SearchByFrom(dtFrom);
                }

                if (!string.IsNullOrWhiteSpace(InDateTo))
                {
                    InDateTo = InDateTo.Trim();
                    DateTime dtTo = Convert.ToDateTime(InDateTo).AddDays(1);
                    view = view.SearchByTo(dtTo);
                }
                else
                {
                    DateTime dtTo = DateTime.Now.Date.AddDays(-1);
                    view = view.SearchByTo(dtTo);
                }

                if (!string.IsNullOrWhiteSpace(Model))
                {
                    Model = Model.Trim();
                    view = view.SearchByModel(Model);
                }

                if (!string.IsNullOrWhiteSpace(DeadLineFrom))
                {
                    DeadLineFrom = DeadLineFrom.Trim();
                }

                if (!string.IsNullOrWhiteSpace(ClientName))
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByClientName(ClientName);
                }



                Response.Write(view.ToMyPage(DeadLineFrom, page, rows).Json());
            }
        }

        protected void Download()
        {
            string InDateFrom = Request.Form["InDateFrom"];
            string InDateTo = Request.Form["InDateTo"];
            string DeadLineFrom = Request.Form["DeadLineFrom"];

            List<StorageModel> dtStorage = new List<StorageModel>();
            if (string.IsNullOrEmpty(DeadLineFrom))
            {
                InDateFrom = InDateFrom.Trim();
                DateTime dtFrom = Convert.ToDateTime(InDateFrom);

                InDateTo = InDateTo.Trim();
                DateTime dtTo = Convert.ToDateTime(InDateTo).AddDays(1);

                TimeSpan timeSpan = dtTo - dtFrom;
                if (timeSpan.Days > 31)
                {
                    Response.Write((new
                    {
                        success = false,
                        message = "只能导出一个月之内的数据",

                    }).Json());
                    return;
                }


                var decInfos = new Needs.Ccs.Services.Views.InputViewDownload().Where(t => t.DDate > dtFrom.AddDays(-10) && t.DDate < dtTo.AddDays(3)).ToArray();
                var InputInfos = new Needs.Ccs.Services.Views.Sz_Cfb_InViewOrigin().Where(t => t.InStoreDate > dtFrom && t.InStoreDate < dtTo).ToArray();

                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                var InputView = from input in InputInfos
                                join dec in decInfos on input.InputID equals dec.InputID into decs
                                from decinfo in decs.DefaultIfEmpty()
                                orderby input.InStoreDate
                                select new InStoreViewModel
                                {
                                    ID = decinfo == null ? "" : decinfo.ID,
                                    InStoreDate = input.InStoreDate,
                                    GName = decinfo == null ? "" : decinfo.GName,
                                    GoodsBrand = decinfo == null ? "" : decinfo.GoodsBrand,
                                    GoodsModel = decinfo == null ? "" : decinfo.GoodsModel,
                                    GQty = decinfo == null ? 1 : decinfo.GQty,
                                    Gunit = decinfo == null ? "" : decinfo.Gunit,
                                    DeclPrice = decinfo == null ? 0 : decinfo.DeclPrice,
                                    TaxedPrice = decinfo == null ? 0 : decinfo.TaxedPrice,
                                    TradeCurr = decinfo == null ? "" : decinfo.TradeCurr,
                                    ContrNo = decinfo == null ? "" : decinfo.ContrNo,
                                    DDate = decinfo == null ? null : decinfo.DDate,
                                    OwnerName = decinfo == null ? "" : decinfo.OwnerName,
                                    OperatorID = decinfo == null ? "" : input.OperatorID,
                                    TariffTaxNumber = decinfo == null ? "" : decinfo.TariffTaxNumber,
                                    ValueAddedTaxNumber = decinfo == null ? "" : decinfo.ValueAddedTaxNumber,
                                    VoyNo = decinfo == null ? "" : decinfo.VoyNo
                                };
                stopwatch1.Stop();
                string s1 = stopwatch1.ElapsedMilliseconds.ToString();


                var OutputView = new Needs.Ccs.Services.Views.Sz_Cfb_OutViewOrigin().Where(t => t.OutStoreDate > dtFrom && t.OutStoreDate < dtTo).
                                     GroupBy(t => t.OrderItemID).Select(t => new
                                     {
                                         OrderItemID = t.Key,
                                         Qty = t.Sum(n => n.OutQty)
                                     }).AsEnumerable();

                dtStorage = (from dtInput in InputView
                             join dtOutput in OutputView on dtInput.OrderItemID equals dtOutput.OrderItemID into dtOuts
                             from dtOut in dtOuts.DefaultIfEmpty()
                             select new StorageModel
                             {
                                 ID = dtInput.ID,
                                 GName = dtInput.GName,
                                 GoodsModel = dtInput.GoodsModel,
                                 GQty = dtInput.GQty,
                                 Gunit = dtInput.Gunit,
                                 PurchasingPrice = dtInput.PurchasingPrice,
                                 TaxedPrice = dtInput.TaxedPrice,
                                 TradeCurr = dtInput.TradeCurr,
                                 ContrNo = dtInput.ContrNo,
                                 InStoreDate = dtInput.InStoreDate,
                                 OperatorID = dtInput.OperatorID,
                                 TariffTaxNumber = dtInput.TariffTaxNumber,
                                 ValueAddedTaxNumber = dtInput.ValueAddedTaxNumber,
                                 OutQty = dtOut == null ? 0 : dtOut.Qty.Value,
                                 OwnerName = dtInput.OwnerName,
                             }).ToList();
            }
            else
            {
                string DeadLineStart = DeadLineFrom.Substring(0, 7) + "-01";
                DateTime dtDeadLineFrom = Convert.ToDateTime(DeadLineStart);

                DeadLineFrom = DeadLineFrom.Trim();
                DateTime dtDeadLineTo = Convert.ToDateTime(DeadLineFrom).AddDays(1);

                var InputView = new Needs.Ccs.Services.Views.InputViewDownload().Where(t => t.InStoreDate > dtDeadLineFrom && t.InStoreDate < dtDeadLineTo).AsEnumerable();


                var OutputView = new Needs.Ccs.Services.Views.Sz_Cfb_OutViewOrigin().Where(t => t.OutStoreDate > dtDeadLineFrom && t.OutStoreDate < dtDeadLineTo).
                                     GroupBy(t => t.OrderItemID).Select(t => new
                                     {
                                         OrderItemID = t.Key,
                                         Qty = t.Sum(n => n.OutQty)
                                     }).AsEnumerable();

                dtStorage = (from dtInput in InputView
                             join dtOutput in OutputView on dtInput.OrderItemID equals dtOutput.OrderItemID into dtOuts
                             from dtOut in dtOuts.DefaultIfEmpty()
                             select new StorageModel
                             {
                                 ID = dtInput.ID,
                                 GName = dtInput.GName,
                                 GoodsModel = dtInput.GoodsModel,
                                 GQty = dtInput.GQty,
                                 Gunit = dtInput.Gunit,
                                 PurchasingPrice = dtInput.PurchasingPrice,
                                 TaxedPrice = dtInput.TaxedPrice,
                                 TradeCurr = dtInput.TradeCurr,
                                 ContrNo = dtInput.ContrNo,
                                 InStoreDate = dtInput.InStoreDate,
                                 OperatorID = dtInput.OperatorID,
                                 TariffTaxNumber = dtInput.TariffTaxNumber,
                                 ValueAddedTaxNumber = dtInput.ValueAddedTaxNumber,
                                 OutQty = dtOut == null ? 0 : dtOut.Qty.Value,
                                 OwnerName = dtInput.OwnerName,
                             }).ToList();
            }



            //Stopwatch stopwatch1 = new Stopwatch();
            //stopwatch1.Start();

            //stopwatch1.Stop();
            //string s1 = stopwatch1.ElapsedMilliseconds.ToString();

            Func<Needs.Ccs.Services.Models.InStoreViewModel, object> convert = item => new
            {
                item.ID,
                item.GName,
                item.GoodsModel,
                item.GQty,
                item.Gunit,
                item.PurchasingPrice,
                item.TaxedPrice,
                item.TradeCurr,
                item.ContrNo,
                item.InStoreDate,
                item.OperatorID,
                item.TariffTaxNumber,
                item.ValueAddedTaxNumber,
                item.OwnerName,
            };

            //Stopwatch stopwatch2 = new Stopwatch();
            //stopwatch2.Start();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("GName");
            dt.Columns.Add("GoodsModel");
            dt.Columns.Add("GQty");
            dt.Columns.Add("Gunit");
            dt.Columns.Add("PurchasingPrice");
            dt.Columns.Add("TaxedPrice");
            dt.Columns.Add("TradeCurr");
            dt.Columns.Add("ContrNo");
            dt.Columns.Add("InStoreDate");
            dt.Columns.Add("OperatorID");
            dt.Columns.Add("TariffTaxNumber");
            dt.Columns.Add("ValueAddedTaxNumber");
            dt.Columns.Add("OwnerName");


            foreach (var item in dtStorage)
            {
                if (item.GQty - item.OutQty > 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = item.ID;
                    dr["GName"] = item.GName;
                    dr["GoodsModel"] = item.GoodsModel;
                    dr["GQty"] = item.GQty - item.OutQty;
                    dr["Gunit"] = item.Gunit;
                    dr["PurchasingPrice"] = item.PurchasingPrice;
                    dr["TaxedPrice"] = item.TaxedPrice;
                    dr["TradeCurr"] = item.TradeCurr;
                    dr["ContrNo"] = item.ContrNo;
                    dr["InStoreDate"] = item.InStoreDate;
                    dr["OperatorID"] = item.OperatorID;
                    dr["TariffTaxNumber"] = item.TariffTaxNumber;
                    dr["ValueAddedTaxNumber"] = item.ValueAddedTaxNumber;
                    dr["OwnerName"] = item.OwnerName;

                    dt.Rows.Add(dr);
                }

            }


            //stopwatch2.Stop();
            //string s2 = stopwatch2.ElapsedMilliseconds.ToString();
            //int count = dt.Rows.Count;

            try
            {
                string fileName = DateTime.Now.Ticks + ".xlsx";

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                //设置导出格式
                var excelconfig = new ExcelConfig();
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.Title = "库存数据";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ID", ExcelColumn = "编号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GName", ExcelColumn = "报关品名", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsModel", ExcelColumn = "型号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GQty", ExcelColumn = "数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Gunit", ExcelColumn = "单位", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PurchasingPrice", ExcelColumn = "进价", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxedPrice", ExcelColumn = "完税价格", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TradeCurr", ExcelColumn = "币制" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContrNo", ExcelColumn = "合同号" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InStoreDate", ExcelColumn = "入库日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OperatorID", ExcelColumn = "入库人", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TariffTaxNumber", ExcelColumn = "关税税费单号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ValueAddedTaxNumber", ExcelColumn = "增值税税费单号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OwnerName", ExcelColumn = "客户名称", Alignment = "center" });
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
                string xx = ex.ToString();
            }
        }

        private class StorageModel
        {
            public string ID { get; set; }
            public string GName { get; set; }
            public string GoodsModel { get; set; }
            public decimal GQty { get; set; }
            public string Gunit { get; set; }
            public decimal PurchasingPrice { get; set; }
            public decimal? TaxedPrice { get; set; }
            public string TradeCurr { get; set; }
            public string ContrNo { get; set; }
            public DateTime? InStoreDate { get; set; }
            public string OperatorID { get; set; }
            public string TariffTaxNumber { get; set; }
            public string ValueAddedTaxNumber { get; set; }
            public decimal OutQty { get; set; }
            public string OwnerName { get; set; }
        }
    }
}