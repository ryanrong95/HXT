using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Npoi
{
    /// <summary>
    ///样式对象  Title为大标题，Head是列头。
    /// </summary>
    public class StyleConfig
    {
        private short _titlePoint;
        private string _titleFont;
        private short _boldWeight;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 前景色
        /// </summary>
        public Color ForeColor { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// 标题字号
        /// </summary>
        public short TitlePoint
        {
            get
            {
                if (_titlePoint == 0)
                {
                    return 20;
                }
                else
                {
                    return _titlePoint;
                }
            }
            set { _titlePoint = value; }
        }

        /// <summary>
        /// 标题字体
        /// </summary>
        public string TitleFont
        {
            get
            {
                if (string.IsNullOrEmpty(_titleFont))
                {
                    return "微软雅黑";
                }
                else
                {
                    return _titleFont;
                }
            }
            set { _titleFont = value; }
        }

        /// <summary>
        /// 标题高度
        /// </summary>
        public short TitleHeight { get; set; }
        /// <summary>
        /// 字体粗细
        /// </summary>
        public short Boldweight
        {
            get
            {
                if (_boldWeight == 0)
                {
                    return 700;
                }
                else
                {
                    return _boldWeight;
                }
            }
            set { _boldWeight = value; }
        }


        private short _headPoint;
        /// <summary>
        /// 列头字号
        /// </summary>
        public short HeadPoint
        {
            get
            {
                if (_headPoint == 0)
                {
                    return 10;
                }
                else
                {
                    return _headPoint;
                }
            }
            set { _headPoint = value; }
        }

        /// <summary>
        /// 列标题高度
        /// </summary>
        public short HeadHeight { get; set; }

        private string _headFont;
        /// <summary>
        /// 列头字体
        /// </summary>
        public string HeadFont
        {
            get
            {
                if (string.IsNullOrEmpty(_headFont))
                {
                    return "微软雅黑";
                }
                else
                {
                    return _headFont;
                }
            }
            set { _headFont = value; }
        }
    }
}
