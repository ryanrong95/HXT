using Needs.Ccs.Services.Models.Declare.PackingData;
using Needs.Utils;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单的装箱单
    /// </summary>
    public class PackingList
    {
        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNo { get; set; }

        /// <summary>
        /// 受益人
        /// 报关公司
        /// 代理报关关系中的买方
        /// </summary>
        public Beneficiary Beneficiary { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public string TradeDate { get; set; }

        public decimal GrossWet { get; set; }

        public decimal NetWet { get; set; }

        /// <summary>
        /// 产品总数量
        /// </summary>
        public decimal TotalProductQutity
        {
            get
            {
                return this.items.Sum(item => item.Quantity);
            }
        }

        /// <summary>
        /// 总净重
        /// </summary>
        public decimal TotalNetWeight
        {
            get
            {
                return this.items.Sum(item => item.NetWeight);
            }
        }

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal TotalGrossWeight
        {
            get
            {
                return this.items.Sum(item => item.GrossWeight);
            }
        }

        /// <summary>
        /// 总件数
        /// </summary>
        public decimal TotalPacks { get; set; }

        public List<DecProduct> PackItems { get; set; }

        private IEnumerable<DecProduct> items;
        /// <summary>
        /// 产品明细
        /// </summary>
        public IEnumerable<DecProduct> Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.items = value;
            }
        }

        internal PackingList()
        {

        }

        public PackingList(DecHead decHead)
        {
            this.Beneficiary = new Beneficiary
            {
                Company = new Company
                {
                    Name = decHead.AgentName,
                    CustomsCode = decHead.AgentCusCode,
                    CIQCode = decHead.AgentCiqCode
                }
            };

            this.Items = decHead.Lists.Select(item => new DecProduct
            {
                BoxIndex = item.CaseNo,
                Name = item.GName,
                Model = item.GoodsModel,
                Manufacturer = item.GoodsBrand,
                Origin = item.OriginCountry,
                Quantity = item.GQty,
                UnitPrice = item.DeclPrice,
                TotalPrice = item.DeclTotal.ToRound(2),
                Currency = item.TradeCurr,
                NetWeight = item.NetWt.HasValue ? item.NetWt.Value : 0,
                GrossWeight = item.GrossWt.HasValue ? item.GrossWt.Value : 0,
            });

            //特殊处理：叉车卡板的重量，分摊到型号上
            this.PackItems = this.Items.ToList();
            this.GrossWet = decHead.GrossWt;
            var addedWeight = this.GrossWet - this.TotalGrossWeight;
            if (addedWeight >= 1)
            {
                var sumQty = this.PackItems.Sum(t=>t.Quantity);
                foreach (var item in this.PackItems)
                {
                    item.GrossWeight = item.GrossWeight + (addedWeight * item.Quantity / sumQty).ToRound(2);
                }
            }

            this.TradeDate = decHead.IEDate;
            this.ContractNo = decHead.ContrNo;
            this.TotalPacks = decHead.PackNo;
            //this.GrossWet = decHead.GrossWt;
            this.NetWet = decHead.NetWt;
        }

        /*
        public PdfDocument ToPDF()
        {
            #region Pdf对象声明

            //Create a pdf document
            PdfDocument doc = new PdfDocument();

            //Set the margin
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(0.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(1.57f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;

            //Create one page
            PdfPageBase page = doc.Pages.Add(PdfPageSize.A4, margin);

            float y = 5;

            //Title
            PdfBrush brush1 = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 12f, FontStyle.Bold), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 9f, FontStyle.Regular), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头
            //TODO:如何很好的区分报关公司？
            var agentName = PurchaserContext.Current.CompanyName;

            page.Canvas.DrawString(VendorContext.Current.CompanyName, font1, brush1, page.Canvas.ClientSize.Width / 2, y, formatCenter);

            y = y + font1.MeasureString("SALES CONTRACT ", formatCenter).Height;
            y = y + 2;

            page.Canvas.DrawString(VendorContext.Current.AddressEN, font1, brush1, page.Canvas.ClientSize.Width / 2, y, formatCenter);


            y = y + font2.MeasureString("SALES CONTRACT ", formatCenter).Height;
            y = y + 5;

            page.Canvas.DrawString("装 箱 单", font1, brush1, page.Canvas.ClientSize.Width / 2, y, formatCenter);
            y = y + font2.MeasureString("SALES CONTRACT ", formatCenter).Height;
            y = y + 2;

            page.Canvas.DrawString("PACKING LIST", font2, brush1, page.Canvas.ClientSize.Width / 2, y, formatCenter);
            y = y + font2.MeasureString("SALES CONTRACT ", formatCenter).Height;
            y = y + 2;

            var ddate = DateTime.ParseExact(this.TradeDate, "yyyyMMdd", new System.Globalization.CultureInfo("zh-CN"), System.Globalization.DateTimeStyles.AllowWhiteSpaces);

            page.Canvas.DrawString("致:" + agentName, font2, brush1, 0, y, formatLeft);
            page.Canvas.DrawString(" 日期：" + ddate.ToString("yyyy-MM-dd"), font2, brush1, page.Canvas.ClientSize.Width, y, formatRight);
            y = y + font2.MeasureString("合同编号：WL20180525-01", formatRight).Height;
            y = y + 2;
            page.Canvas.DrawString("地址："+PurchaserContext.Current.Address, font2, brush1, 0, y, formatLeft);
            page.Canvas.DrawString("合同编号：" + this.ContractNo, font2, brush1, page.Canvas.ClientSize.Width, y, formatRight);
            y = y + font2.MeasureString("合同编号：WL20180525-01", formatRight).Height;
            y = y + 5;

            #endregion

            #region 表格

            PdfGrid grid = new PdfGrid();

            grid.Columns.Add(8);

            grid.Columns[0].Width = 30;
            grid.Columns[1].Width = 30;
            grid.Columns[2].Width = 120;
            grid.Columns[3].Width = 120;
            grid.Columns[4].Width = 80;
            grid.Columns[5].Width = 40;
            grid.Columns[6].Width = 40;
            grid.Columns[7].Width = 40;

            #region 标题
            PdfGridRow rowTitle = grid.Rows.Add();

            rowTitle.Cells[0].Value = "箱号";
            rowTitle.Cells[1].Value = "序号";
            rowTitle.Cells[2].Value = "货物名称";
            rowTitle.Cells[3].Value = "货物型号";
            rowTitle.Cells[4].Value = "品牌";
            rowTitle.Cells[5].Value = "数量(PCS)";
            rowTitle.Cells[6].Value = "净重(KGS)";
            rowTitle.Cells[7].Value = "毛重(KGS)";
            rowTitle.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);
            #endregion

            int rownumber = 1;

            decimal totalGwt = Math.Ceiling(this.TotalGrossWeight < 2M ? this.GrossWet : this.TotalGrossWeight);
            decimal totalNwt = this.TotalNetWeight.ToRound(2) < 1M ? this.NetWet : this.TotalNetWeight.ToRound(2);
            string totalQty = this.TotalProductQutity.ToString("0.####");

            var packs = this.items.Select(t => t.BoxIndex).Distinct().OrderBy(item=>item);
            int[] rowmerge = new int[packs.Count()];           

            var i = 0;           
            foreach (var pack in packs)
            {               
                rowmerge[i] = rownumber;                
                
                foreach (var m in this.items.Where(t => t.BoxIndex == pack))
                {
                    PdfGridRow row = grid.Rows.Add();
                    row.Cells[0].Value = m.BoxIndex;
                    row.Cells[1].Value = rownumber.ToString();
                    row.Cells[2].Value = m.Name;
                    row.Cells[3].Value = m.Model;
                    row.Cells[4].Value = m.Manufacturer;
                    row.Cells[5].Value = m.Quantity.ToString("0.####");
                    row.Cells[6].Value = m.NetWeight.ToRound(2).ToString("0.##");
                    row.Cells[7].Value = m.GrossWeight.ToRound(2).ToString("0.##");                  
                    ++rownumber;
                    row.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);
                }
                ++i;
            }

            //合计行
            PdfGridRow rowSum = grid.Rows.Add();
            rowSum.Cells[0].Value = "合计:";
            rowSum.Cells[1].Value = "";
            rowSum.Cells[2].Value = "";
            rowSum.Cells[3].Value = "";
            rowSum.Cells[4].Value = "";
            rowSum.Cells[5].Value = totalQty;
            rowSum.Cells[6].Value = totalNwt.ToRound(2).ToString();
            rowSum.Cells[7].Value = totalGwt.ToString();
            rowSum.Cells[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            rowSum.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);

            //箱号，毛重合并
            var irow = 0;
            foreach (var pack in packs)
            {
                var span = this.items.Count(t => t.BoxIndex == pack);
                grid.Rows[rowmerge[irow]].Cells[0].RowSpan = span;
                grid.Rows[rowmerge[irow]].Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                ////计算合并后的毛重值
                decimal combineGw = 0;
                foreach (var gridrow in grid.Rows)
                {
                    if (gridrow.Cells[0].Value.ToString() == pack)
                    {
                        combineGw += Convert.ToDecimal(gridrow.Cells[7].Value);
                    }
                }
                grid.Rows[rowmerge[irow]].Cells[7].Value = combineGw.ToString("0.##");
                grid.Rows[rowmerge[irow]].Cells[7].RowSpan = span;
                grid.Rows[rowmerge[irow]].Cells[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                irow++;
            }

            //合计行合并
            grid.Rows[rownumber].Cells[0].ColumnSpan = 5;
            grid.Rows[rownumber].Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //Set color of border
            PdfBorders border = new PdfBorders();
            border.All = new PdfPen(Color.LightBlue, 0.01f);

            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders = border;
                }
            }


            PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
            y = y + result.Bounds.Height;
            #endregion

            #region 尾

            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VendorContext.Current.SealUrl));
            page.Canvas.DrawImage(image, 80, y);

            page.Canvas.DrawString("总件数: " + this.TotalPacks + " 纸箱", font2, brush1, 0, y, formatLeft);
            y = y + font2.MeasureString("SALES CONTRACT ", formatLeft).Height;
            y = y + 5;

            page.Canvas.DrawString("总数量: " + totalQty + " PCS", font2, brush1, 0, y, formatLeft);
            y = y + font2.MeasureString("SALES CONTRACT ", formatLeft).Height;
            y = y + 5;

            page.Canvas.DrawString("总净重: " + totalNwt.ToRound(2) + " KGS", font2, brush1, 0, y, formatLeft);
            y = y + font2.MeasureString("SALES CONTRACT ", formatLeft).Height;
            y = y + 5;

            page.Canvas.DrawString("总毛重: " + totalGwt + " KGS", font2, brush1, 0, y, formatLeft);
            y = y + font2.MeasureString("SALES CONTRACT ", formatLeft).Height;
            y = y + 5;


            #endregion

            return doc;

        }
        */

        public string[] SaveAs(Vendor vendor)
        {
            var result = new string[3];
            string fileName = this.ContractNo + "-箱单.pdf";

            try
            {
                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(SysConfig.DeclareDirectory);
                fileDic.CreateDataDirectory();
                //doc.SaveToFile(fileDic.FilePath);

                //箱单删除后再生成
                if (System.IO.File.Exists(fileDic.FilePath))
                {
                    System.IO.File.Delete(fileDic.FilePath);
                }


                string totalQty = this.TotalProductQutity.ToString("0.####");
                decimal totalNwt = this.TotalNetWeight.ToRound(2) < 1M ? this.NetWet : this.TotalNetWeight.ToRound(2);
                decimal totalGwt = Math.Ceiling(this.TotalGrossWeight < 2M ? this.GrossWet : this.TotalGrossWeight);

                if (!string.IsNullOrEmpty(vendor.PdfStyle) && vendor.PdfStyle == "style1")
                {
                    PackingListCY packingListCY = new PackingListCY(
                        公司名: vendor.CompanyName,
                        致: PurchaserContext.Current.CompanyName,
                        日期: DateTime.ParseExact(this.TradeDate, "yyyyMMdd", new System.Globalization.CultureInfo("zh-CN"),
                                                    System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy-MM-dd"),
                        地址: PurchaserContext.Current.Address,
                        合同编号: this.ContractNo,

                        总件数: this.TotalPacks + " 纸箱",
                        总数量: totalQty,
                        总净重: totalNwt.ToRound(2).ToString(),
                        总毛重: Math.Ceiling(this.GrossWet).ToString(),
                        items: this.PackItems,
                        vendor: vendor,
                        fileName: fileDic.FilePath);
                    packingListCY.Execute();
                }
                else if (!string.IsNullOrEmpty(vendor.PdfStyle) && vendor.PdfStyle == "style2")
                {
                    PackingListWLT packingListWLT = new PackingListWLT(
                        公司名: vendor.CompanyName,
                        致: PurchaserContext.Current.CompanyName,
                        日期: DateTime.ParseExact(this.TradeDate, "yyyyMMdd", new System.Globalization.CultureInfo("zh-CN"),
                                                    System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy-MM-dd"),
                        地址: PurchaserContext.Current.Address,
                        合同编号: this.ContractNo,

                        总件数: this.TotalPacks + " 纸箱",
                        总数量: totalQty,
                        总净重: totalNwt.ToRound(2).ToString(),
                        总毛重: Math.Ceiling(this.GrossWet).ToString(),
                        items: this.PackItems,
                        vendor: vendor,
                        fileName: fileDic.FilePath);
                    packingListWLT.Execute();
                }
                else
                {
                    //PdfDocument doc = this.ToPDF();
                    PackingPdf pdf = new PackingPdf(
                        公司名: vendor.CompanyName,
                        致: PurchaserContext.Current.CompanyName,
                        日期: DateTime.ParseExact(this.TradeDate, "yyyyMMdd", new System.Globalization.CultureInfo("zh-CN"),
                                                    System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy-MM-dd"),
                        地址: PurchaserContext.Current.Address,
                        合同编号: this.ContractNo,

                        总件数: this.TotalPacks + " 纸箱",
                        总数量: totalQty,
                        总净重: totalNwt.ToRound(2).ToString(),
                        总毛重: Math.Ceiling(this.GrossWet).ToString(),//totalGwt.ToString(),

                        items: this.PackItems);
                    pdf.Save(fileDic.FilePath, vendor);
                }

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileDic.FilePath);
                result[0] = fileName;
                result[1] = fileDic.VirtualPath;
                result[2] = fileInfo.Length.ToString();
            }
            catch (Exception ex)
            {
                //filePath = "";
                throw ex;
            }

            return result;
        }
    }
}