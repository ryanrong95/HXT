using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.SpirePdf;
using Spire.Pdf;
using Spire.Pdf.Annotations;
using Spire.Pdf.Fields;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using Spire.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfDocument = Needs.Utils.SpirePdf.PdfDocument;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 货物提货委托书
    /// </summary>
    public sealed class DeliveryAgentProxy : IUnique
    {
        #region 属性

        /// <summary>
        /// ID/运输批次号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public string CarrierCode { get; set; }

        public Carrier Carrier { get; set; }

        /// <summary>
        /// 司机
        /// </summary>
        public string DriverName { get; set; }

        public Driver Driver { get; set; }

        /// <summary>
        /// 车辆
        /// </summary>
        public string HKLicense { get; set; }

        public Vehicle Vehicle { get; set; }

        /// <summary>
        /// 运输时间
        /// </summary>
        public DateTime? TransportTime { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public int TotalPackNo { get; set; }

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal TotalGrossWt { get; set; }

        /// <summary>
        /// 导出提货委托书时，查出 Voyage 表的 DriverCode
        /// </summary>
        public string VoyageDriverCode { get; set; }

        #endregion

        public DeliveryAgentProxy()
        {

        }

        /// <summary>
        /// 导出PDF
        /// </summary>
        /// <returns></returns>
        public PdfDocument ToPdf()
        {
            var purchaser = PurchaserContext.Current;

            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();
            pdf.PageSettings.Margins = new PdfMargins(60, 60);

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4);
            int pageCount = pdf.Pages.Count;

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            PdfSolidBrush brush2 = new PdfSolidBrush(Color.Red);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion
            #region 头
            PdfTrueTypeFont font4 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Bold), true);
            PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("SimSun", 13f), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 12f, FontStyle.Bold), true);
            float x = 0, y = 5f;
            float width = page.Canvas.ClientSize.Width;
            string message = "致：香港畅运 林（先生）00852-31019258";
            PdfTrueTypeFont Font = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Bold), true);
            page.Canvas.DrawString(message, Font, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height+2;
            message = "FM：深圳关务部 韦小姐 0755-28685930";
            page.Canvas.DrawString(message, font4, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height+2;
            message = "日期：" + this.TransportTime?.ToString("yyyy年MM月dd日");
            page.Canvas.DrawString(message, font4, brush2, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height + 20;

            string message2 = "货物提货委托书";
            PdfTrueTypeFont headerFont = new PdfTrueTypeFont(new Font("SimSun", 18f, FontStyle.Bold), true);
            page.Canvas.DrawString(message2, headerFont, brush, width / 2, y, formatCenter);
            y += headerFont.MeasureString(message2, formatCenter).Height + 20;

            #endregion

            #region 内容明细

            var totalPackNo = this.TotalPackNo.ToString();

            message = "正常货物：";
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "到货时间：" + this.TransportTime?.ToString("yyyy.MM.dd");
            page.Canvas.DrawString(message, font2, brush2, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "预计到货时间：" + this.TransportTime?.ToString("yyyy.MM.dd");
            page.Canvas.DrawString(message, font2, brush2, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            //message = "合计：" + totalPackNo + "件  总重量：" + this.TotalGrossWt.ToString("0.##") + "KGS";
            message = "合计：";
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            page.Canvas.DrawString(this.TotalPackNo.ToString(), font2, brush2, width * 0.1f, y, formatLeft);
            page.Canvas.DrawString("件  总重量：", font2, brush, width * 0.15f, y, formatLeft);
            page.Canvas.DrawString(this.TotalGrossWt.ToString("0.##"), font2, brush2, width * 0.38f, y, formatLeft);
            page.Canvas.DrawString("KGS", font2, brush, width * 0.48f, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "姓名：" + this.DriverName;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "身份证件号：" + this.VoyageDriverCode ?? string.Empty;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "境内车牌：" + this.Vehicle?.License;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "境外车牌：" + this.HKLicense;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "海关编号：" + this.Driver?.HSCode;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "司机卡号：" + this.Driver?.DriverCardNo;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "大陆手机：" + this.Driver?.Mobile;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "香港手机：" + this.Driver?.HKMobile;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "牌头公司：" + this.Carrier?.Name;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "地址：" + this.Carrier?.Address;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "电话：" + this.Carrier?.Contact.Mobile;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "传真：" + this.Carrier?.Contact.Fax;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "口岸电子：" + this.Driver?.PortElecNo;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "车重：" + this.Vehicle?.Weight;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "寮步密码：" + this.Driver?.LaoPaoCode;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height;
            message = "车型：" + this.Vehicle?.VehicleType.GetDescription();
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height + 10;

            #endregion

            #region 尾

            string message1 =
                     "提货司机:" + "                         " + "司机身份核实:" + "  " + "\r\n" +
                     "\r\n" +
                       "车辆检查:" + "                                    " + " (备注:货物详见当日报关单)";
            page.Canvas.DrawString(message1, font3, brush, 0, y, formatLeft);
            y += font3.MeasureString(message1, formatLeft).Height + 3;
            //创建一个PDF表格，并添加两行
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 13f), true);
            PdfGridRow row1 = grid.Rows.Add();
            PdfGridRow row2 = grid.Rows.Add();
            PdfGridRow row3 = grid.Rows.Add();
            PdfGridRow row4 = grid.Rows.Add();
            //设置表格的单元格内容和边框之间的上、下边距
            grid.Style.CellPadding.Top = 8f;
            grid.Style.CellPadding.Bottom = 8f;

            //添加三列，并设置列宽
            grid.Columns.Add(3);
            grid.Columns[0].Width = width * 0.3f;
            grid.Columns[1].Width = width * 0.4f;
            grid.Columns[2].Width = width * 0.3f;

            PdfStringFormat stringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            //写入数据
            row1.Cells[0].Value = "检查项目";
            row1.Cells[0].StringFormat = stringFormat;
            row1.Cells[1].Value = "检查结果";
            row1.Cells[1].StringFormat = stringFormat;
            row1.Cells[2].Value = "备注";
            row1.Cells[2].StringFormat = stringFormat;
            row2.Cells[0].Value = "1.驾驶室";
            row2.Cells[1].Value = "□正常     □不正常";
            row2.Cells[1].StringFormat = stringFormat;
            row2.Cells[2].Value = "";
            row3.Cells[0].Value = "2.车厢";
            row3.Cells[1].Value = "□正常     □不正常";
            row3.Cells[1].StringFormat = stringFormat;
            row3.Cells[2].Value = "";
            row4.Cells[0].Value = "3.底盘";
            row4.Cells[1].Value = "□正常     □不正常";
            row4.Cells[1].StringFormat = stringFormat;
            row4.Cells[2].Value = "";

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }
            PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
            y += result.Bounds.Height + 5;
            message = " 收货人： ";
            page.Canvas.DrawString(message, font3, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatCenter).Height + 10;
            #endregion

            #region 公共组件

            //页眉、页脚、二维码、水印
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(pdf);
            string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.HeaderImg);
            pdf.PdfMargins = new PdfMargins(60, 15);
            pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, PurchaserContext.Current.OfficalWebsite);
            pdfDocumentHandle.HeaderFooter.GenerateFooter(PurchaserContext.Current.CompanyName);
            pdfDocumentHandle.Barcode.GenerateQRCode(this.ID, imageUrl);
            pdfDocumentHandle.Watermark.DrawWatermark(PurchaserContext.Current.CompanyName);

            #endregion

            return pdf;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveAs(string filePath)
        {
            PdfDocument pdf = this.ToPdf();
            pdf.SaveToFile(filePath);
            pdf.Close();
        }
    }
}
