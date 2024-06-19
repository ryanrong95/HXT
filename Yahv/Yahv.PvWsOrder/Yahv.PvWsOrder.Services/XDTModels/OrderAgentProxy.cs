using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.SpirePdf;
using PdfDocument = Yahv.Utils.SpirePdf.PdfDocument;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    /// <summary>
    /// 代理报关委托书
    /// </summary>
    public class OrderAgentProxy : IUnique
    {
        #region 属性
        /// <summary>
        /// 订单ID
        /// </summary>
        public string ID { get; set; }

        public WsOrder Order { get; set; }

        public WsClient Client { get; set; }

        public Contact ClientContact { get; set; }

        public OrderItem[] Orderitems { get; set; }

        public int? PackNo { get; set; }

        public Package WarpType { get; set; }


        Purchaser purchaser = PurchaserContext.Current;
        Vendor vendor = VendorContext.Current;
        #endregion


        /// <summary>
        /// 导出代理报关委托书PDF
        /// </summary>
        /// <returns></returns>
        public PdfDocument ToPdf(Yahv.PvWsOrder.Services.Enums.ClientType clientType)
        {
            #region pdf对象声明
            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();
            pdf.PageSettings.Margins = new PdfMargins(10, 60, 10, 10);

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4);
            int pageCount = pdf.Pages.Count;

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 14f, FontStyle.Bold), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);
            #endregion

            #region 头
            float x = 0, y = 5f;

            float width = page.Canvas.ClientSize.Width;
            string message = $"{purchaser.ShortName}-代理报关委托书";
            page.Canvas.DrawString(message, font1, brush, width / 2, y, formatCenter);
            y += font1.MeasureString(message, formatCenter).Height + 8;

            //20190909 合同编号改为订单编号
            message = "订单编号: ";
            page.Canvas.DrawString(message + this.ID, font2, brush, width, y, formatRight);
            y += font2.MeasureString(message + this.ID, formatRight).Height + 5;

            #endregion

            #region 表格

            //创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(11);
            grid.Columns[0].Width = width * 3 / 100;
            grid.Columns[1].Width = width * 6 / 100;
            grid.Columns[2].Width = width * 20 / 100;
            grid.Columns[3].Width = width * 15 / 100;
            grid.Columns[4].Width = width * 20 / 100;
            grid.Columns[5].Width = width * 6 / 100;
            grid.Columns[6].Width = width * 6 / 100;
            grid.Columns[7].Width = width * 6 / 100;
            grid.Columns[8].Width = width * 6 / 100;
            grid.Columns[9].Width = width * 6 / 100;
            grid.Columns[10].Width = width * 6 / 100;

            //委托方、代理方信息
            PdfGridRow row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 11;
            row.Cells[0].Value = "委托方名称: " + this.Client.Name;

            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 11;
            row.Cells[0].Value = "委托方收货信息: " + this.Client.Name + "/地址: " + this.Client.RegAddress +
                                "/联系人: " + this.ClientContact?.Name + "/电话: " + this.ClientContact?.Mobile;

            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 11;
            row.Cells[0].Value = "代理方名称: " + purchaser.CompanyName;

            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 11;
            row.Cells[0].Value = "代理方收货信息: " + vendor.CompanyName + "/地址: " + vendor.Address +
                                "/联系人: " + vendor.Contact + "/电话: " + vendor.Tel;

            //产品信息
            row = grid.Rows.Add();
            row.Cells[0].Value = "序号";
            row.Cells[1].Value = "批号";
            row.Cells[2].Value = "品名";
            row.Cells[3].Value = "品牌";
            row.Cells[4].Value = "规格型号";
            row.Cells[5].Value = "产地";
            row.Cells[6].Value = "数量";
            row.Cells[7].Value = "单位";
            row.Cells[8].Value = "报关单价" + "(" + this.Order.Input.Currency.ToString() + ")";
            row.Cells[9].Value = "报关总价" + "(" + this.Order.Input.Currency.ToString() + ")";
            row.Cells[10].Value = "关税率";
            for (int i = 0; i < row.Cells.Count; i++)
            {
                if (i == 2 || i == 4)
                {
                    row.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                }
                else
                {
                    row.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                }
            }

            int sn = 0;
            foreach (var item in this.Orderitems)
            {
                sn++;
                row = grid.Rows.Add();
                row.Cells[0].Value = sn.ToString();
                row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[1].Value = item.DateCode;
                row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[2].Value = item.ClassfiedName;
                row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                row.Cells[3].Value = item.Product.Manufacturer;
                row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[4].Value = item.Product.PartNumber;
                row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                row.Cells[5].Value = Enum.GetValues(typeof(Origin)).Cast<Origin>().Select(c => c.GetOrigin()).FirstOrDefault(c => c.Code == item.Origin).ChineseName;
                row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[6].Value = item.Quantity.ToString("0.####");
                row.Cells[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[7].Value = item.Unit.GetDescription();
                row.Cells[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[8].Value = item.UnitPrice.ToString("0.0000");
                row.Cells[8].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[9].Value = item.TotalPrice.ToString("0.00");
                row.Cells[9].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[10].Value = item.TraiffRate.ToString("0.0000");
                row.Cells[10].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }

            //合计行
            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 6;
            row.Cells[0].Value = "合计:";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[6].Value = this.Orderitems.Select(item => item.Quantity).Sum().ToString("0.####");
            row.Cells[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[9].Value = this.Orderitems.Select(item => item.TotalPrice).Sum().ToString("0.00");
            row.Cells[9].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }

            PdfLayoutResult result = grid.Draw(page, new PointF(x, y));

            //是否换页
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);

            y += result.Bounds.Height;

            
            var totalGwt = this.Orderitems.Where(item => item.GrossWeight != null).Select(item => item.GrossWeight).Sum().Value;
            message = "包装类型: " + this.WarpType.GetDescription();
            if (this.PackNo != null)
            {
                message += " 总件数: " + this.PackNo;
            }
            if (totalGwt > 0M)
            {
                message += " 总毛重: " + totalGwt.ToString("0.##") + " KG";
            }
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font1.MeasureString(message + this.ID, formatLeft).Height + 5;

            #endregion

            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);


            #region 尾

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;

            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 2;
            row.Cells[0].Value = "温馨提示:\r\n" +
                                "1.我单位保证遵守《中华人民共和国海关法》及国家有关法规保证所提供的委托信息与所报的货物相符。\r\n" +
                                "2.委托方务必真实填写，若因委托方虚报、假报、多报、少报等因素造成的扣关、罚款等后果由委托方自行承担。\r\n" +
                                "3.委托方一份报关单最多20个号，超过部分按另一单结算，依此类推。\r\n" +
                                "4.委托方需即时签字盖章回传，如因委托方延误回传造成的延迟申报，代理方不承担责任。";
            row = grid.Rows.Add();
            row.Height = 30f;
            row.Cells[0].Value = "代理方盖章:：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "委托方(签字/盖章):";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);



            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;


            //y += font2.MeasureString(message, formatLeft).Height + 20;

            //大赢家加上委托方章
            if (clientType == Enums.ClientType.Internal)
            {
                PdfImage imageInternal = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.Client.Name + ".png"));
                page.Canvas.DrawImage(imageInternal, 410, y - 50);
            }

            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.SealUrl));
            page.Canvas.DrawImage(image, 100, y - 50);

            #endregion

            #region 公共组件

            //页眉、页脚、二维码、水印
            pdf.PdfMargins = new PdfMargins(10, 10);
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(pdf);
            string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.HeaderImg);

            pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, PurchaserContext.Current.OfficalWebsite);
            pdfDocumentHandle.HeaderFooter.GenerateFooter(PurchaserContext.Current.CompanyName);
            pdfDocumentHandle.Barcode.GenerateQRCode(this.ID, imageUrl);
            pdfDocumentHandle.Watermark.DrawWatermark(PurchaserContext.Current.CompanyName);

            #endregion

            return pdf;
        }

        /// <summary>
        /// 导出代理报关委托书PDF -- xin
        /// </summary>
        /// <returns></returns>
        public PdfDocument ToPdfNew(Yahv.PvWsOrder.Services.Enums.ClientType clientType)
        {
            #region pdf对象声明
            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();
            pdf.PageSettings.Margins = new PdfMargins(40, 60, 40, 20);

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4);
            int pageCount = pdf.Pages.Count;

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 14f, FontStyle.Bold), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);
            #endregion

            #region 头
            float x = 0, y = 5f;

            float width = page.Canvas.ClientSize.Width;
            string message = $"委托进口货物确认单";
            page.Canvas.DrawString(message, font1, brush, width / 2, y, formatCenter);
            y += font1.MeasureString(message, formatCenter).Height + 8;

            page.Canvas.DrawString("GOODS IMPORT CONFIRM FORM", font1, brush, width / 2, y, formatCenter);
            y += font1.MeasureString(message, formatCenter).Height + 8;

            //20190909 合同编号改为订单编号
            //message = "订单编号: ";
            //page.Canvas.DrawString(message + this.ID, font2, brush, width, y, formatRight);
            //y += font2.MeasureString(message + this.ID, formatRight).Height + 5;


            //创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = font2;
            PdfBorders borders = new PdfBorders();
            borders.All = new PdfPen(PdfBrushes.White, 0);

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;

            PdfGridRow row = grid.Rows.Add();
            row.Cells[0].Value = "编号:  " + this.ID;
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[0].Style.Borders = borders;
            row.Cells[1].Value = "委托日期:  " + DateTime.Now.ToString("yyyy-MM-dd");
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[1].Style.Borders = borders;

            row = grid.Rows.Add();
            row.Cells[0].Value = "委托方:  " + this.Client.Name;
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[0].Style.Borders = borders;
            row.Cells[1].Value = "代理方:  " + purchaser.CompanyName;
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[1].Style.Borders = borders;

            row = grid.Rows.Add();
            row.Cells[0].Value = "委托方地址:  " + this.Client.RegAddress;
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[0].Style.Borders = borders;
            row.Cells[1].Value = "代理方收货公司:  " + vendor.CompanyName;
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[1].Style.Borders = borders;

            row = grid.Rows.Add();
            row.Cells[0].Value = "委托方联系人:  " + this.ClientContact?.Name + "  " + this.ClientContact?.Mobile;
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[0].Style.Borders = borders;
            row.Cells[1].Value = "代理方收货人:  " + vendor.Contact + "  " + vendor.Tel;
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[1].Style.Borders = borders;

            PdfLayoutResult result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);

            y += result.Bounds.Height + 5;

            #endregion

            #region 表格

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(10);
            grid.Columns[0].Width = width * 3 / 100;
            grid.Columns[1].Width = width * 17 / 100;
            grid.Columns[2].Width = width * 17 / 100;
            grid.Columns[3].Width = width * 22 / 100;
            grid.Columns[4].Width = width * 6 / 100;
            grid.Columns[5].Width = width * 6 / 100;
            grid.Columns[6].Width = width * 6 / 100;
            grid.Columns[7].Width = width * 8 / 100;
            grid.Columns[8].Width = width * 8 / 100;
            grid.Columns[9].Width = width * 6 / 100;

            //委托方、代理方信息
            //row = grid.Rows.Add();
            //row.Cells[0].ColumnSpan = 11;
            //row.Cells[0].Value = "委托方名称: " + this.Client.Name;

            //row = grid.Rows.Add();
            //row.Cells[0].ColumnSpan = 11;
            //row.Cells[0].Value = "委托方收货信息: " + this.Client.Name + "/地址: " + this.Client.RegAddress +
            //                    "联系人: " + this.ClientContact?.Name + "/电话: " + this.ClientContact?.Mobile;

            //row = grid.Rows.Add();
            //row.Cells[0].ColumnSpan = 11;
            //row.Cells[0].Value = "代理方名称: " + purchaser.CompanyName;

            //row = grid.Rows.Add();
            //row.Cells[0].ColumnSpan = 11;
            //row.Cells[0].Value = "代理方收货信息: " + vendor.CompanyName + "/地址: " + vendor.Address +
            //                    "/联系人: " + vendor.Contact + "/电话: " + vendor.Tel;

            //产品信息
            row = grid.Rows.Add();
            row.Cells[0].Value = "序号";
            //row.Cells[1].Value = "批号";
            row.Cells[1].Value = "品名";
            row.Cells[2].Value = "品牌";
            row.Cells[3].Value = "型号";
            row.Cells[4].Value = "产地";
            row.Cells[5].Value = "数量";
            row.Cells[6].Value = "单位";
            row.Cells[7].Value = "报关单价" + "(" + this.Order.Input.Currency.ToString() + ")";
            row.Cells[8].Value = "报关总价" + "(" + this.Order.Input.Currency.ToString() + ")";
            row.Cells[9].Value = "关税率";
            for (int i = 0; i < row.Cells.Count; i++)
            {
                if (i == 2 || i == 4)
                {
                    row.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                }
                else
                {
                    row.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                }
            }

            int sn = 0;
            foreach (var item in this.Orderitems)
            {
                sn++;
                row = grid.Rows.Add();
                row.Cells[0].Value = sn.ToString();
                row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                //row.Cells[1].Value = item.DateCode;
                //row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[1].Value = item.ClassfiedName;
                row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[2].Value = item.Product.Manufacturer;
                row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[3].Value = item.Product.PartNumber;
                row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[4].Value = Enum.GetValues(typeof(Origin)).Cast<Origin>().Select(c => c.GetOrigin()).FirstOrDefault(c => c.Code == item.Origin).ChineseName;
                row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[5].Value = item.Quantity.ToString("0.####");
                row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[6].Value = item.Unit.GetDescription();
                row.Cells[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[7].Value = item.UnitPrice.ToString("0.0000");
                row.Cells[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[8].Value = item.TotalPrice.ToString("0.00");
                row.Cells[8].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[9].Value = item.TraiffRate.ToString("0.0000");
                row.Cells[9].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }

            //合计行
            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 5;
            row.Cells[0].Value = "合计Total:";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[5].Value = this.Orderitems.Select(item => item.Quantity).Sum().ToString("0.####");
            row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[8].Value = this.Orderitems.Select(item => item.TotalPrice).Sum().ToString("0.00");
            row.Cells[8].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }

            result = grid.Draw(page, new PointF(x, y));

            //是否换页
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);

            y += result.Bounds.Height + 5;


            var totalGwt = this.Orderitems.Where(item => item.GrossWeight != null).Select(item => item.GrossWeight).Sum().Value;
            message = "包装类型: " + this.WarpType.GetDescription();
            if (this.PackNo != null)
            {
                message += " 总件数: " + this.PackNo;
            }
            if (totalGwt > 0M)
            {
                message += " 总毛重: " + totalGwt.ToString("0.##") + " KG";
            }
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font1.MeasureString(message + this.ID, formatLeft).Height + 5;



            #endregion

            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);


            #region 尾

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;

            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 2;
            row.Cells[0].Value = "说明:\r\n" +
                                "委托方需提供完整的进口货物单证和资料，并保证其真实、准确；\r\n" +
                                "因委托方系统虚假信息被扣关、罚款等后果由委托方自行承担；\r\n" +
                                "委托方需及时将购买货物所需的全部货款、代理费付至乙方帐户。\r\n";
            row.Cells[0].Style.Borders = borders;
            row = grid.Rows.Add();
            row.Height = 30f;
            row.Cells[0].Value = " ";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[0].Style.Borders = borders;
            row.Cells[1].Value = "委托方签字并盖章确认:";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[1].Style.Borders = borders;



            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;


            //y += font2.MeasureString(message, formatLeft).Height + 20;

            //大赢家加上委托方章
            //if (clientType == Enums.ClientType.Internal)
            //{
            //    PdfImage imageInternal = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.Client.Name + ".png"));
            //    page.Canvas.DrawImage(imageInternal, 410, y - 50);
            //}

            //PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.SealUrl));
            //page.Canvas.DrawImage(image, 100, y - 50);

            #endregion

            #region 公共组件

            //页眉、页脚、二维码、水印
            pdf.PdfMargins = new PdfMargins(20, 20);
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(pdf);
            string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.HeaderImg);

            pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, PurchaserContext.Current.OfficalWebsite);
            //pdfDocumentHandle.HeaderFooter.GenerateFooter(PurchaserContext.Current.CompanyName);
            //pdfDocumentHandle.Barcode.GenerateQRCode(this.ID, imageUrl);
            //pdfDocumentHandle.Watermark.DrawWatermark(PurchaserContext.Current.CompanyName);

            #endregion

            return pdf;
        }


        /// <summary>
        /// 更新页数、页面、偏移量
        /// </summary>
        /// <param name="pdf">PDF文档</param>
        /// <param name="pageCount">新增页面前的页数</param>
        /// <param name="page">PDF页面</param>
        /// <param name="x">X轴偏移量</param>
        /// <param name="y">Y轴偏移量</param>
        private void UpdateIfNewPageCreated(PdfDocument pdf, out int pageCount, out PdfPageBase page, out float x, out float y)
        {
            pageCount = pdf.Pages.Count;
            page = pdf.Pages[pdf.Pages.Count - 1];
            x = pdf.PageSettings.Margins.Left;
            y = pdf.PageSettings.Margins.Top;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveAs(string filePath, Yahv.PvWsOrder.Services.Enums.ClientType clientType)
        {
            int pagecount = 0;
            var pdf = this.ToPdfNew(clientType);
            pagecount = pdf.Pages.Count;
            pdf.SaveToFile(filePath);
            pdf.Close();
            if (pagecount > 1)
            {
                PagingSeal(filePath);
            }
        }

        private void PagingSeal(string filePath)
        {
            //加载PDF文档
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(filePath);

            PdfUnitConvertor convert = new PdfUnitConvertor();
            PdfPageBase pageBase = null;

            //获取分割后的印章图片
            //芯达通或者创新恒远图片
            GetImage(doc.Pages.Count, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.purchaser.SealUrl), this.purchaser.CompanyName);
            //大赢家图片
            GetImage(doc.Pages.Count, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.Client.Name + ".png"), this.Client.Name);
            float x = 0;
            float y = 0;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //将图片画到PDF页面上的指定位置
            for (int i = 0; i < doc.Pages.Count; i++)
            {
                string agentPath = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", this.purchaser.CompanyName, doc.Pages.Count.ToString(), i.ToString() + ".png");
                PdfImage agentImage = PdfImage.FromFile(agentPath);
                pageBase = doc.Pages[i];
                x = pageBase.Size.Width - agentImage.Width + 9;
                y = pageBase.Size.Height / 2;
                //pageBase.Canvas.DrawImage(PdfImage.FromImage(agentImages[i]), new PointF(x, y));               
                pageBase.Canvas.DrawImage(agentImage, new PointF(x, y));

                string clientPath = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", this.Client.Name, doc.Pages.Count.ToString(), i.ToString() + ".png");
                PdfImage clientImage = PdfImage.FromFile(clientPath);
                x = pageBase.Size.Width - clientImage.Width + 9;
                y = pageBase.Size.Height / 2 + convert.ConvertToPixels(agentImage.Height, PdfGraphicsUnit.Point) + 30;
                //pageBase.Canvas.DrawImage(PdfImage.FromImage(clientImages[i]), new PointF(x, y));               
                pageBase.Canvas.DrawImage(clientImage, new PointF(x, y));
            }

            //保存PDF文件
            doc.SaveToFile(filePath);
            doc.Close();
        }

        private bool GetImage(int num, string picUrl, string clientName)
        {
            try
            {
                //看是否有切好的图片
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", clientName, num.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var images = Directory.GetFiles(path, ".", SearchOption.AllDirectories).Where(s => s.EndsWith(".png"));

                if (images.Count() == num)
                {
                    return true;
                }
                else
                {
                    //生成图片
                    Image image = Image.FromFile(picUrl);
                    int w = image.Width / num;
                    Bitmap bitmap = null;
                    for (int i = 0; i < num; i++)
                    {
                        bitmap = new Bitmap(w, image.Height);
                        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                        {
                            g.Clear(Color.White);
                            Rectangle rect = new Rectangle(i * w, 0, w, image.Height);
                            g.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), rect, GraphicsUnit.Pixel);
                        }
                        bitmap.MakeTransparent(Color.White);
                        string fileUrl = @path + "\\" + i.ToString() + ".png";
                        bitmap.Save(fileUrl);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
