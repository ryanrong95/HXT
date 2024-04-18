using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using System;
using System.Data;
using System.Drawing;
using Yahv.Utils.SpirePdf;
using PdfDocument = Yahv.Utils.SpirePdf.PdfDocument;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    /// <summary>
    /// 付汇委托书
    /// </summary>
    public class PrePayAgentProxy
    {
        private PrePayApplyExtends Proxy;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="application"></param>
        public PrePayAgentProxy(PrePayApplyExtends proxy)
        {
            this.Proxy = proxy;
        }

        //导出付汇委托书
        public PdfDocument ToPDF()
        {
            var CompanyName = PvClientConfig.CompanyName;


            //两列无标题表格
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(string));

            #region 添加行值
            DataRow dr1 = dt.NewRow();
            dr1["Name"] = "供应商名称";
            dr1["Value"] = this.Proxy.SupplierEnglishName;
            dt.Rows.Add(dr1);
            DataRow dr2 = dt.NewRow();
            dr2["Name"] = "供应商地址";
            dr2["Value"] = this.Proxy.SupplierAddress;
            dt.Rows.Add(dr2);
            DataRow dr3 = dt.NewRow();
            dr3["Name"] = "申请日期";
            dr3["Value"] = this.Proxy.CreateDate.ToString("yyyy-MM-dd");
            dt.Rows.Add(dr3);
            DataRow dr4 = dt.NewRow();
            dr4["Name"] = "本次申请金额(" + this.Proxy.Currency + ")";
            dr4["Value"] = this.Proxy.TotalPrice.ToString("#0.00");
            dt.Rows.Add(dr4);
            DataRow dr5 = dt.NewRow();
            dr5["Name"] = "供应商银行名称(英文)";
            dr5["Value"] = this.Proxy.BankName;
            dt.Rows.Add(dr5);
            DataRow dr6 = dt.NewRow();
            dr6["Name"] = "供应商银行地址(英文)";
            dr6["Value"] = this.Proxy.BankAddress;
            dt.Rows.Add(dr6);
            DataRow dr7 = dt.NewRow();
            dr7["Name"] = "银行账号";
            dr7["Value"] = this.Proxy.BankAccount;
            dt.Rows.Add(dr7);
            DataRow dr8 = dt.NewRow();
            dr8["Name"] = "银行代码";
            dr8["Value"] = this.Proxy.SwiftCode;
            dt.Rows.Add(dr8);
            DataRow dr9 = dt.NewRow();
            dr9["Name"] = "备注";
            dr9["Value"] = "1、请仔细填写帐户信息，因委托方银行信息填写错误造成损失的，由委托方自行承担责任。\r\n2、受托人可自行支付或委托第三方支付。\r\n3、该付款委托书传真件、扫描件与原件具有同等效力。";
            dt.Rows.Add(dr9);

            #endregion

            //Create a pdf document
            PdfDocument doc = new PdfDocument();

            //set margin
            PdfMargins margin = new PdfMargins(30, 60);

            //Create one page
            PdfPageBase page = doc.Pages.Add(PdfPageSize.A4, margin);

            float y = 5;

            //Title
            PdfBrush brush1 = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 12f, FontStyle.Regular), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 12f, FontStyle.Bold), true);
            PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("SimSun", 9f, FontStyle.Regular), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #region 头
            page.Canvas.DrawString("代付货款委托书", font2, brush1, page.Canvas.ClientSize.Width / 2, y, formatCenter);
            y = y + font2.MeasureString("代付货款委托书", formatCenter).Height;
            y = y + 5;
            page.Canvas.DrawString("委托方：" + this.Proxy.ClientName, font2, brush1, 0, y, formatLeft);
            y = y + font2.MeasureString("委托方：" + this.Proxy.ClientName, formatLeft).Height;
            y = y + 5;
            page.Canvas.DrawString($"受托方：{CompanyName}", font2, brush1, 0, y, formatLeft);
            y = y + font2.MeasureString($"受托方：{CompanyName}", formatLeft).Height;
            y = y + 9;
            #endregion

            #region 表格

            PdfTable table = new PdfTable();

            table.DataSourceType = PdfTableDataSourceType.TableDirect;
            table.DataSource = dt;

            table.Style.CellPadding = 5f;
            table.Style.BorderPen = new PdfPen(Color.Gray, 0.1f);
            table.Style.ShowHeader = false;

            float width = page.Canvas.ClientSize.Width - (table.Columns.Count + 1) * table.Style.BorderPen.Width;
            table.Columns[0].Width = width * 0.25f * width;
            table.Columns[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            table.Columns[1].Width = width * 0.75f * width;
            table.Columns[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            table.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowLayout);

            PdfLayoutResult result = table.Draw(page, new PointF(0, y));
            y = y + result.Bounds.Height + 30;
            #endregion

            #region 尾

            page.Canvas.DrawString("委托方签字、盖章确认：", font3, brush1, width - 200, y, formatLeft);
            y = y + font3.MeasureString("委托方签字、盖章确认：", formatLeft).Height;
            page.Canvas.DrawLine(new PdfPen(Color.Black, 0.2f), new PointF(width - 100, y), new PointF(width, y));

            //大赢家加上委托方章
            //if (this.PayExchangeApply.Client.ClientType == Enums.ClientType.Internal)
            //{
            //    PdfImage imageInternal = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.PayExchangeApply.Client.Company.Name + ".png"));
            //    page.Canvas.DrawImage(imageInternal, 400, y - 50);
            //}

            #endregion

            #region 公共组件

            //页眉、页脚、二维码、水印
            var purchaser = PurchaserContext.Current;
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(doc);
            string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.HeaderImg);

            pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, purchaser.OfficalWebsite);
            pdfDocumentHandle.HeaderFooter.GenerateFooter(CompanyName);
            pdfDocumentHandle.Watermark.DrawWatermark(CompanyName);

            #endregion
            return doc;
        }

        private void table_BeginRowLayout(object sender, BeginRowLayoutEventArgs args)
        {
            //Set the color of table cell border
            PdfCellStyle cellStyle = new PdfCellStyle();
            cellStyle.BorderPen = new PdfPen(Color.Gray, 0.1f);
            cellStyle.Font = new PdfTrueTypeFont(new Font("SimSun", 9f), true);
            args.CellStyle = cellStyle;
            args.MinimalHeight = 20f;
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public void SaveAs(string fileName)
        {
            this.ToPDF().SaveToFile(fileName);
        }

        /// <summary>
        /// 文件保存（未完成，使用者自行开发完成）
        /// 保存的目录为系统配置或默认的目录，并将保存后的信息返回
        /// </summary>
        /// <returns></returns>
        public string[] Save()
        {
            //文件保存，根据业务需求进行开发
            //保存的目录为系统配置或默认的目录，并将保存后的信息返回
            return new string[] { "", "", "" };
        }
    }
}
