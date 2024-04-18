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
namespace WebApp.ReportManage.ClearCustoms
{
    public partial class list : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            //string StartTime = Request.QueryString["StartTime"];
            //string EndTime = Request.QueryString["EndTime"];         
            string VoyNo = Request.QueryString["VoyNo"];
            if (string.IsNullOrEmpty(VoyNo))
            {
                Response.Write(new
                {
                    total = 0,
                    Size = 20,
                    Index = 1,

                }.Json());
                return;
            }

            using (var query = new Needs.Ccs.Services.Views.ClearCustomsView())
            {
                var view = query;              
                if (!string.IsNullOrEmpty(VoyNo))
                {
                    view = view.SearchByVoyNo(VoyNo);
                }
              
                Response.Write(view.ToMyPage(page, rows).Json());
            }

        }

        protected void Export()
        {
            try
            {
                //string StartTime = Request.Form["StartTime"];
                //string EndTime = Request.Form["EndTime"];
             
                string VoyNo = Request.Form["VoyNo"];
                List<Needs.Ccs.Services.Models.CustomsClearance> datas = new List<Needs.Ccs.Services.Models.CustomsClearance>();

                using (var query = new Needs.Ccs.Services.Views.ClearCustomsView())
                {
                    var view = query;
                    if (!string.IsNullOrEmpty(VoyNo))
                    {
                        view = view.SearchByVoyNo(VoyNo);
                    }

                    var ienum_clearanceData = view.ToArray();

                    var group_clear = from c in ienum_clearanceData
                                      group c by new { c.HSCode8, c.GName, c.Currency, c.UnitName } into g
                                      orderby g.Key.HSCode8
                                      select new Needs.Ccs.Services.Models.CustomsClearance
                                      {
                                          HSCode = g.Key.HSCode8,
                                          GName = g.Key.GName,
                                          UnitName = g.Key.UnitName,
                                          Currency = g.Key.Currency,
                                          Qty = g.Sum(b => b.Qty),
                                          FirstQty = g.Sum( b=> b.FirstQty),
                                          DecTotal = g.Sum(b => b.DecTotal),
                                          NetWt = g.Sum(b => b.NetWt)
                                      };

                    datas = group_clear.ToList();
                }

 
                #region 设置导出格式

                DataTable dtBillSummary = new DataTable();
                dtBillSummary.Columns.Add("HSCode");
                dtBillSummary.Columns.Add("GName");
                dtBillSummary.Columns.Add("UnitName");
                dtBillSummary.Columns.Add("UnitShow");               
                dtBillSummary.Columns.Add("FirstQty");
                dtBillSummary.Columns.Add("Qty");
                dtBillSummary.Columns.Add("Currency");
                dtBillSummary.Columns.Add("DecTotal");
                dtBillSummary.Columns.Add("NetWt");
            

                foreach (var orderPremium in datas)
                {
                    DataRow dr = dtBillSummary.NewRow();
                    dr["HSCode"] = orderPremium.HSCode;
                    dr["GName"] = orderPremium.GName ;
                    dr["UnitName"] = orderPremium.UnitName;
                    dr["UnitShow"] = orderPremium.UnitShow;
                    dr["FirstQty"] = orderPremium.FirstQty;
                    dr["Qty"] = orderPremium.Qty;
                    dr["Currency"] = orderPremium.Currency;
                    dr["DecTotal"] = orderPremium.DecTotal;
                    dr["NetWt"] = orderPremium.NetWt;                  
                    dtBillSummary.Rows.Add(dr);
                }

            
                string fileName = "清关数据" + VoyNo + ".xls";
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                var excelconfig = new ExcelConfig();
                excelconfig.AutoMergedColumn = 0;//合并相同列,参数为0
                excelconfig.Title = "清关数据";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "HSCode", ExcelColumn = "海关编码", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GName", ExcelColumn = "品名", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UnitName", ExcelColumn = "单位", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UnitShow", ExcelColumn = "单位码", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FirstQty", ExcelColumn = "法一数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Qty", ExcelColumn = "数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DecTotal", ExcelColumn = "金额", Alignment = "center" });                        
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "NetWt", ExcelColumn = "净重", Alignment = "center" });
             
                #endregion

                //调用导出方法
                NPOIHelper.ExcelDownload(dtBillSummary, excelconfig);

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