extern alias globalB;

using globalB::iTextSharp.text;
using globalB::iTextSharp.text.pdf;
using Needs.Utils.Flow.Event;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PI83Anda : PIBuilder
    {
        public PI83Anda(string fileName, List<Views.OrderItemSupplierViewModel> items, string piNo, string clientName) : base(fileName, items, piNo, clientName)
        {

        }

        public override void Execute()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("型号信息", new { Items = this.Items, });
            parameters.Add("PINO", new { PINO = this.PINO, });
            parameters.Add("ClientName", new { ClientName = this.ClientName, });

            PdfBuilder pdfBuilder = new PdfBuilder(this.FileName, parameters);
            pdfBuilder.SetDocumentMargins(10f, 10f, 2f, 2f);

            pdfBuilder.AddStep(PartBigMargin);
            pdfBuilder.AddStep(PartTitle);
            pdfBuilder.AddStep(PartAddress);
            pdfBuilder.AddStep(PartTel);
            pdfBuilder.AddStep(PartBigTitle);
            pdfBuilder.AddStep(PartAUTH);
            pdfBuilder.AddStep(PartBillToShipTo);
            pdfBuilder.AddStep(PartTelFax);
            pdfBuilder.AddStep(PartData);
            pdfBuilder.AddStep(PartLast);

            pdfBuilder.ToPdf();
        }

        private static object PartBigMargin(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("  ", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("  ", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            return table;
        }

        private static object PartTitle(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 70, 30, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("  ", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Anda International Trade Group Limited", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            Image image11 = Image.GetInstance(System.Configuration.ConfigurationManager.AppSettings["ContentPath"] + @"\Content\Images\Logos\安达.png");
            image11.ScalePercent(45f);
            var bbbb = new PdfPCell(image11) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
            bbbb.DisableBorderSide(15);
            table.AddCell(bbbb);

            return table;
        }

        private static object PartAddress(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("Suite 909, 9/F, Two Grand Tower, 625 Nathan Road, Mongkok, Kowloon,Hong Kong", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            return table;
        }

        private static object PartTel(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("                TEL: " + "852-62279866" + "           FAX:" + "", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            return table;
        }

        private static object PartBigTitle(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("COMMERCIAL INVOICE", new Font(baseFont, 16f, Font.NORMAL)));
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell1.PaddingLeft = 170;
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            return table;
        }

        private static object PartAUTH(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["PINO"];
            string piNo = (string)param.GetType().GetProperty("PINO").GetValue(param);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 60, 40, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("AUTH# BY:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("SALES PERSON:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("  ", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("ATTN:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            var cell2 = new PdfPCell();
            cell2.AddElement(new Phrase("PI INV. NO.:  " + piNo, new Font(baseFont, 8f, Font.NORMAL)));
            cell2.AddElement(new Phrase("GST REG#:", new Font(baseFont, 8f, Font.NORMAL)));
            cell2.AddElement(new Phrase("P/O#:", new Font(baseFont, 8f, Font.NORMAL)));
            cell2.AddElement(new Phrase("DATE:   " + DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), new Font(baseFont, 8f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            return table;
        }

        private static object PartBillToShipTo(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;
            object client = e.Entity.Params["ClientName"];
            string clientName = (string)client.GetType().GetProperty("ClientName").GetValue(client);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            PdfPCell cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("BILL TO:  " + clientName, new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("SHIP TO:  HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO LIMITED", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            return table;
        }

        private static object PartTelFax(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            PdfPCell cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("TEL:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("FAX:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            return table;
        }

        private static object PartData(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["型号信息"];
            var items = (List<Views.OrderItemSupplierViewModel>)param.GetType().GetProperty("Items").GetValue(param);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            DataTable dt = GetData(items);

            PdfPTable table = new PdfPTable(dt.Columns.Count); //列数
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;

            //设置行宽
            int[] widths = new int[] { 15, 15, 25, 15, 15, 15, }; //百分比的感觉
            table.SetWidths(widths);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(dt.Rows[i][j].ToString(), new Font(baseFont, 6f)))
                    {
                        MinimumHeight = 15,
                        BorderWidth = 0.06f,
                    };

                    //合并行
                    if (i == dt.Rows.Count - 1)
                    {
                        if (j == 0)
                        {
                            cell.Colspan = 3;
                            j += 2;
                            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                        }
                    }

                    table.AddCell(cell);
                }
            }

            //为当前document加入内容
            table.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            return table;
        }

        private static object PartLast(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 75, 25, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("Our bank information for US$ payments:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Beneficiary Bank：BANK OF CHINA (HONG KONG) LIMITED", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Beneficiary Bank Address：", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("bank of china tower 1Garden Road centlal Hong Kong", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("SWIFT Code：BKCHHKHH", new Font(baseFont, 8f, Font.NORMAL)));

            cell1.AddElement(new Phrase("Bank Code：012", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Company Name：Anda International Trade Group Limited", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Company bank account：012-573-2-011526-6", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Comments:Please   inspect   the   above   information   concerned   carefully.If", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("incorrect,please contact me timely.", new Font(baseFont, 8f, Font.NORMAL)));

            cell1.AddElement(new Phrase("1.To refurbished part,we can guarantee for 15 days.", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            Image image11 = Image.GetInstance(System.Configuration.ConfigurationManager.AppSettings["ContentPath"] + @"\Content\Images\Signs\Anda.png");
            image11.ScalePercent(40f);
            var bbbb = new PdfPCell(image11) { VerticalAlignment = Rectangle.ALIGN_BOTTOM, PaddingLeft = 5f, };
            bbbb.DisableBorderSide(15);
            table.AddCell(bbbb);

            return table;
        }






        static DataTable GetData(List<Views.OrderItemSupplierViewModel> items)
        {
            #region datatable数据

            DataTable dt = new DataTable();
            dt.Columns.Add("序号", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("型号", typeof(string));
            dt.Columns.Add("数量", typeof(string));
            dt.Columns.Add("单价", typeof(string));
            dt.Columns.Add("总价", typeof(string));

            DataRow drTitle = dt.NewRow();
            drTitle["序号"] = "NO";
            drTitle["描述"] = "DESCRIPTION";
            drTitle["型号"] = "P/O NO";
            drTitle["数量"] = "QUANTITY";
            drTitle["单价"] = "UNIT PRICE";
            drTitle["总价"] = "AMOUNT";
            dt.Rows.Add(drTitle);

            int index = 0;
            foreach (var entity in items)
            {
                DataRow dr1 = dt.NewRow();
                dr1["序号"] = index + 1;
                dr1["描述"] = "";
                dr1["型号"] = entity.Model;
                dr1["数量"] = entity.Quantity.ToString("0.####");
                dr1["单价"] = entity.UnitPrice.ToRound(4).ToString("0.####");
                dr1["总价"] = entity.TotalPrice.ToRound(2);
                dt.Rows.Add(dr1);

                index++;
            }

            decimal allQuantity = items.Sum(t => t.Quantity);
            decimal allTotalPrice = items.Sum(t => t.TotalPrice);

            DataRow dtLastRow1 = dt.NewRow();
            dtLastRow1["序号"] = "TOTAL:";
            dtLastRow1["描述"] = "";
            dtLastRow1["型号"] = "";
            dtLastRow1["数量"] = allQuantity.ToString("0.####");
            dtLastRow1["单价"] = "";
            dtLastRow1["总价"] = allTotalPrice.ToRound(2);
            dt.Rows.Add(dtLastRow1);

            #endregion

            return dt;
        }
    }
}
