using Needs.Utils.SpirePdf;
using Needs.Wl.Models.Views;
using System;
using System.Data;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 付汇申请的付汇委托书
    /// </summary>
    public class PayExchangeAgentProxy
    {
        private readonly PayExchangeApply PayExchangeApply;
        private readonly Client Client;

        public PayExchangeAgentProxy(PayExchangeApply payExchangeApply)
        {
            this.PayExchangeApply = payExchangeApply;
            this.Client = new ClientsView()[payExchangeApply.ClientID];
        }

        /// <summary>
        /// 生成Pdf
        /// </summary>
        /// <returns></returns>
        public PdfDocument ToPdf()
        {
            //var environment = Needs.Wl.Environment.Environment.Current;

            //两列无标题表格
            DataTable table = new DataTable();

            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Value", typeof(string));

            #region 添加行值

            DataRow row = table.NewRow();
            row["Name"] = "供应商名称";
            row["Value"] = this.PayExchangeApply.SupplierName;
            table.Rows.Add(row);

            row = table.NewRow();
            row["Name"] = "供应商地址";
            row["Value"] = this.PayExchangeApply.SupplierAddress;
            table.Rows.Add(row);

            var expectPayDate = this.PayExchangeApply.ExpectPayDate;
            row = table.NewRow();
            row["Name"] = "期望付汇日期";
            row["Value"] = expectPayDate == null ? "" : ((DateTime)expectPayDate).ToString("yyyy-MM-dd");
            table.Rows.Add(row);

            row = table.NewRow();
            row["Name"] = "付汇金额(" + this.PayExchangeApply.Currency + ")";
            decimal price = 0M;
            foreach (var item in this.PayExchangeApply.Items)
            {
                price += item.Amount;
            }
            row["Value"] = price.ToString("#0.00");
            table.Rows.Add(row);

            row = table.NewRow();
            row["Name"] = "供应商公司全称(英文)";
            row["Value"] = this.PayExchangeApply.SupplierEnglishName;
            table.Rows.Add(row);

            row = table.NewRow();
            row["Name"] = "供应商银行名称(英文)";
            row["Value"] = this.PayExchangeApply.BankName;
            table.Rows.Add(row);

            row = table.NewRow();
            row["Name"] = "供应商银行地址(英文)";
            row["Value"] = this.PayExchangeApply.BankAddress;
            table.Rows.Add(row);

            row = table.NewRow();
            row["Name"] = "银行账号";
            row["Value"] = this.PayExchangeApply.BankAccount;
            table.Rows.Add(row);

            row = table.NewRow();
            row["Name"] = "银行代码";
            row["Value"] = this.PayExchangeApply.SwiftCode;
            table.Rows.Add(row);

            row = table.NewRow();
            row["Name"] = "其它相关资料";
            row["Value"] = this.PayExchangeApply.OtherInfo;
            table.Rows.Add(row);

            row = table.NewRow();
            row["Name"] = "备注";
            if (string.IsNullOrEmpty(this.PayExchangeApply.Summary))
            {
                row["Value"] = "请仔细填写帐户信息，因委托方银行信息填写错误造成损失的，由委托方自行承担责任";
            }
            else
            {
                row["Value"] = this.PayExchangeApply.Summary;
            }

            table.Rows.Add(row);

            #endregion

            //Create a pdf document
            PdfDocument doc = new PdfDocument();

            //set margin
            //PdfMargins margin = new PdfMargins(30, 60);

            //Create one page
            //PdfPageBase page = doc.Pages.Add(PdfPageSize.A4, margin);

            //float y = 5;

            //Title
            //PdfBrush brush1 = PdfBrushes.Black;
            //字体
            //PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 12f, FontStyle.Regular), true);
            //PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 12f, FontStyle.Bold), true);
            //PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("SimSun", 9f, FontStyle.Regular), true);
            //字体对齐方式
            //PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            //PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            //PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #region 头

            //page.Canvas.DrawString("付汇委托书", font2, brush1, page.Canvas.ClientSize.Width / 2, y, formatCenter);
            //y = y + font2.MeasureString("付汇委托书", formatCenter).Height;
            //y = y + 5;
            //page.Canvas.DrawString("委托方：" + this.Client.Company.Name, font2, brush1, 0, y, formatLeft);
            //y = y + font2.MeasureString("委托方：" + this.Client.Company.Name, formatLeft).Height;
            //y = y + 5;
            //page.Canvas.DrawString($"代理方：{environment.Company.Name}", font2, brush1, 0, y, formatLeft);
            //y = y + font2.MeasureString($"代理方：{environment.Company.Name}", formatLeft).Height;
            //y = y + 9;
            #endregion

            #region 表格

            //PdfTable pdfTable = new PdfTable();

            //pdfTable.DataSourceType = PdfTableDataSourceType.TableDirect;
            //pdfTable.DataSource = table;

            //pdfTable.Style.CellPadding = 5f;
            //pdfTable.Style.BorderPen = new PdfPen(Color.Gray, 0.1f);
            //pdfTable.Style.ShowHeader = false;

            //pdfTable width = page.Canvas.ClientSize.Width - (table.Columns.Count + 1) * pdfTable.Style.BorderPen.Width;
            //pdfTable.Columns[0].Width = width * 0.25f * width;
            //pdfTable.Columns[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            //pdfTable.Columns[1].Width = width * 0.75f * width;
            //pdfTable.Columns[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            //pdfTable.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowLayout);

            //PdfLayoutResult result = pdfTable.Draw(page, new PointF(0, y));
            //y = y + result.Bounds.Height + 30;

            #endregion

            #region 尾

            //page.Canvas.DrawString("委托方签字、盖章确认：", font3, brush1, width - 200, y, formatLeft);
            //y = y + font3.MeasureString("委托方签字、盖章确认：", formatLeft).Height;
            //page.Canvas.DrawLine(new PdfPen(Color.Black, 0.2f), new PointF(width - 100, y), new PointF(width, y));

            //自有公司加上委托方章
            if (this.Client.ClientType == Enums.ClientType.Internal)
            {
                //PdfImage imageInternal = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.Client.Company.Name + ".png"));
                //page.Canvas.DrawImage(imageInternal, 420, y - 50);
            }

            #endregion

            #region 公共组件

            //页眉、页脚、二维码、水印
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(doc);
            //string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, environment.HeaderImg);

            //pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, environment.OfficalWebsite);
            //pdfDocumentHandle.HeaderFooter.GenerateFooter(environment.Company.Name);
            //pdfDocumentHandle.Watermark.DrawWatermark(environment.Company.Name);

            #endregion

            return doc;
        }

        //private void table_BeginRowLayout(object sender, BeginRowLayoutEventArgs args)
        //{
        //    //Set the color of table cell border
        //    PdfCellStyle cellStyle = new PdfCellStyle();
        //    cellStyle.BorderPen = new PdfPen(Color.Gray, 0.1f);
        //    cellStyle.Font = new PdfTrueTypeFont(new Font("SimSun", 9f), true);
        //    args.CellStyle = cellStyle;
        //    args.MinimalHeight = 20f;
        //}

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public void SaveAs(string fileName)
        {
            //this.ToPdf().SaveToFile(fileName);
        }
    }
}
