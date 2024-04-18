using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NPOI.HSSF.Record.Chart;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.AutomaticFields;

namespace Yahv.Utils.SpirePdf
{
    /// <summary>
    /// PDF页眉页脚
    /// </summary>
    public class HeaderFooter
    {
        private PdfDocument pdf;

        private HeaderFooter()
        {
        }

        internal HeaderFooter(PdfDocument Doc)
        {
            this.pdf = Doc;
        }

        /// <summary>
        /// 创建PDF页眉
        /// </summary>
        /// <param name="imageUrl">页眉图片</param>
        /// <param name="text">页眉文字</param>s

        public void GenerateHeader(string imageUrl = "", string text = "")
        {
            SizeF pageSize = pdf.Pages[0].Size == null ? PdfPageSize.A4 : pdf.Pages[0].Size;

            //画笔
            PdfPen pen = new PdfPen(pdf.StyleConfig.PenColor, pdf.StyleConfig.PenWidth);
            //文字样式和位置 字体和对齐方式
            PdfTrueTypeFont font = new PdfTrueTypeFont(new Font(pdf.StyleConfig.HeaderFont, pdf.StyleConfig.HeaderEmSize));

            for (int i = 0; i < pdf.Pages.Count; i++)
            {
                //偏移变量
                float x = pdf.PdfMargins.Left;
                float y = 0;

                //图片size
                float width = 0;
                float height = 0;

                //绘制图片
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    PdfImage headerImage = PdfImage.FromFile(imageUrl);
                    width = headerImage.Width / 4;
                    height = headerImage.Height / 4;
                    y = pdf.PdfMargins.Top;
                    pdf.Pages[i].Canvas.DrawImage(headerImage, x, y, width, height);
                }

                //绘制线段
                float x1 = pdf.PdfMargins.Left + width + 30;
                float x2 = pageSize.Width - pdf.PdfMargins.Right;
                float y1 = pdf.PdfMargins.Top + height;

                pdf.Pages[i].Canvas.DrawLine(pen, x1, y1, x2, y1);

                if (!string.IsNullOrEmpty(text))
                {
                    // 绘入文字
                    SizeF size = font.MeasureString(text);
                    x = pageSize.Width - pdf.PdfMargins.Right - size.Width;
                    y = height - size.Height + (pdf.PdfMargins.Top / 3 * 2);
                    pdf.Pages[i].Canvas.DrawString(text, font, PdfBrushes.Black, x, y);
                }
            }
        }

        /// <summary>
        /// 创建PDF页脚
        /// </summary>
        /// <param name="footerText">页脚文本</param>
        /// <returns></returns>
        public void GenerateFooter(string footerText = "")
        {
            if (!string.IsNullOrEmpty(footerText))
            {
                //获取页面大小
                SizeF pageSize = pdf.Pages[0].Size == null ? PdfPageSize.A4 : pdf.Pages[0].Size;

                //偏移量
                float x = 0;
                float y = 0;

                //画笔
                PdfPen pen = new PdfPen(pdf.StyleConfig.PenColor, pdf.StyleConfig.PenWidth);

                //文字样式和位置
                PdfTrueTypeFont font = new PdfTrueTypeFont(new Font(pdf.StyleConfig.FooterFont, pdf.StyleConfig.FooterEmSize), true);

                for (int i = 0; i < pdf.Pages.Count; i++)
                {
                    //绘制文字
                    SizeF size = font.MeasureString(footerText);
                    x = (pageSize.Width - size.Width) / 2;
                    y = pageSize.Height - pdf.PdfMargins.Bottom - size.Height - 2;
                    pdf.Pages[i].Canvas.DrawString(footerText, font, PdfBrushes.Black, x, y);
                }
            }
        }
    }
}