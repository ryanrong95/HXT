using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 深圳库房的入库通知
    /// </summary>
    public class SZEntryNotice : EntryNotice
    {
        /// <summary>
        /// 报关单
        /// </summary>
        public override DecHead DecHead { get; set; }

        /// <summary>
        /// 出库通知明细项
        /// </summary>
        SZEntryNoticeItems items;
        public SZEntryNoticeItems SZItems
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.SZEntryNoticeItemView())
                    {
                        var query = view.Where(item => item.EntryNoticeID == this.ID);
                        this.SZItems = new SZEntryNoticeItems(query);
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

                this.items = new SZEntryNoticeItems(value, new Action<SZEntryNoticeItem>(delegate (SZEntryNoticeItem item)
                {
                    item.EntryNoticeID = this.ID;
                }));
            }
        }

        public event WarehouseEntriedEventHanlder SZEntried;
        public SZEntryNotice() : base()
        {
            base.WarehouseType = WarehouseType.ShenZhen;
            base.SortingRequire = SortingRequire.Packed;
            SZEntried += Warehouse_Entried;


        }

        virtual protected void OnSZEntried()
        {
            if (this != null && this.SZEntried != null)
            {
                this.SZEntried(this, new WarehouseEntriedEventArgs(this));
            }
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Abandon()
        {
            base.Abandon();
        }

        /// <summary>
        /// 导出换汇文件
        /// </summary>
        /// <returns></returns>
        public string ToPDf()
        {
            PdfDocument pdf = ToInBoundPDf();
            string fileName = this.ID + "入库单" + ".pdf";
            FileDirectory fileDic = new FileDirectory(fileName);
            fileDic.SetChildFolder(SysConfig.Warehouse);
            fileDic.CreateDataDirectory();
            pdf.SaveToFile(fileDic.FilePath);
            pdf.Close();
            return fileDic.FilePath;
        }

        private PdfDocument ToInBoundPDf()
        {
            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();

            //设置margin
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(0.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(1.57f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4, margin);

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 12f, FontStyle.Bold), true);

            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头
            float y = 5;
            float width = page.Canvas.ClientSize.Width;
            #endregion

            #region   
            ////创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 8f), true);

            //设置列宽
            grid.Columns.Add(8);
            grid.Columns[0].Width = width * 0.05f;
            grid.Columns[1].Width = width * 0.2f;
            grid.Columns[2].Width = width * 0.15f;
            grid.Columns[3].Width = width * 0.05f;
            grid.Columns[4].Width = width * 0.1f;
            grid.Columns[5].Width = width * 0.1f;
            grid.Columns[6].Width = width * 0.2f;
            grid.Columns[7].Width = width * 0.15f;

            PdfGridRow row = grid.Rows.Add();
            row.Height = 20;
            row.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 14f, FontStyle.Bold), true); ;
            row.Cells[0].ColumnSpan = 8;
            row.Cells[0].Value = "入库单";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //产品信息
            row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].ColumnSpan = 2;
            row.Cells[0].Value = "";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[2].ColumnSpan = 4;
            row.Cells[2].Value = "入库日期：" + this.CreateDate.ToString("yyyy-MM-dd");
            row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[6].Value = "序号";
            row.Cells[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[7].Value = this.DecHead.ID.ToString();
            row.Cells[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            //信息列表
            row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = "序号";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "品名";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[2].Value = "型号";
            row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[3].Value = "单位";
            row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = "数量";
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[5].Value = "单价";
            row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[6].Value = "金额";
            row.Cells[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[7].Value = "备注";
            row.Cells[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            // decimal totalAmount = 0;
            int count = 1;
            foreach (var item in this.SZItems)
            {
                row = grid.Rows.Add();
                row.Height = 20;
                row.Cells[0].Value = count.ToString();
                row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                count++;
                row.Cells[1].Value = item.DecList.GName.ToString();
                row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[2].Value = item.DecList.GoodsModel.ToString();
                row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[3].Value = item.DecList.GUnit.ToString();
                row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[4].Value = item.DecList.GQty.ToString();
                row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[5].Value = item.DecList.DeclPrice.ToString();
                row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[6].Value = item.DecList.DeclTotal.ToString();
                row.Cells[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                row.Cells[7].Value = item?.Summary;
                row.Cells[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }
            PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
            y += result.Bounds.Height + 1;
            font2 = new PdfTrueTypeFont(new Font("SimSun", 11f, FontStyle.Regular), true);
            page.Canvas.DrawString("  主管：鲁亚慧", font2, brush, 10, y, formatLeft);
            page.Canvas.DrawString("  会计：鲁亚慧", font2, brush, 130, y, formatLeft);
            page.Canvas.DrawString("  报管员：商庆房", font2, brush, 250, y, formatLeft);
            page.Canvas.DrawString("  经手人：杨端峰", font2, brush, 370, y, formatLeft);
            #endregion
            return pdf;
        }

        /// <summary>
        /// 深圳入库(即，上架)
        /// </summary>
        public void Entry(string StockCode)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新自身状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(new { EntryNoticeStatus = Enums.EntryNoticeStatus.Boxed }, item => item.ID == this.ID);
                //更新通知项状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(new { EntryNoticeStatus = Enums.EntryNoticeStatus.Boxed }, item => item.EntryNoticeID == this.ID);


                //上架
                foreach (var item in SZItems)
                {
                    var szSorting = new SZSortingsView(reponsitory).Where(t => t.EntryNoticeItemID == item.ID).FirstOrDefault();
                    SZStoreStorage storage = new SZStoreStorage();
                    storage.StockCode = StockCode;
                    storage.Purpose = StockPurpose.Storaged;
                    storage.InStore(szSorting);
                    //20190420
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { SZPackingDate = DateTime.Now }, x => x.EntryNoticeItemID == item.ID);
                }
            }

            this.OnSZEntried();
        }

        private void Warehouse_Entried(object sender, WarehouseEntriedEventArgs e)
        {
            var order = e.EntryNotice.Order;
            var admin = e.EntryNotice.Operator;
            order.Trace(admin, OrderTraceStep.SZProcessing, "您的订单进入【深圳库房】,准备出库");
        }


        #region 深圳库房上架

        //public event SZWarehouseOnStockedEventHanlder SZWarehouseOnStocked;

        //virtual protected void OnSZWarehouseOnStocked(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, string entryNoticeID)
        //{
        //    if (this != null && this.SZWarehouseOnStocked != null)
        //    {
        //        this.SZWarehouseOnStocked(this, new SZWarehouseOnStockedEventArgs(reponsitory, entryNoticeID));
        //    }
        //}

        //private void SZWarehouse_OnStocked((object sender, SZWarehouseOnStockedEventArgs e)
        //{

        //}

        public void OnStock(string voyageID, string stockCode, List<Views.SZOnStockView.TargetSortingModel> listBox)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
            {
                foreach (var box in listBox)
                {
                    OnStockOneBox(reponsitory, box.OrderID, voyageID, box.BoxIndex, stockCode);
                }

                reponsitory.Submit();
            }
        }

        private void OnStockOneBox(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, string orderID, string voyageID, string boxIndex, string stockCode)
        {
            
            List<Views.SZOnStockView.TargetSortingModel> listTargetSortingModel = new Views.SZOnStockView(reponsitory).GetTargetSortingModel(voyageID, orderID, boxIndex).ToList();
            if (listTargetSortingModel != null && listTargetSortingModel.Any())
            {
                List<Layer.Data.Sqls.ScCustoms.StoreStorages> listLayerStoreStorages = new List<Layer.Data.Sqls.ScCustoms.StoreStorages>();

                var sortingIDs = listTargetSortingModel.Select(t => t.SortingID).ToArray();

                var existStoreStorages = (from storeStorage in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StoreStorages>()
                                         where sortingIDs.Contains(storeStorage.SortingID) && storeStorage.Purpose == (int)Enums.StockPurpose.Storaged
                                            && storeStorage.Status == (int)Enums.Status.Normal
                                         select new
                                         {
                                             StoreStorageID = storeStorage.ID,
                                             SortingID = storeStorage.SortingID,
                                         }).ToList();


                foreach (var targetSortingModel in listTargetSortingModel)
                {
                    var existStoreStorage = existStoreStorages.Where(t => t.SortingID == targetSortingModel.SortingID).FirstOrDefault();

                    if (existStoreStorage == null)
                    {
                        listLayerStoreStorages.Add(new Layer.Data.Sqls.ScCustoms.StoreStorages
                        {
                            ID = Needs.Overall.PKeySigner.Pick(PKeyType.StoreStorage),
                            OrderItemID = targetSortingModel.OrderItemID,
                            SortingID = targetSortingModel.SortingID,
                            //ProductID = targetSortingModel.ProductID,
                            Purpose = (int)Enums.StockPurpose.Storaged,
                            Quantity = targetSortingModel.Quantity,
                            StockCode = stockCode,
                            BoxIndex = targetSortingModel.BoxIndex,
                            Status = (int)Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                        });
                    }
                    else
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.StoreStorages>(new { StockCode = stockCode, UpdateDate = DateTime.Now, },
                            t => t.ID == existStoreStorage.StoreStorageID);
                    }
                }

                reponsitory.Insert(listLayerStoreStorages.ToArray());
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { SZPackingDate = DateTime.Now }, t => sortingIDs.Contains(t.ID));
            }

        }

        public void Complete(string voyageID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                List<Views.SZOnStockView.SZEntryNoticeModel> listSZEntryNoticeModel = new Views.SZOnStockView(reponsitory).GetSZEntryNoticeModel(voyageID).ToList();

                if (listSZEntryNoticeModel != null && listSZEntryNoticeModel.Any())
                {
                    var EntryNoticeIDs = listSZEntryNoticeModel.Select(t => t.EntryNoticeID).ToArray();
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNotices>(new { EntryNoticeStatus = Enums.EntryNoticeStatus.Boxed, UpdateDate = DateTime.Now, }, 
                        t => EntryNoticeIDs.Contains(t.ID) && t.Status == (int)Enums.Status.Normal && t.EntryNoticeStatus == (int)Enums.EntryNoticeStatus.UnBoxed);
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(new { EntryNoticeStatus = Enums.EntryNoticeStatus.Boxed, UpdateDate = DateTime.Now, }, 
                        t => EntryNoticeIDs.Contains(t.EntryNoticeID) && t.Status == (int)Enums.Status.Normal && t.EntryNoticeStatus == (int)Enums.EntryNoticeStatus.UnBoxed);
                }
            }
        }

        #endregion
    }
}