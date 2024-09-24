using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.SpirePdf;
using Needs.Wl.Models;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using Spire.Pdf.Tables;
using PdfDocument = Needs.Utils.SpirePdf.PdfDocument;
using System.Drawing;
using Needs.Ccs.Services.Enums;
using System.IO;
using Layers.Data.Sqls;
using Yahv.Services.Models;

namespace Needs.Ccs.Services.Models
{

    public class MainOrderBillViewModel
    {
        public string MainOrderID { get; set; }
        public string ClientName { get; set; }
        public string ClientCode { get; set; }
        public string ClientTel { get; set; }
        public string ClientAddress { get; set; }
        public ClientType ClientType { get; set; }
        public string AgentName { get; set; }
        public string AgentAddress { get; set; }
        public string AgentTel { get; set; }
        public string AgentFax { get; set; }
        public string Purchaser { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
        public string AccountId { get; set; }
        public string SealUrl { get; set; }
        public string Currency { get; set; }
        public string FileID { get; set; }
        public string FileStatus { get; set; }
        public string FileName { get; set; }
        public OrderFileStatus FileStatusValue { get; set; }
        public string Url { get; set; }
        /// <summary>
        /// 是否代垫货款
        /// </summary>
        public bool IsLoan { get; set; }
        public string DueDate { get; set; }
        public string CreateDate { get; set; }
        /// <summary>
        /// 客户端 对账单用
        /// </summary>
        public decimal summaryTotalPrice { get; set; }
        /// <summary>
        /// 客户端 对账单用
        /// </summary>
        public decimal summaryTotalCNYPrice { get; set; }
        //外单有点垫资 对账单使用 
        public decimal summaryTotalCNYPrice1 { get; set; }
        public decimal? summaryPay { get; set; }
        public decimal? summaryPayAmount { get; set; }
        public decimal? summaryTotalTariff { get; set; }
        public decimal? summaryTotalExciseTax { get; set; }
        public decimal? summaryTotalAddedValueTax { get; set; }
        public decimal summaryTotalAgencyFee { get; set; }
        public decimal summaryTotalIncidentalFee { get; set; }
        public decimal summaryTotalQty { get; set; }

        public decimal summaryTotalQty1 { get; set; }

        public bool HasYBZ { get; set; }

        public ClientAgreement Agreement { get; set; }
        /// <summary>
        /// 显示用
        /// </summary>
        public List<MainOrderBillItem> Bills { get; set; }

        public List<string> OrderIDs { get; set; }


        #region Icgoo要的详情
        public List<MainOrderBillItemProduct> ProductsForIcgoo { get; set; }

        public List<MainOrderBillItemProduct> PartProductsForIcgoo { get; set; }
        #endregion


        /// <summary>
        /// 导出PDF
        /// </summary>
        public Utils.SpirePdf.PdfDocument ToPdf()
        {

            #region pdf对象声明

            //创建一个PdfDocument类对象
            Utils.SpirePdf.PdfDocument pdf = new PdfDocument();
            pdf.PageSettings.Margins = new PdfMargins(10, 60, 10, 10);

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4);
            int pageCount = pdf.Pages.Count;

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 14f, FontStyle.Bold), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头
            float x = 0, y = 5f;

            float width = page.Canvas.ClientSize.Width;
            string message = "委托进口货物报关对帐单";
            page.Canvas.DrawString(message, font1, brush, width / 2, y, formatCenter);

            y += font1.MeasureString(message, formatCenter).Height + 8;

            //创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;

            //表头信息
            PdfGridRow row = grid.Rows.Add();
            row.Cells[0].Value = "委托方: " + this.ClientName;
            row.Cells[1].Value = "被委托方: " + this.AgentName;
            row = grid.Rows.Add();
            row.Cells[0].Value = "电话：" + this.ClientTel;
            row.Cells[1].Value = "电话：" + this.AgentTel;

            //设置边框
            SetCellBorder(grid);

            PdfLayoutResult result = grid.Draw(page, new PointF(x, y));
            y += result.Bounds.Height + 5;

            #endregion




            decimal subtotalQty = 0;
            decimal subtotalPrice = 0, subtotalCNYPrice = 0;
            decimal subtotalAgencyFee = 0, subtotalIncidentalFee = 0;
            decimal subtotalTraiff = 0, subtotalExciseTax = 0, subtotalAddedValueTax = 0;
            decimal subtotalTaxFee = 0, subtotalAmount = 0;


            //2023-09-28 更新
            //特殊客户单抬头开票增值税不包含运保杂计算，当前有两个客户webconfig
            var NoTransPremiumInsurance = System.Configuration.ConfigurationManager.AppSettings["NoTransPremiumInsurance"];
            var HasYBZ = !NoTransPremiumInsurance.Split(',').Contains(this.ClientCode);

            foreach (var bill in this.Bills)
            {
                #region 报关商品明细

                //创建一个PdfGrid对象
                grid = new PdfGrid();
                grid.Style.Font = font2;

                //设置列宽
                grid.Columns.Add(15);
                grid.Columns[0].Width = width * 3f / 100;
                grid.Columns[1].Width = width * 12f / 100;
                grid.Columns[2].Width = width * 12f / 100;
                grid.Columns[3].Width = width * 5f / 100;
                grid.Columns[4].Width = width * 7f / 100;
                grid.Columns[5].Width = width * 7f / 100;
                grid.Columns[6].Width = width * 5f / 100;
                grid.Columns[7].Width = width * 7f / 100;
                grid.Columns[8].Width = width * 5f / 100;
                grid.Columns[9].Width = width * 6f / 100;
                grid.Columns[10].Width = width * 6f / 100;
                grid.Columns[11].Width = width * 6f / 100;
                grid.Columns[12].Width = width * 5f / 100;
                grid.Columns[13].Width = width * 7f / 100;
                grid.Columns[14].Width = width * 7f / 100;

                //汇率 合同信息
                row = grid.Rows.Add();
                row.Cells[0].ColumnSpan = 7;
                row.Cells[0].Value = "订单编号：" + bill.OrderID + (bill.ContrNo == null ? "" : (" 合同号：" + bill.ContrNo));
                row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                row.Cells[7].ColumnSpan = 8;
                row.Cells[7].Value = "实时汇率：" + bill.RealExchangeRate + (HasYBZ ? (" 海关汇率：" + bill.CustomsExchangeRate) : "");
                row.Cells[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);



                //产品信息
                row = grid.Rows.Add();
                row.Cells[0].Value = "序号";
                row.Cells[1].Value = "报关品名";
                row.Cells[2].Value = "规格型号";
                row.Cells[3].Value = "数量";
                row.Cells[4].Value = "报关单价" + "(" + this.Currency + ")";
                row.Cells[5].Value = "报关总价" + "(" + this.Currency + ")";
                row.Cells[6].Value = "关税率";
                row.Cells[7].Value = "报关货值(CNY)";
                row.Cells[8].Value = "关税(CNY)";
                row.Cells[9].Value = "消费税(CNY)";
                row.Cells[10].Value = "增值税(CNY)";
                row.Cells[11].Value = "代理费(CNY)";
                row.Cells[12].Value = "杂费(CNY)";
                row.Cells[13].Value = "税费合计(CNY)";
                row.Cells[14].Value = "报关总金额(CNY)";


                int sn = 0;
                decimal totalQty = 0;
                decimal totalPrice = 0, totalCNYPrice = 0;
                decimal totalAgencyFee = 0, totalIncidentalFee = 0;
                decimal totalTraiff = 0, totalExciseTax = 0, totalAddedValueTax = 0;
                decimal totalTaxFee = 0, totalAmount = 0;

                totalAgencyFee = bill.AgencyFee;
                subtotalAgencyFee += totalAgencyFee;
                foreach (var item in bill.Products)
                {

                    totalQty += item.Quantity;
                    totalPrice += item.TotalPrice;
                    totalCNYPrice += item.TotalCNYPrice;
                    totalTraiff += item.Traiff.Value;
                    totalExciseTax += item.ExciseTax.Value;
                    totalAddedValueTax += item.AddedValueTax.Value;
                    //totalAgencyFee += item.AgencyFee;
                    totalIncidentalFee += item.IncidentalFee;

                    sn++;
                    row = grid.Rows.Add();
                    row.Cells[0].Value = sn.ToString();
                    row.Cells[1].Value = item.ProductName;
                    row.Cells[2].Value = item.Model;
                    row.Cells[3].Value = item.Quantity.ToString("0.####");
                    row.Cells[4].Value = item.UnitPrice.ToString("0.0000");
                    row.Cells[5].Value = item.TotalPrice.ToRound(2).ToString("0.00");
                    row.Cells[6].Value = item.TariffRate.ToString("0.0000");
                    row.Cells[7].Value = item.TotalCNYPrice.ToRound(2).ToString("0.00");
                    row.Cells[8].Value = item.Traiff.Value.ToRound(2).ToString("0.00");
                    row.Cells[9].Value = item.ExciseTax.Value.ToRound(2).ToString("0.00");
                    row.Cells[10].Value = item.AddedValueTax.Value.ToRound(2).ToString("0.00");
                    row.Cells[11].Value = item.AgencyFee.ToRound(2).ToString("0.00");
                    row.Cells[12].Value = item.IncidentalFee.ToRound(2).ToString("0.00");
                    row.Cells[13].Value = (item.Traiff.Value + item.ExciseTax.Value + item.AddedValueTax.Value + item.AgencyFee + item.IncidentalFee).ToRound(2).ToString("0.00");
                    row.Cells[14].Value = (item.TotalCNYPrice + item.Traiff.Value + item.ExciseTax.Value + item.AddedValueTax.Value + item.AgencyFee + item.IncidentalFee).ToRound(2).ToString("0.00");
                    //row.Cells[12].Value = (itemTariff + itemAddedValueTax + itemAgencyFee + itemInspFee + aveOtherFee).ToRound(2).ToString("0.00");
                    //row.Cells[13].Value = (item.TotalPrice * this.ProductFeeExchangeRate + itemTariff + itemAddedValueTax + itemAgencyFee + itemInspFee + aveOtherFee).ToRound(2).ToString("0.00");
                }

                //内单和Icgoo的对账单如果关税总和小于50，则显示0
                //ryan 20210113 外单税费小于50不收 钟苑平
                //if (bill.OrderType != OrderType.Outside && totalTraiff < 50)
                if (totalTraiff < 50)
                {
                    totalTraiff = 0;
                }
                //内单和Icgoo的对账单如果消费税总和小于50，则显示0
                //ryan 20210113 外单税费小于50不收 钟苑平
                if (totalExciseTax < 50)
                {
                    totalExciseTax = 0;
                }
                //内单和Icgoo的对账单如果增值税总和小于50，则显示0
                //ryan 20210113 外单税费小于50不收 钟苑平
                if (totalAddedValueTax < 50)
                {
                    totalAddedValueTax = 0;
                }

                totalTaxFee = totalTraiff + totalExciseTax + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;
                totalAmount = totalCNYPrice + totalTraiff + totalExciseTax + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;

                subtotalQty += totalQty;
                subtotalPrice += totalPrice;
                subtotalCNYPrice += totalCNYPrice;
                subtotalTraiff += totalTraiff;
                subtotalExciseTax += totalExciseTax;
                subtotalAddedValueTax += totalAddedValueTax;
                //subtotalAgencyFee += totalAgencyFee;
                subtotalIncidentalFee += totalIncidentalFee;
                subtotalTaxFee += totalTaxFee;
                subtotalAmount += totalAmount;

                //合计行
                row = grid.Rows.Add();
                row.Cells[0].ColumnSpan = 3;
                row.Cells[0].Value = "合计:";
                row.Cells[3].Value = totalQty.ToString("0.####");
                row.Cells[5].Value = totalPrice.ToRound(2).ToString("0.00");
                row.Cells[7].Value = totalCNYPrice.ToRound(2).ToString("0.00");
                row.Cells[8].Value = totalTraiff.ToRound(2).ToString("0.00");
                row.Cells[9].Value = totalExciseTax.ToRound(2).ToString("0.00");
                row.Cells[10].Value = totalAddedValueTax.ToRound(2).ToString("0.00");
                row.Cells[11].Value = totalAgencyFee.ToRound(2).ToString("0.00");
                row.Cells[12].Value = totalIncidentalFee.ToRound(2).ToString("0.00");
                row.Cells[13].Value = totalTaxFee.ToRound(2).ToString("0.00");
                row.Cells[14].Value = totalAmount.ToRound(2).ToString("0.00");
                //row.Cells[12].Value = (totalTraiff + totalAddedValueTax + totalAgencyFee + totalInspFee + totalOtherFee).ToRound(2).ToString("0.00");
                //row.Cells[13].Value = (totalCNYPrice + totalTraiff + totalAddedValueTax + totalAgencyFee + totalInspFee + totalOtherFee).ToRound(2).ToString("0.00");

                foreach (var pgr in grid.Rows)
                {
                    for (int i = 0; i < pgr.Cells.Count; i++)
                    {
                        if (i == 1 || i == 2)
                        {
                            pgr.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                            continue;
                        }
                        pgr.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                    }
                }

                //设置边框
                SetCellBorder(grid);

                //表格换页的时候，留下页脚的空间
                if (y > 760)
                {
                    y += 10;
                }

                result = grid.Draw(page, new PointF(x, y));
                if (pdf.Pages.Count > pageCount)
                    UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
                y += result.Bounds.Height + 5;

                #endregion
            }



            #region 费用合计明细

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width * 0.8f;
            grid.Columns[1].Width = width * 0.2f;

            row = grid.Rows.Add();
            row.Cells[0].Value = "货值小计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = this.Currency + " " + subtotalPrice.ToRound(2).ToString("0.00") + "\r\nCNY " + subtotalCNYPrice.ToRound(2).ToString("0.00");
            //row.Cells[1].Value = this.Currency + " " + totalPrice.ToRound(2).ToString("0.00") + "\r\nCNY " + totalCNYPrice.ToRound(2).ToString("0.00");          
            row = grid.Rows.Add();
            row.Cells[0].Value = "税代费小计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "CNY " + subtotalTaxFee.ToRound(2).ToString("0.00");
            //var totalTaxFee = totalTraiff + totalAddedValueTax + totalAgencyFee + totalInspFee + totalOtherFee;
            //row.Cells[1].Value = "CNY " + totalTaxFee.ToRound(2).ToString("0.00");          
            row = grid.Rows.Add();
            row.Cells[0].Value = "应收总金额合计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "CNY " + subtotalAmount.ToRound(2).ToString("0.00");
            //var totalAmount = totalCNYPrice + totalTraiff + totalAddedValueTax + totalAgencyFee + totalInspFee + totalOtherFee;
            //row.Cells[1].Value = "CNY " + totalAmount.ToRound(2).ToString("0.00");        

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;

            #endregion

            #region 尾

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(1);
            grid.Columns[0].Width = width;

            row = grid.Rows.Add();
            row.Cells[0].Value = this.AgentName + " " + this.AgentAddress + " 电话：" + this.AgentTel + " 传真：" + this.AgentFax;
            row = grid.Rows.Add();
            row.Cells[0].Value = $"开户行:{this.Bank} 开户名：{this.Account} 账户：{this.AccountId}";

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;


            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 2;
            row.Cells[0].Value = "备注:\r\n" +
                                //"1.我司" + this.AgentName + "为委托方代垫" +
                                //"本金(" + (this.summaryTotalCNYPrice1 == 0 ? 0 : this.summaryTotalCNYPrice1) + "元)" +//this.IsLoan ? subtotalCNYPrice.ToRound(2).ToString("0.00") : "0.00"
                                //"+关税(" + subtotalTraiff.ToRound(2).ToString("0.00") + "元)" +
                                //"+消费税(" + subtotalExciseTax.ToRound(2).ToString("0.00") + "元)" +
                                //"+增值税(" + subtotalAddedValueTax.ToRound(2).ToString("0.00") + "元)" +
                                //"+代理费(" + subtotalAgencyFee.ToRound(2).ToString("0.00") + "元)" +
                                //"+杂费(" + (subtotalIncidentalFee).ToRound(2).ToString("0.00") + "元)," +
                                //"共计应收人民币(" + (this.summaryTotalCNYPrice1 == 0 ? subtotalTaxFee.ToRound(2).ToString("0.00") : (summaryTotalCNYPrice1 + subtotalTaxFee).ToRound(2).ToString("0.00")) + "元)，" +//this.IsLoan ? subtotalAmount.ToRound(2).ToString("0.00") : subtotalTaxFee.ToRound(2).ToString("0.00")
                                //"委托方需在(" + this.DueDate + ")前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。\r\n" +
                                //"2.客户在90天内完成付汇手续，付汇汇率为实际付汇当天的中国银行上午十点后的第一个现汇卖出价。\r\n" +
                                "1.委托方需在报关协议约定的付款日前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。\r\n" +
                                "2.委托方在90天内完成付汇，付汇汇率为报关协议约定的实际付汇当天的汇率。\r\n" +
                                "3.委托方应在收到帐单之日起二个工作日内签字和盖章确认回传。\r\n" +
                                "4.此传真件、扫描件、复印件与原件具有同等法律效力。\r\n" +
                                "5.如若对此帐单有发生争议的，双方应友好协商解决，如协商不成的，可通过被委托方所在地人民法院提起诉讼解决。";
            row = grid.Rows.Add();
            row.Height = 30f;
            row.Cells[0].Value = "委托方确认签字或盖章：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "被委托方签字或盖章：";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;




            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.SealUrl));
            page.Canvas.DrawImage(image, 350, y - 80);

            //大赢家加上委托方章
            if (this.ClientType == Enums.ClientType.Internal)
            {
                PdfImage imageInternal = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.ClientName + ".png"));
                page.Canvas.DrawImage(imageInternal, 100, y - 80);
            }

            #endregion



            #region 公共组件

            //页眉、页脚、二维码、水印
            pdf.PdfMargins = new PdfMargins(10, 10);
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(pdf);
            string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.HeaderImg);

            pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, PurchaserContext.Current.OfficalWebsite);
            pdfDocumentHandle.HeaderFooter.GenerateFooter(PurchaserContext.Current.CompanyName);
            pdfDocumentHandle.Barcode.GenerateQRCode(this.MainOrderID, imageUrl);
            pdfDocumentHandle.Watermark.DrawWatermark(PurchaserContext.Current.CompanyName);

            #endregion

            return pdf;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PdfDocument ToPdfNew()
        {
            var currencyName = this.Currency;
            var currencyCode = this.Currency;

            #region pdf对象声明
            //创建一个PdfDocument类对象
            Utils.SpirePdf.PdfDocument pdf = new PdfDocument();
            pdf.PageSettings.Margins = new PdfMargins(20, 50, 20, 10);

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4);
            int pageCount = pdf.Pages.Count;

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 14f, FontStyle.Bold), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            var tbHeight = 12f;
            #endregion

            #region 头
            float x = 0, y = 5f;

            float width = page.Canvas.ClientSize.Width;
            string message = "委托进口货物对账单";
            page.Canvas.DrawString(message, font1, brush, width / 2, y, formatCenter);
            y += font1.MeasureString(message, formatCenter).Height + 8;

            message = "Statement of Entrusted Import Goods";
            page.Canvas.DrawString(message.ToUpper(), new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Bold), true), brush, width / 2, y, formatCenter);
            y += font1.MeasureString(message, formatCenter).Height + 8;

            //创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;

            //表头信息
            PdfGridRow row = grid.Rows.Add();
            row.Cells[0].Value = "委托方: " + this.ClientName;
            row.Cells[1].Value = "代理方: " + this.AgentName;
            row = grid.Rows.Add();
            row.Cells[0].Value = "委托方地址：" + this.ClientAddress;
            row.Cells[1].Value = "代理方地址：" + this.AgentAddress;
            row = grid.Rows.Add();
            row.Cells[0].Value = "委托方电话：" + this.ClientTel;
            row.Cells[1].Value = "代理方电话：" + this.AgentTel;

            //设置无边框
            SetCellNoBorder(grid);

            PdfLayoutResult result = grid.Draw(page, new PointF(x, y));
            y += result.Bounds.Height + 5;


            /////////////////////////////////////////////////////
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;

            //表头信息
            var preAgency = (this.Agreement.PreAgency.HasValue && this.Agreement.PreAgency > 0) ? (this.Agreement.PreAgency.Value.ToString("0.##") + "元 + ") : "";
            var PEIsTen = this.Agreement.IsTen == Enums.PEIsTen.Ten ? "（进口当天中国银行10:00之后第一个外汇卖出价）" : "（进口当天中国银行09:30之后第一个外汇卖出价）";

            //税费汇率
            var TaxExchangeRateName = "";
            if (this.Agreement.TaxFeeClause.ExchangeRateType == ExchangeRateType.Custom)
            {
                TaxExchangeRateName = "（进口当月海关汇率）";
            }
            else if (this.Agreement.TaxFeeClause.ExchangeRateType == ExchangeRateType.RealTime)
            {
                TaxExchangeRateName = "（进口当天中国银行10:00之后第一个外汇卖出价）";
            }
            else
            {
                TaxExchangeRateName = "（双方约定汇率）";
            }

            //服务费汇率
            var AgencyExchangeRateName = "";
            if (this.Agreement.AgencyFeeClause.ExchangeRateType == ExchangeRateType.Custom)
            {
                AgencyExchangeRateName = "（进口当月海关汇率）";
            }
            else if (this.Agreement.AgencyFeeClause.ExchangeRateType == ExchangeRateType.RealTime)
            {
                AgencyExchangeRateName = "（进口当天中国银行10:00之后第一个外汇卖出价）";
            }
            else
            {
                AgencyExchangeRateName = "（双方约定汇率）";
            }

            row = grid.Rows.Add();
            row.Cells[0].Value = "订单编号: " + this.MainOrderID;
            row.Cells[0].Style.Font = new PdfTrueTypeFont(new Font("SimSun", 8f, FontStyle.Bold), true);
            //row.Cells[1].Value = "实时汇率: " + Bills[0].RealExchangeRate.ToString("0.######") + AgencyExchangeRateName;
            row.Cells[1].Value = "实时汇率: " + Bills[0].RealExchangeRate.ToString("0.######") + PEIsTen;

            row = grid.Rows.Add();
            row.Cells[0].Value = "服务费收费标准: " + "服务费率：" + preAgency + (this.Agreement.AgencyRate * 100).ToString("0.##") + "% ，最低消费" + this.Agreement.MinAgencyFee.ToString("0.##") + "元/单";
            row.Cells[1].Value = "税款汇率: " + Bills[0].CustomsExchangeRate.ToString("0.######") + TaxExchangeRateName;


            //设置无边框
            SetCellNoBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            y += result.Bounds.Height + 5;
            #endregion


            #region 报关商品明细
            //var tinyorderids = this.orderitems.Select(item => item.TinyOrderID).Distinct().OrderBy(item => item);

            decimal subtotalQty = 0;
            decimal subtotalPrice = 0, subtotalCNYPrice = 0;
            decimal subtotalAgencyFee = 0, subtotalIncidentalFee = 0;
            decimal subtotalTraiff = 0, subtotalExciseTax = 0, subtotalAddedValueTax = 0;
            decimal subtotalTaxFee = 0, subtotalAmount = 0;


            foreach (var bill in this.Bills)
            {
                ////获取合同号
                //using (ScCustomReponsitory res = new ScCustomReponsitory())
                //{
                //    this.ContrNo = res.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>().SingleOrDefault(item => item.OrderID == id && item.IsSuccess == true)?.ContrNo;
                //}
                var items = bill.Products;


                //创建一个PdfGrid对象
                grid = new PdfGrid();
                grid.Style.Font = font2;

                //设置列宽
                grid.Columns.Add(15);
                grid.Columns[0].Width = width * 3f / 100;
                grid.Columns[1].Width = width * 12f / 100;
                grid.Columns[2].Width = width * 13f / 100;
                grid.Columns[3].Width = width * 5f / 100;
                grid.Columns[4].Width = width * 6f / 100;
                grid.Columns[5].Width = width * 8f / 100;
                grid.Columns[6].Width = width * 8f / 100;
                grid.Columns[7].Width = width * 5f / 100;
                grid.Columns[8].Width = width * 6f / 100;
                grid.Columns[9].Width = width * 6f / 100;
                grid.Columns[10].Width = width * 6f / 100;
                grid.Columns[11].Width = width * 6f / 100;
                grid.Columns[12].Width = width * 8f / 100;
                grid.Columns[13].Width = width * 8f / 100;

                //产品信息
                row = grid.Rows.Add();
                row.Height = tbHeight;
                row.Cells[0].Value = "序号";
                row.Cells[1].Value = "报关品名";
                row.Cells[2].Value = "规格型号";
                row.Cells[3].Value = "数量";
                row.Cells[4].Value = "单价" + "" + currencyCode + "";
                row.Cells[5].Value = "原货值" + "" + currencyCode + "";
                row.Cells[6].Value = "货值RMB";
                row.Cells[7].Value = "关税率";
                row.Cells[8].Value = "关税";
                row.Cells[9].Value = "增值税";
                row.Cells[10].Value = "服务费";
                row.Cells[11].Value = "杂费";
                row.Cells[12].Value = "税费合计";
                row.Cells[13].Value = "含税总金额";

                int sn = 0;

                foreach (var item in bill.Products)
                {
                    sn++;
                    row = grid.Rows.Add();
                    row.Height = tbHeight;
                    row.Cells[0].Value = sn.ToString();
                    row.Cells[1].Value = item.ProductName;
                    row.Cells[2].Value = item.Model;
                    row.Cells[3].Value = item.Quantity.ToString("0.####");
                    row.Cells[4].Value = item.UnitPrice.ToString("0.####");
                    row.Cells[5].Value = item.TotalPrice.ToString("0.00");
                    row.Cells[6].Value = item.TotalCNYPrice.ToString("0.00");
                    row.Cells[7].Value = item.TariffRate.ToString("0.##");
                    row.Cells[8].Value = item.Traiff.Value.ToString("0.##");
                    row.Cells[9].Value = item.AddedValueTax.Value.ToString("0.00");
                    row.Cells[10].Value = item.AgencyFee.ToString("0.##");
                    row.Cells[11].Value = item.IncidentalFee.ToString("0.##");
                    row.Cells[12].Value = "¥" + (item.Traiff.Value + item.ExciseTax.Value + item.AddedValueTax.Value + item.AgencyFee + item.IncidentalFee).ToRound(2).ToString("0.00");
                    row.Cells[13].Value = "¥" + (item.TotalCNYPrice + item.Traiff.Value + item.ExciseTax.Value + item.AddedValueTax.Value + item.AgencyFee + item.IncidentalFee).ToRound(2).ToString("0.00");
                }

                decimal totalQty = items.Sum(item => item.Quantity);
                decimal totalPrice = items.Sum(item => item.TotalPrice), totalCNYPrice = items.Sum(item => item.TotalCNYPrice);
                decimal totalAgencyFee = items.Sum(item => item.AgencyFee), totalIncidentalFee = items.Sum(item => item.IncidentalFee);
                decimal totalTraiff = items.Sum(item => item.Traiff.Value), totalAddedValueTax = items.Sum(item => item.AddedValueTax.Value);

                if (totalTraiff < 50)
                {
                    totalTraiff = 0;
                    foreach (var item in items)
                    {
                        item.Traiff = 0;
                    }
                }

                if (totalAddedValueTax < 50)
                {
                    totalAddedValueTax = 0;
                    foreach (var item in items)
                    {
                        item.AddedValueTax = 0;
                    }
                }

                decimal totalTaxFee = totalTraiff + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;
                decimal totalAmount = totalCNYPrice + totalTraiff + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;

                subtotalQty += totalQty;
                subtotalPrice += totalPrice;
                subtotalCNYPrice += totalCNYPrice;
                subtotalTraiff += totalTraiff;
                subtotalAddedValueTax += totalAddedValueTax;
                subtotalAgencyFee += totalAgencyFee;
                subtotalIncidentalFee += totalIncidentalFee;
                subtotalTaxFee += totalTaxFee;
                subtotalAmount += totalAmount;

            }

            //合计行
            row = grid.Rows.Add();
            row.Height = tbHeight;
            row.Cells[0].ColumnSpan = 3;
            row.Cells[0].Value = "合计:";
            row.Cells[3].Value = subtotalQty.ToString("0.####");
            row.Cells[5].Value = subtotalPrice.ToString("0.##");
            row.Cells[6].Value = subtotalCNYPrice.ToString("0.00");
            row.Cells[8].Value = subtotalTraiff.ToString("0.##");
            row.Cells[9].Value = subtotalAddedValueTax.ToString("0.##");
            row.Cells[10].Value = subtotalAgencyFee.ToString("0.##");
            row.Cells[11].Value = subtotalIncidentalFee.ToString("0.##");
            row.Cells[12].Value = "¥" + subtotalTaxFee.ToString("0.00");
            row.Cells[13].Value = "¥" + subtotalAmount.ToString("0.00");

            foreach (var pgr in grid.Rows)
            {
                for (int i = 0; i < pgr.Cells.Count; i++)
                {
                    if (i == 1 || i == 2)
                    {
                        pgr.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                        continue;
                    }
                    pgr.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                }
            }


            //小计行部分

            //decimal TotalPrice = subtotalQty, TotalCNYPrice = orderitems.Sum(item => item.DeclareTotalPrice);
            //decimal TotalTraiff = this.orderitems.Sum(item => item.Traiff), TotalExcise = this.orderitems.Sum(item => item.ExcisePrice), TotalAddedValueTax = orderitems.Sum(item => item.AddTax);
            //decimal TotalAgencyFee = this.orderitems.Sum(item => item.AgencyFee), TotalIncidentalFee = this.orderitems.Sum(item => item.InspectionFee);
            decimal TotalTax = subtotalTraiff + subtotalAddedValueTax + subtotalAgencyFee + subtotalIncidentalFee;
            //decimal TotalAmount = TotalCNYPrice + TotalTax;

            row = grid.Rows.Add();
            row.Height = 18f;
            row.Cells[0].ColumnSpan = 4;
            row.Cells[0].Value = "备注:";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            row.Cells[4].ColumnSpan = 4;
            row.Cells[4].Value = "货值小计：" + currencyCode + " " + subtotalPrice.ToString("0.00") + "   ¥" + subtotalCNYPrice.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            row.Cells[8].ColumnSpan = 3;
            row.Cells[8].Value = "税代费小计：" + "¥" + TotalTax.ToString("0.00");
            row.Cells[8].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            row.Cells[11].ColumnSpan = 3;
            row.Cells[11].Value = "发票总金额：" + "¥" + subtotalAmount.ToString("0.00");
            row.Cells[11].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            row = grid.Rows.Add();
            row.Height = 18f;
            row.Cells[0].ColumnSpan = 11;
            row.Cells[0].Value = "特别提示: 请在协议约定账期内付款，超出按照日 0.0005 支付逾期利息";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[0].Style.Font = new PdfTrueTypeFont(new Font("SimSun", 9f, FontStyle.Bold), true);

            row.Cells[11].ColumnSpan = 3;
            row.Cells[11].Value = "应收总金额：" + "¥" + TotalTax.ToString("0.00");
            row.Cells[11].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //设置边框
            SetCellBorder(grid);

            //表格换页的时候，留下页脚的空间
            if (y > 760)
            {
                y += 10;
            }

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 15;


            #endregion



            #region 尾

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;


            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 2;
            row.Cells[0].Value = "特别约定:\r\n" +
                                "1.受托方收款账户: " + this.AgentName + " 开户行：宁波银行股份有限公司深圳坪山支行 账号：86021110000213646\r\n" +
                                "2.委托方需在进口后90天内完成付汇，付汇汇率为《供应链服务协议》约定的付汇汇率。\r\n" +
                                "3.委托方需在收到此帐单后确认内容并盖章确认回传。\r\n" +
                                "4.委托方不得使用个人账户向受托方收款账户打款。\r\n" +
                                "5.委托方承诺打款账户不涉及地下钱庄、非法集资、诈骗、洗钱等违法交易行为，因违反上述规定产生的一切责任均由委托方承担，并赔偿受托方一切算损失。";
            row = grid.Rows.Add();
            row.Height = 50f;
            row.Cells[0].Value = "";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "委托方确认签字或盖章：";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            //设置边框
            SetCellNoBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;


            #endregion

            #region 公共组件

            //页眉、页脚、二维码、水印
            pdf.PdfMargins = new PdfMargins(20, 20);
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(pdf);
            string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.HeaderImg);

            pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, PurchaserContext.Current.OfficalWebsite);
            //pdfDocumentHandle.HeaderFooter.GenerateFooter(PurchaserContext.Current.CompanyName);
            //pdfDocumentHandle.Barcode.GenerateQRCode(Order.ID, imageUrl);
            //pdfDocumentHandle.Watermark.DrawWatermark(PurchaserContext.Current.CompanyName);

            #endregion

            return pdf;
        }


        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveAs(string filePath)
        {
            int pagecount = 0;
            PdfDocument pdf = this.ToPdfNew();
            pagecount = pdf.Pages.Count;
            pdf.SaveToFile(filePath);
            pdf.Close();
            if (pagecount > 1)
            {
                PagingSeal(filePath);
            }
        }

        /// <summary>
        /// 更新页数、页面、偏移量
        /// </summary>
        /// <param name="pdf">PDF文档</param>
        /// <param name="pageCount">新增页面前的页数</param>
        /// <param name="page">PDF页面</param>
        /// <param name="x">X轴偏移量</param>
        /// <param name="y">Y轴偏移量</param>
        private void UpdateIfNewPageCreated(PdfDocument pdf, out int pageCount, out PdfPageBase page, out float x, out float y)
        {
            pageCount = pdf.Pages.Count;
            page = pdf.Pages[pdf.Pages.Count - 1];
            x = pdf.PageSettings.Margins.Left;
            y = pdf.PageSettings.Margins.Top;
        }

        /// <summary>
        /// 设置pdfgrid单元格边框样式
        /// </summary>
        /// <param name="grid"></param>
        private void SetCellBorder(PdfGrid grid)
        {
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }
        }

        /// <summary>
        /// 设置pdfgrid单元格边框样式
        /// </summary>
        /// <param name="grid"></param>
        private void SetCellNoBorder(PdfGrid grid)
        {
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.White, 0f);
                }
            }
        }

        /// <summary>
        /// 审核对账单
        /// </summary>
        public void Approve()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.MainOrderFiles>(new { FileStatus = Enums.OrderFileStatus.Audited }, item => item.ID == this.FileID);
                new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, this.FileID);
            }

            //this.OnApproved();
            #region 审批通过调用代仓储
            var confirm = new FileApprove
            {
                OrderID = this.MainOrderID,
                Type = Enums.FileType.OrderBill
            };

            var apisetting = new ApiSettings.PvWsOrderApiSetting();
            var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.FileApprove;
            var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, confirm);
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Underly.JMessage>(result);

            if (message.code != 200)
            {
                throw new Exception(message.data);
            }
            #endregion

        }

        private void PagingSeal(string filePath)
        {
            //加载PDF文档
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(filePath);

            PdfUnitConvertor convert = new PdfUnitConvertor();
            PdfPageBase pageBase = null;

            //获取分割后的印章图片
            //华芯通或者创新恒远图片
            GetImage(doc.Pages.Count, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.SealUrl), this.AgentName);

            //内单需要盖章
            if (this.ClientType == ClientType.Internal)
            {
                //大赢家图片
                GetImage(doc.Pages.Count, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.ClientName + ".png"), this.ClientName);
            }
            float x = 0;
            float y = 0;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //将图片画到PDF页面上的指定位置
            for (int i = 0; i < doc.Pages.Count; i++)
            {
                string agentPath = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", this.AgentName, doc.Pages.Count.ToString(), i.ToString() + ".png");
                PdfImage agentImage = PdfImage.FromFile(agentPath);
                pageBase = doc.Pages[i];
                x = pageBase.Size.Width - agentImage.Width + 9;
                y = pageBase.Size.Height / 2;
                //pageBase.Canvas.DrawImage(PdfImage.FromImage(agentImages[i]), new PointF(x, y));               
                pageBase.Canvas.DrawImage(agentImage, new PointF(x, y));

                if (this.ClientType == ClientType.Internal)
                {
                    string clientPath = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", this.ClientName, doc.Pages.Count.ToString(), i.ToString() + ".png");
                    PdfImage clientImage = PdfImage.FromFile(clientPath);
                    x = pageBase.Size.Width - clientImage.Width + 9;
                    y = pageBase.Size.Height / 2 + convert.ConvertToPixels(agentImage.Height, PdfGraphicsUnit.Point) + 30;
                    //pageBase.Canvas.DrawImage(PdfImage.FromImage(clientImages[i]), new PointF(x, y));               
                    pageBase.Canvas.DrawImage(clientImage, new PointF(x, y));
                }
            }

            //保存PDF文件
            doc.SaveToFile(filePath);
            doc.Close();
        }

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
                    Image image = Image.FromFile(picUrl);
                    int w = image.Width / num;
                    Bitmap bitmap = null;
                    for (int i = 0; i < num; i++)
                    {
                        bitmap = new Bitmap(w, image.Height);
                        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                        {
                            g.Clear(Color.White);
                            Rectangle rect = new Rectangle(i * w, 0, w, image.Height);
                            g.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), rect, GraphicsUnit.Pixel);
                        }
                        bitmap.MakeTransparent(Color.White);
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

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveASIcgoo(string filePath)
        {
            int pagecount = 0;
            PdfDocument pdf = this.ToIcgooPdf();
            pagecount = pdf.Pages.Count;
            pdf.SaveToFile(filePath);
            pdf.Close();
            if (pagecount > 1)
            {
                PagingSeal(filePath);
            }
        }

        public Utils.SpirePdf.PdfDocument ToIcgooPdf()
        {
            #region pdf对象声明

            //创建一个PdfDocument类对象
            Utils.SpirePdf.PdfDocument pdf = new PdfDocument();
            pdf.PageSettings.Margins = new PdfMargins(10, 60, 10, 10);

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4);
            int pageCount = pdf.Pages.Count;

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 14f, FontStyle.Bold), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头
            float x = 0, y = 5f;

            float width = page.Canvas.ClientSize.Width;
            string message = "委托进口货物报关对帐单";
            page.Canvas.DrawString(message, font1, brush, width / 2, y, formatCenter);

            y += font1.MeasureString(message, formatCenter).Height + 8;

            //主订单号
            message = "主订单编号:" + this.MainOrderID;
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatLeft).Height + 8;

            //创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;

            //表头信息
            PdfGridRow row = grid.Rows.Add();
            row.Cells[0].Value = "委托方: " + this.ClientName;
            row.Cells[1].Value = "被委托方: " + this.AgentName;
            row = grid.Rows.Add();
            row.Cells[0].Value = "电话：" + this.ClientTel;
            row.Cells[1].Value = "电话：" + this.AgentTel;

            //设置边框
            SetCellBorder(grid);

            PdfLayoutResult result = grid.Draw(page, new PointF(x, y));
            y += result.Bounds.Height + 5;


            //创建子订单号，合同号，海关汇率，实施汇率表格
            grid = new PdfGrid();
            grid.Style.Font = font2;
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;

            foreach (var bill in this.Bills)
            {
                row = grid.Rows.Add();
                row.Cells[0].Value = "订单编号：" + bill.OrderID + (bill.ContrNo == null ? "" : (" 合同号：" + bill.ContrNo));
                row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                row.Cells[1].Value = "实时汇率：" + bill.RealExchangeRate + " 海关汇率：" + bill.CustomsExchangeRate;
                row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            }

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            y += result.Bounds.Height + 5;
            #endregion


            decimal subtotalPrice = this.summaryTotalPrice, subtotalCNYPrice = this.summaryTotalCNYPrice;
            decimal subtotalAgencyFee = this.summaryTotalAgencyFee, subtotalIncidentalFee = this.summaryTotalIncidentalFee;
            decimal subtotalTraiff = this.summaryTotalTariff.Value, subtotalExciseTax = this.summaryTotalExciseTax.Value, subtotalAddedValueTax = this.summaryTotalAddedValueTax.Value;
            decimal subtotalTaxFee = this.summaryPay.Value, subtotalAmount = this.summaryPayAmount.Value;

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(15);
            grid.Columns[0].Width = width * 3f / 100;
            grid.Columns[1].Width = width * 12f / 100;
            grid.Columns[2].Width = width * 12f / 100;
            grid.Columns[3].Width = width * 5f / 100;
            grid.Columns[4].Width = width * 7f / 100;
            grid.Columns[5].Width = width * 7f / 100;
            grid.Columns[6].Width = width * 5f / 100;
            grid.Columns[7].Width = width * 7f / 100;
            grid.Columns[8].Width = width * 5f / 100;
            grid.Columns[9].Width = width * 6f / 100;
            grid.Columns[10].Width = width * 6f / 100;
            grid.Columns[11].Width = width * 6f / 100;
            grid.Columns[12].Width = width * 5f / 100;
            grid.Columns[13].Width = width * 7f / 100;
            grid.Columns[14].Width = width * 7f / 100;


            //产品信息
            row = grid.Rows.Add();
            row.Cells[0].Value = "序号";
            row.Cells[1].Value = "报关品名";
            row.Cells[2].Value = "规格型号";
            row.Cells[3].Value = "数量";
            row.Cells[4].Value = "报关单价" + "(" + this.Currency + ")";
            row.Cells[5].Value = "报关总价" + "(" + this.Currency + ")";
            row.Cells[6].Value = "关税率";
            row.Cells[7].Value = "报关货值(CNY)";
            row.Cells[8].Value = "关税(CNY)";
            row.Cells[9].Value = "消费税(CNY)";
            row.Cells[10].Value = "增值税(CNY)";
            row.Cells[11].Value = "代理费(CNY)";
            row.Cells[12].Value = "杂费(CNY)";
            row.Cells[13].Value = "税费合计(CNY)";
            row.Cells[14].Value = "报关总金额(CNY)";

            int sn = 0;

            foreach (var item in this.ProductsForIcgoo)
            {
                #region 报关商品明细
                sn++;
                row = grid.Rows.Add();
                row.Cells[0].Value = sn.ToString();
                row.Cells[1].Value = item.ProductName;
                row.Cells[2].Value = item.Model;
                row.Cells[3].Value = item.Quantity.ToString("0.####");
                row.Cells[4].Value = item.UnitPrice.ToString("0.0000");
                row.Cells[5].Value = item.TotalPrice.ToRound(2).ToString("0.00");
                row.Cells[6].Value = item.TariffRate.ToString("0.0000");
                row.Cells[7].Value = item.TotalCNYPrice.ToRound(2).ToString("0.00");
                row.Cells[8].Value = item.Traiff.Value.ToRound(2).ToString("0.00");
                row.Cells[9].Value = item.ExciseTax.Value.ToRound(2).ToString("0.00");
                row.Cells[10].Value = item.AddedValueTax.Value.ToRound(2).ToString("0.00");
                row.Cells[11].Value = item.AgencyFee.ToRound(2).ToString("0.00");
                row.Cells[12].Value = item.IncidentalFee.ToRound(2).ToString("0.00");
                row.Cells[13].Value = (item.Traiff.Value + item.ExciseTax.Value + item.AddedValueTax.Value + item.AgencyFee + item.IncidentalFee).ToRound(2).ToString("0.00");
                row.Cells[14].Value = (item.TotalCNYPrice + item.Traiff.Value + item.ExciseTax.Value + item.AddedValueTax.Value + item.AgencyFee + item.IncidentalFee).ToRound(2).ToString("0.00");
                #endregion
            }

            row = grid.Rows.Add();
            row.Cells[0].Value = "合计";
            row.Cells[0].ColumnSpan = 3;

            row.Cells[3].Value = this.summaryTotalQty.ToString("0.####");
            row.Cells[4].Value = "";
            row.Cells[5].Value = this.summaryTotalPrice.ToRound(2).ToString("0.00");
            row.Cells[6].Value = "";
            row.Cells[7].Value = this.summaryTotalCNYPrice.ToRound(2).ToString("0.00");
            row.Cells[8].Value = this.summaryTotalTariff.Value.ToRound(2).ToString("0.00");
            row.Cells[9].Value = this.summaryTotalExciseTax.Value.ToRound(2).ToString("0.00");
            row.Cells[10].Value = this.summaryTotalAddedValueTax.Value.ToRound(2).ToString("0.00");
            row.Cells[11].Value = this.summaryTotalAgencyFee.ToRound(2).ToString("0.00");
            row.Cells[12].Value = this.summaryTotalIncidentalFee.ToRound(2).ToString("0.00");
            row.Cells[13].Value = this.summaryPay.Value.ToRound(2).ToString("0.00");
            row.Cells[14].Value = this.summaryPayAmount.Value.ToRound(2).ToString("0.00");

            foreach (var pgr in grid.Rows)
            {
                for (int i = 0; i < pgr.Cells.Count; i++)
                {
                    if (i == 1 || i == 2)
                    {
                        pgr.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                        continue;
                    }
                    pgr.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                }
            }

            //设置边框
            SetCellBorder(grid);

            //表格换页的时候，留下页脚的空间
            if (y > 760)
            {
                y += 10;
            }

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;



            #region 费用合计明细

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width * 0.8f;
            grid.Columns[1].Width = width * 0.2f;

            row = grid.Rows.Add();
            row.Cells[0].Value = "货值小计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = this.Currency + " " + subtotalPrice.ToRound(2).ToString("0.00") + "\r\nCNY " + subtotalCNYPrice.ToRound(2).ToString("0.00");
            //row.Cells[1].Value = this.Currency + " " + totalPrice.ToRound(2).ToString("0.00") + "\r\nCNY " + totalCNYPrice.ToRound(2).ToString("0.00");          
            row = grid.Rows.Add();
            row.Cells[0].Value = "税代费小计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "CNY " + subtotalTaxFee.ToRound(2).ToString("0.00");
            //var totalTaxFee = totalTraiff + totalAddedValueTax + totalAgencyFee + totalInspFee + totalOtherFee;
            //row.Cells[1].Value = "CNY " + totalTaxFee.ToRound(2).ToString("0.00");          
            row = grid.Rows.Add();
            row.Cells[0].Value = "应收总金额合计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "CNY " + subtotalAmount.ToRound(2).ToString("0.00");
            //var totalAmount = totalCNYPrice + totalTraiff + totalAddedValueTax + totalAgencyFee + totalInspFee + totalOtherFee;
            //row.Cells[1].Value = "CNY " + totalAmount.ToRound(2).ToString("0.00");        

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;

            #endregion

            #region 尾

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(1);
            grid.Columns[0].Width = width;

            row = grid.Rows.Add();
            row.Cells[0].Value = this.AgentName + " " + this.AgentAddress + " 电话：" + this.AgentTel + " 传真：" + this.AgentFax;
            row = grid.Rows.Add();
            row.Cells[0].Value = $"开户行:{this.Bank} 开户名：{this.Account} 账户：{this.AccountId}";

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;


            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 2;
            row.Cells[0].Value = "备注:\r\n" +
                                //"1.我司" + this.AgentName + "为委托方代垫" +
                                //"本金(" + (this.IsLoan ? subtotalCNYPrice.ToRound(2).ToString("0.00") : "0.00") + "元)" +
                                //"+关税(" + subtotalTraiff.ToRound(2).ToString("0.00") + "元)" +
                                //"+消费税(" + subtotalExciseTax.ToRound(2).ToString("0.00") + "元)" +
                                //"+增值税(" + subtotalAddedValueTax.ToRound(2).ToString("0.00") + "元)" +
                                //"+代理费(" + subtotalAgencyFee.ToRound(2).ToString("0.00") + "元)" +
                                //"+杂费(" + (subtotalIncidentalFee).ToRound(2).ToString("0.00") + "元)," +
                                //"共计应收人民币(" + (this.IsLoan ? subtotalAmount.ToRound(2).ToString("0.00") : subtotalTaxFee.ToRound(2).ToString("0.00")) + "元)，" +
                                //"委托方需在(" + this.DueDate + ")前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。\r\n" +
                                //"2.客户在90天内完成付汇手续，付汇汇率为实际付汇当天的中国银行上午十点后的第一个现汇卖出价。\r\n" +
                                "1.委托方需在报关协议约定的付款日前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。\r\n" +
                                "2.委托方在90天内完成付汇，付汇汇率为报关协议约定的实际付汇当天的汇率。\r\n" +
                                "3.委托方应在收到帐单之日起二个工作日内签字和盖章确认回传。\r\n" +
                                "4.此传真件、扫描件、复印件与原件具有同等法律效力。\r\n" +
                                "5.如若对此帐单有发生争议的，双方应友好协商解决，如协商不成的，可通过被委托方所在地人民法院提起诉讼解决。";
            row = grid.Rows.Add();
            row.Height = 30f;
            row.Cells[0].Value = "委托方确认签字或盖章：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "被委托方签字或盖章：";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;




            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.SealUrl));
            page.Canvas.DrawImage(image, 350, y - 80);

            //大赢家加上委托方章
            if (this.ClientType == Enums.ClientType.Internal)
            {
                PdfImage imageInternal = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.ClientName + ".png"));
                page.Canvas.DrawImage(imageInternal, 100, y - 80);
            }

            #endregion



            #region 公共组件

            //页眉、页脚、二维码、水印
            pdf.PdfMargins = new PdfMargins(10, 10);
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(pdf);
            string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.HeaderImg);

            pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, PurchaserContext.Current.OfficalWebsite);
            pdfDocumentHandle.HeaderFooter.GenerateFooter(PurchaserContext.Current.CompanyName);
            pdfDocumentHandle.Barcode.GenerateQRCode(this.MainOrderID, imageUrl);
            pdfDocumentHandle.Watermark.DrawWatermark(PurchaserContext.Current.CompanyName);

            #endregion

            return pdf;
        }
    }
}
