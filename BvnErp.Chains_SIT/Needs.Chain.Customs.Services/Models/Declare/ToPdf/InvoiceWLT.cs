extern alias globalB;

using globalB::iTextSharp.text;
using globalB::iTextSharp.text.pdf;
using Needs.Utils.Flow.Event;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceWLT
    {

        static DataTable GetData(List<DecProduct> items, string currency, decimal totalQutity, decimal totalPrice, decimal orderTotalPrice)
        {
            #region datatable数据

            DataTable dt = new DataTable();
            dt.Columns.Add("序号", typeof(string));
            dt.Columns.Add("货物名称", typeof(string));
            dt.Columns.Add("货物型号", typeof(string));
            dt.Columns.Add("品牌", typeof(string));
            dt.Columns.Add("产地", typeof(string));
            dt.Columns.Add("数量(PCS)", typeof(string));
            dt.Columns.Add("单价(USD)", typeof(string));
            dt.Columns.Add("总价(USD)", typeof(string));

            DataRow drTitle = dt.NewRow();
            drTitle["序号"] = "NO";
            drTitle["货物名称"] = "DESCRIPTION";
            drTitle["货物型号"] = "PRODUCT NAME";
            drTitle["品牌"] = "BRAND";
            drTitle["产地"] = "MFGR";
            drTitle["数量(PCS)"] = "QTY\r\n(PCS)";
            drTitle["单价(USD)"] = "UNIT PRICE\r\n(" + currency + ")";
            drTitle["总价(USD)"] = "AMOUNT\r\n(" + currency + ")";
            dt.Rows.Add(drTitle);

            int index = 0;
            foreach (var entity in items)
            {
                DataRow dr1 = dt.NewRow();
                dr1["序号"] = index + 1;
                dr1["货物名称"] = entity.Name;
                dr1["货物型号"] = entity.Model;
                dr1["品牌"] = entity.Manufacturer;
                dr1["产地"] = entity.Origin;
                dr1["数量(PCS)"] = entity.Quantity.ToString("0.####");
                dr1["单价(USD)"] = entity.OrderUnitPrice.ToString("0.####");//entity.UnitPrice.ToString("0.####");
                dr1["总价(USD)"] = entity.OrderTotalPrice.ToRound(2);//entity.TotalPrice.ToRound(2);
                dt.Rows.Add(dr1);

                index++;
            }
            #region 增加一行运保杂费 2020-09-02 by yeshuangshuang

            DataRow dtLastRow0 = dt.NewRow();
            dtLastRow0["序号"] = "运保杂费";
            dtLastRow0["货物名称"] = "";
            dtLastRow0["货物型号"] = "";
            dtLastRow0["品牌"] = "";
            dtLastRow0["产地"] = "";
            dtLastRow0["数量(PCS)"] = "";
            dtLastRow0["单价(USD)"] = "";
            dtLastRow0["总价(USD)"] = (totalPrice - orderTotalPrice).ToRound(2);//运保杂费 
            dt.Rows.Add(dtLastRow0);

            #endregion

            DataRow dtLastRow1 = dt.NewRow();
            dtLastRow1["序号"] = "合计";
            dtLastRow1["货物名称"] = "";
            dtLastRow1["货物型号"] = "";
            dtLastRow1["品牌"] = "";
            dtLastRow1["产地"] = "";
            dtLastRow1["数量(PCS)"] = totalQutity.ToString("0.####");
            dtLastRow1["单价(USD)"] = "";
            dtLastRow1["总价(USD)"] = totalPrice.ToRound(2);
            dt.Rows.Add(dtLastRow1);

            //DataRow dtLastRow2 = dt.NewRow();
            //dtLastRow2["序号"] = "成交方式： CIF深圳";
            //dtLastRow2["货物名称"] = "";
            //dtLastRow2["货物型号"] = "";
            //dtLastRow2["品牌"] = "总金额";
            //dtLastRow2["产地"] = "叁仟玖佰零陆元伍角壹分 美元";
            //dtLastRow2["数量(PCS)"] = "";
            //dtLastRow2["单价(USD)"] = "";
            //dtLastRow2["总价(USD)"] = "";
            //dt.Rows.Add(dtLastRow2);

            #endregion

            return dt;
        }

        private Vendor Vendor { get; set; }
        private string FileName { get; set; } = string.Empty;
        private PaymentInstruction PaymentInstruction { get; set; }

        public InvoiceWLT(Vendor vendor, string fileName, PaymentInstruction paymentInstruction)
        {
            this.Vendor = vendor;
            this.FileName = fileName;
            this.PaymentInstruction = paymentInstruction;
        }

        public void Execute()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Part标题及Logo", new { Vendor = this.Vendor, PaymentInstruction = this.PaymentInstruction, });
            parameters.Add("Part公司名称", new { Vendor = this.Vendor, });
            parameters.Add("Part公司地址", new { Vendor = this.Vendor, });
            parameters.Add("PartTOADDRESS", new { PaymentInstruction = this.PaymentInstruction, });
            parameters.Add("Part数据", new { PaymentInstruction = this.PaymentInstruction, });
            parameters.Add("Part成交方式", new { PaymentInstruction = this.PaymentInstruction, });
            parameters.Add("Part印章", new { Vendor = this.Vendor, });
            parameters.Add("Part最后公司名称", new { Vendor = this.Vendor, });

            PdfBuilder pdfBuilder = new PdfBuilder(this.FileName, parameters);
            pdfBuilder.SetDocumentMargins(10f, 10f, 2f, 2f);

            pdfBuilder.AddStep(Part标题及Logo);
            pdfBuilder.AddStep(Part公司名称);
            pdfBuilder.AddStep(Margin4f);
            pdfBuilder.AddStep(Part公司地址);
            pdfBuilder.AddStep(Margin8f);
            pdfBuilder.AddStep(PartTOADDRESS);
            pdfBuilder.AddStep(Margin8f);
            pdfBuilder.AddStep(Part数据);
            pdfBuilder.AddStep(Part成交方式);
            pdfBuilder.AddStep(Part印章);
            pdfBuilder.AddStep(Part最后公司名称);
             
            pdfBuilder.DottedFunNames.Add("PartTOADDRESS");

            pdfBuilder.ToPdf();
        }

        private static object Part标题及Logo(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part标题及Logo"];
            var vendor = (Vendor)param.GetType().GetProperty("Vendor").GetValue(param);
            var paymentInstruction = (PaymentInstruction)param.GetType().GetProperty("PaymentInstruction").GetValue(param);

            string contractNo = paymentInstruction.ContractNo;
            var ddate = DateTime.ParseExact(paymentInstruction.TradeDate, "yyyyMMdd", new System.Globalization.CultureInfo("zh-CN"), System.Globalization.DateTimeStyles.AllowWhiteSpaces);


            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table1 = new PdfPTable(3);
            table1.TotalWidth = 570;
            table1.LockedWidth = true;
            table1.SetWidths(new int[] { 5, 70, 45 });

            var sdf = new PdfPCell(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            sdf.DisableBorderSide(15);
            table1.AddCell(sdf);

            BaseFont baseFont1 = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var titlePara1 = new Paragraph(new Chunk("COMMERCIAL INVOICE", new Font(baseFont1, 22f, Font.BOLD | Font.UNDERLINE)))
            {
                Alignment = Rectangle.ALIGN_CENTER,
            };
            var titlePara2 = new Paragraph(new Chunk("                                 DATE: " + ddate.ToString("yyyy-MM-dd"), new Font(baseFont, 8f, Font.NORMAL)))
            {
                Alignment = Rectangle.ALIGN_LEFT,
            };
            var titlePara3 = new Paragraph(new Chunk("                           INVOICE NO: " + contractNo, new Font(baseFont, 8f, Font.NORMAL)))
            {
                Alignment = Rectangle.ALIGN_LEFT,
            };

            var cellTitle = new PdfPCell()
            {
                PaddingTop = 6f,
                HorizontalAlignment = Rectangle.ALIGN_CENTER,
            };
            cellTitle.AddElement(titlePara1);
            cellTitle.AddElement(new Paragraph(new Chunk(" ", new Font(baseFont, 6f, Font.NORMAL))));
            cellTitle.AddElement(new Paragraph(new Chunk(" ", new Font(baseFont, 6f, Font.NORMAL))));
            cellTitle.AddElement(titlePara2);
            cellTitle.AddElement(titlePara3);
            cellTitle.AddElement(new Paragraph(new Chunk(" ", new Font(baseFont, 6f, Font.NORMAL))));

            cellTitle.DisableBorderSide(15);
            table1.AddCell(cellTitle);


            Image imageLogo = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.LogoUrl));
            imageLogo.ScalePercent(32f);
            var cellLogo = new PdfPCell(imageLogo) { VerticalAlignment = Rectangle.ALIGN_RIGHT, PaddingTop = 10f, PaddingLeft = 84f, };
            cellLogo.DisableBorderSide(15);
            table1.AddCell(cellLogo);

            return table1;
        }

        private static object Part公司名称(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part公司名称"];
            var vendor = (Vendor)param.GetType().GetProperty("Vendor").GetValue(param);

            BaseFont baseFont2 = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Paragraph paragraphTitle1 = new Paragraph(new Chunk(vendor.CompanyName, new Font(baseFont2, 12f, Font.BOLD)));
            paragraphTitle1.Alignment = Rectangle.ALIGN_CENTER;

            return paragraphTitle1;
        }

        private static object Part公司地址(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part公司地址"];
            var vendor = (Vendor)param.GetType().GetProperty("Vendor").GetValue(param);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            Paragraph paragraphTitle2 = new Paragraph(new Chunk(vendor.AddressEN, new Font(baseFont, 8f, Font.NORMAL)));
            paragraphTitle2.Alignment = Rectangle.ALIGN_CENTER;

            return paragraphTitle2;
        }

        private static object PartTOADDRESS(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["PartTOADDRESS"];
            var paymentInstruction = (PaymentInstruction)param.GetType().GetProperty("PaymentInstruction").GetValue(param);

            string agentName = paymentInstruction.Beneficiary.Company.Name;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table2 = new PdfPTable(2);
            table2.TotalWidth = 300; //绝对宽度
            table2.LockedWidth = true;
            table2.SetWidths(new int[] { 20, 80 });

            var cell1 = new PdfPCell(new Phrase("TO:", new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                PaddingTop = 1f,
            };
            cell1.DisableBorderSide(10);
            var cell2 = new PdfPCell(new Phrase(agentName, new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
                PaddingTop = 1f,
            };
            cell2.DisableBorderSide(6);
            var cell3 = new PdfPCell(new Phrase("ADDRESS:", new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                PaddingBottom = 2f,
            };
            cell3.DisableBorderSide(9);
            var cell4 = new PdfPCell(new Phrase(PurchaserContext.Current.Address, new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
                PaddingBottom = 2f,
            };
            cell4.DisableBorderSide(5);



            table2.AddCell(cell1);
            table2.AddCell(cell2);
            table2.AddCell(cell3);
            table2.AddCell(cell4);

            return table2;
        }

        private static object Part数据(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part数据"];
            var paymentInstruction = (PaymentInstruction)param.GetType().GetProperty("PaymentInstruction").GetValue(param);

            var items = paymentInstruction.Items.ToList();
            string currency = paymentInstruction.Currency;
            decimal totalQutity = paymentInstruction.TotalProductQutity;
            decimal totalPrice = paymentInstruction.TotalPrice;
            decimal orderTotalPrice = paymentInstruction.OrderTotalPrice;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            DataTable dt = GetData(items, currency, totalQutity, totalPrice, orderTotalPrice);

            PdfPTable table = new PdfPTable(dt.Columns.Count); //列数
            table.TotalWidth = 574; //绝对宽度
            table.LockedWidth = true;

            //设置行宽
            int[] widths = new int[] { 4, 22, 20, 12, 8, 6, 8, 8 }; //百分比的感觉
            table.SetWidths(widths);

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(dt.Rows[i][j].ToString(), new Font(baseFont, 6f)))
                    {
                        BorderWidth = 0.06f,
                    };

                    //合并行
                    if (i == dt.Rows.Count - 1 || i == dt.Rows.Count - 2)
                    {
                        if (j == 0)
                        {
                            cell.Colspan = 5;
                            j = j + 4;
                        }
                    }

                    //对部分边框隐藏 Begin


                    if (cell.Phrase.Content == "合计" || cell.Phrase.Content == "运保杂费")
                    {
                        cell.DisableBorderSide(8);
                    }
                    else
                    {
                        if (j == 0)
                        {
                            cell.DisableBorderSide(8);
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            cell.DisableBorderSide(4);
                        }
                        else
                        {
                            cell.DisableBorderSide(12);
                        }

                    }

                    //对部分边框隐藏 End

                    table.AddCell(cell);

                }
            }

            //为当前document加入内容
            table.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            return table;
        }

        private static object Part成交方式(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part成交方式"];
            var paymentInstruction = (PaymentInstruction)param.GetType().GetProperty("PaymentInstruction").GetValue(param);

            //取币制的中文
            string CurrencyName = "";
            CurrencyName = new Needs.Ccs.Services.Views.BaseCurrenciesView().Where(item => item.Code == paymentInstruction.Currency).FirstOrDefault()?.Name;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return new Paragraph(new Chunk(" 成交方式: CIF深圳         总金额:     " + paymentInstruction.TotalPrice.ToRound(2).ToChineseAmount() + " " + CurrencyName,
                new Font(baseFont, 8f, Font.NORMAL)));
        }

        private static object Part印章(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part印章"];
            var vendor = (Vendor)param.GetType().GetProperty("Vendor").GetValue(param);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table3 = new PdfPTable(3);
            table3.TotalWidth = 500; //绝对宽度
            table3.LockedWidth = true;
            table3.SetWidths(new int[] { 30, 13, 67, });

            var table3Cell1 = new PdfPCell(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell1.DisableBorderSide(15);
            table3.AddCell(table3Cell1);

            var table3Cell2 = new PdfPCell();
            table3Cell2.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell2.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell2.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell2.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell2.AddElement(new Phrase("Authorized by", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell2.DisableBorderSide(15);
            table3.AddCell(table3Cell2);

            Image image1 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.SealUrl));
            image1.ScalePercent(70f);
            var b = new PdfPCell(image1) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
            b.DisableBorderSide(15);
            table3.AddCell(b);

            return table3;
        }

        private static object Part最后公司名称(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part最后公司名称"];
            var vendor = (Vendor)param.GetType().GetProperty("Vendor").GetValue(param);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return new Paragraph(new Chunk("                                            " + vendor.CompanyName,
                new Font(baseFont, 8f, Font.NORMAL)));
        }








        private static object Margin2f(object sender, EventArgs<PdfBuilder> e)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return new Chunk(" ", new Font(baseFont, 2f, Font.NORMAL));
        }

        private static object Margin4f(object sender, EventArgs<PdfBuilder> e)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return new Chunk(" ", new Font(baseFont, 4f, Font.NORMAL));
        }

        private static object Margin6f(object sender, EventArgs<PdfBuilder> e)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return new Chunk(" ", new Font(baseFont, 6f, Font.NORMAL));
        }

        private static object Margin8f(object sender, EventArgs<PdfBuilder> e)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return new Chunk(" ", new Font(baseFont, 8f, Font.NORMAL));
        }

    }
}
