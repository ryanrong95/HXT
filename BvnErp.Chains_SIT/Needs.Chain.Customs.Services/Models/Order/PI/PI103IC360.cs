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
    public class PI103IC360 : PIBuilder
    {
        public PI103IC360(string fileName, List<Views.OrderItemSupplierViewModel> items, string piNo, string clientName) : base(fileName, items, piNo, clientName)
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
            pdfBuilder.AddStep(PartFREIGHT);
            pdfBuilder.AddStep(PartBigMargin2);
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
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 25, 75, });

            //Image image11 = Image.GetInstance(System.Environment.CurrentDirectory + @"\Content\Images\Logos\远大芯城.png");
            Image image11 = Image.GetInstance(System.Configuration.ConfigurationManager.AppSettings["ContentPath"] + @"\Content\Images\Logos\远大芯城.png");
            image11.ScalePercent(25f);
            var bbbb = new PdfPCell(image11) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
            bbbb.DisableBorderSide(15);
            table.AddCell(bbbb);

            var cell2 = new PdfPCell();
            cell2.AddElement(new Phrase("      IC360 ELECTRONICS LIMITED", new Font(baseFont, 18f, Font.NORMAL)));
            cell2.AddElement(new Phrase(" ", new Font(baseFont, 6f, Font.NORMAL)));
            cell2.AddElement(new Phrase("Unit B1, 2/F., Houtex Ind. Bldg., 16 Hung To Rd., Kwun Tong, Kowloon. HongKong", new Font(baseFont, 9f, Font.NORMAL)));
            cell2.AddElement(new Phrase("            Tel:+852 34210474 Fax: +852 34210530 Web: www.ic360.cn", new Font(baseFont, 9f, Font.NORMAL)));
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
            cell2.AddElement(new Phrase("                   COMMERCIAL INVOICE", new Font(baseFont, 18f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            cell2.PaddingBottom = 10;
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
            //cell1.AddElement(new Phrase("BILL TO:" + "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO LIMITED", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("BILL TO:" + clientName, new Font(baseFont, 8f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            var cell2 = new PdfPCell();
            cell2.AddElement(new Phrase("DATE.:", new Font(baseFont, 8f, Font.NORMAL)));
            cell2.AddElement(new Phrase("P/I NO:", new Font(baseFont, 8f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            var cell3 = new PdfPCell();
            cell3.AddElement(new Phrase(DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), new Font(baseFont, 8f, Font.NORMAL)));
            cell3.AddElement(new Phrase(piNo, new Font(baseFont, 8f, Font.NORMAL)));
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
            cell1.AddElement(new Phrase("SHIP TO:" + "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO LIMITED", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            var cell2 = new PdfPCell();
            cell2.AddElement(new Phrase("PO NO:", new Font(baseFont, 8f, Font.NORMAL)));
            cell2.AddElement(new Phrase("PAGE:", new Font(baseFont, 8f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            var cell3 = new PdfPCell();
            cell3.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            cell3.AddElement(new Phrase("1/1", new Font(baseFont, 8f, Font.NORMAL)));
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
            cell2.AddElement(new Phrase("    ATTEN:", new Font(baseFont, 8f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            return table;
        }

        private static object PartFREIGHT(object sender, EventArgs<PdfBuilder> e)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            var cell2 = new PdfPCell();
            cell2.AddElement(new Phrase("  FREIGHT/EXPENSES INCLUDED UP TO:", new Font(baseFont, 8f, Font.NORMAL)));
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            return table;
        }

        private static object PartBigMargin2(object sender, EventArgs<PdfBuilder> e)
        {
            //var entity = (PdfBuilder)e.Entity;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 500; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("  ", new Font(baseFont, 1f, Font.NORMAL)));
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
                    if (i == dt.Rows.Count - 2)
                    {
                        if (j == 0)
                        {
                            cell.Colspan = 3;
                            j += 2;
                            cell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                        }
                    }

                    //合并行
                    if (i == dt.Rows.Count - 1)
                    {
                        if (j == 0)
                        {
                            cell.Colspan = 6;
                            j += 5;
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
            cell1.AddElement(new Phrase("1. PAYMENT TERMS:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("2. BANKING INFORMATION:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("BENEFICIARY NAME: IC360 Electronics Limited", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Beneficiary Account Number：    601-341-8746-1(usd)", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Beneficiary Bank: Wing Lung Bank Limited", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Address：45 Des Voeux Road Central ,HongKong", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("Bank code: 020", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("SWIFT Code：WUBAHKHH", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("3. PACKING: STANDARD CARTON", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("4. BUYER ACCOUNT NO.:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("5. DELIVERY:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("7. REMARKS: PLS CONFIRM THE ORDER WITHIN 2 DAYS", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("IC360 RESERVES THE RIGHT TO CANCEL OR CHANGE THE ORDER 10 DAYS BEFORE SHIPMENT", new Font(baseFont, 8f, Font.NORMAL)));
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
            table.SetWidths(new int[] { 70, 30, });

            var cell1 = new PdfPCell();
            cell1.AddElement(new Phrase("CONFIRMATION BY THE BUYER:              " + "ISSUED BY:", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.AddElement(new Phrase("TO:                         IC360 ELECTRONICS LIMITED", new Font(baseFont, 8f, Font.NORMAL)));
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            Image image11 = Image.GetInstance(System.Configuration.ConfigurationManager.AppSettings["ContentPath"] + @"\Content\Images\Signs\ic360elec.png");
            image11.ScalePercent(40f);
            var bbbb = new PdfPCell(image11) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
            bbbb.DisableBorderSide(15);
            table.AddCell(bbbb);

            return table;
        }





        static DataTable GetData(List<Views.OrderItemSupplierViewModel> items)
        {
            #region datatable数据

            DataTable dt = new DataTable();
            dt.Columns.Add("MARK", typeof(string));
            dt.Columns.Add("序号", typeof(string));
            dt.Columns.Add("型号", typeof(string));
            dt.Columns.Add("数量", typeof(string));
            dt.Columns.Add("单价", typeof(string));
            dt.Columns.Add("总价", typeof(string));

            DataRow drTitle = dt.NewRow();
            drTitle["MARK"] = "DESCRIPTION";
            drTitle["序号"] = "NO";
            drTitle["型号"] = "P/O NO";
            drTitle["数量"] = "QUANTITY";
            drTitle["单价"] = "UNIT PRICE";
            drTitle["总价"] = "AMOUNT";
            dt.Rows.Add(drTitle);

            int index = 0;
            foreach (var entity in items)
            {
                DataRow dr1 = dt.NewRow();
                dr1["MARK"] = "N/M";
                dr1["序号"] = index + 1;
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
            dtLastRow1["MARK"] = "TOTAL AMOUNT OF THE INVOICE";
            dtLastRow1["序号"] = "";
            dtLastRow1["型号"] = "";
            dtLastRow1["数量"] = allQuantity.ToString("0.####");
            dtLastRow1["单价"] = "";
            dtLastRow1["总价"] = allTotalPrice.ToRound(2);
            dt.Rows.Add(dtLastRow1);

            DataRow dtLastRow2 = dt.NewRow();
            dtLastRow2["MARK"] = "SAY TOTAL:";
            dtLastRow2["序号"] = "";
            dtLastRow2["型号"] = "";
            dtLastRow2["数量"] = "";
            dtLastRow2["单价"] = "";
            dtLastRow2["总价"] = "";
            dt.Rows.Add(dtLastRow2);

            #endregion

            return dt;
        }
    }
}
