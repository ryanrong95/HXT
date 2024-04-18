using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.SpirePdf;
using Needs.Wl.Models;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using Spire.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfDocument = Needs.Utils.SpirePdf.PdfDocument;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理报关委托书
    /// </summary>
    public class OrderAgentProxy : IUnique
    {
        /// <summary>
        /// 订单ID\合同编号\订单编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public Order Order { get; set; }

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
                        var query = view.Where(item => item.OrderID == this.ID && item.Status == Enums.Status.Normal);
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

        /// <summary>
        /// 代理报关委托书文件
        /// </summary>
        public OrderFile File { get; set; }

        public MainOrderFile MainFile { get; set; }

        Purchaser purchaser = PurchaserContext.Current;
        

        /// <summary>
        /// 当跟单员审核代理报关委托书通过时发生
        /// </summary>
        public event OrderFileAuditedHanlder Approved;

        public OrderAgentProxy()
        {
            this.Approved += OrderAgentProxy_Approved;
        }

        private void OrderAgentProxy_Approved(object sender, OrderFileAuditedEventArgs e)
        {
            var order = e.AgentProxy.Order;
            if (order.Admin != null)
            {
                order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]审核通过了客户上传的代理报关委托书");
            }
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        public void Approve()
        {
            this.File.FileStatus = Enums.OrderFileStatus.Audited;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderFiles>(new { FileStatus = Enums.OrderFileStatus.Audited }, item => item.ID == this.File.ID);
            }

            this.OnApproved();
        }

        void OnApproved()
        {
            if (this.Approved != null)
            {
                this.Approved(this, new OrderFileAuditedEventArgs(this));
            }
        }

        /// <summary>
        /// 导出代理报关委托书PDF
        /// </summary>
        public PdfDocument ToPdf(Vendor vendor)
        {
            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();
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
            string message = $"{purchaser.ShortName}-代理报关委托书";
            page.Canvas.DrawString(message, font1, brush, width / 2, y, formatCenter);
            y += font1.MeasureString(message, formatCenter).Height + 8;

            //20190909 合同编号改为订单编号
            message = "订单编号: ";
            page.Canvas.DrawString(message + this.ID, font2, brush, width, y, formatRight);
            y += font2.MeasureString(message + this.ID, formatRight).Height + 5;

            #endregion

            #region 表格

            //创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(11);
            grid.Columns[0].Width = width * 3 / 100;
            grid.Columns[1].Width = width * 6 / 100;
            grid.Columns[2].Width = width * 20 / 100;
            grid.Columns[3].Width = width * 15 / 100;
            grid.Columns[4].Width = width * 20 / 100;
            grid.Columns[5].Width = width * 6 / 100;
            grid.Columns[6].Width = width * 6 / 100;
            grid.Columns[7].Width = width * 6 / 100;
            grid.Columns[8].Width = width * 6 / 100;
            grid.Columns[9].Width = width * 6 / 100;
            grid.Columns[10].Width = width * 6 / 100;

            //委托方、代理方信息
            PdfGridRow row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 11;
            row.Cells[0].Value = "委托方名称: " + this.Client.Company.Name;

            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 11;
            row.Cells[0].Value = "委托方收货信息: " + this.Client.Company.Name + "/地址: " + this.Client.Company.Address +
                                "/联系人: " + this.Client.Company.Contact.Name + "/电话: " + this.Client.Company.Contact.Mobile;

            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 11;
            row.Cells[0].Value = "代理方名称: " + purchaser.CompanyName;

            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 11;
            row.Cells[0].Value = "代理方收货信息: " + vendor.CompanyName + "/地址: " + vendor.Address +
                                "/联系人: " + vendor.Contact + "/电话: " + vendor.Tel;

            //产品信息
            row = grid.Rows.Add();
            row.Cells[0].Value = "序号";
            row.Cells[1].Value = "批号";
            row.Cells[2].Value = "品名";
            row.Cells[3].Value = "品牌";
            row.Cells[4].Value = "规格型号";
            row.Cells[5].Value = "产地";
            row.Cells[6].Value = "数量";
            row.Cells[7].Value = "单位";
            row.Cells[8].Value = "报关单价" + "(" + this.Currency + ")";
            row.Cells[9].Value = "报关总价" + "(" + this.Currency + ")";
            row.Cells[10].Value = "关税率";
            for (int i = 0; i < row.Cells.Count; i++)
            {
                if (i == 2 || i == 4)
                {
                    row.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                }
                else
                {
                    row.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                }
            }

            int sn = 0;
            var units = new Views.BaseUnitsView().ToList();
            foreach (var item in this.Items)
            {
                sn++;
                row = grid.Rows.Add();
                row.Cells[0].Value = sn.ToString();
                row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[1].Value = item.Batch;
                row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[2].Value = item.Category?.Name ?? item.Name;
                row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                row.Cells[3].Value = item.Manufacturer;
                row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[4].Value = item.Model;
                row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                row.Cells[5].Value = item.Origin;
                row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[6].Value = item.Quantity.ToString("0.####");
                row.Cells[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[7].Value = units.Where(u => u.Code == item.Unit).FirstOrDefault()?.Name ?? item.Unit;
                row.Cells[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[8].Value = item.UnitPrice.ToString("0.0000");
                row.Cells[8].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[9].Value = item.TotalPrice.ToRound(2).ToString("0.00");
                row.Cells[9].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[10].Value = item.ImportTax?.Rate.ToString("0.0000");
                row.Cells[10].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }

            //合计行
            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 6;
            row.Cells[0].Value = "合计:";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[6].Value = this.Items.Select(item => item.Quantity).Sum().ToString("0.####");
            row.Cells[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[9].Value = this.Items.Select(item => item.TotalPrice).Sum().ToString("0.00");
            row.Cells[9].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }

            PdfLayoutResult result = grid.Draw(page, new PointF(x, y));
            //是否换页
            if (pdf.Pages.Count > pageCount)
            {
                pageCount = pdf.Pages.Count;
                page = pdf.Pages[pdf.Pages.Count - 1];
                x = pdf.PageSettings.Margins.Left;
                y = pdf.PageSettings.Margins.Top;
            }
            y += result.Bounds.Height;

            var types = new Views.BasePackTypesView().ToList();
            var wrapType = types.Where(t => t.Code == this.WarpType).FirstOrDefault()?.Name;
            var totalGwt = this.Items.Where(item => item.GrossWeight != null).Select(item => item.GrossWeight).Sum().Value.ToRound(2);
            message = "包装类型: " + wrapType;
            if (this.PackNo != null)
            {
                message += " 总件数: " + this.PackNo;
            }
            if (totalGwt > 0M)
            {
                message += " 总毛重: " + totalGwt.ToString("0.##") + " KG";
            }
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font1.MeasureString(message + this.ID, formatLeft).Height + 5;

            #endregion

            #region 尾

            message = "温馨提示:\r\n" +
                      "1.我单位保证遵守《中华人民共和国海关法》及国家有关法规保证所提供的委托信息与所报的货物相符。\r\n" +
                      "2.委托方务必真实填写，若因委托方虚报、假报、多报、少报等因素造成的扣关、罚款等后果由委托方自行承担。\r\n" +
                      "3.委托方一份报关单最多20个型号，超过部分按另一单结算，依此类推。\r\n" +
                      "4.委托方需即时签字盖章回传，如因委托方延误回传造成的延迟申报，代理方不承担责任。";
            page.Canvas.DrawString(message, font2, brush, x, y, formatLeft);
            y += font2.MeasureString(message, formatLeft).Height + 20;

            message = "代理方盖章:\r\n" +
                      "日期:";
            page.Canvas.DrawString(message, font2, brush, 100, y, formatRight);
            message = "委托方(签字/盖章):\r\n" +
                      "日期:";
            page.Canvas.DrawString(message, font2, brush, 400, y, formatRight);

            //大赢家加上委托方章
            if (this.Order.Client.ClientType == Enums.ClientType.Internal)
            {
                PdfImage imageInternal = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.Order.Client.Company.Name + ".png"));
                page.Canvas.DrawImage(imageInternal, 410, y - 50);
            }

            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.SealUrl));
            page.Canvas.DrawImage(image, 100, y - 50);

            #endregion

            #region 公共组件

            //页眉、页脚、二维码、水印
            pdf.PdfMargins = new PdfMargins(10, 10);
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(pdf);
            string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.HeaderImg);

            pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, PurchaserContext.Current.OfficalWebsite);
            pdfDocumentHandle.HeaderFooter.GenerateFooter(PurchaserContext.Current.CompanyName);
            pdfDocumentHandle.Barcode.GenerateQRCode(this.ID, imageUrl);
            pdfDocumentHandle.Watermark.DrawWatermark(PurchaserContext.Current.CompanyName);

            #endregion

            return pdf;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveAs(string filePath,  Vendor vendor)
        {
            var pdf = this.ToPdf(vendor);
            pdf.SaveToFile(filePath);
            pdf.Close();
        }
    }
}
