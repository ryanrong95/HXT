using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NPOI.SS.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.CustomClearance
{
    public partial class Cleared : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string VoyageNo = Request.QueryString["VoyageNo"];
            string StartTime = Request.QueryString["StartTime"];
            string EndTime = Request.QueryString["EndTime"];

            var manifest = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OutputWayBill.OrderByDescending(x => x.Voyage.LoadingDate).ThenBy(x => x.Voyage.ID)
               .Where(item => item.Voyage.HKDeclareStatus == true).AsQueryable();
            if (!string.IsNullOrEmpty(VoyageNo))
            {
                VoyageNo = VoyageNo.Trim();
                manifest = manifest.Where(item => item.Voyage.ID.Contains(VoyageNo));
            }

            if (!string.IsNullOrEmpty(StartTime))
            {
                var from = DateTime.Parse(StartTime);
                manifest = manifest.Where(t => t.Voyage.LoadingDate >= from);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                var to = DateTime.Parse(EndTime);
                manifest = manifest.Where(t => t.Voyage.LoadingDate <= to.AddDays(1));
            }

            Func<OutputWayBill, object> convert = item => new
            {
                item.ID,
                VoyageNo = item.Voyage?.ID,
                BillNo = item.ID,
                HKLicense = item.Voyage?.HKLicense,
                LoadingDate = item.Voyage?.LoadingDate?.ToString("yyyy-MM-dd"),
                CarrierName = item.Voyage?.Carrier?.Name,
                ProductQty = item.PackNum,
                TotalPrice = item.GoodsValue,
                item.Currency,
                Status = (item.Voyage?.HKDeclareStatus == true) ? "已申报" : "未申报",
            };
            this.Paging(manifest, convert);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        protected void Export()
        {
            string VoyageNo = Request.Form["VoyageNo"];
            string StartTime = Request.Form["StartTime"];
            string EndTime = Request.Form["EndTime"];

            string fileName = DateTime.Now.Ticks + ".xlsx";
            FileDirectory fileDic = new FileDirectory(fileName);
            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
            fileDic.CreateDataDirectory();

            var manifest = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OutputWayBill.OrderByDescending(x => x.Voyage.LoadingDate).ThenBy(x => x.Voyage.ID)
              .Where(item => item.Voyage.HKDeclareStatus == true).AsQueryable();

            if (!string.IsNullOrEmpty(VoyageNo))
            {
                VoyageNo = VoyageNo.Trim();
                manifest = manifest.Where(item => item.Voyage.ID.Contains(VoyageNo));
            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                var from = DateTime.Parse(StartTime);
                manifest = manifest.Where(t => t.Voyage.LoadingDate >= from);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                var to = DateTime.Parse(EndTime);
                manifest = manifest.Where(t => t.Voyage.LoadingDate <= to.AddDays(1));
            }
            Func<OutputWayBill, object> convert = item => new
            {
                item.ID,
                VoyageNo = item.Voyage?.ID,
                BillNo = item.ID,
                HKLicense = item.Voyage?.HKLicense,
                LoadingDate = item.Voyage?.LoadingDate?.ToString("yyyy-MM-dd"),
                CarrierName = string.IsNullOrEmpty(item.Voyage?.Carrier?.Name) ? "" : item.Voyage?.Carrier?.Name,
                ProductQty = item.PackNum,
                TotalPrice = item.GoodsValue,
                item.Currency,
                Status = (item.Voyage?.HKDeclareStatus == true) ? "已申报" : "未申报",
            };

            //写入数据
            DataTable dt = NPOIHelper.JsonToDataTable(manifest.Select(convert).ToArray().Json());

            //设置导出格式
            var excelconfig = new ExcelConfig();
            excelconfig.FilePath = fileDic.FilePath;
            excelconfig.Title = "香港出口清关导出";
            excelconfig.TitleFont = "微软雅黑";
            excelconfig.TitlePoint = 25;
            excelconfig.FileName = fileName;
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
            excelconfig.ColumnEntity = listColumnEntity;
            ColumnEntity columnentity = new ColumnEntity();
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "VoyageNo", ExcelColumn = "运输批次号" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "BillNo", ExcelColumn = "运单号" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "HKLicense", ExcelColumn = "车牌号" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "LoadingDate", ExcelColumn = "出口日期" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarrierName", ExcelColumn = "承运商" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ProductQty", ExcelColumn = "产品数量" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalPrice", ExcelColumn = "总金额" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Status", ExcelColumn = "状态" });
            //调用导出方法
            NPOIHelper.ExcelDownload(dt, excelconfig);

            Response.Write((new
            {
                success = true,
                message = "导出成功",
                url = fileDic.FileUrl
            }).Json());
        }
    }
}