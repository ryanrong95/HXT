using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf.Graphics;

namespace Yahv.Utils.SpirePdf
{
    public class StyleConfig
    {
        internal StyleConfig()
        {

        }

        private PdfBrush penColor;

        /// <summary>
        /// 画笔颜色
        /// </summary>
        public PdfBrush PenColor
        {
            get
            {
                if (penColor == null)
                {
                    return penColor = PdfBrushes.Gray;
                }
                else
                {
                    return penColor;
                }
            }
            set { penColor = value; }
        }

        private float penWidth;
        
        /// <summary>
        /// 页眉字号
        /// </summary>
        public float PenWidth
        {
            get
            {
                if (penWidth <= 0)
                {
                    return penWidth = 0.5f;
                }
                else
                {
                    return penWidth;
                }
            }
            set { penWidth = value; }
        }

        private string headerFont;

        /// <summary>
        /// 页眉字体
        /// </summary>
        public string HeaderFont
        {
            get
            {
                if (string.IsNullOrEmpty(headerFont))
                {
                    return headerFont = "微软雅黑";
                }
                else
                {
                    return headerFont;
                }
            }
            set { headerFont = value; }
        }


        private float headerEmSize;
      
        /// <summary>
        /// 页眉字号
        /// </summary>
        public float HeaderEmSize
        {
            get
            {
                if (headerEmSize <= 0)
                {
                    return headerEmSize = 8f;
                }
                else
                {
                    return headerEmSize;
                }
            }
            set { headerEmSize = value; }
        }

        private string footerFont;

        /// <summary>
        /// 页脚字体
        /// </summary>
        public string FooterFont
        {
            get
            {
                if (string.IsNullOrEmpty(footerFont))
                {
                    return footerFont = "宋体";
                }
                else
                {
                    return footerFont;
                }
            }
            set { footerFont = value; }
        }


        private float footerEmSize;
      
        /// <summary>
        /// 页脚字号
        /// </summary>
        public float FooterEmSize
        {
            get
            {
                if (footerEmSize <= 0)
                {
                    return footerEmSize = 9f;
                }
                else
                {
                    return footerEmSize;
                }
            }
            set { footerEmSize = value; }
        }

        private string waterMarkfont;

        /// <summary>
        /// 水印字体
        /// </summary>
        public string WaterMarkFont
        {
            get
            {
                if (string.IsNullOrEmpty(waterMarkfont))
                {
                    return waterMarkfont = "楷体";
                }
                else
                {
                    return waterMarkfont;
                }
            }
            set { waterMarkfont = value; }
        }


        private float waterMarkEmSize;
  
        /// <summary>
        /// 水印字号
        /// </summary>
        public float WaterMarkEmSize
        {
            get
            {
                if (waterMarkEmSize <= 0)
                {
                    return waterMarkEmSize = 30f;
                }
                else
                {
                    return waterMarkEmSize;
                }
            }
            set { waterMarkEmSize = value; }
        }


        private float transparency;

        /// <summary>
        /// 透明度
        /// </summary>
        public float Transparency
        {
            get
            {
                if (transparency <= 0)
                {
                    return transparency = 0.5f;
                }
                else
                {
                    return transparency;
                }
            }
            set { transparency = value; }
        }

        private float angle;

        /// <summary>
        /// 旋转角度
        /// </summary>
        public float Angle
        {
            get
            {
                if (angle <= 0)
                {
                    return angle = -45;
                }
                else
                {
                    return angle;
                }
            }
            set { angle = value; }
        }

    }
}
