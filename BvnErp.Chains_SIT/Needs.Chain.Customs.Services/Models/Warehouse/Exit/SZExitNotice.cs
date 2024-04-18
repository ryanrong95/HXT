using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Descriptions;
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
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using PdfDocument = Needs.Utils.SpirePdf.PdfDocument;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 深圳库房的出库通知
    /// </summary>
    public class SZExitNotice : ExitNotice
    {
        /// <summary>
        /// 香港出库通知明细项
        /// </summary>
        SZExitNoticeItems items;

        public SZExitNoticeItems SZItems
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.SZExitNoticeItemView())
                    {
                        var query = view.Where(item => item.ExitNoticeID == this.ID);
                        this.SZItems = new SZExitNoticeItems(query);
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

                this.items = new SZExitNoticeItems(value, new Action<SZExitNoticeItem>(delegate (SZExitNoticeItem item)
                {
                    item.ExitNoticeID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 送货状态
        /// </summary>
        private string deliveryStatus;
        public string DeliveryStatus
        {
            get
            {
                switch (this.ExitType)
                {
                    case ExitType.PickUp:
                        if (ExitNoticeStatus == ExitNoticeStatus.Exited || ExitNoticeStatus == ExitNoticeStatus.Completed)
                            deliveryStatus = "已提货";
                        else
                            deliveryStatus = "待提货";
                        break;
                    case ExitType.Delivery:
                    case ExitType.Express:
                        if (ExitNoticeStatus == ExitNoticeStatus.Exited || ExitNoticeStatus == ExitNoticeStatus.Completed)
                            deliveryStatus = "已送货";
                        else
                            deliveryStatus = "待送货";
                        break;
                    default:
                        deliveryStatus = null;
                        break;

                }

                return this.deliveryStatus;
            }
        }

        public CgPickingExcuteStatus CenterExeStatus { get; set; }

        public WaybillType CenterExitType { get; set; }
        public decimal CenterPackNo { get; set; }

        public SZExitNotice() : base()
        {
            base.WarehouseType = WarehouseType.ShenZhen;

        }

        public void OutStock()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新NoticeItems表状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>(new { ExitNoticeStatus = ExitNoticeStatus.Exited }, item => item.ExitNoticeID == this.ID);
                //更新ExitNotice主表的状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(new
                {
                    ExitNoticeStatus = (int)ExitNoticeStatus.Exited,
                    OutStockTime = DateTime.Now,
                }, item => item.ID == this.ID);

                var OrderIDs = this.SZItems.Select(t => t.Sorting.OrderID).Distinct().ToList();

                foreach(var orderid in OrderIDs)
                {
                    //上报出库动作
                    var order = new DeclaredOrdersView(reponsitory)[orderid];

                    order?.SetAdmin(this.Admin);
                    if (order != null)
                    {
                        order.ExitNoticeId = this.ID;
                    }
                    order?.WarehouseExit();
                }
               
            }
        }
        public void UpdatePrintStatus()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                if (this.IsPrint == Enums.IsPrint.UnPrint)
                {
                    //更新ExitNotice主表的状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(new { IsPrint = (int)Enums.IsPrint.Printed }, item => item.ID == this.ID);
                }

            }
        }

        public override void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //保存出库通知
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ExitNotice);
                reponsitory.Insert(this.ToLinq());

                //保存出库信息
                if (ExitType == ExitType.PickUp)
                {
                    this.ExitDeliver.Consignee.Enter();
                }
                else if (ExitType == ExitType.Delivery)
                {
                    this.ExitDeliver.Deliver.Enter();
                }
                else if (ExitType == ExitType.Express)
                {
                    this.ExitDeliver.Expressage.Enter();
                }
                this.ExitDeliver.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ExitDeliver);
                this.ExitDeliver.ExitNoticeID = this.ID;
                this.ExitDeliver.Code = this.ID;
                reponsitory.Insert(this.ExitDeliver.ToLinq());

                //保存出库通知项
                foreach (var item in this.Items)
                {
                    item.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ExitNoticeItem);
                    item.ExitNoticeID = this.ID;
                    reponsitory.Insert(item.ToLinq());
                }
            }

            base.OnEnterSuccess();
        }

        //Purchaser purchaser = PurchaserContext.Current;

        /// <summary>
        /// 导出PDF
        /// </summary>
        public PdfDocument ToPdf()
        {
            Purchaser purchaser = PurchaserContext.Current;

            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();
            pdf.PageSettings.Margins = new PdfMargins(30, 60);

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4);
            int pageCount = pdf.Pages.Count;

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 14f, FontStyle.Bold), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);
            PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头
            float x = 0, y = 5f;
            var exitDeliver = this.ExitDeliver;
            var ladingBill = this.LadingBill;
            float width = page.Canvas.ClientSize.Width;

            string message = $"{purchaser.CompanyName}提货单";
            page.Canvas.DrawString(message, font1, brush, width / 2, y, formatCenter);
            y += font1.MeasureString(message, formatCenter).Height + 14;
            //var PackingDate = new EntryNoticeView().FirstOrDefault(item => item.Order.ID == this.Order.ID && item.EntryNoticeStatus == EntryNoticeStatus.Sealed).UpdateDate;
            string orderMessage = "订单编号：" + this.Order.ID; //+ "      装箱日期：" + PackingDate.ToString("yyyy-MM-dd");
            string packNo = "总件数：" + exitDeliver.PackNo;
            page.Canvas.DrawString(orderMessage, font3, brush, 0, y);
            page.Canvas.DrawString(packNo, font3, brush, width, y, formatRight);
            y += font1.MeasureString(orderMessage, formatRight).Height + 8;
            //创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(1);
            grid.Columns[0].Width = width;

            //表头信息
            PdfGridRow row = grid.Rows.Add();
            row.Cells[0].Value = "收货人：" + exitDeliver.Name;
            row = grid.Rows.Add();
            row.Cells[0].Value = "提货人：" + exitDeliver.Consignee.Name + "\r证件类型：" + ladingBill.IDType + "    证件号码：" + ladingBill.IDCard + "    电话：" + ladingBill.DeliveryTel;

            //设置边框
            SetCellBorder(grid);
            PdfLayoutResult result = grid.Draw(page, new PointF(x, y));
            y += result.Bounds.Height + 5;

            #endregion

            #region 货物明细

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(7);
            grid.Columns[0].Width = width * 6f / 100;
            grid.Columns[1].Width = width * 10f / 100;
            grid.Columns[2].Width = width * 14f / 100;
            grid.Columns[3].Width = width * 25f / 100;
            grid.Columns[4].Width = width * 25f / 100;
            grid.Columns[5].Width = width * 10f / 100;
            grid.Columns[6].Width = width * 10f / 100;

            //产品信息
            row = grid.Rows.Add();
            row.Cells[0].Value = "序号";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "库位号";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[2].Value = "箱号";
            row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[3].Value = "品名";
            row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = "型号";
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[5].Value = "品牌";
            row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[6].Value = "数量";
            row.Cells[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            var sn = 0;
            foreach (var item in this.SZItems)
            {
                sn++;
                row = grid.Rows.Add();
                row.Cells[0].Value = sn.ToString();
                row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[1].Value = item.StoreStorage.StockCode;
                row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[2].Value = item.StoreStorage.BoxIndex;
                row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[3].Value = item.Sorting.OrderItem.Category.Name;
                row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[4].Value = item.Sorting.OrderItem.Model;
                row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[5].Value = item.Sorting.OrderItem.Manufacturer;
                row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[6].Value = item.Quantity.ToString("0");
                row.Cells[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;

            #endregion

            #region 服务评价

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width * 0.2f;
            grid.Columns[1].Width = width * 0.8f;

            row = grid.Rows.Add();
            row.Cells[0].Value = "对本次服务评价：";
            row.Cells[1].Value = "□  优秀    □  一般    □  差";
            row = grid.Rows.Add();
            row.Cells[0].Value = "客户意见或建议：";
            row.Cells[1].Value = "";
            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 2;
            row.Cells[0].Value = "本公司已如数收到上述货物和发票，无货物数量损失，无货物损坏。";

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 15;

            #endregion

            #region 尾

            message = "提货人签字/签章:";
            page.Canvas.DrawString(message, font2, brush, width * 0.6f, y, formatLeft);
            y += font1.MeasureString(message, formatCenter).Height + 2;
            message = "提货日期: _____年 ____月 ____ 日";
            page.Canvas.DrawString(message, font2, brush, width * 0.6f, y, formatLeft);
            y += font1.MeasureString(message, formatCenter).Height + 8;
            #endregion

            #region 公共组件

            //页眉、页脚、二维码、水印
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(pdf);
            string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.HeaderImg);

            pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, PurchaserContext.Current.OfficalWebsite);
            pdfDocumentHandle.HeaderFooter.GenerateFooter(PurchaserContext.Current.CompanyName);
            pdfDocumentHandle.Barcode.GenerateQRCode(this.ExitDeliver.Code, imageUrl);
            pdfDocumentHandle.Watermark.DrawWatermark(PurchaserContext.Current.CompanyName);

            #endregion

            return pdf;
        }

        /// <summary>
        /// 保存内存流
        /// </summary>
        public MemoryStream SaveAs()
        {
            var pdf = this.ToPdf();
            MemoryStream mStream = new MemoryStream();
            pdf.SaveToStream(mStream);
            pdf.Close();
            return mStream;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveAs(string filePath)
        {
            PdfDocument pdf = this.ToPdf();
            pdf.SaveToFile(filePath);
            pdf.Close();
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
    }
}