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
using System.Data;

namespace Needs.Ccs.Services.Models
{
    public class SalesContractToPdf : ITextPDFBase
    {
        #region 属性

        public SalesContract SalesContract { get; set; }

        #endregion

        public SalesContractToPdf(SalesContract salesContract)
        {
            this.SalesContract = salesContract;

        }


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
                tb1.SetWidths(new float[] { 245, 148, 198 });

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
                titleCell.Phrase = new Paragraph(new Chunk($"销售合同", FontTitle))
                {
                    Alignment = Rectangle.ALIGN_CENTER,
                };
                titleCell.DisableBorderSide(-1);
                titleCell.FixedHeight = 50;
                titleCell.PaddingTop = 5;
                tb1.AddCell(titleCell);

                //二维码
                BarcodeQRCode code = new BarcodeQRCode(this.SalesContract.ID, 58, 58, null);//this.SalesContract.OrderID
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

                ////合同号
                //var tb2 = new PdfPTable(1);
                //tb2.HorizontalAlignment = 0;
                //tb2.TotalWidth = 574;//594磅
                //tb2.LockedWidth = true;
                //tb2.SetWidths(new float[] { 574 });

                //var orderIdCell = new PdfPCell();
                //orderIdCell.Phrase = new Paragraph(new Phrase("合同号:" + this.SalesContract.ID, FontContent))
                //{
                //    Alignment = Rectangle.ALIGN_RIGHT,
                //};
                //orderIdCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //orderIdCell.DisableBorderSide(-1);
                //orderIdCell.PaddingBottom = 2;
                //tb2.AddCell(orderIdCell);
                //document.Add(tb2);

                #endregion

                #region 内容表格2：买卖方信息

                var tb3 = new PdfPTable(2);
                tb3.HorizontalAlignment = 0;
                tb3.TotalWidth = 574;//594磅
                tb3.LockedWidth = true;
                tb3.SetWidths(new float[] { 287, 287 });

                var salesDate = new PdfPCell();
                salesDate.Phrase = new Paragraph(new Chunk("日期：" + this.SalesContract.SalesDateText, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };//this.SalesContract.OrderID
                salesDate.HorizontalAlignment = Element.ALIGN_LEFT;
                salesDate.DisableBorderSide(-1);
                salesDate.PaddingBottom = 2;
                tb3.AddCell(salesDate);

                var orderId = new PdfPCell();
                orderId.Phrase = new Paragraph(new Chunk("合同编号：" + this.SalesContract.ID, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                orderId.HorizontalAlignment = Element.ALIGN_LEFT;
                orderId.DisableBorderSide(-1);
                orderId.PaddingBottom = 2;
                tb3.AddCell(orderId);

                var buyer = new PdfPCell();
                buyer.Phrase = new Paragraph(new Chunk("买 方：" + this.SalesContract.Buyer.Title, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                buyer.HorizontalAlignment = Element.ALIGN_LEFT;
                buyer.DisableBorderSide(-1);
                buyer.PaddingBottom = 2;
                tb3.AddCell(buyer);

                var seller = new PdfPCell();
                seller.Phrase = new Paragraph(new Chunk("卖  方：" + this.SalesContract.Seller.Title, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                seller.HorizontalAlignment = Element.ALIGN_LEFT;
                seller.DisableBorderSide(-1);
                seller.PaddingBottom = 2;
                tb3.AddCell(seller);

                var buyerBankName = new PdfPCell();
                buyerBankName.Phrase = new Paragraph(new Chunk("开户行：" + this.SalesContract.Buyer.BankName, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                buyerBankName.HorizontalAlignment = Element.ALIGN_LEFT;
                buyerBankName.DisableBorderSide(-1);
                buyerBankName.PaddingBottom = 2;
                tb3.AddCell(buyerBankName);

                var sellerBankName = new PdfPCell();
                sellerBankName.Phrase = new Paragraph(new Chunk("开户行：" + this.SalesContract.Seller.BankName, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                sellerBankName.HorizontalAlignment = Element.ALIGN_LEFT;
                sellerBankName.DisableBorderSide(-1);
                sellerBankName.PaddingBottom = 2;
                tb3.AddCell(sellerBankName);

                var buyerBankAccount = new PdfPCell();
                buyerBankAccount.Phrase = new Paragraph(new Chunk("账号：" + this.SalesContract.Buyer.BankAccount, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                buyerBankAccount.HorizontalAlignment = Element.ALIGN_LEFT;
                buyerBankAccount.DisableBorderSide(-1);
                buyerBankAccount.PaddingBottom = 2;
                tb3.AddCell(buyerBankAccount);

                var sellerBankAccount = new PdfPCell();
                sellerBankAccount.Phrase = new Paragraph(new Chunk("账号：" + this.SalesContract.Seller.BankAccount, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                sellerBankAccount.HorizontalAlignment = Element.ALIGN_LEFT;
                sellerBankAccount.DisableBorderSide(-1);
                sellerBankAccount.PaddingBottom = 2;
                tb3.AddCell(sellerBankAccount);

                var buyerAddress = new PdfPCell();
                buyerAddress.Phrase = new Paragraph(new Chunk("地址：" + this.SalesContract.Buyer.Address, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                buyerAddress.HorizontalAlignment = Element.ALIGN_LEFT;
                buyerAddress.DisableBorderSide(-1);
                buyerAddress.PaddingBottom = 2;
                tb3.AddCell(buyerAddress);

                var sellerAddress = new PdfPCell();
                sellerAddress.Phrase = new Paragraph(new Chunk("地址：" + this.SalesContract.Seller.Address, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                sellerAddress.HorizontalAlignment = Element.ALIGN_LEFT;
                sellerAddress.DisableBorderSide(-1);
                sellerAddress.PaddingBottom = 2;
                tb3.AddCell(sellerAddress);

                var buyerTel = new PdfPCell();
                buyerTel.Phrase = new Paragraph(new Chunk("电话：" + this.SalesContract.Buyer.Tel, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                buyerTel.HorizontalAlignment = Element.ALIGN_LEFT;
                buyerTel.DisableBorderSide(-1);
                buyerTel.PaddingBottom = 2;
                tb3.AddCell(buyerTel);

                var sellerTel = new PdfPCell();
                sellerTel.Phrase = new Paragraph(new Chunk("电话：" + this.SalesContract.Seller.Tel, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                sellerTel.HorizontalAlignment = Element.ALIGN_LEFT;
                sellerTel.DisableBorderSide(-1);
                sellerTel.PaddingBottom = 2;
                tb3.AddCell(sellerTel);

                var buyerFax = new PdfPCell();
                buyerFax.Phrase = new Paragraph(new Chunk("传真：" + this.SalesContract.Buyer.Fax, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                buyerFax.HorizontalAlignment = Element.ALIGN_LEFT;
                buyerFax.DisableBorderSide(-1);
                buyerFax.PaddingBottom = 2;
                tb3.AddCell(buyerFax);

                var sellerFax = new PdfPCell();
                sellerFax.Phrase = new Paragraph(new Chunk("传真：" + this.SalesContract.Seller.Fax, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                sellerFax.HorizontalAlignment = Element.ALIGN_LEFT;
                sellerFax.DisableBorderSide(-1);
                sellerFax.PaddingBottom = 2;
                tb3.AddCell(sellerFax);

                document.Add(tb3);
                #endregion

                #region 表格3：合同条款1
                //空白间隔
                document.Add(new Paragraph(new Chunk(" ", new Font(baseFont, 5f))));

                var tb4 = new PdfPTable(1);
                tb4.HorizontalAlignment = 0;
                tb4.TotalWidth = 574;//594磅
                tb4.LockedWidth = true;
                tb4.SetWidths(new float[] { 574 });

                var note0 = new PdfPCell();
                note0.Phrase = new Paragraph(new Phrase(@"根据《中华人民共和国合同法》及相关法律法规，本着平等自愿、等价有偿、诚实信用的原则，经买卖双方同意由卖方出售买方购进如下货物，并按下列条款签定本合同 ：", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                note0.HorizontalAlignment = Element.ALIGN_LEFT;
                note0.DisableBorderSide(-1);
                note0.PaddingBottom = 2;
                tb4.AddCell(note0);

                var note1 = new PdfPCell();
                note1.Phrase = new Paragraph(new Phrase("1.产品名称、规格、数量、金额：", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                note1.HorizontalAlignment = Element.ALIGN_LEFT;
                note1.DisableBorderSide(-1);
                note1.PaddingBottom = 5;
                tb4.AddCell(note1);

                document.Add(tb4);

                #endregion

                #region  表格内容4 ：表体

                var tb5 = new PdfPTable(8);
                tb5.HorizontalAlignment = 0;
                tb5.TotalWidth = 574;
                tb5.LockedWidth = true;
                tb5.SetWidths(new float[] { 17.22f, 114.8f, 114.8f, 34.44f, 34.44f, 34.44f, 34.44f, 34.44f });

                tb5.AddCell(new PdfPCell(new Phrase("行号", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase("品名", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase("规格型号", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase("数量", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase("单位", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase("单价", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase("总价", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase("币值", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });

                int sn = 0;
                //var units = new Views.BaseUnitsView().ToList();
                foreach (var item in this.SalesContract.ContractItems)
                {
                    sn++;
                    //型号内容
                    tb5.AddCell(new PdfPCell(new Phrase(sn.ToString(), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb5.AddCell(new PdfPCell(new Phrase(item.ProductName.ToString(), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb5.AddCell(new PdfPCell(new Phrase(item.Model.ToString(), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb5.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString("0.####"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb5.AddCell(new PdfPCell(new Phrase(item.Unit, FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });//units.Where(u => u.Code == item.Unit).FirstOrDefault()?.Name ?? item.Unit
                    tb5.AddCell(new PdfPCell(new Phrase(item.UnitPrice.ToString("0.0000"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb5.AddCell(new PdfPCell(new Phrase(item.TotalPrice.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb5.AddCell(new PdfPCell(new Phrase("RMB", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                }

                //合计行
                tb5.AddCell(new PdfPCell(new Phrase("合计Total：", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE, Colspan = 3 });
                tb5.AddCell(new PdfPCell(new Phrase(this.SalesContract.ContractItems.Select(item =>item.Quantity).Sum().ToString("0.####"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase("", FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase("", FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase(this.SalesContract.ContractItems.Select(item => item.TotalPrice).Sum().ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });//this.Items.Select(item => item.TotalPrice).Sum().ToString("0.00")
                tb5.AddCell(new PdfPCell(new Phrase("", FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });

                document.Add(tb5);

                #endregion

                #region 表格内容5：合同条款

                var tb6 = new PdfPTable(2);
                tb6.HorizontalAlignment = 0;
                tb6.TotalWidth = 574;//594磅
                tb6.LockedWidth = true;
                tb6.SetWidths(new float[] { 287, 287 });

                var note2 = new PdfPCell();
                note2.Phrase = new Paragraph(new Phrase("2.质量要求：按照买卖双方约定；" , FontContent))
                {
                    Alignment = Rectangle.ALIGN_CENTER,
                };
                note2.DisableBorderSide(-1);
                note2.PaddingTop = 4;
                tb6.AddCell(note2);

                var note3 = new PdfPCell();
                note3.Phrase = new Paragraph(new Phrase("3.付款方式：转账；", FontContent))
                {
                    Alignment = Rectangle.ALIGN_CENTER,
                };
                note3.DisableBorderSide(-1);
                note3.PaddingTop = 4;
                tb6.AddCell(note3);

                var note4 = new PdfPCell();
                note4.Phrase = new Paragraph(new Phrase("4.交货地点：买方指定地点；", FontContent))
                {
                    Alignment = Rectangle.ALIGN_CENTER,
                };
                note4.DisableBorderSide(-1);
                note4.PaddingTop = 4;
                tb6.AddCell(note4);

                var note5 = new PdfPCell();
                note5.Phrase = new Paragraph(new Phrase("5.交货期限：约定；", FontContent))
                {
                    Alignment = Rectangle.ALIGN_CENTER,
                };
                note5.DisableBorderSide(-1);
                note5.PaddingTop = 4;
                tb6.AddCell(note5);

                document.Add(tb6);

                var tb7 = new PdfPTable(1);
                tb7.HorizontalAlignment = 0;
                tb7.TotalWidth = 574;//594磅
                tb7.LockedWidth = true;
                tb7.SetWidths(new float[] { 574 });

                var note6 = new PdfPCell();
                note6.Phrase = new Paragraph(new Phrase("6.付款期限：按双方签订的《供应链服务协议》执行；", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT
                };
                note6.DisableBorderSide(-1);
                note6.PaddingTop = 4;
                tb7.AddCell(note6);

                var note7 = new PdfPCell();
                note7.Phrase = new Paragraph(new Phrase("7.运输及保险：运输方式由买方发货前确认，由此产生的运输费用及保险费用按照双方签订的《供应链服务协议 》执行；", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT
                };
                note7.DisableBorderSide(-1);
                note7.PaddingTop = 4;
                tb7.AddCell(note7);

                var note8 = new PdfPCell();
                note8.Phrase = new Paragraph(new Phrase("8.解决合同争议：在执行本协议过程中所发生的纠纷应首先通过友好协商解决；协商不成的，任何一方均可向深圳市龙岗区人民法院提起诉讼；", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT
                };
                note8.DisableBorderSide(-1);
                note8.PaddingTop = 4;
                tb7.AddCell(note8);

                var note9 = new PdfPCell();
                note9.Phrase = new Paragraph(new Phrase("9.合同有效期：经方盖章、签字后生效，有效期至" + this.SalesContract.ValidDate +"；", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT
                };
                note9.DisableBorderSide(-1);
                note9.PaddingTop = 4;
                tb7.AddCell(note9);

                var note10 = new PdfPCell();
                note10.Phrase = new Paragraph(new Phrase("10.本合同一式两份，双方各执一份。", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT
                };
                note10.DisableBorderSide(-1);
                note10.PaddingTop = 4;
                tb7.AddCell(note10);

                document.Add(tb7);

                #endregion

                #region 表格内容8：盖章

                var tb8 = new PdfPTable(2);
                tb8.HorizontalAlignment = 0;
                tb8.TotalWidth = 574;//594磅
                tb8.LockedWidth = true;
                //tb8.DisableBorderSide(-1);
                tb8.SetWidths(new float[] { 287, 287 });

                var buyerName = new PdfPCell();
                buyerName.Phrase = new Paragraph(new Phrase("\n" + "买方：" + "\n ", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                buyerName.DisableBorderSide(-1);
                buyerName.PaddingTop = 5;
                tb8.AddCell(buyerName);

                var sellerName = new PdfPCell();
                sellerName.Phrase = new Paragraph(new Phrase("\n" + "卖方：" +this.SalesContract.Seller.Title+ "\n ", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                sellerName.DisableBorderSide(-1);
                sellerName.PaddingTop = 5;
                tb8.AddCell(sellerName);

                var buyerSeal = new PdfPCell();
                buyerSeal.Phrase = new Paragraph(new Phrase("\n" + "签字盖章：" + "\n ", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                buyerSeal.DisableBorderSide(-1);
                buyerSeal.PaddingTop = 2;
                tb8.AddCell(buyerSeal);

                var sellerSeal = new PdfPCell();
                sellerSeal.Phrase = new Paragraph(new Phrase("\n" + "签字盖章：" + "\n ", FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                sellerSeal.DisableBorderSide(-1);
                sellerSeal.PaddingTop = 2;
                tb8.AddCell(sellerSeal);


                var c1 = new PdfPCell();
                c1.DisableBorderSide(-1);
                tb8.AddCell(c1);

                //Image img2 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.SalesContract.Seller.SealUrl));
                //img2.ScalePercent(75f);
                //var c2 = new PdfPCell(img2)
                //{
                //    VerticalAlignment = Rectangle.ALIGN_MIDDLE,
                //    PaddingLeft = 55f,
                //    PaddingTop = -75
                //};
                //c2.DisableBorderSide(-1);
                //tb8.AddCell(c2);

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

    }

}

