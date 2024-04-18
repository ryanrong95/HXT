using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Common
{
    public class ITextPDFBase : PdfPageEventHelper
    {
        #region 属性
        private string _fontFilePathForHeaderFooter = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "SIMHEI.TTF");
        /// <summary>
        /// 页眉/页脚所用的字体
        /// </summary>
        public string FontFilePathForHeaderFooter
        {
            get
            {
                return _fontFilePathForHeaderFooter;
            }

            set
            {
                _fontFilePathForHeaderFooter = value;
            }
        }

        private string _fontFilePathForBody = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "SIMSUN.TTC,1");

        /// <summary>
        /// 正文内容所用的字体
        /// </summary>
        public string FontFilePathForBody
        {
            get { return _fontFilePathForBody; }
            set { _fontFilePathForBody = value; }
        }


        private PdfPTable _header;
        /// <summary>
        /// 页眉
        /// </summary>
        public PdfPTable Header
        {
            get { return _header; }
            private set { _header = value; }
        }

        private PdfPTable _footer;
        /// <summary>
        /// 页脚
        /// </summary>
        public PdfPTable Footer
        {
            get { return _footer; }
            private set { _footer = value; }
        }


        private BaseFont _baseFontForHeaderFooter;

        /// <summary>
        /// 页眉页脚所用的字体
        /// </summary>
        public BaseFont BaseFontForHeaderFooter
        {
            get { return _baseFontForHeaderFooter; }
            set { _baseFontForHeaderFooter = value; }
        }

        private BaseFont _baseFontForBody;

        /// <summary>
        /// 正文所用的字体
        /// </summary>
        public BaseFont BaseFontForBody
        {
            get { return _baseFontForBody; }
            set { _baseFontForBody = value; }
        }

        private Document _document;

        /// <summary>
        /// PDF的Document
        /// </summary>
        public Document Document
        {
            get { return _document; }
            private set { _document = value; }
        }

        #endregion

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                BaseFontForHeaderFooter = BaseFont.CreateFont(FontFilePathForHeaderFooter, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                BaseFontForBody = BaseFont.CreateFont(FontFilePathForBody, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                Document = document;
            }
            catch (DocumentException de)
            {
                throw de;
            }
            catch (System.IO.IOException ioe)
            {
                throw ioe;
            }
        }

        #region 生成页头
        /// <summary>
        /// 生成页眉
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public virtual PdfPTable GenerateHeader(PdfWriter writer)
        {
            return null;
        }
        #endregion

        #region 生成页脚
        /// <summary>
        /// 生成页脚
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public virtual PdfPTable GenerateFooter(PdfWriter writer)
        {
            return null;
        }
        #endregion

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            //输出页眉
            Header = GenerateHeader(writer);
            Header.TotalWidth = document.PageSize.Width - 20f;
            ///调用PdfTable的WriteSelectedRows方法。该方法以第一个参数作为开始行写入。
            ///第二个参数-1表示没有结束行，并且包含所写的所有行。
            ///第三个参数和第四个参数是开始写入的坐标x和y.
            Header.WriteSelectedRows(0, -1, 10, document.PageSize.Height - 10, writer.DirectContent);

            //输出页脚
            Footer = GenerateFooter(writer);
            Footer.TotalWidth = document.PageSize.Width - 20f;
            Footer.WriteSelectedRows(0, -1, 10, document.PageSize.GetBottom(20), writer.DirectContent);
        }
    }

    public class VoucherToPdf : ITextPDFBase
    {
        private Voucher _voucher;

        public VoucherToPdf() { }

        public VoucherToPdf(Voucher voucher)
        {
            this._voucher = voucher;
        }

        public Document ToDocument(string FilePath)
        {
            //中文字体
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font FontTitle = new Font(baseFont, 14f, Font.BOLD);
            Font FontLarger = new Font(baseFont, 10f, Font.BOLD);
            Font FontContent = new Font(baseFont, 7f, Font.NORMAL);
            var tbLineWidth = 0.01f;
            Rectangle rec = new Rectangle(PageSize.A4);
            //创建一个文档实例。 去除边距
            Document document = new Document(rec);
            document.SetMargins(10f, 10f, 55f, 10f);

            try
            {
                //创建一个writer实例
                var writer = PdfWriter.GetInstance(document, new FileStream(FilePath, FileMode.OpenOrCreate));
                writer.PageEvent = this;
                //打开当前文档
                document.Open();

                #region 内容1：我方图标

                var tb1 = new PdfPTable(4);
                tb1.HorizontalAlignment = 0;
                tb1.TotalWidth = 574;
                tb1.LockedWidth = true;
                tb1.SetWidths(new float[] { 198, 198, 99, 99 });

                Image img1 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\Themes\\images\\logo.png"));
                img1.ScalePercent(35f);
                var c1 = new PdfPCell(img1)
                {
                    VerticalAlignment = Rectangle.ALIGN_MIDDLE,
                    PaddingLeft = 0f,
                    PaddingTop = -40f,

                };
                c1.DisableBorderSide(-1);
                tb1.AddCell(c1);

                //占位
                var eCell = new PdfPCell();
                eCell.Phrase = new Paragraph(new Chunk(" ", FontTitle))
                {
                    Alignment = Rectangle.ALIGN_CENTER,
                };
                eCell.DisableBorderSide(-1);
                eCell.FixedHeight = 30;
                tb1.AddCell(eCell);


                //占位
                var eCell1 = new PdfPCell();
                eCell1.Phrase = new Paragraph(new Chunk(" ", FontTitle))
                {
                    Alignment = Rectangle.ALIGN_CENTER,
                };
                eCell1.DisableBorderSide(-1);
                eCell1.FixedHeight = 30;
                tb1.AddCell(eCell1);

                Image img3 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\Themes\\images\\农业银行二维码.png"));
                img3.ScalePercent(22f);
                var c3 = new PdfPCell(img3)
                {
                    VerticalAlignment = Rectangle.ALIGN_MIDDLE,
                    HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                    PaddingRight = 20,
                    PaddingTop = -40,
                    FixedHeight = 30,
                };
                c3.DisableBorderSide(-1);
                tb1.AddCell(c3);

                document.Add(tb1);
                #endregion

                if (_voucher.Mode == Enums.VoucherMode.Receivables)
                {

                    #region 内容2：我方信息

                    var tb2 = new PdfPTable(1);
                    tb2.HorizontalAlignment = 0;
                    tb2.TotalWidth = 574;
                    tb2.LockedWidth = true;

                    PdfPCell 深圳市芯达通供应链管理有限公司 = new PdfPCell(new Phrase("深圳市芯达通供应链管理有限公司", FontLarger))
                    {
                        BorderWidth = 0.0F,
                    };
                    PdfPCell 深圳市龙岗区吉华路393号英达丰科技园 = new PdfPCell(new Phrase("深圳市龙岗区吉华路393号英达丰科技园", FontContent))
                    {
                        BorderWidth = 0.0F,
                    };
                    PdfPCell 电话传真 = new PdfPCell(new Phrase("电话：0755-83988698  传真：0755-83995933", FontContent))
                    {
                        BorderWidth = 0.0F,
                    };
                    PdfPCell 账单币种 = new PdfPCell(new Phrase("账单币种：" + Currency.CNY.ToString(), FontContent))
                    {
                        BorderWidth = 0.0F,
                    };

                    tb2.AddCell(深圳市芯达通供应链管理有限公司);
                    tb2.AddCell(深圳市龙岗区吉华路393号英达丰科技园);
                    tb2.AddCell(电话传真);
                    tb2.AddCell(账单币种);

                    document.Add(tb2);

                    #endregion

                    #region 内容3：客户信息

                    var tb3 = new PdfPTable(2);
                    tb3.HorizontalAlignment = 0;
                    tb3.TotalWidth = 574;
                    tb3.LockedWidth = true;
                    tb3.SetWidths(new float[] { 52f, 522f });

                    var client = new Views.Origins.ClientsOrigin().SingleOrDefault(t => t.ID == _voucher.PayerID);
                    var invoice = new Views.Origins.InvoicesOrigin().FirstOrDefault(t => t.ClientID == _voucher.PayerID);

                    tb3.AddCell(new PdfPCell(new Phrase("账单期号：" + _voucher.CutDateIndex.ToString(), FontTitle)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE, Colspan = 2 });
                    tb3.AddCell(new PdfPCell(new Phrase("客户名称：", FontContent)) { BorderWidth = tbLineWidth });
                    tb3.AddCell(new PdfPCell(new Phrase(client?.Name, FontContent)) { BorderWidth = tbLineWidth });
                    tb3.AddCell(new PdfPCell(new Phrase("客户地址：", FontContent)) { BorderWidth = tbLineWidth });
                    tb3.AddCell(new PdfPCell(new Phrase(invoice?.RegAddress, FontContent)) { BorderWidth = tbLineWidth });
                    tb3.AddCell(new PdfPCell(new Phrase("联系人/电话：", FontContent)) { BorderWidth = tbLineWidth });
                    tb3.AddCell(new PdfPCell(new Phrase(invoice?.Contact + "(" + invoice?.Phone + ")", FontContent)) { BorderWidth = tbLineWidth });

                    document.Add(tb3);

                    #endregion

                    #region  内容4：表体

                    var tb4 = new PdfPTable(8);
                    tb4.HorizontalAlignment = 0;
                    tb4.TotalWidth = 574;//594磅
                    tb4.LockedWidth = true;
                    tb4.SetWidths(new float[] { 52f, 71.75f, 71.75f, 71.75f, 71.75f, 71.75f, 71.75f, 91.5f });

                    //列表标题
                    tb4.AddCell(new PdfPCell(new Phrase("列账日期", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase("订单编号", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase("下单日期", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase("所属业务", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase("科目", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase("单价", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase("数量", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase("金额", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });

                    var orders = new SzMvc.Services.Views.Origins.OrdersOrigin().ToArray();
                    var payeeLefts = new SzMvc.Services.Views.Origins.PayeeLeftsOrigin().Where(t => t.PayerID == _voucher.PayerID && t.CutDateIndex == _voucher.CutDateIndex).ToArray();
                    var items = from entity in payeeLefts
                                join order in orders on entity.FormID equals order.ID
                                select new
                                {
                                    CreateDate = entity.CreateDate.ToString("yyyy-MM-dd"),
                                    OrderID = order.ID,
                                    Status = order.Status.GetDescription(),
                                    OrderDate = order.CreateDate.ToString("yyyy-MM-dd"),
                                    Conduct = entity.Conduct.GetDescription(),
                                    entity.Subject,
                                    UnitPrice = entity.UnitPrice.ToString("0.0000"),
                                    entity.Quantity,
                                    Total = entity.Total,
                                };
                    foreach (var item in items)
                    {
                        tb4.AddCell(new PdfPCell(new Phrase(item.CreateDate, FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                        tb4.AddCell(new PdfPCell(new Phrase(item.OrderID, FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                        tb4.AddCell(new PdfPCell(new Phrase(item.OrderDate, FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                        tb4.AddCell(new PdfPCell(new Phrase(item.Conduct, FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                        tb4.AddCell(new PdfPCell(new Phrase(item.Subject, FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                        tb4.AddCell(new PdfPCell(new Phrase(item.UnitPrice, FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                        tb4.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                        tb4.AddCell(new PdfPCell(new Phrase(item.Total.ToString("0.0000"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    }
                    //合计行
                    tb4.AddCell(new PdfPCell(new Phrase("合计：", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE, Colspan = 7 });
                    tb4.AddCell(new PdfPCell(new Phrase(items.Sum(t => t.Total).ToString("0.0000"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE, });

                    document.Add(tb4);

                    #endregion

                    #region 内容5：说明

                    var tb5 = new PdfPTable(2);
                    tb5.HorizontalAlignment = 0;
                    tb5.TotalWidth = 574;
                    tb5.LockedWidth = true;
                    tb5.SetWidths(new float[] { 49f, 525f });

                    tb5.AddCell(new PdfPCell(new Phrase("说明：", FontContent)) { BorderWidth = 0.0f, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb5.AddCell(new PdfPCell(new Phrase("1.此对账单仅包含对账期内发生的款项（不包括对账日期之后），如果错漏，请及时与我司联系。", FontContent)) { BorderWidth = 0.0f });
                    tb5.AddCell(new PdfPCell(new Phrase("", FontContent)) { BorderWidth = 0.0f });
                    tb5.AddCell(new PdfPCell(new Phrase("2.收到此对账单核对无误后请及时确认并反馈我司。如在收到上述对账单后三日内未反馈我司，视同确认。", FontContent)) { BorderWidth = 0.0f });
                    tb5.AddCell(new PdfPCell(new Phrase("", FontContent)) { BorderWidth = 0.0f });
                    tb5.AddCell(new PdfPCell(new Phrase("3.请贵司核对无误后及时安排付款。", FontContent)) { BorderWidth = 0.0f });
                    tb5.AddCell(new PdfPCell(new Phrase("衷心感谢贵司一直以来的支持与合作！", FontContent)) { BorderWidth = 0.0f, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE, Colspan = 2 });

                    document.Add(tb5);

                    #endregion
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return document;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveAs(string filePath)
        {
            var tempPath = filePath.Replace(".pdf", "_1.pdf");
            Document doc = this.ToDocument(tempPath);
            //关闭document,导出
            doc.Close();

            //加水印
            setWatermark(tempPath, filePath, "深圳市芯达通供应链管理有限公司");
        }

        #region 添加水印

        public void setWatermark(string inputfilepath, string outputfilepath, string waterMarkName)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(inputfilepath);
                pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));
                int total = pdfReader.NumberOfPages + 1;//获取PDF的总页数;
                Rectangle psize = pdfReader.GetPageSize(1);//获取第一页
                float width = psize.Width;//PDF页面的宽度，用于计算水印倾斜
                float height = psize.Height;
                PdfContentByte content;
                BaseFont font = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\SIMKAI.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                PdfGState gs = new PdfGState();
                for (var i = 1; i < total; i++)
                {
                    //content = pdfStamper.GetOverContent(i);//在内容上方加水印
                    content = pdfStamper.GetUnderContent(i);//在内容下方加水印
                    //透明度
                    gs.FillOpacity = 0.35f;
                    content.SetGState(gs);
                    //content.SetGrayFill(0.3f);
                    //开始写入文本
                    content.BeginText();
                    content.SetColorFill(BaseColor.LIGHT_GRAY);
                    content.SetFontAndSize(font, 30);
                    content.SetTextMatrix(0, 0);
                    content.ShowTextAligned(Element.ALIGN_CENTER, waterMarkName, width / 2, height / 2, 55);
                    content.EndText();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();

                File.Delete(inputfilepath);
            }
        }

        #endregion

        #region   生成页眉

        public override PdfPTable GenerateHeader(PdfWriter writer)
        {
            var headFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable header = new PdfPTable(2);
            header.HorizontalAlignment = 0;
            header.TotalWidth = 600;
            header.LockedWidth = true;
            float[] widths = new float[] { 130, 450 };//三列列宽不同若果是浮点数需要加f
            header.SetWidths(widths);

            PdfPCell cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            cell.BorderWidthBottom = 1f;
            cell.BorderColorBottom = new BaseColor(170, 170, 170);
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            header.AddCell(cell);
            return header;
        }
        #endregion

        #region   生成页脚

        public override PdfPTable GenerateFooter(PdfWriter writer)
        {
            var footFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable footer = new PdfPTable(1);
            footer.HorizontalAlignment = 0;
            footer.TotalWidth = 594;
            footer.LockedWidth = true;

            var cell = new PdfPCell(new Paragraph("", new Font(footFont, 8, Font.NORMAL)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            cell.DisableBorderSide(-1);
            footer.AddCell(cell);
            return footer;
        }

        #endregion

        #region 获取骑缝章图片

        private bool GetImage(int num, string picUrl, string clientName)
        {
            try
            {
                //看是否有切好的图片
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", clientName, num.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var images = Directory.GetFiles(path, ".", SearchOption.AllDirectories).Where(s => s.EndsWith(".png"));

                if (images.Count() == num)
                {
                    return true;
                }
                else
                {
                    //生成图片
                    System.Drawing.Image image = System.Drawing.Image.FromFile(picUrl);
                    int w = image.Width / num;
                    System.Drawing.Bitmap bitmap = null;
                    for (int i = 0; i < num; i++)
                    {
                        bitmap = new System.Drawing.Bitmap(w, image.Height);
                        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                        {
                            g.Clear(System.Drawing.Color.White);
                            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(i * w, 0, w, image.Height);
                            g.DrawImage(image, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), rect, System.Drawing.GraphicsUnit.Pixel);
                        }
                        bitmap.MakeTransparent(System.Drawing.Color.White);
                        string fileUrl = @path + "\\" + i.ToString() + ".png";
                        bitmap.Save(fileUrl);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        #endregion
    }
}
