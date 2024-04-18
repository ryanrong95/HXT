using Spire.Barcode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.PdaApi.Services.Extends
{
    public static class SpireExtend
    {
        /// <summary>
        /// 条形码
        /// </summary>
        /// <param name="data">用于渲染条形码的数据</param>
        /// <param name="type">条码类型</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">条码高度</param>
        /// <param name="showText">是否显示文本</param>
        /// <returns></returns>
        public static string Barcode(this string data, int type, int width, int height, bool showText)
        {
            //设置key，用于移除水印
            BarcodeSettings.ApplyKey("SI2HX-CTSEC-MR7HH-I1TWS-A6UOT");

            //条形码设置
            BarcodeSettings bs = new BarcodeSettings();
            bs.Data = data;
            bs.Type = (BarCodeType)type;
            bs.Unit = System.Drawing.GraphicsUnit.Pixel;
            bs.ImageWidth = width;
            bs.BarHeight = height;
            bs.ShowText = showText;
            bs.ShowTextOnBottom = true;

            //以base64编码返回
            using (MemoryStream ms = new MemoryStream())
            {
                BarCodeGenerator bg = new BarCodeGenerator(bs);
                bg.GenerateImage().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = ms.ToArray();

                return $"data:image/png;base64,{Convert.ToBase64String(byteImage)}";
            }
        }
    }
}
