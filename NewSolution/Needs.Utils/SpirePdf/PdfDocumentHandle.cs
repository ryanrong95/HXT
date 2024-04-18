using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf.Graphics;

namespace Needs.Utils.SpirePdf
{
    public class PdfDocumentHandle
    {
        private PdfDocument Pdf;

        private PdfDocumentHandle()
        {

        }

        public PdfDocumentHandle(PdfDocument pdf)
        {
            //对pdf对象的限制 
            this.Pdf = pdf;
            PdfDocumentInit(this.Pdf);
        }

        private void PdfDocumentInit(PdfDocument pdf)
        {
            //对边距清零
            pdf.PageSettings.Margins = new PdfMargins(0);
            //pdf要包含page.
            if (pdf.Pages.Count == 0)
            {
                pdf.Pages.Add(Spire.Pdf.PdfPageSize.A4);
            }
        }

        /// <summary>
        /// pdf页眉页脚
        /// </summary>
        public HeaderFooter HeaderFooter
        {
            get
            {
                return new HeaderFooter(Pdf);
            }
        }

        /// <summary>
        /// pdf水印
        /// </summary>
        public Watermark Watermark
        {
            get
            {
                return new Watermark(Pdf);
            }
        }

        /// <summary>
        /// pdf二维码
        /// </summary>
        public Barcode Barcode
        {
            get
            {
                return new Barcode(Pdf);
            }
        }
       
    }

    public class PdfDocument : Spire.Pdf.PdfDocument
    {
        public PdfDocument()
        {
            StyleConfig = new StyleConfig();
            PdfMargins = new PdfMargins(30, 15);
        }

        /// <summary>
        /// 页眉页脚需要的边距
        /// </summary>
        public PdfMargins PdfMargins { get; set; }

        /// <summary>
        /// 页眉 页脚 水印等字体，字号，画笔样式，透明度等
        /// </summary>
        public StyleConfig StyleConfig;

    }

}
