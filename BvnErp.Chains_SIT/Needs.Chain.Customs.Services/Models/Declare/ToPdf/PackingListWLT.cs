extern alias globalB;

using globalB::iTextSharp.text;
using globalB::iTextSharp.text.pdf;
using Needs.Ccs.Services.Models.Declare.PackingData;
using Needs.Utils.Flow.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PackingListWLT
    {
        private string 公司名 { get; set; } = string.Empty;
        private string 致 { get; set; } = string.Empty;
        private string 日期 { get; set; } = string.Empty;
        private string 地址 { get; set; } = string.Empty;
        private string 合同编号 { get; set; } = string.Empty;

        private string 总件数 { get; set; } = string.Empty;
        private string 总数量 { get; set; } = string.Empty;
        private string 总净重 { get; set; } = string.Empty;
        private string 总毛重 { get; set; } = string.Empty;
        private List<DecProduct> Items { get; set; }
        private string FileName { get; set; } = string.Empty;

        private Vendor Vendor { get; set; }

        public PackingListWLT(string 公司名, string 致, string 日期, string 地址, string 合同编号,
                              string 总件数, string 总数量, string 总净重, string 总毛重, List<DecProduct> items,
                              Vendor vendor, string fileName)
        {
            this.公司名 = 公司名;
            this.致 = 致;
            this.日期 = 日期;
            this.地址 = 地址;
            this.合同编号 = 合同编号;

            this.总件数 = 总件数;
            this.总数量 = 总数量;
            this.总净重 = 总净重;
            this.总毛重 = 总毛重;
            this.Items = items;

            this.Vendor = vendor;
            this.FileName = fileName;
        }

        public void Execute()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Part标题及Logo", new { Vendor = this.Vendor, 日期 = this.日期, 合同编号 = this.合同编号, });
            parameters.Add("Part公司名称", new { Vendor = this.Vendor, });
            parameters.Add("PartTOADDRESS", new { 致 = this.致, });
            parameters.Add("Part数据", new
            {
                Items = this.Items,
                数量 = this.总数量,
                净重 = this.总净重,
                毛重 = this.总毛重,
            });

            parameters.Add("Part签章", new
            {
                Vendor = this.Vendor,
                总件数 = this.总件数,
                总数量 = this.总数量,
                总净重 = this.总净重,
                总毛重 = this.总毛重,
            });

            PdfBuilder pdfBuilder = new PdfBuilder(this.FileName, parameters);
            pdfBuilder.SetDocumentMargins(10f, 10f, 2f, 2f);

            pdfBuilder.AddStep(Part标题及Logo);
            pdfBuilder.AddStep(Part公司名称);
            pdfBuilder.AddStep(Part间隔);
            pdfBuilder.AddStep(PartTOADDRESS);
            pdfBuilder.AddStep(Part间隔);
            pdfBuilder.AddStep(Part数据);
            pdfBuilder.AddStep(Part签章);



            pdfBuilder.DottedFunNames.Add("PartTOADDRESS");

            pdfBuilder.ToPdf();
        }

        private static object Part标题及Logo(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part标题及Logo"];
            var vendor = (Vendor)param.GetType().GetProperty("Vendor").GetValue(param);
            var 日期 = (string)param.GetType().GetProperty("日期").GetValue(param);
            var 合同编号 = (string)param.GetType().GetProperty("合同编号").GetValue(param);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table1 = new PdfPTable(3);
            table1.TotalWidth = 570;
            table1.LockedWidth = true;
            table1.SetWidths(new int[] { 5, 70, 45 });

            var sdf = new PdfPCell(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            sdf.DisableBorderSide(15);
            table1.AddCell(sdf);

            BaseFont baseFont1 = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var titlePara1 = new Paragraph(new Chunk("PACKING LIST", new Font(baseFont1, 22f, Font.BOLD | Font.UNDERLINE)))
            {
                Alignment = Rectangle.ALIGN_CENTER,
            };
            var titlePara2 = new Paragraph(new Chunk("                                 DATE: " + 日期, new Font(baseFont, 8f, Font.NORMAL)))
            {
                Alignment = Rectangle.ALIGN_LEFT,
            };
            var titlePara3 = new Paragraph(new Chunk("                           INVOICE NO: " + 合同编号, new Font(baseFont, 8f, Font.NORMAL)))
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

        private static object Part间隔(object sender, EventArgs<PdfBuilder> e)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 574; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 100 });

            var cell1 = new PdfPCell(new Phrase(" ", new Font(baseFont, 4f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
            };
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);
            return table;
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

        private static object PartTOADDRESS(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["PartTOADDRESS"];
            var 致 = (string)param.GetType().GetProperty("致").GetValue(param);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 300; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 20, 80 });

            var cell1 = new PdfPCell(new Phrase("TO:", new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                PaddingTop = 2f,
            };
            cell1.DisableBorderSide(10);
            var cell2 = new PdfPCell(new Phrase(致, new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
                PaddingTop = 2f,
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

            table.AddCell(cell1);
            table.AddCell(cell2);
            table.AddCell(cell3);
            table.AddCell(cell4);

            return table;
        }

        private static object Part数据(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part数据"];
            var items = (List<DecProduct>)param.GetType().GetProperty("Items").GetValue(param);
            var 数量 = (string)param.GetType().GetProperty("数量").GetValue(param);
            var 净重 = (string)param.GetType().GetProperty("净重").GetValue(param);
            var 毛重 = (string)param.GetType().GetProperty("毛重").GetValue(param);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            float TableFontSize = 7f;

            //1. Title
            TitleDataTable titleDataTable = new TitleDataTable();

            PdfPTable table = new PdfPTable(titleDataTable.ColumnsCount); //列数
            table.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            table.TotalWidth = 574; //绝对宽度
            table.LockedWidth = true;

            //设置行宽
            int[] widths = new int[] { 6, 4, 22, 20, 12, 6, 8, 8 }; //百分比的感觉
            table.SetWidths(widths);

            for (int j = 0; j < titleDataTable.ColumnsCount; j++)
            {
                PdfPCell cell = new PdfPCell(new Phrase(titleDataTable.Rows[0][j].ToString(), new Font(baseFont, TableFontSize)))
                {
                    BorderWidth = 0.01f,
                    //BorderColor = new BaseColor(0, 139, 139),
                };
                if (j == 0 || j == 7)
                {
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                }
                table.AddCell(cell);
            }

            //2. Body
            BodyDataTable bodyDataTable = new BodyDataTable(items);

            //检查纵向合并单元格信息 Begin
            List<RowspanInfo> rowspanInfos = new List<RowspanInfo>();
            string flagXiangHao = (string)bodyDataTable.Rows[0]["箱号"];
            int flagNo = 1;

            int lastTwoBeginNo = 0;
            int lastTwoRowSpanCount = 0;

            for (int i = 0; i < bodyDataTable.Rows.Count; i++)
            {
                string currentXiangHao = (string)bodyDataTable.Rows[i]["箱号"];
                string strCurrentNo = (string)bodyDataTable.Rows[i]["序号"];
                int currentNo = Convert.ToInt32(strCurrentNo);

                if (i == bodyDataTable.Rows.Count - 1)
                {
                    rowspanInfos.Add(new RowspanInfo()
                    {
                        BeginNo = flagNo,
                        RowSpanCount = currentNo - flagNo,
                        BeginBox = currentXiangHao,
                    });

                    lastTwoBeginNo = flagNo;
                    lastTwoRowSpanCount = currentNo - flagNo;

                    break;
                }

                if (flagXiangHao != currentXiangHao)
                {
                    rowspanInfos.Add(new RowspanInfo()
                    {
                        BeginNo = flagNo,
                        RowSpanCount = currentNo - flagNo,
                        BeginBox = currentXiangHao,
                    });

                    flagXiangHao = currentXiangHao;
                    flagNo = currentNo;
                }
            }
            //检查纵向合并单元格信息 End

            //补最后一个 RowspanInfo
            //lastTwoBeginNo    4
            //lastTwoRowSpanCount   1


            var maybeRowspanInfo = new RowspanInfo()
            {
                BeginNo = lastTwoBeginNo + lastTwoRowSpanCount,
                RowSpanCount = 1 + bodyDataTable.Rows.Count - (lastTwoBeginNo + lastTwoRowSpanCount),
                BeginBox = (string)bodyDataTable.Rows[bodyDataTable.Rows.Count - 1]["箱号"],
            };

            string sdfd = (string)bodyDataTable.Rows[bodyDataTable.Rows.Count - 1]["箱号"];

            //if (maybeRowspanInfo.BeginBox != rowspanInfos.LastOrDefault().BeginBox)

            //{
            //    rowspanInfos.Add(maybeRowspanInfo);
            //}
            //else
            //{
            //    //rowspanInfos.Add(maybeRowspanInfo);

            //    //maybeRowspanInfo 的箱号 和 rowspanInfos 最后一个箱号一样
            //    var last = rowspanInfos.LastOrDefault();
            //    last.RowSpanCount = last.RowSpanCount + 1;

            //    rowspanInfos.RemoveAt(rowspanInfos.Count - 1);
            //    rowspanInfos.Add(last);
            //}


            if (bodyDataTable.Rows.Count > 2 && (string)bodyDataTable.Rows[bodyDataTable.Rows.Count - 1]["箱号"] == (string)bodyDataTable.Rows[bodyDataTable.Rows.Count - 2]["箱号"])
            {
                //maybeRowspanInfo 的箱号 和 rowspanInfos 最后一个箱号一样
                var last = rowspanInfos.LastOrDefault();
                last.RowSpanCount = last.RowSpanCount + 1;

                rowspanInfos.RemoveAt(rowspanInfos.Count - 1);
                rowspanInfos.Add(last);
            }
            else
            {
                rowspanInfos.Add(maybeRowspanInfo);
            }




            rowspanInfos = rowspanInfos.Where(t => t.RowSpanCount != 0).ToList();
            List<int> nos = rowspanInfos.Select(t => t.BeginNo).ToList();

            for (int i = 0; i < bodyDataTable.Rows.Count; i++)
            {
                string strCurrentNo = (string)bodyDataTable.Rows[i]["序号"];
                int currentNo = int.Parse(strCurrentNo);

                for (int j = 0; j < bodyDataTable.ColumnsCount; j++)
                {
                    if ((j == 0) && !nos.Contains(currentNo))
                    {
                        continue;
                    }

                    PdfPCell cell = new PdfPCell(new Phrase(bodyDataTable.Rows[i][j].ToString(), new Font(baseFont, TableFontSize)))
                    {
                        BorderWidth = 0.01f,
                        //BorderColor = new BaseColor(0, 139, 139),
                    };

                    if (j == 0 || j == 7)
                    {
                        cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    }

                    if ((j == 0) && nos.Contains(currentNo))
                    {
                        //合并列
                        if (nos.Contains(currentNo))
                        {
                            var ads = rowspanInfos.Where(t => t.BeginNo == currentNo).FirstOrDefault();
                            cell.Rowspan = ads.RowSpanCount;
                        }
                    }

                    table.AddCell(cell);
                }
            }


            //3. Footer
            FooterDataTable footerDataTable = new FooterDataTable(数量, 净重, 毛重);
            for (int j = 0; j < footerDataTable.ColumnsCount; j++)
            {
                PdfPCell cell = new PdfPCell(new Phrase(footerDataTable.Rows[0][j].ToString(), new Font(baseFont, TableFontSize)))
                {
                    BorderWidth = 0.01f,
                    //BorderColor = new BaseColor(0, 139, 139),
                };

                if (j == 0)
                {
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                    cell.Colspan = 5;
                    j = j + 4;
                }
                if (j == 0 || j == 7)
                {
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                }
                table.AddCell(cell);
            }

            return table;
        }

        private static object Part签章(object sender, EventArgs<PdfBuilder> e)
        {
            object param = e.Entity.Params["Part签章"];
            var vendor = (Vendor)param.GetType().GetProperty("Vendor").GetValue(param);
            string 总件数 = (string)param.GetType().GetProperty("总件数").GetValue(param);
            string 总数量 = (string)param.GetType().GetProperty("总数量").GetValue(param);
            string 总净重 = (string)param.GetType().GetProperty("总净重").GetValue(param);
            string 总毛重 = (string)param.GetType().GetProperty("总毛重").GetValue(param);

            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 574; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 30, 70 });

            string[] str1 = new string[]
            {
                "总件数: " + 总件数,
                "总数量: " + 总数量,
                "总净重: " + 总净重,
                "总毛重: " + 总毛重,
            };
            StringBuilder sb1 = new StringBuilder();
            foreach (var str in str1)
            {
                sb1.Append(str);
                sb1.Append("\r\n");
            }

            var cell1 = new PdfPCell(new Phrase(sb1.ToString(), new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
                PaddingLeft = 50f,
                PaddingTop = 10f,
            };
            cell1.DisableBorderSide(15);


            //cell2 内容 Begin

            PdfPTable table3 = new PdfPTable(2);
            table3.TotalWidth = 400; //绝对宽度
            table3.LockedWidth = true;
            table3.SetWidths(new int[] { 20, 80, });

            var table3Cell2 = new PdfPCell();
            table3Cell2.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell2.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell2.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell2.AddElement(new Phrase(" ", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell2.AddElement(new Phrase("Authorized by", new Font(baseFont, 8f, Font.NORMAL)));
            table3Cell2.DisableBorderSide(15);
            table3Cell2.PaddingTop = 10f;
            table3Cell2.PaddingLeft = 15f;
            table3.AddCell(table3Cell2);

            Image image1 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.SealUrl));
            image1.ScalePercent(70f);
            var b = new PdfPCell(image1) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
            b.DisableBorderSide(15);
            b.PaddingTop = 10f;
            table3.AddCell(b);

            //cell2 内容 End

            var cell2 = new PdfPCell();
            cell2.AddElement(table3);
            cell2.AddElement(new Paragraph(new Chunk("   " + vendor.CompanyName, new Font(baseFont, 8f, Font.NORMAL))));
            cell2.DisableBorderSide(15);

            table.AddCell(cell1);
            table.AddCell(cell2);

            return table;
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
