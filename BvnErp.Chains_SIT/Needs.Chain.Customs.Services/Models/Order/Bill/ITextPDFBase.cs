﻿extern alias globalB;

using globalB::iTextSharp.text;
using globalB::iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Needs.Ccs.Services.Models
{
    public class ITextPDFBase : PdfPageEventHelper
    {
        #region 属性
        private String _fontFilePathForHeaderFooter = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "SIMHEI.TTF");
        /// <summary>
        /// 页眉/页脚所用的字体
        /// </summary>
        public String FontFilePathForHeaderFooter
        {
            get
            {
                return _fontFilePathForHeaderFooter;
            }

            set
            {
                _fontFilePathForHeaderFooter = value;
            }
        }

        private String _fontFilePathForBody = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "SIMSUN.TTC,1");

        /// <summary>
        /// 正文内容所用的字体
        /// </summary>
        public String FontFilePathForBody
        {
            get { return _fontFilePathForBody; }
            set { _fontFilePathForBody = value; }
        }


        private PdfPTable _header;
        /// <summary>
        /// 页眉
        /// </summary>
        public PdfPTable Header
        {
            get { return _header; }
            private set { _header = value; }
        }

        private PdfPTable _footer;
        /// <summary>
        /// 页脚
        /// </summary>
        public PdfPTable Footer
        {
            get { return _footer; }
            private set { _footer = value; }
        }


        private BaseFont _baseFontForHeaderFooter;

        /// <summary>
        /// 页眉页脚所用的字体
        /// </summary>
        public BaseFont BaseFontForHeaderFooter
        {
            get { return _baseFontForHeaderFooter; }
            set { _baseFontForHeaderFooter = value; }
        }

        private BaseFont _baseFontForBody;

        /// <summary>
        /// 正文所用的字体
        /// </summary>
        public BaseFont BaseFontForBody
        {
            get { return _baseFontForBody; }
            set { _baseFontForBody = value; }
        }

        private Document _document;

        /// <summary>
        /// PDF的Document
        /// </summary>
        public Document Document
        {
            get { return _document; }
            private set { _document = value; }
        }

        #endregion


        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                BaseFontForHeaderFooter = BaseFont.CreateFont(FontFilePathForHeaderFooter, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                BaseFontForBody = BaseFont.CreateFont(FontFilePathForBody, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                Document = document;
            }
            catch (DocumentException de)
            {
                throw de;
            }
            catch (System.IO.IOException ioe)
            {
                throw ioe;
            }
        }

        #region 生成页头
        /// <summary>
        /// 生成页眉
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public virtual PdfPTable GenerateHeader(PdfWriter writer)
        {
            return null;
        }
        #endregion

        #region 生成页脚
        /// <summary>
        /// 生成页脚
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public virtual PdfPTable GenerateFooter(PdfWriter writer)
        {
            return null;
        }
        #endregion

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            //输出页眉
            Header = GenerateHeader(writer);
            Header.TotalWidth = document.PageSize.Width - 20f;
            ///调用PdfTable的WriteSelectedRows方法。该方法以第一个参数作为开始行写入。
            ///第二个参数-1表示没有结束行，并且包含所写的所有行。
            ///第三个参数和第四个参数是开始写入的坐标x和y.
            Header.WriteSelectedRows(0, -1, 10, document.PageSize.Height - 10, writer.DirectContent);

            //输出页脚
            Footer = GenerateFooter(writer);
            Footer.TotalWidth = document.PageSize.Width - 20f;
            Footer.WriteSelectedRows(0, -1, 10, document.PageSize.GetBottom(20), writer.DirectContent);
        }
    }

}
