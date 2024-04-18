using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
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

namespace WebApp.Declaration.Declare
{
    public partial class DecList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        private void load()
        {

        }

        protected void data()
        {
            string DeclarationID = Request.QueryString["ID"];
        
            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];
            var isCiq = DecHead.IsInspection;           

            var DecList = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecOriginList.Where(item => item.DeclarationID == DeclarationID).OrderBy(item=>item.GNo).AsQueryable();
            string GName = Request.QueryString["GName"]; 
            string GNO = Request.QueryString["GNO"];
            string OriginPlcae = Request.QueryString["OriginPlace"];

            //var DecList = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareList.AsQueryable();

            if (!string.IsNullOrEmpty(GName))
            {
                DecList = DecList.Where(item => item.GName == GName);
            }
            if (!string.IsNullOrEmpty(GNO))
            {
                DecList = DecList.Where(item => item.GNo == Convert.ToInt16(GNO));
            }
            if (!string.IsNullOrEmpty(OriginPlcae))
            {
                DecList = DecList.Where(item => item.OriginCountry == OriginPlcae);
            }

            Func<Needs.Ccs.Services.Models.DecList, object> convert = declareList => new
            {
                ID = declareList.ID,
                GNo = declareList.GNo,
                CodeTS = declareList.CodeTS,
                CiqCode = isCiq?declareList.CiqCode:"",
                GName = declareList.GName,
                GModel = declareList.GModel,
                GQty = declareList.GQty,
                GUnit = declareList.GUnit,
                FirstQty = declareList.FirstQty,
                FirstUnit = declareList.FirstUnit,
                SecondQty = declareList.SecondQty,
                SecondUnit = declareList.SecondUnit,
                DeclPrice = declareList.DeclPrice,
                DeclTotal = declareList.DeclTotal,
                TradeCurr = declareList.TradeCurr,
                OriginCountry = declareList.OriginCountry,
                DestinationCountry = declareList.DestinationCountry,
                DistrictCode = declareList.DistrictCode,
                DestCode = declareList.DestCode,
                DutyMode = declareList.DutyMode,
                CaseNo = declareList.CaseNo,
                NetWt = declareList.NetWt,
                GrossWt = declareList.GrossWt,
                GoodsSpec = declareList.GoodsSpec,
                GoodsAttr = declareList.GoodsAttrName,
                Purpose = declareList.PurposeName,
                GoodsBrand = declareList.GoodsBrand,
                CiqName = declareList.CiqName,
                GoodsBatch = declareList.GoodsBatch
            };
            this.Paging(DecList, convert);
        }

        protected void Check()
        {
            try
            {
                string DeclarationID = Request.Form["ID"];
                var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];

                CheckDocuments doc = new CheckDocuments(DecHead);
                string fileName = Server.MapPath("~/Files/"+Needs.Ccs.Services.SysConfig.Export+"/") + DecHead.ContrNo + ".xlsx";
                doc.SaveAs(fileName);

                string FileName = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"] + "/" + DecHead.ContrNo + ".xlsx";

                Response.Write(new { result=true,info=fileName }.Json());
            }
            catch(Exception ex)
            {
                Response.Write(new { result = false, info = "保存错误" + ex.ToString() }.Json());
            }
            
        }

        protected void CheckHtml()
        {
            string DeclarationID = Request.Form["ID"];
            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];
            CheckDocuments doc = new CheckDocuments(DecHead);
            string jsonResult = doc.Json();
            
            Response.Write(new { result = true, info = jsonResult }.Json());
        }

        //导出表体汇总
        protected void Export()
        {
            try
            {
                string DeclarationID = Request.Form["ID"];

                var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];

                var declist = DecHead.Lists.ToArray();

                var iemuns = from l in declist
                             group l by new { l.CodeTS, l.GName, l.OriginCountry, l.TradeCurr } into g
                             select new
                             {
                                 CodeTS = g.Key.CodeTS,
                                 GName = g.Key.GName,
                                 OriginCountry = g.Key.OriginCountry,
                                 GQty = g.Sum(t => t.GQty),
                                 DeclTotal = g.Sum(t => t.DeclTotal),
                                 Currency = g.Key.TradeCurr
                             };
                var datas = iemuns.ToList();

                #region 设置导出格式

                DataTable dtDecListSummary = new DataTable();
                dtDecListSummary.Columns.Add("CodeTS");
                dtDecListSummary.Columns.Add("GName");
                dtDecListSummary.Columns.Add("OriginCountry");
                dtDecListSummary.Columns.Add("GQty");
                dtDecListSummary.Columns.Add("DeclTotal");
                dtDecListSummary.Columns.Add("Currency");

                foreach (var data in datas)
                {
                    DataRow dr = dtDecListSummary.NewRow();
                    dr["CodeTS"] = data.CodeTS;
                    dr["GName"] = data.GName;
                    dr["OriginCountry"] = data.OriginCountry;
                    dr["GQty"] = data.GQty;
                    dr["DeclTotal"] = data.DeclTotal;
                    dr["Currency"] = data.Currency;
                    dtDecListSummary.Rows.Add(dr);
                }


                string fileName = "表体汇总" + DecHead.ContrNo + ".xls";
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                var excelconfig = new ExcelConfig();
                excelconfig.AutoMergedColumn = 0;//合并相同列,参数为0
                excelconfig.Title = "表体汇总" + DecHead.ContrNo;
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CodeTS", ExcelColumn = "商品编码", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GName", ExcelColumn = "商品名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OriginCountry", ExcelColumn = "原产地", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GQty", ExcelColumn = "数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclTotal", ExcelColumn = "金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });

                #endregion

                //调用导出方法
                NPOIHelper.ExcelDownload(dtDecListSummary, excelconfig);

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