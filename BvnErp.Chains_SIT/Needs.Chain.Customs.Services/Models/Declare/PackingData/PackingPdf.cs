extern alias globalB;
using globalB::iTextSharp.text;
using globalB::iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.Declare.PackingData
{
    public class PackingPdf
    {
        private string _公司名 = string.Empty;
        private string _致 = string.Empty;
        private string _日期 = string.Empty;
        private string _地址 = string.Empty;
        private string _合同编号 = string.Empty;

        private string _总件数 = string.Empty;
        private string _总数量 = string.Empty;
        private string _总净重 = string.Empty;
        private string _总毛重 = string.Empty;

        private IEnumerable<DecProduct> _items;

        public PackingPdf(string 公司名, string 致, string 日期, string 地址, string 合同编号,
            string 总件数, string 总数量, string 总净重, string 总毛重,
            IEnumerable<DecProduct> items)
        {
            this._公司名 = 公司名;
            this._致 = 致;
            this._日期 = 日期;
            this._地址 = 地址;
            this._合同编号 = 合同编号;

            this._总件数 = 总件数;
            this._总数量 = 总数量;
            this._总净重 = 总净重;
            this._总毛重 = 总毛重;

            this._items = items;
        }

        Document document = new Document(new Rectangle(PageSize.A4));
        BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        float TableFontSize = 7f;

        private Paragraph AddTitle1()
        {
            Paragraph paragraphTitle1 = new Paragraph(new Chunk(this._公司名, new Font(baseFont, 12f, Font.NORMAL)));
            paragraphTitle1.Alignment = Rectangle.ALIGN_CENTER;
            return paragraphTitle1;
        }

        private Paragraph AddTitle2()
        {
            Paragraph paragraph = new Paragraph(new Chunk(" ", new Font(baseFont, 8f, Font.NORMAL)));
            paragraph.Alignment = Rectangle.ALIGN_LEFT;
            paragraph.Leading = 8f;
            return paragraph;
        }

        private Paragraph AddTitle3()
        {
            Paragraph paragraph = new Paragraph(new Chunk("装 箱 单", new Font(baseFont, 12f, Font.NORMAL)));
            paragraph.Alignment = Rectangle.ALIGN_CENTER;
            return paragraph;
        }

        private Paragraph AddTitle4()
        {
            Paragraph paragraph = new Paragraph(new Chunk("PACKING LIST", new Font(baseFont, 10f, Font.NORMAL)));
            paragraph.Alignment = Rectangle.ALIGN_CENTER;
            paragraph.Leading = 12f;
            return paragraph;
        }

        private void AddTitle5()
        {
            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 579; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 80, 20 });

            var cell1 = new PdfPCell(new Phrase("致:" + this._致, new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
            };
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            var cell2 = new PdfPCell(new Phrase("日期：" + this._日期, new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
            };
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            document.Add(table);
        }

        private void AddTitle6()
        {
            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 579; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 80, 20 });

            var cell1 = new PdfPCell(new Phrase("地址：" + this._地址, new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_LEFT,
            };
            cell1.DisableBorderSide(15);
            table.AddCell(cell1);

            var cell2 = new PdfPCell(new Phrase("合同编号：" + this._合同编号, new Font(baseFont, 8f, Font.NORMAL)))
            {
                HorizontalAlignment = Rectangle.ALIGN_RIGHT,
            };
            cell2.DisableBorderSide(15);
            table.AddCell(cell2);

            document.Add(table);
        }

        private void AddTitle7()
        {
            Paragraph paragraph = new Paragraph(new Chunk(" ", new Font(baseFont, 8f, Font.NORMAL)));
            paragraph.Alignment = Rectangle.ALIGN_LEFT;
            paragraph.Leading = 2f;
            document.Add(paragraph);
        }

        private void AddTail1(Vendor vendor)
        {
            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 574; //绝对宽度
            table.LockedWidth = true;
            table.SetWidths(new int[] { 15, 85 });

            //1
            var a = new PdfPCell();
            a.AddElement(new Phrase("总件数: " + this._总件数, new Font(baseFont, 8f, Font.NORMAL)));
            a.AddElement(new Phrase("总数量: " + this._总数量 + " PCS", new Font(baseFont, 8f, Font.NORMAL)));
            a.AddElement(new Phrase("总净重: " + this._总净重 + " KGS", new Font(baseFont, 8f, Font.NORMAL)));
            a.AddElement(new Phrase("总毛重: " + this._总毛重 + " KGS", new Font(baseFont, 8f, Font.NORMAL)));
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

            document.Add(table);
        }

        private void AddData()
        {
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
                    BorderColor = new BaseColor(0, 139, 139),
                };
                if (j == 0 || j == 7)
                {
                    cell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                }
                table.AddCell(cell);
            }

            //2. Body
            BodyDataTable bodyDataTable = new BodyDataTable(this._items);

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
                        BorderColor = new BaseColor(0, 139, 139),
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
            FooterDataTable footerDataTable = new FooterDataTable(数量: this._总数量, 净重: this._总净重, 毛重: this._总毛重);
            for (int j = 0; j < footerDataTable.ColumnsCount; j++)
            {
                PdfPCell cell = new PdfPCell(new Phrase(footerDataTable.Rows[0][j].ToString(), new Font(baseFont, TableFontSize)))
                {
                    BorderWidth = 0.01f,
                    BorderColor = new BaseColor(0, 139, 139),
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

            document.Add(table);
        }

        public void AddContent(Vendor vendor)
        {
            document.SetMargins(10f, 10f, 2f, 2f);

            try
            {
                document.Open();

                //标题
                PdfPTable tableTitle = new PdfPTable(3);
                tableTitle.TotalWidth = 574; //绝对宽度
                tableTitle.LockedWidth = true;
                tableTitle.SetWidths(new int[] { 10, 80, 10 });

                //1 Tile 左侧 Logo
                Image imageLogo = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.LogoUrl));
                //同比例缩放
                imageLogo.ScalePercent(24f);
                var titleLeftCell = new PdfPCell(imageLogo) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 8f, };
                titleLeftCell.DisableBorderSide(15);
                tableTitle.AddCell(titleLeftCell);

                //2 Tile 中间 卖方公司名称 
                var titleMiddleCell = new PdfPCell();
                titleMiddleCell.AddElement(AddTitle1());
                titleMiddleCell.AddElement(AddTitle2());
                titleMiddleCell.AddElement(AddTitle3());
                titleMiddleCell.AddElement(AddTitle4());
                titleMiddleCell.DisableBorderSide(15);
                tableTitle.AddCell(titleMiddleCell);

                //3 Tile 右侧 空白占位
                var titleRightCell = new PdfPCell(new Phrase("", new Font(baseFont, 8f, Font.NORMAL)));
                titleRightCell.DisableBorderSide(15);
                tableTitle.AddCell(titleRightCell);

                document.Add(tableTitle);


                AddTitle5();
                AddTitle6();
                AddTitle7();
                AddData();
                AddTail1(vendor);

            }
            catch (DocumentException docEx)
            {
                throw (docEx);
            }
            catch (IOException ex)
            {
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                //关闭document
                if (document != null)
                {
                    document.Close();
                }
            }
        }

        public void Save(string fullPath, Vendor vendor)
        {
            var pdfWriter = PdfWriter.GetInstance(document, new FileStream(fullPath, FileMode.OpenOrCreate));
            AddContent(vendor);
            pdfWriter.Close();
        }
    }

    public class RowspanInfo
    {
        public int BeginNo { get; set; }

        public int RowSpanCount { get; set; }

        public string BeginBox { get; set; }
    }
}
