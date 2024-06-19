using Layers.Data.Sqls;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.SpirePdf;
using Contact = Yahv.Services.Models.Contact;
using PdfDocument = Yahv.Utils.SpirePdf.PdfDocument;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    public class OrderBillProxy
    {
        #region 属性
        /// <summary>
        /// 订单
        /// </summary>
        public WsOrder Order { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public WsClient Client { get; set; }

        public ClientAgreement ClientAgreement { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }


        public Contact ClientContact { get; set; }

        /// <summary>
        /// 对账单结算日期
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem[] orderitems { get; set; }

        public Purchaser purchaser = PurchaserContext.Current;
        public Vendor vendor = VendorContext.Current;
        #endregion

        public PdfDocument ToPdf(XDTOrderView MyXDTOrder, decimal amountFor代垫本金, Yahv.PvWsOrder.Services.Enums.ClientType clientType)
        {
            var currencyName = this.Order.Output.Currency.GetDescription();
            var currencyCode = this.Order.Output.Currency.ToString();

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
            row.Cells[0].Value = "委托方: " + this.Client.Name;
            row.Cells[1].Value = "被委托方: " + purchaser.CompanyName;
            row = grid.Rows.Add();
            row.Cells[0].Value = "电话：" + this.ClientContact?.Tel;
            row.Cells[1].Value = "电话：" + purchaser.Tel;

            //设置边框
            SetCellBorder(grid);

            PdfLayoutResult result = grid.Draw(page, new PointF(x, y));
            y += result.Bounds.Height + 5;

            #endregion

            //var isPay = MyXDTOrder.Any(item => item.MainOrderID == this.Order.ID && item.PaidExchangeAmount > 0);
            var isIcgoo = MyXDTOrder.Any(item => item.MainOrderID == this.Order.ID && item.Type == 300);
            var isInside = MyXDTOrder.Any(item => item.MainOrderID == this.Order.ID && item.Type == 100);

            #region 报关商品明细
            var tinyorderids = this.orderitems.Select(item => item.TinyOrderID).Distinct().OrderBy(item => item);

            foreach (var id in tinyorderids)
            {
                var items = this.orderitems.Where(item => item.TinyOrderID == id);
                //获取合同号
                using (ScCustomReponsitory res = new ScCustomReponsitory())
                {
                    this.ContrNo = res.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>().SingleOrDefault(item => item.OrderID == id && item.IsSuccess == true)?.ContrNo;
                }

                //创建一个PdfGrid对象
                grid = new PdfGrid();
                grid.Style.Font = font2;

                //设置列宽
                grid.Columns.Add(15);
                grid.Columns[0].Width = width * 3f / 100;
                grid.Columns[1].Width = width * 12f / 100;
                grid.Columns[2].Width = width * 12f / 100;
                grid.Columns[3].Width = width * 6f / 100;
                grid.Columns[4].Width = width * 6f / 100;
                grid.Columns[5].Width = width * 6f / 100;
                grid.Columns[6].Width = width * 6f / 100;
                grid.Columns[7].Width = width * 6f / 100;
                grid.Columns[8].Width = width * 6f / 100;
                grid.Columns[9].Width = width * 6f / 100;
                grid.Columns[10].Width = width * 6f / 100;
                grid.Columns[11].Width = width * 6f / 100;
                grid.Columns[12].Width = width * 6f / 100;
                grid.Columns[13].Width = width * 6f / 100;
                grid.Columns[14].Width = width * 7f / 100;

                //汇率 合同信息
                row = grid.Rows.Add();
                row.Cells[0].ColumnSpan = 7;
                row.Cells[0].Value = "订单编号：" + id + (this.ContrNo == null ? "" : (" 合同号：" + this.ContrNo));
                row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                row.Cells[7].ColumnSpan = 8;
                row.Cells[7].Value = "实时汇率：" + items.First().RealExchangeRate + " 海关汇率：" + items.First().CustomsExchangeRate;
                row.Cells[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

                //产品信息
                row = grid.Rows.Add();
                row.Cells[0].Value = "序号";
                row.Cells[1].Value = "报关品名";
                row.Cells[2].Value = "规格型号";
                row.Cells[3].Value = "数量";
                row.Cells[4].Value = "报关单价" + "(" + currencyCode + ")";
                row.Cells[5].Value = "报关总价" + "(" + currencyCode + ")";
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

                foreach (var item in items)
                {
                    sn++;
                    row = grid.Rows.Add();
                    row.Cells[0].Value = sn.ToString();
                    row.Cells[1].Value = item.ClassfiedName;
                    row.Cells[2].Value = item.Product.PartNumber;
                    row.Cells[3].Value = item.Quantity.ToString("0.####");
                    row.Cells[4].Value = item.UnitPrice.ToString("0.0000");
                    row.Cells[5].Value = item.TotalPrice.ToString("0.00");
                    row.Cells[6].Value = item.TraiffRate.ToString("0.0000");
                    row.Cells[7].Value = item.DeclareTotalPrice.ToString("0.00");
                    row.Cells[8].Value = item.Traiff.ToString("0.00");
                    row.Cells[9].Value = item.ExcisePrice.ToString("0.00");
                    row.Cells[10].Value = item.AddTax.ToString("0.00");
                    row.Cells[11].Value = item.AgencyFee.ToString("0.00");
                    row.Cells[12].Value = item.InspectionFee.ToString("0.00");
                    row.Cells[13].Value = (item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee).ToString("0.00");
                    row.Cells[14].Value = (item.DeclareTotalPrice + item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee).ToString("0.00");
                }

                ////内单和Icgoo的对账单如果关税总和小于50，则显示0
                //if (bill.OrderType != OrderType.Outside && totalTraiff < 50)
                //{
                //    totalTraiff = 0;
                //}
                ////内单和Icgoo的对账单如果增值税总和小于50，则显示0
                //if (bill.OrderType != OrderType.Outside && totalAddedValueTax < 50)
                //{
                //    totalAddedValueTax = 0;
                //}
                decimal totalQty = items.Sum(item => item.Quantity);
                decimal totalPrice = items.Sum(item => item.TotalPrice), totalCNYPrice = items.Sum(item => item.DeclareTotalPrice);
                decimal totalAgencyFee = items.Sum(item => item.AgencyFee).Round(), totalIncidentalFee = items.Sum(item => item.InspectionFee).Round();
                decimal totalTraiff = items.Sum(item => item.Traiff), totalExcise = items.Sum(item => item.ExcisePrice), totalAddedValueTax = items.Sum(item => item.AddTax);
                //内单和icgoo：关税、增值税和消费税的小订单合计<50时设为0
                //if (isIcgoo || isInside)
                //{
                if (totalTraiff < 50)
                {
                    totalTraiff = 0;
                    foreach (var item in items)
                    {
                        item.Traiff = 0;
                    }
                }
                if (totalExcise < 50)
                {
                    totalExcise = 0;
                    foreach (var item in items)
                    {
                        item.ExcisePrice = 0;
                    }
                }
                if (totalAddedValueTax < 50)
                {
                    totalAddedValueTax = 0;
                    foreach (var item in items)
                    {
                        item.AddTax = 0;
                    }
                }
                //}
                decimal totalTaxFee = totalTraiff + totalExcise + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;
                decimal totalAmount = totalCNYPrice + totalExcise + totalTraiff + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;

                //合计行
                row = grid.Rows.Add();
                row.Cells[0].ColumnSpan = 3;
                row.Cells[0].Value = "合计:";
                row.Cells[3].Value = totalQty.ToString("0.####");
                row.Cells[5].Value = totalPrice.ToString("0.00");
                row.Cells[7].Value = totalCNYPrice.ToString("0.00");
                row.Cells[8].Value = totalTraiff.ToString("0.00");
                row.Cells[9].Value = totalExcise.ToString("0.00");
                row.Cells[10].Value = totalAddedValueTax.ToString("0.00");
                row.Cells[11].Value = totalAgencyFee.ToString("0.00");
                row.Cells[12].Value = totalIncidentalFee.ToString("0.00");
                row.Cells[13].Value = totalTaxFee.ToString("0.00");
                row.Cells[14].Value = totalAmount.ToString("0.00");

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

            }
            #endregion

            #region 费用合计明细

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width * 0.8f;
            grid.Columns[1].Width = width * 0.2f;

            decimal TotalPrice = this.orderitems.Sum(item => item.TotalPrice), TotalCNYPrice = orderitems.Sum(item => item.DeclareTotalPrice);
            decimal TotalTraiff = this.orderitems.Sum(item => item.Traiff), TotalExcise = this.orderitems.Sum(item => item.ExcisePrice), TotalAddedValueTax = orderitems.Sum(item => item.AddTax);
            decimal TotalAgencyFee = this.orderitems.Sum(item => item.AgencyFee), TotalIncidentalFee = this.orderitems.Sum(item => item.InspectionFee);
            decimal TotalTax = TotalTraiff + TotalExcise + TotalAddedValueTax + TotalAgencyFee + TotalIncidentalFee;
            decimal TotalAmount = TotalCNYPrice + TotalTax;

            row = grid.Rows.Add();
            row.Cells[0].Value = "货值小计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = currencyCode + " " + TotalPrice.ToString("0.00") + "\r\nCNY " + TotalCNYPrice.ToString("0.00");
            row = grid.Rows.Add();
            row.Cells[0].Value = "税代费小计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "CNY " + TotalTax.ToString("0.00");
            row = grid.Rows.Add();
            row.Cells[0].Value = "应收总金额合计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "CNY " + TotalAmount.ToString("0.00");

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
            row.Cells[0].Value = purchaser.CompanyName + " " + purchaser.Address + " 电话：" + purchaser.Tel + " 传真：" + purchaser.UseOrgPersonTel;
            row = grid.Rows.Add();
            row.Cells[0].Value = $"开户行:{purchaser.BankName} 开户名：{purchaser.AccountName} 账户：{purchaser.AccountId}";

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
                                //"1.我司" + purchaser.CompanyName + "为委托方代垫" +
                                ////"本金(" + (isPay ? TotalCNYPrice : 0).ToString("0.00") + "元)" +
                                //"本金(" + amountFor代垫本金.ToString("0.00") + "元)" +
                                //"+关税(" + TotalTraiff.ToString("0.00") + "元)" +
                                //"+消费税(" + TotalExcise.ToString("0.00") + "元)" +
                                //"+增值税(" + TotalAddedValueTax.ToString("0.00") + "元)" +
                                //"+代理费(" + TotalAgencyFee.ToString("0.00") + "元)" +
                                //"+杂费(" + (TotalIncidentalFee).ToString("0.00") + "元)," +
                                ////"共计应收人民币(" + (isPay ? TotalCNYPrice : TotalTax).ToString("0.00") + "元)，" +
                                //"共计应收人民币(" + (TotalTax + amountFor代垫本金).ToString("0.00") + "元)，" +
                                //"委托方需在(" + this.DueDate.ToString("yyyy年MM月dd日") + ")前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。\r\n" +
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




            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.SealUrl));
            page.Canvas.DrawImage(image, 350, y - 80);

            //大赢家加上委托方章
            if (clientType == Enums.ClientType.Internal)
            {
                PdfImage imageInternal = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.Client.Name + ".png"));
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
            pdfDocumentHandle.Barcode.GenerateQRCode(Order.ID, imageUrl);
            pdfDocumentHandle.Watermark.DrawWatermark(PurchaserContext.Current.CompanyName);

            #endregion

            return pdf;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="MyXDTOrder"></param>
        /// <param name="amountFor代垫本金"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public PdfDocument ToPdfNew(XDTOrderView MyXDTOrder, decimal amountFor代垫本金, Yahv.PvWsOrder.Services.Enums.ClientType clientType)
        {
            var currencyName = this.Order.Output.Currency.GetDescription();
            var currencyCode = this.Order.Output.Currency.ToString();

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
            row.Cells[0].Value = "委托方: " + this.Client.Name;
            row.Cells[1].Value = "代理方: " + purchaser.CompanyName;
            row = grid.Rows.Add();
            row.Cells[0].Value = "委托方地址：" + this.Client.RegAddress;
            row.Cells[1].Value = "代理方地址：" + purchaser.Address;
            row = grid.Rows.Add();
            row.Cells[0].Value = "委托方电话：" + this.ClientContact?.Tel;
            row.Cells[1].Value = "代理方电话：" + purchaser.Tel;

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
            var preAgency = (this.ClientAgreement.PreAgency.HasValue && this.ClientAgreement.PreAgency > 0) ? (this.ClientAgreement.PreAgency.Value.ToString("0.##") + "元 + ") : "";
            var PEIsTen = this.ClientAgreement.IsTen == XDTClientView.PEIsTen.Ten ? "（进口当天中国银行10:00之后第一个外汇卖出价）" : "（进口当天中国银行09:30之后第一个外汇卖出价）";

            //税费汇率
            var TaxExchangeRateName = "";
            if (this.ClientAgreement.clientFeeSettlements.FirstOrDefault(t => t.FeeType == 2).ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Custom)
            {
                TaxExchangeRateName = "（进口当月海关汇率）";
            }
            else if (this.ClientAgreement.clientFeeSettlements.FirstOrDefault(t => t.FeeType == 2).ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.RealTime)
            {
                TaxExchangeRateName = "（进口当天中国银行10:00之后第一个外汇卖出价）";
            }
            else
            {
                TaxExchangeRateName = "（双方约定汇率）";
            }

            //服务费汇率
            var AgencyExchangeRateName = "";
            if (this.ClientAgreement.clientFeeSettlements.FirstOrDefault(t => t.FeeType == 3).ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Custom)
            {
                AgencyExchangeRateName = "（进口当月海关汇率）";
            }
            else if (this.ClientAgreement.clientFeeSettlements.FirstOrDefault(t => t.FeeType == 3).ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.RealTime)
            {
                AgencyExchangeRateName = "（进口当天中国银行10:00之后第一个外汇卖出价）";
            }
            else
            {
                AgencyExchangeRateName = "（双方约定汇率）";
            }

            row = grid.Rows.Add();
            row.Cells[0].Value = "订单编号: " + this.Order.ID;
            row.Cells[0].Style.Font = new PdfTrueTypeFont(new Font("SimSun", 8f, FontStyle.Bold), true);
            row.Cells[1].Value = "实时汇率: " + this.orderitems[0].RealExchangeRate.ToString("0.######") + AgencyExchangeRateName;

            row = grid.Rows.Add();
            row.Cells[0].Value = "服务费收费标准: " + "服务费率：" + preAgency + (this.ClientAgreement.AgencyRate * 100).ToString("0.##") + "% ，最低消费" + this.ClientAgreement.MinAgencyFee.ToString("0.##") + "元/单";
            row.Cells[1].Value = "税款汇率: " + this.orderitems[0].CustomsExchangeRate.ToString("0.######") + TaxExchangeRateName;


            //设置无边框
            SetCellNoBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            y += result.Bounds.Height + 5;
            #endregion


            #region 报关商品明细
            var tinyorderids = this.orderitems.Select(item => item.TinyOrderID).Distinct().OrderBy(item => item);

            decimal subtotalQty = 0;
            decimal subtotalPrice = 0, subtotalCNYPrice = 0;
            decimal subtotalAgencyFee = 0, subtotalIncidentalFee = 0;
            decimal subtotalTraiff = 0, subtotalExciseTax = 0, subtotalAddedValueTax = 0;
            decimal subtotalTaxFee = 0, subtotalAmount = 0;

            foreach (var id in tinyorderids)
            {
                var items = this.orderitems.Where(item => item.TinyOrderID == id);
                //获取合同号
                using (ScCustomReponsitory res = new ScCustomReponsitory())
                {
                    this.ContrNo = res.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>().SingleOrDefault(item => item.OrderID == id && item.IsSuccess == true)?.ContrNo;
                }

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

                foreach (var item in items)
                {
                    sn++;
                    row = grid.Rows.Add();
                    row.Height = tbHeight;
                    row.Cells[0].Value = sn.ToString();
                    row.Cells[1].Value = item.ClassfiedName;
                    row.Cells[2].Value = item.Product.PartNumber;
                    row.Cells[3].Value = item.Quantity.ToString("0.####");
                    row.Cells[4].Value = item.UnitPrice.ToString("0.####");
                    row.Cells[5].Value = item.TotalPrice.ToString("0.00");
                    row.Cells[6].Value = item.DeclareTotalPrice.ToString("0.00");
                    row.Cells[7].Value = item.TraiffRate.ToString("0.##");
                    row.Cells[8].Value = item.Traiff.ToString("0.##");
                    row.Cells[9].Value = item.AddTax.ToString("0.00");
                    row.Cells[10].Value = item.AgencyFee.ToString("0.##");
                    row.Cells[11].Value = item.InspectionFee.ToString("0.##");
                    row.Cells[12].Value = "¥" + (item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee).ToString("0.00");
                    row.Cells[13].Value = "¥" + (item.DeclareTotalPrice + item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee).ToString("0.00");
                }

                decimal totalQty = items.Sum(item => item.Quantity);
                decimal totalPrice = items.Sum(item => item.TotalPrice), totalCNYPrice = items.Sum(item => item.DeclareTotalPrice);
                decimal totalAgencyFee = items.Sum(item => item.AgencyFee).Round(), totalIncidentalFee = items.Sum(item => item.InspectionFee).Round();
                decimal totalTraiff = items.Sum(item => item.Traiff), totalExcise = items.Sum(item => item.ExcisePrice), totalAddedValueTax = items.Sum(item => item.AddTax);

                if (totalTraiff < 50)
                {
                    totalTraiff = 0;
                    foreach (var item in items)
                    {
                        item.Traiff = 0;
                    }
                }
                if (totalExcise < 50)
                {
                    totalExcise = 0;
                    foreach (var item in items)
                    {
                        item.ExcisePrice = 0;
                    }
                }
                if (totalAddedValueTax < 50)
                {
                    totalAddedValueTax = 0;
                    foreach (var item in items)
                    {
                        item.AddTax = 0;
                    }
                }

                decimal totalTaxFee = totalTraiff + totalExcise + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;
                decimal totalAmount = totalCNYPrice + totalExcise + totalTraiff + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;


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

            decimal TotalPrice = this.orderitems.Sum(item => item.TotalPrice), TotalCNYPrice = orderitems.Sum(item => item.DeclareTotalPrice);
            decimal TotalTraiff = this.orderitems.Sum(item => item.Traiff), TotalExcise = this.orderitems.Sum(item => item.ExcisePrice), TotalAddedValueTax = orderitems.Sum(item => item.AddTax);
            decimal TotalAgencyFee = this.orderitems.Sum(item => item.AgencyFee), TotalIncidentalFee = this.orderitems.Sum(item => item.InspectionFee);
            decimal TotalTax = TotalTraiff + TotalExcise + TotalAddedValueTax + TotalAgencyFee + TotalIncidentalFee;
            decimal TotalAmount = TotalCNYPrice + TotalTax;

            row = grid.Rows.Add();
            row.Height = 18f;
            row.Cells[0].ColumnSpan = 4;
            row.Cells[0].Value = "备注:";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            row.Cells[4].ColumnSpan = 4;
            row.Cells[4].Value = "货值小计：" + currencyCode + " " + TotalPrice.ToString("0.00") + "   ¥" + TotalCNYPrice.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            row.Cells[8].ColumnSpan = 3;
            row.Cells[8].Value = "税代费小计：" + "¥" + TotalTax.ToString("0.00");
            row.Cells[8].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            row.Cells[11].ColumnSpan = 3;
            row.Cells[11].Value = "发票总金额：" + "¥" + TotalAmount.ToString("0.00");
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
                                "1.受托方收款账户: " + purchaser.CompanyName + " 开户行：" + purchaser.BankName + " 账号：" + purchaser.AccountId + "\r\n" +
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
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveAs(string filePath, XDTOrderView MyXDTOrder, decimal amountFor代垫本金, Yahv.PvWsOrder.Services.Enums.ClientType clientType)
        {
            int pagecount = 0;
            var pdf = this.ToPdfNew(MyXDTOrder, amountFor代垫本金, clientType);
            pagecount = pdf.Pages.Count;
            pdf.SaveToFile(filePath);
            pdf.Close();
            if (pagecount > 1)
            {
                PagingSeal(filePath);
            }
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

        private void PagingSeal(string filePath)
        {

            //加载PDF文档
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(filePath);

            PdfUnitConvertor convert = new PdfUnitConvertor();
            PdfPageBase pageBase = null;

            //获取分割后的印章图片
            //芯达通或者创新恒远图片
            GetImage(doc.Pages.Count, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.SealUrl), this.purchaser.CompanyName);
            //大赢家图片
            GetImage(doc.Pages.Count, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.Client.Name + ".png"), this.Client.Name);
            float x = 0;
            float y = 0;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //将图片画到PDF页面上的指定位置
            for (int i = 0; i < doc.Pages.Count; i++)
            {
                string agentPath = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", this.purchaser.CompanyName, doc.Pages.Count.ToString(), i.ToString() + ".png");
                PdfImage agentImage = PdfImage.FromFile(agentPath);
                pageBase = doc.Pages[i];
                x = pageBase.Size.Width - agentImage.Width + 9;
                y = pageBase.Size.Height / 2;
                //pageBase.Canvas.DrawImage(PdfImage.FromImage(agentImages[i]), new PointF(x, y));               
                pageBase.Canvas.DrawImage(agentImage, new PointF(x, y));

                string clientPath = Path.Combine(baseDirectory, "Content\\images\\CutImages\\", this.Client.Name, doc.Pages.Count.ToString(), i.ToString() + ".png");
                PdfImage clientImage = PdfImage.FromFile(clientPath);
                x = pageBase.Size.Width - clientImage.Width + 9;
                y = pageBase.Size.Height / 2 + convert.ConvertToPixels(agentImage.Height, PdfGraphicsUnit.Point) + 30;
                //pageBase.Canvas.DrawImage(PdfImage.FromImage(clientImages[i]), new PointF(x, y));               
                pageBase.Canvas.DrawImage(clientImage, new PointF(x, y));
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
    }
}
