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
    public class PI114CY : PIBuilder
    {
        public PI114CY(string fileName, List<Views.OrderItemSupplierViewModel> items, string piNo, string clientName) : base(fileName, items, piNo, clientName)
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
            pdfBuilder.AddStep(PartTitle2);
            pdfBuilder.AddStep(PartBillTo);
            pdfBuilder.AddStep(PartShipTo);
            pdfBuilder.AddStep(PartATTEN);
            pdfBuilder.AddStep(PartData);
            pdfBuilder.AddStep(PartLast1);
            pdfBuilder.AddStep(PartLast2);

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
            table.TotalWidth = 550; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 20, 80, });

            Image image11 = Image.GetInstance(System.Configuration.ConfigurationManager.AppSettings["ContentPath"] + @"\Content\Images\Logos\畅运.png");
            image11.ScalePercent(20f);
            var bbbb = new PdfPCell(image11) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
            bbbb.DisableBorderSide(15);
            table.AddCell(bbbb);

            var cell2 = new PdfPCell();
            cell2.AddElement(new Phrase("HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO., LIMITED", new Font(baseFont, 14f, Font.NORMAL)));
            cell2.AddElement(new Phrase(" ", new Font(baseFont, 4f, Font.NORMAL)));
            cell2.AddElement(new Phrase("                香港暢運國際物流有限公司", new Font(baseFont, 14f, Font.NORMAL)));
            cell2.AddElement(new Phrase(" ", new Font(baseFont, 4f, Font.NORMAL)));
            cell2.AddElement(new Phrase("              Unit B, 2/F.,Houtex Ind. Bldg., 16 Hung To Rd., Kwun Tong, Kowloon", new Font(baseFont, 8f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            return table;
        }

        private static object PartTitle2(object sender, EventArgs<PdfBuilder> e)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            var cell2 = new PdfPCell();
            cell2.AddElement(new Phrase("                    COMMERCIAL INVOICE", new Font(baseFont, 17f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            return table;
        }

        private static object PartBillTo(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["PINO"];
            string piNo = (string)param.GetType().GetProperty("PINO").GetValue(param);

            object client = e.Entity.Params["ClientName"];
            string clientName = (string)client.GetType().GetProperty("ClientName").GetValue(client);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 70, 15, 15, });

            var cell1 = new PdfPCell();
            //.AddElement(new Phrase("BILL TO:" + "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO LIMITED", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.AddElement(new Phrase("BILL TO:" + clientName, new Font(baseFont, 10f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            var cell2 = new PdfPCell();
            cell2.AddElement(new Phrase("DATE.:", new Font(baseFont, 10f, Font.NORMAL)));
            cell2.AddElement(new Phrase("P/I NO:", new Font(baseFont, 10f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            var cell3 = new PdfPCell();
            cell3.AddElement(new Phrase(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), new Font(baseFont, 10f, Font.NORMAL)));
            cell3.AddElement(new Phrase(piNo, new Font(baseFont, 10f, Font.NORMAL)));
            cell3.DisableBorderSide(15);
            table.AddCell(cell3);

            return table;
        }

        private static object PartShipTo(object sender, EventArgs<PdfBuilder> e)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 70, 15, 15, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("SHIP TO:" + "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO LIMITED", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            var cell2 = new PdfPCell();
            cell2.AddElement(new Phrase("PO NO.:", new Font(baseFont, 10f, Font.NORMAL)));
            cell2.AddElement(new Phrase("PAGE:", new Font(baseFont, 10f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            var cell3 = new PdfPCell();
            cell3.AddElement(new Phrase(" ", new Font(baseFont, 10f, Font.NORMAL)));
            cell3.AddElement(new Phrase(" ", new Font(baseFont, 10f, Font.NORMAL)));
            cell3.DisableBorderSide(15);
            table.AddCell(cell3);

            return table;
        }

        private static object PartATTEN(object sender, EventArgs<PdfBuilder> e)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            var cell2 = new PdfPCell();
            cell2.AddElement(new Phrase("ATTEN:", new Font(baseFont, 10f, Font.NORMAL)));
            cell2.AddElement(new Phrase("SHIPPED FROM: HK", new Font(baseFont, 10f, Font.NORMAL)));
            cell2.AddElement(new Phrase("FREIGHT/EXPENSES INCLUDED UP TO: EXW HK WAREHOUSE", new Font(baseFont, 10f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            cell2.PaddingBottom = 5;
            table.AddCell(cell2);

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

        private static object PartLast1(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("Beneficiary Bank：    " + "BANK OF CHINA (HONG KONG) LIMITED", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Beneficiary Bank Address：", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            return table;
        }

        private static object PartLast2(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 75, 25, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("bank of china tower 1Garden Road centlal Hong Kong", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Swift Code：   " + "BKCHHKHH", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Bank Code：      " + "012", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Company Name：  " + "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO., LIMITED", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Company bank account：   " + "012- 591-2004496-8", new Font(baseFont, 10f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            Image image11 = Image.GetInstance(System.Configuration.ConfigurationManager.AppSettings["ContentPath"] + @"\Content\Images\Signs\ChangYun.png");
            image11.ScalePercent(40f);
            var bbbb = new PdfPCell(image11) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
            bbbb.DisableBorderSide(15);
            bbbb.PaddingTop = 15;
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
