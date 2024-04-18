using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CutImage
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string savePath { get; set; }
        public string fileExt { get; set; }
        public string filename { get; set; }

        public void Cut()
        {
            Bitmap b = new Bitmap(@"D:\Vs2015_Projects\BvnErp.Chains_SIT-分支\WebApp\Content\images\CutTest\HK.png");
            Bitmap newbit = GetPartOfImageRec(b, 100, 50, 20, 10);
            newbit.Save(@"D:\Vs2015_Projects\BvnErp.Chains_SIT-分支\WebApp\Content\images\CutTest\HKCut.png");
        }

        /// <summary>
        /// 切割图片
        /// </summary>
        /// <param name="sourceBitmap">图片对象</param>
        /// <param name="width">切割的宽度</param>
        /// <param name="height">切割的高度</param>
        /// <param name="offsetX">开始的x</param>
        /// <param name="offsetY">开始的y</param>
        /// <returns></returns>
        private Bitmap GetPartOfImageRec(Bitmap sourceBitmap, int width, int height, int offsetX, int offsetY)
        {
            Bitmap resultBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resultBitmap))
            {
                Rectangle resultRectangle = new Rectangle(0, 0, width, height);
                Rectangle sourceRectangle = new Rectangle(0 + offsetX, 0 + offsetY, width, height);
                g.DrawImage(sourceBitmap, resultRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }
            return resultBitmap;
        }      
    }    
}
