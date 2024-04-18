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
    /// <summary>
    /// 合同(畅运)
    /// </summary>
    public class ContractCY
    {

        static DataTable GetData(List<DecProduct> items, string currency, string 大写金额, decimal totalPrice, decimal orderTotalPrice)
        {
            #region datatable数据
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            dt.Columns.Add("UnitPrice", typeof(string));
            dt.Columns.Add("TotalPrice", typeof(string));

            DataRow drTitle = dt.NewRow();
            drTitle["NO"] = "序号";
            drTitle["Name"] = "商品名称";
            drTitle["Model"] = "货物型号";
            drTitle["Qty"] = "数量(PCS)";
            drTitle["UnitPrice"] = "单价(" + " " + currency + "/ PCS)";
            drTitle["TotalPrice"] = "总价(" + currency + ")";
            dt.Rows.Add(drTitle);

            int index = 0;
            foreach (var entity in items)
            {
                DataRow dr1 = dt.NewRow();
                dr1["NO"] = index + 1;
                dr1["Name"] = entity.Name;
                dr1["Model"] = entity.Model == null ? "" : entity.Model;
                dr1["Qty"] = entity.Quantity.ToString("0.####");
                dr1["UnitPrice"] = entity.OrderUnitPrice.ToRound(4).ToString("0.####");
                dr1["TotalPrice"] = entity.OrderTotalPrice.ToRound(2);
                dt.Rows.Add(dr1);

                index++;
            }
            #region  新增一行运保杂费 2020-09-03 by yeshuangshuang 

            DataRow dtLastRow0 = dt.NewRow();
            dtLastRow0["NO"] = "运保杂费";
            dtLastRow0["Name"] = "";
            dtLastRow0["Model"] = "";
            dtLastRow0["Qty"] = "";
            dtLastRow0["UnitPrice"] = "";
            dtLastRow0["TotalPrice"] = (totalPrice - orderTotalPrice).ToRound(2);
            dt.Rows.Add(dtLastRow0);

            #endregion

            DataRow dtLastRow = dt.NewRow();
            dtLastRow["NO"] = "合同金额(大写):";
            dtLastRow["Name"] = "";
            dtLastRow["Model"] = 大写金额;
            dtLastRow["Qty"] = "";
            dtLastRow["UnitPrice"] = "";
            dtLastRow["TotalPrice"] = totalPrice.ToRound(2);
            dt.Rows.Add(dtLastRow);
            #endregion

            return dt;
        }

        private Vendor Vendor { get; set; }
        private string FileName { get; set; } = string.Empty;
        private Contract Contract { get; set; }

        public ContractCY(Vendor vendor, string fileName, Contract contract)
        {
            this.Vendor = vendor;
            this.FileName = fileName;
            this.Contract = contract;
        }

        public void Execute()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Part标题及Logo", new { Vendor = this.Vendor, });
            parameters.Add("Part卖方买方", new { Vendor = this.Vendor, Contract = this.Contract, });
            parameters.Add("Part数据", new { Contract = this.Contract, });
            parameters.Add("Part签名盖章", new { Vendor = this.Vendor, });

            PdfBuilder pdfBuilder = new PdfBuilder(this.FileName, parameters);
            pdfBuilder.SetDocumentMargins(10f, 10f, 2f, 2f);

            pdfBuilder.AddStep(Part标题及Logo);
            pdfBuilder.AddStep(Part大标题);
            pdfBuilder.AddStep(Part卖方买方);
            pdfBuilder.AddStep(Part数据);
            pdfBuilder.AddStep(Part签名盖章);

            pdfBuilder.ToPdf();
        }

        private static object Part标题及Logo(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part标题及Logo"];
            var vendor = (Vendor)param.GetType().GetProperty("Vendor").GetValue(param);


            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 570;
            table.LockedWidth = true;
            table.SetWidths(new int[] { 20, 80 });

            BaseFont baseFont1 = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            Image imageLogo = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.LogoUrl));
            imageLogo.ScalePercent(20f);
            var cellLogo = new PdfPCell(imageLogo) { VerticalAlignment = Rectangle.ALIGN_LEFT, PaddingTop = 1f, PaddingLeft = 20f, };
            cellLogo.DisableBorderSide(15);
            table.AddCell(cellLogo);


            var cellTitle = new PdfPCell()
            {
                PaddingTop = 4f,
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
            };
            var titlePara1 = new Paragraph(new Chunk(vendor.CompanyName, new Font(baseFont1, 12f, Font.BOLD)))
            {
                Alignment = Rectangle.ALIGN_LEFT,
            };
            var titlePara2 = new Paragraph(new Chunk(vendor.AddressEN, new Font(baseFont, 8f, Font.NORMAL)))
            {
                Alignment = Rectangle.ALIGN_LEFT,
            };
            cellTitle.AddElement(titlePara1);
            cellTitle.AddElement(new Paragraph(new Chunk(" ", new Font(baseFont, 6f, Font.NORMAL))));
            cellTitle.AddElement(titlePara2);
            cellTitle.DisableBorderSide(15);
            table.AddCell(cellTitle);

            return table;
        }

        private static object Part大标题(object sender, EventArgs<PdfBuilder> e)
        {
            BaseFont baseFont2 = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Paragraph paragraph = new Paragraph(new Chunk("SALES CONTRACT", new Font(baseFont2, 18f, Font.BOLD)));
            paragraph.Alignment = Rectangle.ALIGN_CENTER;

            return paragraph;
        }

        private static object Part卖方买方(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part卖方买方"];
            var vendor = (Vendor)param.GetType().GetProperty("Vendor").GetValue(param);
            var contract = (Contract)param.GetType().GetProperty("Contract").GetValue(param);



            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 570; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 15, 85 });

            var cell1 = new PdfPCell(new Phrase("DATE:", new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                PaddingTop = 6f,
            };
            cell1.DisableBorderSide(15);
            var cell2 = new PdfPCell(new Phrase(contract.DDate, new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
                PaddingLeft = 5f,
                PaddingTop = 6f,
            };
            cell2.DisableBorderSide(15);
            var cell3 = new PdfPCell(new Phrase("CONTRACT NO :", new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
            };
            cell3.DisableBorderSide(15);
            var cell4 = new PdfPCell(new Phrase(contract.ContractNo, new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
                PaddingLeft = 5f,
            };
            cell4.DisableBorderSide(15);
            var cell5 = new PdfPCell(new Phrase("SELLER:", new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
            };
            cell5.DisableBorderSide(15);
            var cell6 = new PdfPCell(new Phrase(vendor.CompanyName, new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
                PaddingLeft = 5f,
            };
            cell6.DisableBorderSide(15);
            var cell7 = new PdfPCell(new Phrase("PURCHASER :", new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                PaddingBottom = 6f,
            };
            cell7.DisableBorderSide(15);
            var cell8 = new PdfPCell(new Phrase(PurchaserContext.Current.DomesticConsigneeEname.ToUpper(), new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
                PaddingLeft = 5f,
                PaddingBottom = 6f,
            };
            cell8.DisableBorderSide(15);


            table.AddCell(cell1);
            table.AddCell(cell2);
            table.AddCell(cell3);
            table.AddCell(cell4);
            table.AddCell(cell5);
            table.AddCell(cell6);
            table.AddCell(cell7);
            table.AddCell(cell8);

            return table;
        }

        private static object Part数据(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part数据"];
            var contract = (Contract)param.GetType().GetProperty("Contract").GetValue(param);

            var items = contract.Items.ToList();
            string currency = contract.Currency;

            //取币制的中文
            string CurrencyName = "";
            CurrencyName = new Needs.Ccs.Services.Views.BaseCurrenciesView().Where(item => item.Code == currency).FirstOrDefault()?.Name;
            string 大写金额 = contract.TotalPrice.ToRound(2).ToChineseAmount() + " " + CurrencyName;

            decimal totalPrice = contract.TotalPrice;
            decimal orderTotalPrice = contract.OrderTotalPrice;

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            DataTable dt = GetData(items, currency, 大写金额, totalPrice, orderTotalPrice);

            //DataTable 中增加 Begin
            StringBuilder sb1 = new StringBuilder();
            sb1.Append(@"经买卖双方同意，按以下条款成交");
            sb1.Append("\r\n");
            sb1.Append(@"1、产品的名称，数量，型号，单价，总价");

            string[] strs = new string[]
            {
                "2、成交方式：CIF深圳",
                "3、包装方式：22",
                "4、装运口岸和目的地：HKG000、深圳",
                "5、质量要求：进口产品必须符合中国国家标准或者行业标准，符合双方商定的技术要求。",
                "6、产品的交（提）货期限：合同生效后15天内交（提）货。",
                "7、结算方式：电汇。",
                "8、对产品提出异议的时间和办法：买方在验收中，发现产品的型号、规格、质量不合格，应一面妥善保管，一面在30天内向卖方提出书面异议；卖方在接到买方书面异议后，需在10天内处理。买方签收货物超过30天未提出异议的，视为产品合格。对处理结果有异议，需要索赔的，索赔期限为90天。",
                "9、违约责任：卖方未能按期交货的，承担货款110%的违约金；买方未能按时付款的，承担全部货款5%滞纳金。",
                "10、不可抗力：任何一方由于不可抗力的原因不能履行合同的，应及时告知对方，并免于承担违约责任。",
                "11、其他:本协议履行过程中如发生争议，双方应友好协商解决。如协商不成的，可通过法律途径解决。",
            };

            StringBuilder sb2 = new StringBuilder();
            foreach (var str in strs)
            {
                sb2.Append(str);
                sb2.Append("\r\n");
            }


            DataRow row1 = dt.NewRow();
            row1["NO"] = sb1.ToString();
            row1["Name"] = "";
            row1["Model"] = "";
            row1["Qty"] = "";
            row1["UnitPrice"] = "";
            row1["TotalPrice"] = "";
            dt.Rows.InsertAt(row1, 0);

            DataRow row2 = dt.NewRow();
            row2["NO"] = sb2.ToString();
            row2["Name"] = "";
            row2["Model"] = "";
            row2["Qty"] = "";
            row2["UnitPrice"] = "";
            row2["TotalPrice"] = "";
            dt.Rows.InsertAt(row2, dt.Rows.Count);

            //DataTable 中增加 End

            PdfPTable table = new PdfPTable(dt.Columns.Count); //列数
            table.TotalWidth = 574; //绝对宽度
            table.LockedWidth = true;

            //设置行宽
            int[] widths = new int[] { 4, 34, 32, 8, 10, 12 }; //百分比的感觉
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
                    if (i == 0 || i == dt.Rows.Count - 1)
                    {
                        if (j == 0)
                        {
                            cell.Colspan = 6;
                            j += 5;
                        }
                    }

                    if (i == dt.Rows.Count - 2)
                    {
                        if (j == 0)
                        {
                            cell.Colspan = 2;
                            j++;
                        }
                        else if (j == 2)
                        {
                            cell.Colspan = 3;
                            j += 2;
                        }
                    }
                    if (i == dt.Rows.Count - 3)
                    {
                        if (j == 0)
                        {
                            cell.Colspan = 4;
                            j += 3;
                        }
                    }

                    //对部分边框隐藏 Begin

                    if (cell.Phrase.Content.Contains("合同金额(大写)"))
                    {
                        //cell.DisableBorderSide(8);
                    }
                    else if (cell.Phrase.Content.Contains("经买卖双方同意"))
                    {
                        //cell.DisableBorderSide(2);
                        if (dt.Rows.Count < 50)
                        {
                            cell.SetLeading(1, (float)(1 + (50 - dt.Rows.Count) * 0.02));
                        }
                    }
                    else if (cell.Phrase.Content.Contains("本协议履行过程中如发生争议"))
                    {
                        //cell.DisableBorderSide(1);
                        cell.SetLeading(1, (float)(1 + (50 - dt.Rows.Count) * 0.02));
                    }

                    cell.DisableBorderSide(12);

                    //else
                    //{
                    //    if (j == 0)
                    //    {
                    //        cell.DisableBorderSide(8);
                    //    }
                    //    else if (j == dt.Columns.Count - 1)
                    //    {
                    //        cell.DisableBorderSide(4);
                    //    }
                    //    else
                    //    {
                    //        cell.DisableBorderSide(12);
                    //    }

                    //}

                    //对部分边框隐藏 End

                    table.AddCell(cell);

                }
            }

            //为当前document加入内容
            table.HorizontalAlignment = Rectangle.ALIGN_CENTER;

            return table;
        }

        private static object Part签名盖章(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part签名盖章"];
            var vendor = (Vendor)param.GetType().GetProperty("Vendor").GetValue(param);


            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(4);
            table.TotalWidth = 574; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 10, 50, 10, 30 });

            //1
            var a = new PdfPCell(new Phrase("签名盖章", new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                VerticalAlignment = Rectangle.ALIGN_MIDDLE,
            };
            a.DisableBorderSide(15);
            table.AddCell(a);

            //2
            Image image1 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.SealUrl));
            //同比例缩放
            //float resizeWidth = image.Width;
            //float resizeHeight = image.Height;
            image1.ScalePercent(80f);
            var b = new PdfPCell(image1) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
            b.DisableBorderSide(15);
            table.AddCell(b);

            //3
            var c = new PdfPCell(new Phrase("签名盖章", new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                VerticalAlignment = Rectangle.ALIGN_MIDDLE,
            };
            c.DisableBorderSide(15);
            table.AddCell(c);

            //4
            Image image2 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.SealUrl));
            image2.ScalePercent(80f);
            var d = new PdfPCell(image2) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
            d.DisableBorderSide(15);
            table.AddCell(d);

            return table;
        }


    }
}
