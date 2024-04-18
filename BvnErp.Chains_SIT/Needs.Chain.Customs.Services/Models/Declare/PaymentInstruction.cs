extern alias globalB;
using globalB::iTextSharp.text;
using globalB::iTextSharp.text.pdf;
using Needs.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 商业发票
    /// </summary>
    public class PaymentInstruction
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

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

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
        /// 总金额
        /// </summary>
        public decimal TotalPrice
        {
            get
            {
                return this.items.Sum(item => item.TotalPrice);
            }
        }
        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal OrderTotalPrice
        {
            get { return this.items.Sum(item => item.OrderTotalPrice); }
        }

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

        internal PaymentInstruction()
        {

        }

        public PaymentInstruction(DecHead decHead)
        {
            this.Beneficiary = new Beneficiary
            {
                Company = new Company
                {
                    Name = decHead.AgentName,
                    CustomsCode = decHead.AgentCusCode,
                    CIQCode = decHead.AgentCiqCode,                  
                }
            };

            this.Items = decHead.Lists.OrderBy(item => item.GNo).Select(item => new DecProduct
            {
                BoxIndex = item.CaseNo,
                Name = item.GName,
                Model = item.GoodsModel,
                Manufacturer = item.GoodsBrand,
                Origin = item.OriginCountryName,
                Quantity = item.GQty,
                UnitPrice = item.DeclPrice,
                TotalPrice = item.DeclTotal.ToRound(2),
                Currency = item.TradeCurr,

                // 2020-09-03 by yeshuangshuang
                OrderUnitPrice = item.OrderPrice,
                OrderTotalPrice = item.OrderTotal.ToRound(2)
            });

            this.Currency = this.Items.FirstOrDefault().Currency;

            this.TradeDate = decHead.IEDate;
            this.ContractNo = decHead.ContrNo;
        }
      

        public void ToPDF(string filePath, Vendor vendor)
        {
            #region 数据填充dt

            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("GoodsName", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("Brand", typeof(string));
            dt.Columns.Add("ProductionPlace", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            dt.Columns.Add("UnitPrice", typeof(string));
            dt.Columns.Add("TotalPrice", typeof(string));

            string agentName = this.Beneficiary.Company.Name;

            DataRow drTitle = dt.NewRow();
            drTitle["NO"] = "序号";
            drTitle["GoodsName"] = "货物名称";
            drTitle["Model"] = "货物型号";
            drTitle["Brand"] = "品牌";
            drTitle["ProductionPlace"] = "产地";
            drTitle["Qty"] = "数量(PCS)";
            drTitle["UnitPrice"] = "单价(" + this.Currency + ")";
            drTitle["TotalPrice"] = "总价(" + this.Currency + ")";
            dt.Rows.Add(drTitle);

            decimal totalQutity = this.TotalProductQutity;

            int index = 0;
            foreach (var entity in this.Items)
            {
                DataRow dr = dt.NewRow();
                dr["NO"] = index + 1;
                dr["GoodsName"] = entity.Name;
                dr["Model"] = entity.Model;
                dr["Brand"] = entity.Manufacturer;
                dr["ProductionPlace"] = entity.Origin;
                dr["Qty"] = entity.Quantity.ToString("0.####");
                dr["UnitPrice"] = entity.UnitPrice.ToString("0.####");
                dr["TotalPrice"] = entity.TotalPrice.ToRound(2);
                dt.Rows.Add(dr);

                index++;
            }

            //取币制的中文
            string CurrencyName = "";
            CurrencyName = new Needs.Ccs.Services.Views.BaseCurrenciesView().Where(item => item.Code == this.Currency).FirstOrDefault()?.Name;

            DataRow dtLastRow1 = dt.NewRow();
            dtLastRow1["NO"] = "合计";
            dtLastRow1["GoodsName"] = "";
            dtLastRow1["Model"] = "";
            dtLastRow1["Brand"] = "";
            dtLastRow1["ProductionPlace"] = "";
            dtLastRow1["Qty"] = totalQutity.ToString("0.####");
            dtLastRow1["UnitPrice"] = "";
            dtLastRow1["TotalPrice"] = this.TotalPrice.ToRound(2);
            dt.Rows.Add(dtLastRow1);


            //成交方式 总金额
            DataRow dtLastRow2 = dt.NewRow();
            dtLastRow2["NO"] = "成交方式： CIF深圳";
            dtLastRow2["GoodsName"] = "";
            dtLastRow2["Model"] = "";
            dtLastRow2["Brand"] = "总金额";
            dtLastRow2["ProductionPlace"] = this.TotalPrice.ToRound(2).ToChineseAmount() + " " + CurrencyName; 
            dtLastRow2["Qty"] = "";
            dtLastRow2["UnitPrice"] = "";
            dtLastRow2["TotalPrice"] = "";
            dt.Rows.Add(dtLastRow2);
          
            #endregion

            #region Pdf对象声明

            //中文字体
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Rectangle rec = new Rectangle(PageSize.A4);
            //创建一个文档实例。 去除边距
            Document document = new Document(rec);
            document.SetMargins(10f, 10f, 2f, 2f);

            #endregion
            try
            {
                //创建一个writer实例
                PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.OpenOrCreate));
                //打开当前文档
                document.Open();

                #region 头
                //标题
                PdfPTable tableTitle = new PdfPTable(3);
                tableTitle.TotalWidth = 574; //绝对宽度
                tableTitle.LockedWidth = true;
                tableTitle.SetWidths(new int[] { 10, 80, 10 });

                //1 Tile 左侧 Logo
                Image imageLogo = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.LogoUrl));
                //同比例缩放
                //float resizeWidth = image.Width;
                //float resizeHeight = image.Height;
                imageLogo.ScalePercent(24f);
                //imageLogo.Width = 10;
                //imageLogo.Height = 10;
                var titleLeftCell = new PdfPCell(imageLogo) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 8f, };
                titleLeftCell.DisableBorderSide(15);
                tableTitle.AddCell(titleLeftCell);

                //2 Tile 中间 卖方公司名称 
                Paragraph paragraphTitle1 = new Paragraph(new Chunk(vendor.CompanyName, new Font(baseFont, 12f, Font.NORMAL)));
                paragraphTitle1.Alignment = Rectangle.ALIGN_CENTER;
                //document.Add(paragraphTitle1);

                Paragraph paragraphTitle2 = new Paragraph(new Chunk(vendor.AddressEN, new Font(baseFont, 10f, Font.NORMAL)));
                paragraphTitle2.Alignment = Rectangle.ALIGN_CENTER;
                //document.Add(paragraphTitle2);

                Paragraph paragraphTitle3 = new Paragraph(new Chunk("商 业 发 票", new Font(baseFont, 12f, Font.NORMAL)));
                paragraphTitle3.Alignment = Rectangle.ALIGN_CENTER;
                //document.Add(paragraphTitle3);

                Paragraph paragraphEn = new Paragraph(new Chunk("COMMERCIAL INVOICE", new Font(baseFont, 10f, Font.NORMAL)));
                paragraphEn.Alignment = Rectangle.ALIGN_CENTER;
                paragraphEn.Leading = 12f;
                //document.Add(paragraphEn);

                var titleMiddleCell = new PdfPCell();
                titleMiddleCell.AddElement(paragraphTitle1);
                titleMiddleCell.AddElement(paragraphTitle2);
                titleMiddleCell.AddElement(paragraphTitle3);
                titleMiddleCell.AddElement(paragraphEn);
                titleMiddleCell.DisableBorderSide(15);
                tableTitle.AddCell(titleMiddleCell);

                //3 Tile 右侧 空白占位
                var titleRightCell = new PdfPCell(new Phrase("", new Font(baseFont, 8f, Font.NORMAL)));
                titleRightCell.DisableBorderSide(15);
                tableTitle.AddCell(titleRightCell);

                document.Add(tableTitle);

                

                PdfPTable tableZhi = new PdfPTable(2);
                tableZhi.TotalWidth = 579; //绝对宽度
                tableZhi.LockedWidth = true;
                tableZhi.SetWidths(new int[] { 80, 20 });

                var ddate = DateTime.ParseExact(this.TradeDate, "yyyyMMdd", new System.Globalization.CultureInfo("zh-CN"), System.Globalization.DateTimeStyles.AllowWhiteSpaces);

                var tableZhiCell1 = new PdfPCell(new Phrase("致:" + agentName, new Font(baseFont, 8f, Font.NORMAL)))
                {
                    HorizontalAlignment = Rectangle.ALIGN_LEFT,
                };
                tableZhiCell1.DisableBorderSide(15);
                tableZhi.AddCell(tableZhiCell1);

                var tableZhiCell2 = new PdfPCell(new Phrase(" 日期：" + ddate.ToString("yyyy-MM-dd"), new Font(baseFont, 8f, Font.NORMAL)))
                {
                    HorizontalAlignment = Rectangle.ALIGN_RIGHT
                };
                tableZhiCell2.DisableBorderSide(15);
                tableZhi.AddCell(tableZhiCell2);

                document.Add(tableZhi);

                PdfPTable tableDi = new PdfPTable(2);
                tableDi.TotalWidth = 579; //绝对宽度
                tableDi.LockedWidth = true;
                tableDi.SetWidths(new int[] { 80, 20 });

                var tableDiCell1 = new PdfPCell(new Phrase("地址：" + PurchaserContext.Current.Address, new Font(baseFont, 8f, Font.NORMAL)))
                {
                    HorizontalAlignment = Rectangle.ALIGN_LEFT,
                };
                tableDiCell1.DisableBorderSide(15);
                tableDi.AddCell(tableDiCell1);

                var tableDiCell2 = new PdfPCell(new Phrase("合同编号：" + this.ContractNo, new Font(baseFont, 8f, Font.NORMAL)))
                {
                    HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                };
                tableDiCell2.DisableBorderSide(15);
                tableDi.AddCell(tableDiCell2);

                document.Add(tableDi);
                Paragraph paragraph5 = new Paragraph(new Chunk(" ", new Font(baseFont, 8f, Font.NORMAL)));
                paragraph5.Alignment = Rectangle.ALIGN_LEFT;
                paragraph5.Leading = 2f;

                document.Add(paragraph5);

                #endregion


                #region Table

                PdfPTable table = new PdfPTable(dt.Columns.Count); //列数
                table.TotalWidth = 574; //绝对宽度
                table.LockedWidth = true;

                //设置行宽
                int[] widths = new int[] { 4, 22, 20, 12, 8, 6, 8, 8 }; //百分比的感觉
                table.SetWidths(widths);

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dt.Rows[i][j].ToString(), new Font(baseFont, 6f)))
                        {
                            BorderWidth = 0.06f,
                        };

                        //合并行
                        if (i == dt.Rows.Count - 2)
                        {
                            if (j == 0)
                            {
                                cell.Colspan = 5;
                                j = j + 4;
                            }
                        }

                        if (i == dt.Rows.Count - 1)
                        {
                            if (j == 0)
                            {
                                cell.Colspan = 3;
                                j += 2;
                            }
                            else if (j == 4)
                            {
                                cell.Colspan = 4;
                                j++;
                            }
                        }

                        cell.BorderColor = new BaseColor(0, 139, 139);
                        table.AddCell(cell);

                    }
                }
                //为当前document加入内容
                table.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                document.Add(table);

                #endregion


                #region 尾
                PdfPTable tableQianZhang = new PdfPTable(2);
                tableQianZhang.TotalWidth = 574; //绝对宽度
                tableQianZhang.LockedWidth = true;
                tableQianZhang.SetWidths(new int[] { 10, 90 });

                //1
                var a = new PdfPCell(new Phrase("", new Font(baseFont, 8f, Font.NORMAL)));
                a.DisableBorderSide(15);
                tableQianZhang.AddCell(a);

                //2
                Image image1 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.SealUrl));
                //同比例缩放
                //float resizeWidth = image.Width;
                //float resizeHeight = image.Height;
                image1.ScalePercent(80f);
                var b = new PdfPCell(image1) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
                b.DisableBorderSide(15);
                tableQianZhang.AddCell(b);
                document.Add(tableQianZhang);
                #endregion

            }
            finally
            {
                //关闭document
                if (document != null)
                {
                    document.Close();
                }
            }
        }

        public string[] SaveAs(Vendor vendor)
        {
            var result = new string[3];
            string fileName = this.ContractNo + "-发票.pdf";

            try
            {
               
                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(SysConfig.DeclareDirectory);
                fileDic.CreateDataDirectory();

                //发票删除后再生成
                if (System.IO.File.Exists(fileDic.FilePath))
                {
                    System.IO.File.Delete(fileDic.FilePath);
                }

                if (!string.IsNullOrEmpty(vendor.PdfStyle) && vendor.PdfStyle == "style1")
                {
                    InvoiceCY invoiceCY = new InvoiceCY(vendor, fileDic.FilePath, this);
                    invoiceCY.Execute();
                }
                else if (!string.IsNullOrEmpty(vendor.PdfStyle) && vendor.PdfStyle == "style2")
                {
                    InvoiceWLT invoiceWLT = new InvoiceWLT(vendor, fileDic.FilePath, this);
                    invoiceWLT.Execute();
                }
                else
                {
                    this.ToPDF(fileDic.FilePath, vendor);
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