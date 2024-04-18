using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Spire.Pdf;
using Spire.Pdf.Graphics;

namespace Yahv.Utils.SpirePdf
{
    /// <summary>
    /// PDF水印
    /// </summary>
    public class Watermark
    {
        private PdfDocument Pdf;

        internal Watermark(PdfDocument pdf)
        {
            Pdf = pdf;
        }

        /// <summary>
        /// 生成PDF水印
        /// </summary>
        /// <param name="text">水印文字</param>
        public void DrawWatermark(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                //创建True Type字体
                PdfTrueTypeFont font = new PdfTrueTypeFont(new Font(Pdf.StyleConfig.WaterMarkFont, Pdf.StyleConfig.WaterMarkEmSize), true);

                //测量文字所占的位置大小
                SizeF size = font.MeasureString(text);

                //计算偏移量
                float x = (float)(size.Width * System.Math.Sqrt(2) / 4);
                float y = (float)(size.Height * System.Math.Sqrt(2) / 4);

                //遍历文档页
                foreach (PdfPageBase page in Pdf.Pages)
                {
                    //设置透明度
                    page.Canvas.SetTransparency(Pdf.StyleConfig.Transparency);

                    //将坐标系向右,向下平移
                    page.Canvas.TranslateTransform(page.Canvas.Size.Width / 2 - x - y, page.Canvas.Size.Height / 2 + x - y);

                    //将坐标系逆时针旋转45度
                    page.Canvas.RotateTransform(Pdf.StyleConfig.Angle);

                    //绘制文本
                    page.Canvas.DrawString(text, font, PdfBrushes.DarkGray, 0, 0);
                }
            }
        }
    }
}
