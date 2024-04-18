using Needs.Utils;
using Needs.Wl.Models;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 对单数据
    /// </summary>
    public class CheckDocuments
    {
        /// <summary>
        /// 日期 
        /// </summary>
        public DateTime CheckDate { get; set; }
        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 总件数
        /// </summary>
        public int PackNo { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public decimal TotalQty { get; set; }
        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal GrossWt { get; set; }
        /// <summary>
        /// 总净重
        /// </summary>
        public decimal NetWt { get; set; }
        /// <summary>
        /// 报关总货值
        /// </summary>
        public decimal DeclarePrice { get; set; }
        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

        public string OwnerName { get; set; }

        public List<DecList> lists;

        public List<DecLicenseDocu> LicenseDocus;
        /// <summary>
        /// 运输批次号，航次号
        /// </summary>
        public string VoyNo { get; set; }
        public CheckDocuments(DecHead decHead)
        {
            this.CheckDate = DateTime.Now;
            this.ContrNo = decHead.ContrNo;
            this.PackNo = decHead.PackNo;
            this.GrossWt = decHead.GrossWt;
            this.VoyNo = decHead.VoyNo;
            this.OrderID = decHead.OrderID;
            this.OwnerName = decHead.OwnerName;


            var CountryView = new Needs.Ccs.Services.Views.BaseCountriesView();
            var HOrigin = new Needs.Ccs.Services.Views.BaseCountriesView().Where(t => t.Preferential != "L").ToList();
            var model = decHead.Lists.Select(item => new DecList
            {
                GNo = item.GNo,
                CodeTS = item.CodeTS,
                GName = item.GName,
                GModel = item.GModel,
                GoodsBrand = item.GoodsBrand,
                GoodsModel = item.GoodsModel,
                OriginCountryName = CountryView.Where(a => a.Code == item.OriginCountry).FirstOrDefault()?.Name,
                IsHOrigin = HOrigin.Any(t=>t.Code == item.OriginCountry),
                GQty = item.GQty,
                NetWt = item.NetWt == null ? 0 : Math.Round(item.NetWt.Value, 2, MidpointRounding.AwayFromZero),
                DeclPrice = item.DeclPrice,
                DeclTotal = item.DeclTotal,
                TradeCurr = item.TradeCurr,
                Limits = item.Limits
            }).OrderBy(item => item.GNo).ToList();
            this.lists = model;
            this.Currency = this.lists[0].TradeCurr;
            this.TotalQty = this.lists.Sum(item => item.GQty);
            this.NetWt = this.lists.Sum(item => item.NetWt == null ? 0 : item.NetWt.Value);
            this.DeclarePrice = this.lists.Sum(item => item.DeclTotal);
            //随附单证
            this.LicenseDocus = decHead.LicenseDocus.Select(
                item => new DecLicenseDocu
                {
                    DocuCode = item.DocuCode,
                    CertCode = item.CertCode,
                    FileUrl = item.FileUrl == null ? "" : (FileDirectory.Current.FileServerUrl + @"/" + item.FileUrl.Replace(@"\", @"/"))
                }).ToList();

        }


        public IWorkbook toExcel(string extension)
        {
            IWorkbook workBook = null;
            switch (extension)
            {
                case "xls":
                    workBook = new HSSFWorkbook();
                    break;
                case "xlsx":
                    workBook = new XSSFWorkbook();
                    break;
            }

            ISheet sheet1 = workBook.CreateSheet("对单");

            IFont font = workBook.CreateFont();
            font.FontHeightInPoints = 8;

            ICellStyle style = workBook.CreateCellStyle();
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.WrapText = true;
            style.SetFont(font);

            #region 表头
            if (sheet1.GetRow(0) == null)
            {
                sheet1.CreateRow(0);
                sheet1.GetRow(0).CreateCell(0).SetCellValue("致:" + PurchaserContext.Current.CompanyName);
                sheet1.GetRow(0).CreateCell(5).SetCellValue("日期:" + DateTime.Now.ToString("yyyy-MM-dd"));

            }

            if (sheet1.GetRow(1) == null)
            {
                sheet1.CreateRow(1);
                sheet1.GetRow(1).CreateCell(0).SetCellValue("地址:" + PurchaserContext.Current.Address);
                sheet1.GetRow(1).CreateCell(5).SetCellValue("合同号:" + this.ContrNo);

            }
            #endregion

            #region 标题行
            if (sheet1.GetRow(2) == null)
            {
                sheet1.CreateRow(2);

                sheet1.GetRow(2).CreateCell(0).SetCellValue("序号");
                sheet1.GetRow(2).CreateCell(1).SetCellValue("商品编码");
                sheet1.GetRow(2).CreateCell(2).SetCellValue("品名");
                sheet1.GetRow(2).CreateCell(3).SetCellValue("功能");
                sheet1.GetRow(2).CreateCell(4).SetCellValue("品牌");
                sheet1.GetRow(2).CreateCell(5).SetCellValue("规格型号");
                sheet1.GetRow(2).CreateCell(6).SetCellValue("产地");
                sheet1.GetRow(2).CreateCell(7).SetCellValue("数量(PCS)");
                sheet1.GetRow(2).CreateCell(8).SetCellValue("净重(KGS)");
                sheet1.GetRow(2).CreateCell(9).SetCellValue("报关单价(" + this.Currency + ")");
                sheet1.GetRow(2).CreateCell(10).SetCellValue("报关总价(" + this.Currency + ")");

                sheet1.GetRow(2).GetCell(0).CellStyle = style;
                sheet1.GetRow(2).GetCell(1).CellStyle = style;
                sheet1.GetRow(2).GetCell(2).CellStyle = style;
                sheet1.GetRow(2).GetCell(3).CellStyle = style;
                sheet1.GetRow(2).GetCell(4).CellStyle = style;
                sheet1.GetRow(2).GetCell(5).CellStyle = style;
                sheet1.GetRow(2).GetCell(6).CellStyle = style;
                sheet1.GetRow(2).GetCell(7).CellStyle = style;
                sheet1.GetRow(2).GetCell(8).CellStyle = style;
                sheet1.GetRow(2).GetCell(9).CellStyle = style;
                sheet1.GetRow(2).GetCell(10).CellStyle = style;

            }
            #endregion

            #region 表体
            int aboveRows = 3;
            if (this.lists != null)
            {
                var itemList = this.lists.OrderBy(item => item.GNo).ToArray();
                for (int i = 0; i < itemList.Length; i++)
                {
                    if (sheet1.GetRow(i + aboveRows) == null)
                    {
                        sheet1.CreateRow(i + aboveRows);

                        sheet1.GetRow(i + aboveRows).CreateCell(0).SetCellValue(itemList[i].GNo);
                        sheet1.GetRow(i + aboveRows).CreateCell(1).SetCellValue(itemList[i].CodeTS);
                        sheet1.GetRow(i + aboveRows).CreateCell(2).SetCellValue(itemList[i].GName);
                        sheet1.GetRow(i + aboveRows).CreateCell(3).SetCellValue(itemList[i].GModel);
                        sheet1.GetRow(i + aboveRows).CreateCell(4).SetCellValue(itemList[i].GoodsBrand);
                        sheet1.GetRow(i + aboveRows).CreateCell(5).SetCellValue(itemList[i].GoodsModel);
                        sheet1.GetRow(i + aboveRows).CreateCell(6).SetCellValue(itemList[i].OriginCountryName);
                        sheet1.GetRow(i + aboveRows).CreateCell(7).SetCellValue(itemList[i].GQty.ToString("0"));
                        sheet1.GetRow(i + aboveRows).CreateCell(8).SetCellValue(itemList[i].NetWt == null ? "" : itemList[i].NetWt.Value.ToString("0.00"));
                        sheet1.GetRow(i + aboveRows).CreateCell(9).SetCellValue(itemList[i].DeclPrice.ToString("0.0000"));
                        sheet1.GetRow(i + aboveRows).CreateCell(10).SetCellValue(itemList[i].DeclTotal.ToString("0.00"));

                        sheet1.GetRow(i + aboveRows).GetCell(0).CellStyle = style;
                        sheet1.GetRow(i + aboveRows).GetCell(1).CellStyle = style;
                        sheet1.GetRow(i + aboveRows).GetCell(2).CellStyle = style;
                        sheet1.GetRow(i + aboveRows).GetCell(3).CellStyle = style;
                        sheet1.GetRow(i + aboveRows).GetCell(4).CellStyle = style;
                        sheet1.GetRow(i + aboveRows).GetCell(5).CellStyle = style;
                        sheet1.GetRow(i + aboveRows).GetCell(6).CellStyle = style;
                        sheet1.GetRow(i + aboveRows).GetCell(7).CellStyle = style;
                        sheet1.GetRow(i + aboveRows).GetCell(8).CellStyle = style;
                        sheet1.GetRow(i + aboveRows).GetCell(9).CellStyle = style;
                        sheet1.GetRow(i + aboveRows).GetCell(10).CellStyle = style;
                    }
                }
            }
            #endregion

            #region 表尾
            int endRowIndex = aboveRows;
            if (sheet1.GetRow(endRowIndex + lists.Count) == null)
            {
                sheet1.CreateRow(endRowIndex + lists.Count);
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(1).SetCellValue("总件数");
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(2).SetCellValue(this.PackNo.ToString("0"));
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(3).SetCellValue("CTNS");
                endRowIndex++;
            }
            if (sheet1.GetRow(endRowIndex + lists.Count) == null)
            {
                sheet1.CreateRow(endRowIndex + lists.Count);
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(1).SetCellValue("总数量");
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(2).SetCellValue(this.TotalQty.ToString("0"));
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(3).SetCellValue("PCS");
                endRowIndex++;
            }
            if (sheet1.GetRow(endRowIndex + lists.Count) == null)
            {
                sheet1.CreateRow(endRowIndex + lists.Count);
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(1).SetCellValue("总净重");
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(2).SetCellValue(this.NetWt.ToString("0.00"));
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(3).SetCellValue("KGS");
                endRowIndex++;
            }
            if (sheet1.GetRow(endRowIndex + lists.Count) == null)
            {
                sheet1.CreateRow(endRowIndex + lists.Count);
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(1).SetCellValue("总毛重");
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(2).SetCellValue(this.GrossWt.ToString("0.00"));
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(3).SetCellValue("KGS");
                endRowIndex++;
            }
            if (sheet1.GetRow(endRowIndex + lists.Count) == null)
            {
                sheet1.CreateRow(endRowIndex + lists.Count);
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(1).SetCellValue("总金额");
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(2).SetCellValue(this.DeclarePrice.ToString("0.00"));
                sheet1.GetRow(endRowIndex + lists.Count).CreateCell(3).SetCellValue(this.Currency);
                endRowIndex++;
            }
            #endregion

            #region 设置单元格宽度
            sheet1.SetColumnWidth(0, 5 * 256);
            sheet1.SetColumnWidth(1, 9 * 256);
            sheet1.SetColumnWidth(2, 12 * 256);
            sheet1.SetColumnWidth(3, 50 * 256);
            sheet1.SetColumnWidth(5, 12 * 256);
            sheet1.SetColumnWidth(6, 7 * 256);
            sheet1.SetColumnWidth(7, 7 * 256);
            sheet1.SetColumnWidth(8, 7 * 256);
            sheet1.SetColumnWidth(9, 7 * 256);
            sheet1.SetColumnWidth(10, 7 * 256);
            #endregion

            return workBook;
        }

        public string[] SaveAs(string fileName)
        {
            var result = new string[3];
            try
            {
                IWorkbook doc = this.toExcel("xlsx");

                using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    result[0] = fileName;
                    result[1] = "";
                    result[2] = file.Length.ToString();

                    doc.Write(file);
                    file.Close();
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public string ToHtml()
        {
            string htmlcontext = "";
            htmlcontext += "<div id=\"logContainer\"  style=\"font-size:5px\">";
            htmlcontext += "<table style=\"font-size:1px\">";
            htmlcontext += "<tr>";
            htmlcontext += "<th>序号</th>";
            htmlcontext += "<th>商品编码</th>";
            htmlcontext += "<th>品名</th>";
            htmlcontext += "<th>功能</th>";
            htmlcontext += "<th>品牌</th>";
            htmlcontext += "<th>规格型号</th>";
            htmlcontext += "<th>产地</th>";
            htmlcontext += "<th>数量(PCS)</th>";
            htmlcontext += "<th>净重(KGS)</th>";
            htmlcontext += "<th>报关单价(" + this.Currency + ")</th>";
            htmlcontext += "<th>报关总价(" + this.Currency + ")</th>";
            htmlcontext += "</tr>";

            var CountryView = new Needs.Ccs.Services.Views.BaseCountriesView();
            var itemList = this.lists.OrderBy(item => item.GNo).ToArray();
            for (int i = 0; i < itemList.Length; i++)
            {
                htmlcontext += "<tr>";
                htmlcontext += "<td>" + itemList[i].GNo + "</td>";
                htmlcontext += "<td>" + itemList[i].CodeTS + "</td>";
                htmlcontext += "<td>" + itemList[i].GName + "</td>";
                htmlcontext += "<td>" + itemList[i].GModel + "</td>";
                htmlcontext += "<td>" + itemList[i].GoodsBrand + "</td>";
                htmlcontext += "<td>" + itemList[i].GoodsModel + "</td>";
                htmlcontext += "<td>" + CountryView.Where(item => item.Code == itemList[0].OriginCountry).FirstOrDefault()?.Name + "</td>";
                htmlcontext += "<td>" + itemList[i].GQty.ToString("0") + "</td><td>";
                htmlcontext += itemList[i].NetWt == null ? "" : itemList[i].NetWt.Value.ToString("0.00") + "</td>";
                htmlcontext += "<td>" + itemList[i].DeclPrice.ToString("0.0000") + "</td>";
                htmlcontext += "<td>" + itemList[i].DeclTotal.ToString("0.00") + "</td>";
                htmlcontext += "</tr>";
            }
            htmlcontext += "</table>";
            htmlcontext += "</div>";

            return htmlcontext;
        }

    }
}
