extern alias globalB;

using globalB::iTextSharp.text;
using globalB::iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Enums;

namespace Needs.Ccs.Services.Models
{
    public class AgentProxyToPdf : ITextPDFBase
    {

        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public List<Order> Orders { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// 受益人：这里指代理报关公司，来源于订单的受益人
        /// </summary>
        public Beneficiary Beneficiary { get; set; }

        /// <summary>
        /// 产品明细
        /// </summary>
        OrderItems items;
        public OrderItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.OrderItemsView())
                    {
                        var query = view.Where(item => this.Orders.Select(t => t.ID).ToList().Contains(item.OrderID) && item.Status == Enums.Status.Normal);
                        this.Items = new OrderItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.items = new OrderItems(value, new Action<OrderItem>(delegate (OrderItem item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 报关总货值（外币）
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int? PackNo { get; set; }

        /// <summary>
        /// 包装种类
        /// </summary>
        public string WarpType { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public Enums.OrderStatus OrderStatus { get; set; }


        public MainOrderFile MainFile { get; set; }

        Purchaser purchaser = PurchaserContext.Current;
        Vendor vendor = VendorContext.Current;

        #endregion

        public AgentProxyToPdf() { }


        public Document ToDocument(string FilePath)
        {
            //中文字体
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font FontTitle = new Font(baseFont, 14f, Font.NORMAL);
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

                #region 标题等
                //标题
                var tb1 = new PdfPTable(3);
                tb1.HorizontalAlignment = 0;
                tb1.TotalWidth = 594;//594磅
                tb1.LockedWidth = true;
                tb1.SetWidths(new float[] { 198, 198, 198 });

                //占位
                var eCell = new PdfPCell();
                eCell.Phrase = new Paragraph(new Chunk(" ", FontTitle))
                {
                    Alignment = Rectangle.ALIGN_CENTER,
                };
                eCell.DisableBorderSide(-1);
                eCell.FixedHeight = 50;
                tb1.AddCell(eCell);

                //标题
                var titleCell = new PdfPCell();
                titleCell.Phrase = new Paragraph(new Chunk($"{purchaser.ShortName}-代理报关委托书", FontTitle))
                {
                    Alignment = Rectangle.ALIGN_CENTER,
                };
                titleCell.DisableBorderSide(-1);
                titleCell.FixedHeight = 50;
                titleCell.PaddingTop = 5;
                tb1.AddCell(titleCell);

                //二维码
                BarcodeQRCode code = new BarcodeQRCode(this.ID, 58, 58, null);
                var ewmCell = new PdfPCell(code.GetImage())
                {
                    HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                    VerticalAlignment = Rectangle.ALIGN_MIDDLE
                };
                ewmCell.DisableBorderSide(-1);
                //ewmCell.FixedHeight = 50;
                ewmCell.PaddingRight = 20;
                ewmCell.PaddingTop = -20;
                tb1.AddCell(ewmCell);

                document.Add(tb1);

                #endregion

                #region 内容表格1

                //订单号
                var tbOrderNo = new PdfPTable(1);
                tbOrderNo.HorizontalAlignment = 0;
                tbOrderNo.TotalWidth = 574;//594磅
                tbOrderNo.LockedWidth = true;
                tbOrderNo.SetWidths(new float[] { 574 });
                var ordercell = new PdfPCell();
                ordercell.Phrase = new Paragraph(new Chunk("订单编号:" + this.ID, FontContent))
                {
                    Alignment = Rectangle.ALIGN_RIGHT,
                };
                ordercell.HorizontalAlignment = Element.ALIGN_RIGHT;
                ordercell.DisableBorderSide(-1);
                ordercell.PaddingBottom = 5;
                //ordercell.PaddingRight = -56;
                tbOrderNo.AddCell(ordercell);
                document.Add(tbOrderNo);

                #endregion

                #region 内容表格2：委托方等信息

                var tb2 = new PdfPTable(1);
                tb2.HorizontalAlignment = 0;
                tb2.TotalWidth = 574;//594磅
                tb2.LockedWidth = true;
                tb2.SetWidths(new float[] { 287 });

                PdfPCell 委托方 = new PdfPCell(new Phrase("委托方名称: "+ this.Client.Company.Name, FontContent))
                {
                    BorderWidth = tbLineWidth,
                };

                PdfPCell 委托方收货信息 = new PdfPCell(new Phrase("委托方收货信息: "+ this.Client.Company.Name + "/地址: "+ this.Client.Company.Address + "/联系人: "+ this.Client.Company.Contact.Name + "/电话: "+ this.Client.Company.Contact.Mobile, FontContent))
                {
                    BorderWidth = tbLineWidth,
                };
                PdfPCell 代理方名称 = new PdfPCell(new Phrase("代理方名称: "+ purchaser.CompanyName, FontContent))
                {
                    BorderWidth = tbLineWidth,
                };
                PdfPCell 代理方收货信息 = new PdfPCell(new Phrase("代理方收货信息: "+ vendor.CompanyName + "/地址: " + vendor.Address + "/联系人: " + vendor.Contact + "/电话: " + vendor.Tel, FontContent))
                {
                    BorderWidth = tbLineWidth,
                };

                tb2.AddCell(委托方);
                tb2.AddCell(委托方收货信息);
                tb2.AddCell(代理方名称);
                tb2.AddCell(代理方收货信息);

                document.Add(tb2);

                #endregion

                #region  表格内容4 ：表体

                var tb4 = new PdfPTable(11);
                tb4.HorizontalAlignment = 0;
                tb4.TotalWidth = 574;//594磅
                tb4.LockedWidth = true;//3 6 20 15 20 6各6
                tb4.SetWidths(new float[] { 17.22f, 34.44f, 114.8f, 86.1f, 114.8f, 34.44f, 34.44f, 34.44f, 34.44f, 34.44f, 34.44f });

                tb4.AddCell(new PdfPCell(new Phrase("序号", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("批号", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("品名", FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("品牌", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("规格型号", FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("产地", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("数量", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("单位", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("报关单价" + "\n" + "("+ this.Currency +")", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("报关总价" + "\n" + "(" + this.Currency + ")", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("关税率", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });

                int sn = 0;
                var units = new Views.BaseUnitsView().ToList();
                foreach (var item in this.Items)
                {
                    sn++;
                    //型号内容
                    tb4.AddCell(new PdfPCell(new Phrase(sn.ToString(), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.Batch, FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.Category?.Name ?? item.Name, FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.Manufacturer, FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.Model.Trim(), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.Origin, FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString("0.####"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(units.Where(u => u.Code == item.Unit).FirstOrDefault()?.Name ?? item.Unit, FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.UnitPrice.ToString("0.0000"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.TotalPrice.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.ImportTax?.Rate.ToString("0.0000"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                }

                //合计行
                tb4.AddCell(new PdfPCell(new Phrase("合计：", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE, Colspan = 6 });
                tb4.AddCell(new PdfPCell(new Phrase(this.Items.Select(item => item.Quantity).Sum().ToString("0.####"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("", FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("", FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase(this.Items.Select(item => item.TotalPrice).Sum().ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("", FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });

                document.Add(tb4);

                #endregion

                #region 表格内容5：包装类型

                var tb5 = new PdfPTable(1);
                tb5.HorizontalAlignment = 0;
                tb5.TotalWidth = 574;//594磅
                tb5.LockedWidth = true;
                tb5.SetWidths(new float[] { 574 });

                var types = new Views.BasePackTypesView().ToList();
                var wrapType = types.Where(t => t.Code == this.WarpType).FirstOrDefault()?.Name;
                var totalGwt = this.Items.Where(item => item.GrossWeight != null).Select(item => item.GrossWeight).Sum().Value.ToRound(2);
                var message = "包装类型: " + wrapType;
                if (this.PackNo != null)
                {
                    message += " 总件数: " + this.PackNo;
                }
                if (totalGwt > 0M)
                {
                    message += " 总毛重: " + totalGwt.ToString("0.##") + " KG";
                }

                var packCell = new PdfPCell(new Phrase(message, FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_LEFT };
                packCell.DisableBorderSide(-1);
                tb5.AddCell(packCell);

                document.Add(tb5);

                #endregion

                #region 表格内容6：文字

                //空白间隔
                document.Add(new Paragraph(new Chunk(" ", new Font(baseFont, 5f))));

                var tb6 = new PdfPTable(1);
                tb6.HorizontalAlignment = 0;
                tb6.TotalWidth = 574;//594磅
                tb6.LockedWidth = true;
                tb6.SetWidths(new float[] { 574 });
                tb6.AddCell(new PdfPCell(new Phrase(@"温馨提示:
1.我单位保证遵守《中华人民共和国海关法》及国家有关法规保证所提供的委托信息与所报的货物相符。
2.委托方务必真实填写，若因委托方虚报、假报、多报、少报等因素造成的扣关、罚款等后果由委托方自行承担。
3.委托方一份报关单最多20个型号，超过部分按另一单结算，依此类推。
4.委托方需即时签字盖章回传，如因委托方延误回传造成的延迟申报，代理方不承担责任。", FontContent))
                { BorderWidth = tbLineWidth });

                document.Add(tb6);

                #endregion 

                #region 表格内容8：盖章

                var tb8 = new PdfPTable(2);
                tb8.HorizontalAlignment = 0;
                tb8.TotalWidth = 574;//594磅
                tb8.LockedWidth = true;
                tb8.SetWidths(new float[] { 287, 287 });

                var 委托方Cell = new PdfPCell(new Phrase("\n" + "代理方盖章：" + "\n ", FontContent))
                {
                    BorderWidth = tbLineWidth,
                };
                var 被委托方Cell = new PdfPCell(new Phrase("\n" + "委托方(签字/盖章)：" + "\n ", FontContent))
                {
                    BorderWidth = tbLineWidth,
                };
                tb8.AddCell(委托方Cell);
                tb8.AddCell(被委托方Cell);


                Image img1 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.Client.Company.Name + ".png"));
                img1.ScalePercent(75f);
                var c1 = new PdfPCell(img1)
                {
                    VerticalAlignment = Rectangle.ALIGN_MIDDLE,
                    PaddingLeft = 70f,
                    PaddingTop = -75
                };
                c1.DisableBorderSide(-1);
                tb8.AddCell(c1);

                Image img2 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.SealUrl));
                img2.ScalePercent(75f);
                var c2 = new PdfPCell(img2)
                {
                    VerticalAlignment = Rectangle.ALIGN_MIDDLE,
                    PaddingLeft = 70f,
                    PaddingTop = -75
                };
                c2.DisableBorderSide(-1);
                tb8.AddCell(c2);

                document.Add(tb8);

                #endregion
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
            //根据报关日期，DDate 小于 2019-11-18 00:00:00，使用指定的 香港公司
            this.vendor = new VendorContext(VendorContextInitParam.Instrument, this.Orders.FirstOrDefault().ID, "CaiWu").Current1;

            var tempPath = filePath.Replace(".pdf", "_1.pdf");
            Document doc = this.ToDocument(tempPath);
            //关闭document,导出
            doc.Close();

            //加水印
            setWatermark(tempPath, filePath, PurchaserContext.Current.CompanyName);

        }

        /// <summary>
        /// 添加普通偏转角度文字水印
        /// </summary>
        /// <param name="inputfilepath"></param>
        /// <param name="outputfilepath"></param>
        /// <param name="waterMarkName"></param>
        /// <param name="permission"></param>
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
                    //content.SetColorFill(BaseColor.BLACK);
                    content.EndText();
                }

                #region 骑缝章

                //获取分割后的印章图片
                //华芯通或者创新恒远图片
                GetImage(total - 1, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.purchaser.SealUrl), this.purchaser.CompanyName);
                //大赢家图片
                GetImage(total - 1, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.Client.Company.Name + ".png"), this.Client.Company.Name);

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                for (var i = 1; i < total; i++)
                {
                    content = pdfStamper.GetOverContent(i);
                    gs.FillOpacity = 1f;
                    content.SetGState(gs);

                    string clientPath = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", this.Client.Company.Name, (total - 1).ToString(), (i - 1).ToString() + ".png");
                    Image img1 = Image.GetInstance(clientPath);
                    img1.ScalePercent(75f);
                    img1.SetAbsolutePosition(width - img1.Width * 0.75f, height / 2 - img1.Height * 0.75f - 160f);
                    content.AddImage(img1);


                    string agentPath = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", this.purchaser.CompanyName, (total - 1).ToString(), (i - 1).ToString() + ".png");
                    Image img2 = Image.GetInstance(agentPath);
                    img2.ScalePercent(75f);
                    img2.SetAbsolutePosition(width - img2.Width * 0.75f, height / 2 - img2.Height * 0.75f + 150f);
                    content.AddImage(img2);
                }

                #endregion

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

        #region   生成页眉

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public override PdfPTable GenerateHeader(PdfWriter writer)
        {
            var headFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable header = new PdfPTable(2);
            header.HorizontalAlignment = 0;
            header.TotalWidth = 600;
            header.LockedWidth = true;
            float[] widths = new float[] { 130, 450 };//三列列宽不同若果是浮点数需要加f
            header.SetWidths(widths);

            Image image1 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.HeaderImg));
            image1.ScalePercent(25f);
            var b = new PdfPCell(image1) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 2f, PaddingTop = 2f };
            b.DisableBorderSide(-1);
            header.AddCell(b);

            PdfPCell cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            cell.BorderWidthBottom = 1f;
            cell.BorderColorBottom = new BaseColor(170, 170, 170);
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.Phrase = new Paragraph(PurchaserContext.Current.OfficalWebsite, new Font(headFont, 8, Font.NORMAL));
            header.AddCell(cell);
            return header;
        }
        #endregion

        #region   生成页脚
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public override PdfPTable GenerateFooter(PdfWriter writer)
        {
            var footFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            PdfPTable footer = new PdfPTable(1);
            footer.HorizontalAlignment = 0;
            footer.TotalWidth = 594;
            footer.LockedWidth = true;

            var cell = new PdfPCell(new Paragraph(PurchaserContext.Current.CompanyName, new Font(footFont, 8, Font.NORMAL)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            cell.DisableBorderSide(-1);
            footer.AddCell(cell);
            return footer;
        }

        #endregion

        /// <summary>
        /// 获取骑缝章图片
        /// </summary>
        /// <param name="num"></param>
        /// <param name="picUrl"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
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



    }

}
