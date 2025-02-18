﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Ccs.Utils
{
    /// <summary>
    ///版 本 V1.0
    ///日 期：2019/01/25
    ///描 述：Excel导入导出设置
    /// </summary>
    public class ExcelConfig
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
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
        private short _titlepoint;
        /// <summary>
        /// 标题字号
        /// </summary>
        public short TitlePoint
        {
            get
            {
                if (_titlepoint == 0)
                {
                    return 20;
                }
                else
                {
                    return _titlepoint;
                }
            }
            set { _titlepoint = value; }
        }
        private short _headpoint;
        /// <summary>
        /// 列头字号
        /// </summary>
        public short HeadPoint
        {
            get
            {
                if (_headpoint == 0)
                {
                    return 10;
                }
                else
                {
                    return _headpoint;
                }
            }
            set { _headpoint = value; }
        }
        /// <summary>
        /// 标题高度
        /// </summary>
        public short TitleHeight { get; set; }
        /// <summary>
        /// 列标题高度
        /// </summary>
        public short HeadHeight { get; set; }
        private string _titlefont;
        /// <summary>
        /// 标题字体
        /// </summary>
        public string TitleFont
        {
            get
            {
                if (_titlefont == null)
                {
                    return "微软雅黑";
                }
                else
                {
                    return _titlefont;
                }
            }
            set { _titlefont = value; }
        }
        private string _headfont;
        /// <summary>
        /// 列头字体
        /// </summary>
        public string HeadFont
        {
            get
            {
                if (_headfont == null)
                {
                    return "微软雅黑";
                }
                else
                {
                    return _headfont;
                }
            }
            set { _headfont = value; }
        }
        /// <summary>
        /// 是否按内容长度来适应表格宽度
        /// </summary>
        public bool IsAllSizeColumn { get; set; }
        /// <summary>
        /// 列设置
        /// </summary>
        public List<ColumnEntity> ColumnEntity { get; set; }

        /// <summary>
        /// 文件保存相对路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 设置每行的背景色
        /// 如果设置为true，则必须要有列"ForeColour" 设置每列的前景色
        /// </summary>
        public bool RowForeColour { get; set; }

        /// <summary>
        /// 自动合并列 2020-09-11 by yeshuangshuang
        /// </summary>
        public int? AutoMergedColumn { get; set; } = null;
    }
}
