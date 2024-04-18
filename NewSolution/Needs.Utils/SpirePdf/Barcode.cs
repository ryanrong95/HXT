using Spire.Barcode;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.SpirePdf
{
    public class Barcode
    {
        private PdfDocument pdf;

        private Barcode()
        {
        }

        internal Barcode(PdfDocument Doc)
        {
            this.pdf = Doc;
        }

        #region 二维码
       /// <summary>
       /// 生成二维码
       /// </summary>
       /// <param name="data">二维码信息</param>
       /// <param name="imageUrl">页眉图片路径</param>
        public void GenerateQRCode(string data, string imageUrl = "")
        {
            if (!string.IsNullOrEmpty(data))
            {
                SizeF pageSize = pdf.Pages[0].Size == null ? PdfPageSize.A4 : pdf.Pages[0].Size;

                //创建BarcodeSettings对象
                BarcodeSettings.ApplyKey("SI2HX-CTSEC-MR7HH-I1TWS-A6UOT");

                BarcodeSettings settings = new BarcodeSettings();

                //设置条码类型为二维码
                settings.Type = BarCodeType.QRCode;

                //设置二维码数据
                settings.Data = data;
                settings.ShowText = false;

                //设置数据类型
                settings.QRCodeDataMode = QRCodeDataMode.Auto;

                //设置宽度
                settings.X = 0.7f;

                BarCodeGenerator generator = new BarCodeGenerator(settings);
                Image image = generator.GenerateImage();

                //绘制二维码图形到PDF
                PdfImage pdfImage = PdfImage.FromImage(image);

                float x = pageSize.Width - pdf.PdfMargins.Right - pdfImage.PhysicalDimension.Width;
                float y = 5;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    PdfImage headerImage = PdfImage.FromFile(imageUrl);
                    y += headerImage.Height / 4;
                }

                y += pdf.PdfMargins.Top;
                pdf.Pages[0].Canvas.DrawImage(pdfImage, x, y);
            }
        }
        #endregion
    }
}
