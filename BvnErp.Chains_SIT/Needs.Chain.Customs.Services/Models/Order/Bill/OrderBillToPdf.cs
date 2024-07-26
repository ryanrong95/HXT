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
using System.Diagnostics;

namespace Needs.Ccs.Services.Models
{
    public class OrderBillToPdf : ITextPDFBase
    {
        private MainOrderBillViewModel billview;

        public OrderBillToPdf() { }

        public OrderBillToPdf(MainOrderBillViewModel view)
        {
            this.billview = view;
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
                titleCell.Phrase = new Paragraph(new Chunk("委托进口货物报关对帐单", FontTitle))
                {
                    Alignment = Rectangle.ALIGN_CENTER,
                };
                titleCell.DisableBorderSide(-1);
                titleCell.FixedHeight = 50;
                titleCell.PaddingTop = 5;
                tb1.AddCell(titleCell);

                //二维码
                BarcodeQRCode code = new BarcodeQRCode(billview.MainOrderID, 58, 58, null);
                var ewmCell = new PdfPCell(code.GetImage())
                {
                    HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                    VerticalAlignment = Rectangle.ALIGN_MIDDLE
                };
                ewmCell.DisableBorderSide(-1);
                ewmCell.FixedHeight = 50;
                ewmCell.PaddingRight = 20;
                ewmCell.PaddingTop = -20;
                tb1.AddCell(ewmCell);

                document.Add(tb1);

                #endregion

                #region 内容表格1

                //订单号
                var tbOrderNo = new PdfPTable(1);
                var ordercell = new PdfPCell();
                ordercell.Phrase = new Paragraph(new Chunk("主订单编号:" + billview.MainOrderID, FontContent))
                {
                    Alignment = Rectangle.ALIGN_LEFT,
                };
                ordercell.DisableBorderSide(-1);
                ordercell.PaddingBottom = 5;
                ordercell.PaddingLeft = -56;
                tbOrderNo.AddCell(ordercell);
                document.Add(tbOrderNo);

                #endregion

                #region 内容表格2：委托方等信息

                var tb2 = new PdfPTable(2);
                tb2.HorizontalAlignment = 0;
                tb2.TotalWidth = 574;//594磅
                tb2.LockedWidth = true;
                tb2.SetWidths(new float[] { 287, 287 });

                PdfPCell 委托方 = new PdfPCell(new Phrase("委托方: " + billview.ClientName, FontContent))
                {
                    BorderWidth = tbLineWidth,
                };

                PdfPCell 被委托方 = new PdfPCell(new Phrase("被委托方: " + billview.AgentName, FontContent))
                {
                    BorderWidth = tbLineWidth,
                };
                PdfPCell 委托方电话 = new PdfPCell(new Phrase("电话：" + billview.ClientTel, FontContent))
                {
                    BorderWidth = tbLineWidth,
                };
                PdfPCell 被委托方电话 = new PdfPCell(new Phrase("电话：" + billview.AgentTel, FontContent))
                {
                    BorderWidth = tbLineWidth,
                };

                tb2.AddCell(委托方);
                tb2.AddCell(被委托方);
                tb2.AddCell(委托方电话);
                tb2.AddCell(被委托方电话);

                document.Add(tb2);

                #endregion

                #region 表格内容3：小订单列表

                //空白间隔
                document.Add(new Paragraph(new Chunk(" ", new Font(baseFont, 3f))));

                var tb3 = new PdfPTable(2);
                tb3.HorizontalAlignment = 0;
                tb3.TotalWidth = 574;//594磅
                tb3.LockedWidth = true;
                tb3.SetWidths(new float[] { 287, 287 });


                foreach (var bill in billview.Bills)
                {
                    //
                    PdfPCell 合同号 = new PdfPCell(new Phrase("订单编号：" + bill.OrderID + (bill.ContrNo == null ? "" : (" 合同号：" + bill.ContrNo)), FontContent))
                    {
                        BorderWidth = tbLineWidth,
                    };

                    PdfPCell 汇率 = new PdfPCell(new Phrase("实时汇率：" + bill.RealExchangeRate + " 海关汇率：" + bill.CustomsExchangeRate, FontContent))
                    {
                        BorderWidth = tbLineWidth,
                    };

                    tb3.AddCell(合同号);
                    tb3.AddCell(汇率);
                }

                document.Add(tb3);

                #endregion 

                #region  表格内容4 ：表体

                //空白间隔
                document.Add(new Paragraph(new Chunk(" ", new Font(baseFont, 3f))));

                var tb4 = new PdfPTable(15);
                tb4.HorizontalAlignment = 0;
                tb4.TotalWidth = 574;//594磅
                tb4.LockedWidth = true;
                tb4.SetWidths(new float[] { 17.40f, 74.62f, 74.62f, 34.44f, 40.18f, 40.18f, 40f, 40.18f, 28.7f, 34.44f, 34.44f, 34.44f, 28.7f, 40.18f, 45.92f });

                tb4.AddCell(new PdfPCell(new Phrase("序号", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("报关品名", FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("规格型号", FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("数量", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("报关单价" + "\n" + "(" + billview.Currency + ")", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase("报关总价" + "\n" + "(" + billview.Currency + ")", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase("关税率", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb4.AddCell(new PdfPCell(new Phrase("报关货值" + "\n" + "(CNY)", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase("关税" + "\n" + "(CNY)", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase("消费税" + "\n" + "(CNY)", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase("增值税" + "\n" + "(CNY)", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase("代理费" + "\n" + "(CNY)", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase("杂费" + "\n" + "(CNY)", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase("税费合计" + "\n" + "(CNY)", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase("报关总金额" + "\n" + "(CNY)", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_CENTER });


                int sn = 0;
                decimal subtotalPrice = billview.summaryTotalPrice, subtotalCNYPrice = billview.summaryTotalCNYPrice;
                decimal subtotalAgencyFee = billview.summaryTotalAgencyFee, subtotalIncidentalFee = billview.summaryTotalIncidentalFee;
                decimal subtotalTraiff = billview.summaryTotalTariff.Value, subtotalExciseTax = billview.summaryTotalExciseTax.Value, subtotalAddedValueTax = billview.summaryTotalAddedValueTax.Value;
                decimal subtotalTaxFee = billview.summaryPay.Value, subtotalAmount = billview.summaryPayAmount.Value;

                foreach (var item in billview.ProductsForIcgoo)
                {
                    #region 报关商品明细
                    sn++;
                    //型号内容
                    tb4.AddCell(new PdfPCell(new Phrase(sn.ToString(), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase(item.ProductName, FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.Model, FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                    tb4.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString("0.####"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase(item.UnitPrice.ToString("0.0000"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase(item.TotalPrice.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase(item.TariffRate.ToString("0.0000"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase(item.TotalCNYPrice.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase(item.Traiff.Value.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase(item.ExciseTax.Value.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase(item.AddedValueTax.Value.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase(item.AgencyFee.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase(item.IncidentalFee.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase((item.Traiff.Value + item.ExciseTax.Value + item.AddedValueTax.Value + item.AgencyFee + item.IncidentalFee).ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                    tb4.AddCell(new PdfPCell(new Phrase((item.TotalCNYPrice + item.Traiff.Value + item.ExciseTax.Value + item.AddedValueTax.Value + item.AgencyFee + item.IncidentalFee).ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });

                    #endregion
                }

                //合计行
                tb4.AddCell(new PdfPCell(new Phrase("合计", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Rectangle.ALIGN_MIDDLE, Colspan = 3 });
                tb4.AddCell(new PdfPCell(new Phrase(billview.summaryTotalQty1.ToString("0.####"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });//数量
                tb4.AddCell(new PdfPCell(new Phrase(" ", FontContent)) { BorderWidth = tbLineWidth });
                tb4.AddCell(new PdfPCell(new Phrase(billview.summaryTotalPrice.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase(" ", FontContent)) { BorderWidth = tbLineWidth });
                tb4.AddCell(new PdfPCell(new Phrase(billview.summaryTotalCNYPrice.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase(billview.summaryTotalTariff.Value.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase(billview.summaryTotalExciseTax.Value.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase(billview.summaryTotalAddedValueTax.Value.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase(billview.summaryTotalAgencyFee.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase(billview.summaryTotalIncidentalFee.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase(billview.summaryPay.Value.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });
                tb4.AddCell(new PdfPCell(new Phrase(billview.summaryPayAmount.Value.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth, VerticalAlignment = Rectangle.ALIGN_MIDDLE, HorizontalAlignment = Rectangle.ALIGN_CENTER });

                document.Add(tb4);

                #endregion

                #region 表格内容5：总计

                //空白间隔
                document.Add(new Paragraph(new Chunk(" ", new Font(baseFont, 3f))));

                var tb5 = new PdfPTable(2);
                tb5.HorizontalAlignment = 0;
                tb5.TotalWidth = 574;//594磅
                tb5.LockedWidth = true;
                tb5.SetWidths(new float[] { 459.2f, 114.8f });

                tb5.AddCell(new PdfPCell(new Phrase("货值小计：", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_RIGHT, VerticalAlignment = Rectangle.ALIGN_MIDDLE });
                tb5.AddCell(new PdfPCell(new Phrase(billview.Currency + " " + subtotalPrice.ToRound(2).ToString("0.00") + "\n" + "CNY" + subtotalCNYPrice.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth });
                tb5.AddCell(new PdfPCell(new Phrase("税代费小计：", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_RIGHT });
                tb5.AddCell(new PdfPCell(new Phrase("CNY " + subtotalTaxFee.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth });
                tb5.AddCell(new PdfPCell(new Phrase("应收总金额合计", FontContent)) { BorderWidth = tbLineWidth, HorizontalAlignment = Rectangle.ALIGN_RIGHT });
                tb5.AddCell(new PdfPCell(new Phrase("CNY " + subtotalAmount.ToRound(2).ToString("0.00"), FontContent)) { BorderWidth = tbLineWidth });

                document.Add(tb5);

                #endregion

                #region 表格内容6：银行

                //空白间隔
                document.Add(new Paragraph(new Chunk(" ", new Font(baseFont, 3f))));

                var tb6 = new PdfPTable(1);
                tb6.HorizontalAlignment = 0;
                tb6.TotalWidth = 574;//594磅
                tb6.LockedWidth = true;
                tb6.SetWidths(new float[] { 574 });

                tb6.AddCell(new PdfPCell(new Phrase("深圳市华芯通供应链管理有限公司 深圳市龙华区龙华街道富康社区天汇大厦C栋212 电话：0755-28014789 传真：0755-28014789", FontContent)) { BorderWidth = tbLineWidth });
                tb6.AddCell(new PdfPCell(new Phrase("开户行:中国银行深圳罗岗支行 开户名：深圳市华芯通供应链管理有限公司 账户：86021110000213646", FontContent)) { BorderWidth = tbLineWidth });

                document.Add(tb6);

                //空白间隔
                document.Add(new Paragraph(new Chunk(" ", new Font(baseFont, 3f))));

                var tb7 = new PdfPTable(1);
                tb7.HorizontalAlignment = 0;
                tb7.TotalWidth = 574;//594磅
                tb7.LockedWidth = true;
                tb7.SetWidths(new float[] { 574 });
                tb7.AddCell(new PdfPCell(new Phrase("备注:\r\n" +
                                //"1.我司" + billview.AgentName + "为委托方代垫" +
                                //"本金(" + (billview.IsLoan ? subtotalCNYPrice.ToRound(2).ToString("0.00") : "0.00") + "元)" +
                                //"+关税(" + subtotalTraiff.ToRound(2).ToString("0.00") + "元)" +
                                //"+消费税(" + subtotalExciseTax.ToRound(2).ToString("0.00") + "元)" +
                                //"+增值税(" + subtotalAddedValueTax.ToRound(2).ToString("0.00") + "元)" +
                                //"+代理费(" + subtotalAgencyFee.ToRound(2).ToString("0.00") + "元)" +
                                //"+杂费(" + (subtotalIncidentalFee).ToRound(2).ToString("0.00") + "元)," +
                                //"共计应收人民币(" + (billview.IsLoan ? subtotalAmount.ToRound(2).ToString("0.00") : subtotalTaxFee.ToRound(2).ToString("0.00")) + "元)，" +
                                //"委托方需在(" + billview.DueDate + ")前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。\r\n" +
                                //"2.客户在90天内完成付汇手续，付汇汇率为实际付汇当天的中国银行上午十点后的第一个现汇卖出价。\r\n" +
                                "1.委托方需在报关协议约定的付款日前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。\r\n" +
                                "2.委托方在90天内完成付汇，付汇汇率为报关协议约定的实际付汇当天的汇率。\r\n" +
                                "3.委托方应在收到帐单之日起二个工作日内签字和盖章确认回传。\r\n" +
                                "4.此传真件、扫描件、复印件与原件具有同等法律效力。\r\n" +
                                "5.如若对此帐单有发生争议的，双方应友好协商解决，如协商不成的，可通过被委托方所在地人民法院提起诉讼解决。", FontContent))
                { BorderWidth = tbLineWidth });

                document.Add(tb7);

                #endregion 

                #region 表格内容8：盖章

                var tb8 = new PdfPTable(2);
                tb8.HorizontalAlignment = 0;
                tb8.TotalWidth = 574;//594磅
                tb8.LockedWidth = true;
                tb8.SetWidths(new float[] { 287, 287 });

                var 委托方Cell = new PdfPCell(new Phrase("\n" + "委托方确认签字或盖章：" + "\n ", FontContent))
                {
                    BorderWidth = tbLineWidth,
                };
                var 被委托方Cell = new PdfPCell(new Phrase("\n" + "被委托方签字或盖章：" + "\n ", FontContent))
                {
                    BorderWidth = tbLineWidth,
                };
                tb8.AddCell(委托方Cell);
                tb8.AddCell(被委托方Cell);


                Image img1 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", billview.ClientName + ".png"));
                img1.ScalePercent(75f);
                var c1 = new PdfPCell(img1)
                {
                    VerticalAlignment = Rectangle.ALIGN_MIDDLE,
                    PaddingLeft = 70f,
                    PaddingTop = -75
                };
                c1.DisableBorderSide(-1);
                tb8.AddCell(c1);

                Image img2 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, billview.SealUrl));
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
            var tempPath = filePath.Replace(".pdf", "_1.pdf");
            Document doc = this.ToDocument(tempPath);
            //关闭document,导出
            doc.Close();

            //加水印
            setWatermark(tempPath, filePath, billview.AgentName);

        }

        public void SaveAsForTest(string filePath, ref string generateTime, ref string fileTime, ref string watermarkTime)
        {
            Stopwatch stopWatch1 = new Stopwatch();
            stopWatch1.Start();
            var tempPath = filePath.Replace(".pdf", "_1.pdf");
            Document doc = this.ToDocument(tempPath);
            stopWatch1.Stop();
            string s1 = stopWatch1.ElapsedMilliseconds.ToString();
            generateTime = s1;

            Stopwatch stopWatch2 = new Stopwatch();
            stopWatch2.Start();
            //关闭document,导出
            doc.Close();
            stopWatch2.Stop();
            string s2 = stopWatch2.ElapsedMilliseconds.ToString();
            fileTime = s2;

            Stopwatch stopWatch3 = new Stopwatch();
            stopWatch3.Start();
            //加水印
            setWatermark(tempPath, filePath, billview.AgentName);
            stopWatch3.Stop();
            string s3 = stopWatch3.ElapsedMilliseconds.ToString();
            watermarkTime = s3;

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
                GetImage(total - 1, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, billview.SealUrl), billview.AgentName);
                //大赢家图片
                GetImage(total - 1, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", billview.ClientName + ".png"), billview.ClientName);

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                for (var i = 1; i < total; i++)
                {
                    content = pdfStamper.GetOverContent(i);
                    gs.FillOpacity = 1f;
                    content.SetGState(gs);

                    string clientPath = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", billview.ClientName, (total - 1).ToString(), (i - 1).ToString() + ".png");
                    Image img1 = Image.GetInstance(clientPath);
                    img1.ScalePercent(75f);
                    img1.SetAbsolutePosition(width - img1.Width * 0.75f, height / 2 - img1.Height * 0.75f - 160f);
                    content.AddImage(img1);


                    string agentPath = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", billview.AgentName, (total - 1).ToString(), (i - 1).ToString() + ".png");
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

            var cell = new PdfPCell(new Paragraph(billview.AgentName, new Font(footFont, 8, Font.NORMAL)))
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
