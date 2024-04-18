using Needs.Utils;
using Needs.Utils.Serializers;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using WebApp.Ccs.Utils;
using Needs.Ccs.Services.Enums;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Descriptions;
using Needs.Ccs.Services;
using System.Linq.Expressions;

namespace WebApp.ReportManage.DailyReport
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void data()
        {
            string InvoiceCompany = Request.QueryString["InvoiceCompany"];
            string StartTime = Request.QueryString["StartTime"];
            string EndTime = Request.QueryString["EndTime"];
            string EntryId = Request.QueryString["EntryId"];
            string VoyNo = Request.QueryString["VoyNo"];
            string ContrNo = Request.QueryString["ContrNo"];
            string GName = Request.QueryString["GName"];
            string GoodsModel = Request.QueryString["GoodsModel"];
            string ClientCode = Request.QueryString["ClientCode"];
            string IcgooOrderID = Request.QueryString["IcgooOrderID"];
            string HsCode = Request.QueryString["HsCode"];
            string Brand = Request.QueryString["Brand"];
            string CertCode0 = Request.QueryString["CertCode0"];

            var dailyDeclareList = new Needs.Ccs.Services.Views.DailyDeclareView1();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> expression = t => t.CusDecStatus ==CusItemDecStatus.Normal;
            if (!string.IsNullOrEmpty(InvoiceCompany))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.InvoiceCompany == InvoiceCompany.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                var from = DateTime.Parse(StartTime);
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = item => item.DeclareDate.Value >= from;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                var to = DateTime.Parse(EndTime);
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = item => item.DeclareDate.Value < to.AddDays(1);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(GoodsModel))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.GoodsModel == GoodsModel.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(EntryId))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.EntryId == EntryId.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(VoyNo))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.VoyNo == VoyNo.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ContrNo))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.ContrNo == ContrNo.Trim();
                lamdas.Add(lambda1); 
            }
            if (!string.IsNullOrEmpty(GName))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.GName == GName.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ClientCode))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.OrderID.Contains(ClientCode.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(IcgooOrderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.IcgooOrderID== IcgooOrderID.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(HsCode))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.CodeTS == HsCode.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(Brand))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.GoodsBrand == Brand.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(CertCode0))
            {
                Expression<Func<Needs.Ccs.Services.Models.DailyDeclare, bool>> lambda1 = t => t.CertCode0.Contains(CertCode0.Trim());
                lamdas.Add(lambda1);
            }

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var products = dailyDeclareList.GetPageList(page, rows, expression, lamdas.ToArray());
            Response.Write(new
            {
                rows = products.Select(
                    item => new
                    {
                        ID = item.ID,
                        item.GNo,
                        item.EntryId,
                        CodeTS = item.CodeTS,
                        item.CiqCode,
                        GName = item.GName,
                        GModel = item.GModel,
                        GQty = item.GQty,
                        item.GUnit,
                        item.FirstQty,
                        item.FirstUnit,
                        item.SecondQty,
                        item.SecondUnit,
                        DeclPrice = item.DeclPrice,
                        DeclTotal = item.DeclTotal,
                        TradeCurr = item.TradeCurr,
                        OriginCountry = item.OriginCountryName,
                        item.DestinationCountry,                      
                        item.DistrictCode,
                        DutyMode = item.DutyMode == 1 ? "照章征税" : "",
                        OrderType = item.OrderType.GetDescription(),//
                        ClientName = item.ClientName,                  
                        item.CaseNo,
                        //件数
                        item.NetWt,
                        item.GrossWt,
                        item.GoodsSpec,
                        item.GoodsAttr,
                        item.Purpose,
                        item.GoodsModel,
                        item.Qty,
                        item.ProductName,//品名
                        item.GoodsBrand,                    
                        TariffRate = item.TariffRate,
                        item.InvoiceCompany,
                        item.TotalAmount,//金额
                        item.TaxName,
                        item.TaxCode,
                        item.ContrNo,
                        OrderID = item.OrderID,
                        DeclareDate = item.DeclareDate.Value.ToShortDateString(),
                        item.IcgooOrderID,
                        item.CertCode0,
                    }
                ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

        protected void Export()
        {
            try
            {
                string InvoiceCompany = Request.Form["InvoiceCompany"];
                string StartTime = Request.Form["StartTime"];
                string EndTime = Request.Form["EndTime"];
                string EntryId = Request.Form["EntryId"];
                string VoyNo = Request.Form["VoyNo"];
                string ContrNo = Request.Form["ContrNo"];
                string GName = Request.Form["GName"];
                string GoodsModel = Request.Form["GoodsModel"];
                string ClientCode = Request.Form["ClientCode"];
                string IcgooOrderID = Request.Form["IcgooOrderID"];
                string CertCode0 = Request.Form["CertCode0"];

                var DailyDeclare = new Needs.Ccs.Services.Views.DailyDeclareView2().AsQueryable();

                if (!string.IsNullOrEmpty(InvoiceCompany))
                {
                    DailyDeclare = DailyDeclare.Where(item => item.InvoiceCompany.Contains(InvoiceCompany.Trim()));
                }

                if (!string.IsNullOrEmpty(StartTime))
                {
                    DailyDeclare = DailyDeclare.Where(item => item.DeclareDate.Value.CompareTo(StartTime) >= 0);
                }
                if (!string.IsNullOrEmpty(EndTime))
                {
                    var endTime = DateTime.Parse(EndTime).AddDays(1);
                    DailyDeclare = DailyDeclare.Where(item => item.DeclareDate.Value.CompareTo(endTime) < 0);
                }
                if (!string.IsNullOrEmpty(EntryId))
                {
                    DailyDeclare = DailyDeclare.Where(item => item.EntryId == EntryId.Trim());
                }
                if (!string.IsNullOrEmpty(VoyNo))
                {
                    DailyDeclare = DailyDeclare.Where(item => item.VoyNo == VoyNo);
                }
                if (!string.IsNullOrEmpty(ContrNo))
                {
                    DailyDeclare = DailyDeclare.Where(item => item.ContrNo == ContrNo.Trim());
                }
                if (!string.IsNullOrEmpty(GName))
                {
                    DailyDeclare = DailyDeclare.Where(item => item.GName == GName.Trim());
                }
                if (!string.IsNullOrEmpty(GoodsModel))
                {
                    DailyDeclare = DailyDeclare.Where(item => item.GoodsModel == GoodsModel.Trim());
                }
                if (!string.IsNullOrEmpty(ClientCode))
                {
                    DailyDeclare = DailyDeclare.Where(item => item.ClientCode == ClientCode.Trim());
                }
                if (!string.IsNullOrEmpty(IcgooOrderID))
                {
                    DailyDeclare = DailyDeclare.Where(item => item.IcgooOrderID == IcgooOrderID.Trim());
                }
                if (!string.IsNullOrEmpty(CertCode0))
                {
                    DailyDeclare = DailyDeclare.Where(item => item.CertCode0.Contains(CertCode0.Trim()));
                }

                Func<Needs.Ccs.Services.Models.DailyDeclare, object> convert = item => new
                {
                    item.GNo,
                    RecordNo = "",
                    CodeTS = item.CodeTS,
                    CiqCode = item.IsInspOrQuar ? item.CiqCode : "",
                    GName = item.GName,
                    GModel = item.GModel,
                    GQty = item.GQty.ToString("0.####"),
                    item.GUnit,
                    FirstQty = item.FirstQty,
                    FirstUnit = item.FirstUnit,
                    SecondQty = item.SecondQty==null?"":item.SecondQty.Value.ToString("0.###"),
                    SecondUnit = item.SecondUnit,
                    DeclPrice = item.DeclPrice.ToString("0.####"),
                    DeclTotal = item.DeclTotal.ToString("0.###"),
                    TradeCurr = item.TradeCurr,
                    OriginCountry = item.OriginCountryName,
                    DestinationCountry = "中国",
                    GoodsAttr1="",
                    DomisticDestName= "深圳特区",
                    DestName= "深圳市龙岗区",
                    DutyMode = item.DutyMode == 1 ? "照章征税" : "",
                    item.ContrNo,
                    item.CaseNo,
                    NetWt = item.NetWt?.ToRound(2),
                    GrossWt = item.GrossWt?.ToRound(2),
                    GoodsSpec = item.IsInspOrQuar?(";;;;;"+item.GoodsModel+";"+item.GoodsBrand+";;***") :"",
                    GoodsAttrName = item.IsInspOrQuar?item.GoodsAttrName:"",
                    PurposeName = item.IsInspOrQuar ? item.PurposeName:"",
                    item.GoodsModel,
                    Qty = item.Qty.ToString("0.####"),
                    item.TaxName,
                    item.GoodsBrand,
                    item.TariffRate,
                    item.InvoiceCompany,
                    item.TaxCode,
                    item.EntryId,
                    OrderID = item.OrderID,
                    IcgooOrderID = item.IcgooOrderID,
                    OrderTypeName = item.OrderType.GetDescription(),                  
                    OrderType = item.OrderType,
                    ClientName = item.InvoiceCompany,                    
                    item.ProductName,
                    item.ContrNoSuffix,
                    item.CertCode0,

                    //FirstQty = item.FirstQty?.ToString("0.####"),
                    //item.FirstUnit,
                    //SecondQty = item.SecondQty?.ToString("0.####"),
                    //item.SecondUnit,
                    //item.DestinationCountry,
                    // item.OrigPlaceCode,
                    //item.DistrictCode,
                    //
                    //ClientID = item.Order.Client.ID,
                    //item.GoodsSpec,
                    //GoodsAttr = item.GoodsAttrName,
                    //Purpose = item.PurposeName,
                    //Tariff = item.Tariff.ToRound(2).ToString(),//关税,
                    //TotalAmount = item.TotalAmount.ToString("0.###"),//金额
                };

                //Summary数据               
                var ContrNos = (from c in DailyDeclare
                               select c.ContrNo).Distinct().ToList();

                var DailyDeclareSummary = new Needs.Ccs.Services.Views.DailyDeclareSumaryView().AsQueryable();
                DailyDeclareSummary = DailyDeclareSummary.Where(t => ContrNos.Contains(t.ContrNo));
                DataTable dtSummary = NPOIHelper.JsonToDataTable(DailyDeclareSummary.Json());

                //详情数据
                //DataTable dtsort = NPOIHelper.JsonToDataTable(DailyDeclare.Select(convert).ToArray().Json());
                DataTable dtsort = new DataTable();
                dtsort.Columns.Add("GNo", typeof(System.Int16));               
                dtsort.Columns.Add("RecordNo");
                dtsort.Columns.Add("CodeTS");
                dtsort.Columns.Add("CiqCode");
                dtsort.Columns.Add("GName");
                dtsort.Columns.Add("GModel");
                dtsort.Columns.Add("GQty", typeof(System.Decimal));
                dtsort.Columns.Add("GUnit");
                dtsort.Columns.Add("FirstQty", typeof(System.Int64));
                dtsort.Columns.Add("FirstUnit");
                dtsort.Columns.Add("SecondQty");
                dtsort.Columns.Add("SecondUnit");
                dtsort.Columns.Add("DeclPrice", typeof(System.Decimal));
                dtsort.Columns.Add("DeclTotal", typeof(System.Decimal));
                dtsort.Columns.Add("TradeCurr");
                dtsort.Columns.Add("OriginCountry");
                dtsort.Columns.Add("DestinationCountry");
                dtsort.Columns.Add("GoodsAttr1");
                dtsort.Columns.Add("DomisticDestName");
                dtsort.Columns.Add("DestName");
                dtsort.Columns.Add("DutyMode");
                dtsort.Columns.Add("ContrNo");
                dtsort.Columns.Add("CaseNo");
                dtsort.Columns.Add("NetWt", typeof(System.Decimal));
                dtsort.Columns.Add("GrossWt", typeof(System.Decimal));
                dtsort.Columns.Add("GoodsSpec");
                dtsort.Columns.Add("GoodsAttrName");
                dtsort.Columns.Add("PurposeName");
                dtsort.Columns.Add("GoodsModel");
                dtsort.Columns.Add("Qty");
                dtsort.Columns.Add("TaxName");
                dtsort.Columns.Add("GoodsBrand");
                dtsort.Columns.Add("TariffRate", typeof(System.Decimal));
                dtsort.Columns.Add("InvoiceCompany");
                dtsort.Columns.Add("TaxCode");
                dtsort.Columns.Add("EntryId");
                dtsort.Columns.Add("OrderID");
                dtsort.Columns.Add("IcgooOrderID");
                dtsort.Columns.Add("OrderTypeName");
                dtsort.Columns.Add("OrderType");
                dtsort.Columns.Add("ClientName");
                dtsort.Columns.Add("ProductName");
                dtsort.Columns.Add("ContrNoSuffix", typeof(System.Int16));
                dtsort.Columns.Add("DeclareDate", typeof(System.DateTime));
                dtsort.Columns.Add("CertCode0");

                foreach (var item in DailyDeclare)
                {
                    DataRow drSort = dtsort.NewRow();
                    drSort["GNo"] = item.GNo;
                    drSort["RecordNo"] = "";
                    drSort["CodeTS"] = item.CodeTS;
                    drSort["CiqCode"] = item.IsInspOrQuar ? item.CiqCode : "";
                    drSort["GName"] = item.GName;
                    drSort["GModel"] = item.GModel;
                    drSort["GQty"] = item.GQty.ToString("0.####");
                    drSort["GUnit"] = item.GUnit;
                    drSort["FirstQty"] = item.FirstQty;
                    drSort["FirstUnit"] = item.FirstUnit;
                    drSort["SecondQty"] = item.SecondQty == null ? "" : item.SecondQty.Value.ToString("0.###");
                    drSort["SecondUnit"] = item.SecondUnit;
                    drSort["DeclPrice"] = item.DeclPrice.ToString("0.####");
                    drSort["DeclTotal"] = item.DeclTotal.ToString("0.###");
                    drSort["TradeCurr"] = item.TradeCurr;
                    drSort["OriginCountry"] = item.OriginCountryName;
                    drSort["DestinationCountry"] = "中国";
                    drSort["GoodsAttr1"] = "";
                    drSort["DomisticDestName"] = "深圳特区";
                    drSort["DestName"] = "深圳市龙岗区";
                    drSort["DutyMode"] = item.DutyMode == 1 ? "照章征税" : "";
                    drSort["ContrNo"] = item.ContrNo;
                    drSort["CaseNo"] = item.CaseNo;
                    drSort["NetWt"] = item.NetWt?.ToRound(2);
                    drSort["GrossWt"] = item.GrossWt?.ToRound(2);
                    drSort["GoodsSpec"] = item.IsInspOrQuar ? (";;;;;" + item.GoodsModel + ";" + item.GoodsBrand + ";;***") : "";
                    drSort["GoodsAttrName"] = item.IsInspOrQuar ? item.GoodsAttrName : "";
                    drSort["PurposeName"] = item.IsInspOrQuar ? item.PurposeName : "";
                    drSort["GoodsModel"] = item.GoodsModel;
                    drSort["Qty"] = item.Qty.ToString("0.####");
                    drSort["TaxName"] = item.TaxName;
                    drSort["GoodsBrand"] = item.GoodsBrand;
                    drSort["TariffRate"] = item.TariffRate;
                    drSort["InvoiceCompany"] = item.InvoiceCompany;
                    drSort["TaxCode"] = item.TaxCode;
                    drSort["EntryId"] = item.EntryId;
                    drSort["OrderID"] = item.OrderID;
                    drSort["IcgooOrderID"] = item.IcgooOrderID;
                    drSort["OrderTypeName"] = item.OrderType.GetDescription();
                    drSort["OrderType"] = item.OrderType;
                    drSort["ClientName"] = item.InvoiceCompany;
                    drSort["ProductName"] = item.ProductName;
                    drSort["ContrNoSuffix"] = item.ContrNoSuffix;
                    drSort["DeclareDate"] = ((DateTime)item.DeclareDate).Date;
                    drSort["CertCode0"] = item.CertCode0;


                    dtsort.Rows.Add(drSort);
                }


                DataTable dt = dtsort.Clone();
                dt = dtsort.Rows.Cast<DataRow>().OrderBy(r=>r["DeclareDate"]).ThenBy(r => r["ContrNoSuffix"]).ThenBy(r => r["GNo"]).CopyToDataTable();
               
                //添加颜色列
                dt.Columns.Add("ForeColour", typeof(System.Drawing.Color));
                System.Drawing.Color lightBlue =  System.Drawing.Color.FromArgb(204, 255, 255);
                System.Drawing.Color lightYellow = System.Drawing.Color.FromArgb(255, 255, 204);

                string currentContractNo = dt.Rows[0]["ContrNo"].ToString();
                System.Drawing.Color currentColour = lightBlue;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["ContrNo"].ToString().Equals(currentContractNo))
                    {
                        dt.Rows[i]["ForeColour"] = currentColour;
                    }
                    else
                    {
                        currentContractNo = dt.Rows[i]["ContrNo"].ToString();
                        if(currentColour == lightBlue)
                        {
                            currentColour = lightYellow;
                        }
                        else
                        {
                            currentColour = lightBlue;
                        }
                        dt.Rows[i]["ForeColour"] = currentColour;
                        
                    }
                }
                

                string fileName = "每日报表" + DateTime.Now.Ticks + ".xlsx";

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                //设置导出格式
                var excelconfig = new ExcelConfig();
                excelconfig.FilePath = fileDic.FilePath;              
                excelconfig.IsAllSizeColumn = false;
                excelconfig.RowForeColour = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;

                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GNo", ExcelColumn = "项号", Alignment = "center",Width=6 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "RecordNo", ExcelColumn = "备案序号", Alignment = "center",Width=3 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CodeTS", ExcelColumn = "海关编码", Alignment = "center",Width=10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CiqCode", ExcelColumn = "检验检疫编码", Alignment = "center",Width=3 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GName", ExcelColumn = "商品名称", Alignment = "center",Width=8 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GModel", ExcelColumn = "规格型号", Alignment = "center",Width=35 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GQty", ExcelColumn = "成交数量", Alignment = "center",Width=8 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GUnit", ExcelColumn = "成交单位", Alignment = "center",Width=5 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FirstQty", ExcelColumn = "法定数量", Alignment = "center",Width=8 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FirstUnit", ExcelColumn = "法定单位", Alignment = "center",Width=5 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SecondQty", ExcelColumn = "第二数量", Alignment = "center",Width=8 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SecondUnit", ExcelColumn = "第二单位", Alignment = "center",Width=5 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclPrice", ExcelColumn = "单价", Alignment = "center",Width=10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclTotal", ExcelColumn = "金额", Alignment = "center" ,Width=10});
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TradeCurr", ExcelColumn = "币制",Width=4 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OriginCountry", ExcelColumn = "原产国", Alignment = "center",Width=8 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DestinationCountry", ExcelColumn = "目的国", Alignment = "center",Width=5 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsAttr1", ExcelColumn = "货物属性", Alignment = "center",Width=18 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DomisticDestName", ExcelColumn = "境内目的地", Alignment = "center",Width=5 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DestName", ExcelColumn = "目的地", Alignment = "center",Width=5 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DutyMode", ExcelColumn = "征免", Alignment = "center",Width=5 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContrNo", ExcelColumn = "合同号", Alignment = "center",Width=18 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CaseNo", ExcelColumn = "件数",Width=6 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "NetWt", ExcelColumn = "净重", Alignment = "center",Width=6 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GrossWt", ExcelColumn = "毛重", Alignment = "center",Width=6 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsSpec", ExcelColumn = "检验检疫货物规格", Alignment = "center",Width=10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsAttrName", ExcelColumn = "货物属性", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PurposeName", ExcelColumn = "用途", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsModel", ExcelColumn = "型号", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Qty", ExcelColumn = "数量", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxName", ExcelColumn = "开票品名", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GoodsBrand", ExcelColumn = "品牌", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TariffRate", ExcelColumn = "关税", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "InvoiceCompany", ExcelColumn = "开票公司", Alignment = "center", Width = 25 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TaxCode", ExcelColumn = "税务编码", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "EntryId", ExcelColumn = "报关单号", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单号", Alignment = "center", Width = 10 });               
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "IcgooOrderID", ExcelColumn = "客户委托单号", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderTypeName", ExcelColumn = "订单类型", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CertCode0", ExcelColumn = "反制措施排除代码", Alignment = "center", Width = 10 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ForeColour", ExcelColumn = "", Alignment = "center" });
                //调用导出方法
                //NPOIHelper.ExcelDownload(dt, excelconfig);

                DailyDeclareDataExport dailyDeclareDataExport = new DailyDeclareDataExport(dt, dtSummary, excelconfig);
                dailyDeclareDataExport.Export();

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

        private void CreateExcelData(DataColumnCollection dataColumn, DataRow dr, Dictionary<string, string> dic, ref int rowIndex, ref ISheet sheet)
        {
            var row = sheet.CreateRow(rowIndex);
            var columnCount = 0;
            foreach (KeyValuePair<string, string> item in dic)
            {
                var value = dataColumn.Contains(item.Key) ? dr[item.Key].ToString() : string.Empty;
                row.CreateCell(columnCount).SetCellValue(value);
                columnCount++;
            }
            rowIndex++;
        }

        private ISheet CreateExcelSheet(IWorkbook workbook, string sheetName, Dictionary<string, string> titles)
        {
            var sheet = workbook.CreateSheet(sheetName);
            var titleRow = sheet.CreateRow(0);
            var columnCount = 0;
            foreach (KeyValuePair<string, string> item in titles)
            {
                titleRow.CreateCell(columnCount).SetCellValue(item.Value);
                columnCount++;
            }
            return sheet;
        }


        /// <summary>
        /// 初始化Excel表头
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> InitExcelTitle()
        {
            var dic = new Dictionary<string, string>();

            dic.Add("GNo", "项号");
            //dic.Add("No", "备案序号");
            dic.Add("CodeTS", "海关编码");
            dic.Add("CiqCode", "检验检疫编码");
            dic.Add("GName", "报关品名");
            dic.Add("GoodsBrand", "品牌");
            dic.Add("GoodsModel", "型号");
            dic.Add("GModel", "规格型号");
            dic.Add("GQty", "数量");

            //dic.Add("GQty", "成交数量");
            dic.Add("GUnit", "成交单位");


            //dic.Add("FirstQty", "法定数量");
            //dic.Add("FirstUnit", "法定单位");
            //dic.Add("SecondQty", "第二数量");
            //dic.Add("SecondUnit", "第二单位");
            dic.Add("DeclPrice", "单价");
            dic.Add("DeclTotal", "金额");
            dic.Add("TradeCurr", "币制");
            dic.Add("CaseNo", "箱号");
            dic.Add("NetWt", "净重");
            dic.Add("GrossWt", "毛重");
            dic.Add("OriginCountry", "原产国");
            //dic.Add("DestinationCountry", "目的国");
            //dic.Add("OrigPlaceCode", "原产地区");
            //dic.Add("DistrictCode", "境内目的地");
            //dic.Add("DestCode", "目的地");
            //dic.Add("DutyMode", "征免");
            dic.Add("ContrNo", "合同号");
            dic.Add("EntryID", "报关单号");
            dic.Add("DeclareDate", "报关日期");
            dic.Add("TariffRate", "关税率");
            dic.Add("InvoiceCompany", "开票公司");
            dic.Add("TaxName", "税务名称");
            dic.Add("TaxCode", "税务编码");

            dic.Add("OrderID", "订单号");

            dic.Add("OrderTypeName", "订单类型");
            dic.Add("CertCode0", "反制措施排除代码");


            return dic;
        }


    }
}