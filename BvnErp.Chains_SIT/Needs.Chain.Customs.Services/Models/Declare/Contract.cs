extern alias globalB;
using globalB::iTextSharp.text;
using globalB::iTextSharp.text.pdf;
using Needs.Linq;
using Needs.Utils;
using Needs.Wl.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 合同
    /// </summary>
    public class Contract
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
        /// 交易日期
        /// </summary>
        public string TradeDate { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 申报日期
        /// </summary>
        public string DDate { get; set; }

        /// <summary>
        /// 包装方式
        /// </summary>
        public string WrapType { get; set; }

        /// <summary>
        /// 启运港
        /// </summary>
        public string DespPortCode { get; set; }

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

        internal Contract()
        {

        }

        public Contract(DecHead decHead)
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

            this.Items = decHead.Lists.OrderBy(item => item.GNo).Select(item => new DecProduct
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

                // 2020-09-03 by yeshuangshuang
                OrderUnitPrice = item.OrderPrice,
                OrderTotalPrice = item.OrderTotal.ToRound(2)
            });

            this.Currency = this.Items.FirstOrDefault().Currency;
            this.TradeDate = decHead.IEDate;
            this.ContractNo = decHead.ContrNo;
            this.DDate = decHead.IEDate;
            this.WrapType = decHead.WrapType;
            this.DespPortCode = decHead.DespPortCode;
        }

        /// <summary>
        /// 导出PDF
        /// </summary>
        public void ToPDF(string filePath, Vendor vendor)
        {
            #region 数据填充dt

            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            dt.Columns.Add("UnitPrice", typeof(string));
            dt.Columns.Add("TotalPrice", typeof(string));

            DataRow drTitle = dt.NewRow();

            drTitle["NO"] = "序号";
            drTitle["Name"] = "商品名称";
            drTitle["Model"] = "货物型号";
            drTitle["Qty"] = "数量(PCS)";
            drTitle["UnitPrice"] = "单价(" + this.Currency + " / PCS)";
            drTitle["TotalPrice"] = "总价(" + this.Currency + ")";
            dt.Rows.Add(drTitle);

            int index = 0;
            foreach (var entity in this.Items)
            {
                DataRow dr = dt.NewRow();
                dr["NO"] = index + 1;
                dr["Name"] = entity.Name;
                dr["Model"] = entity.Model == null ? "" : entity.Model;
                dr["Qty"] = entity.Quantity.ToString("0.####");
                dr["UnitPrice"] = entity.UnitPrice.ToRound(4).ToString("0.####");
                dr["TotalPrice"] = entity.TotalPrice.ToRound(2);

                dt.Rows.Add(dr);

                index++;
            }

            //取币制的中文
            string CurrencyName = "";
            CurrencyName = new Needs.Ccs.Services.Views.BaseCurrenciesView().Where(item => item.Code == this.Currency).FirstOrDefault()?.Name;

            #region 增加一行运保杂费 2020-09-02 by yeshuangshuang

            DataRow dtLastRow1 = dt.NewRow();
            dtLastRow1["NO"] = "运保杂费";
            dtLastRow1["Name"] = "";
            dtLastRow1["Model"] = "";
            dtLastRow1["Qty"] = "";
            dtLastRow1["UnitPrice"] = "";
            dtLastRow1["TotalPrice"] = "";//运保杂费
            dt.Rows.Add(dtLastRow1);

            #endregion


            DataRow dtLastRow = dt.NewRow();
            dtLastRow["NO"] = "合同金额(大写):";
            dtLastRow["Name"] = "";
            dtLastRow["Model"] = this.TotalPrice.ToRound(2).ToChineseAmount() + " " + CurrencyName;
            dtLastRow["Qty"] = "";
            dtLastRow["UnitPrice"] = "";
            dtLastRow["TotalPrice"] = this.TotalPrice.ToRound(2);
            dt.Rows.Add(dtLastRow);

            #endregion

            //中文字体
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Rectangle rec = new Rectangle(PageSize.A4);
            //创建一个文档实例。 去除边距
            Document document = new Document(rec);
            document.SetMargins(10f, 10f, 2f, 2f);

            try
            {
                //创建一个writer实例
                PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.OpenOrCreate));
                //打开当前文档
                document.Open();

                //标题
                PdfPTable tableTitle = new PdfPTable(3);
                tableTitle.TotalWidth = 574; //绝对宽度
                tableTitle.LockedWidth = true;
                tableTitle.SetWidths(new int[] { 10, 80, 10 });

                //1 Tile 左侧 Logo
                Image imageLogo = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.LogoUrl));
                //同比例缩放
                imageLogo.ScalePercent(15f);
                var titleLeftCell = new PdfPCell(imageLogo) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 8f, };
                titleLeftCell.DisableBorderSide(15);
                tableTitle.AddCell(titleLeftCell);

                //2 Tile 中间 文档标题 
                Paragraph paragraphTitle = new Paragraph(new Chunk("销 售 合 同", new Font(baseFont, 12f, Font.NORMAL)));
                paragraphTitle.Alignment = Rectangle.ALIGN_CENTER;
                
                Paragraph paragraphEn = new Paragraph(new Chunk("SALES CONTRACT", new Font(baseFont, 11f, Font.NORMAL)));
                paragraphEn.Alignment = Rectangle.ALIGN_CENTER;
                paragraphEn.Leading = 12f;
                

                var titleMiddleCell = new PdfPCell();
                titleMiddleCell.AddElement(paragraphTitle);
                titleMiddleCell.AddElement(paragraphEn);
                titleMiddleCell.DisableBorderSide(15);
                tableTitle.AddCell(titleMiddleCell);

                //3 Tile 右侧 空白占位
                var titleRightCell = new PdfPCell(new Phrase("", new Font(baseFont, 8f, Font.NORMAL)));
                titleRightCell.DisableBorderSide(15);
                tableTitle.AddCell(titleRightCell);

                document.Add(tableTitle);



                PdfPTable tableMaifang = new PdfPTable(2);
                tableMaifang.TotalWidth = 579; //绝对宽度
                tableMaifang.LockedWidth = true;
                tableMaifang.SetWidths(new int[] { 80, 20 });

                var tableMaifangCell1 = new PdfPCell(new Phrase("卖方:" + vendor.CompanyName, new Font(baseFont, 8f, Font.NORMAL)))
                {
                    HorizontalAlignment = Rectangle.ALIGN_LEFT,
                };
                tableMaifangCell1.DisableBorderSide(15);
                tableMaifang.AddCell(tableMaifangCell1);

                var tableMaifangCell2 = new PdfPCell(new Phrase(" 日期：" + this.DDate, new Font(baseFont, 8f, Font.NORMAL)))
                {
                    HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                };
                tableMaifangCell2.DisableBorderSide(15);
                tableMaifang.AddCell(tableMaifangCell2);
                document.Add(tableMaifang);

                Paragraph paragraph2 = new Paragraph(new Chunk("合同编号：" + this.ContractNo, new Font(baseFont, 8f, Font.NORMAL)));
                paragraph2.Alignment = Rectangle.ALIGN_RIGHT;
                paragraph2.Leading = 10f;
                document.Add(paragraph2);

                Paragraph paragraph3 = new Paragraph(new Chunk("经买卖双方同意，按以下条款成交", new Font(baseFont, 8f, Font.NORMAL)));
                paragraph3.Alignment = Rectangle.ALIGN_LEFT;
                paragraph3.Leading = 10f;
                document.Add(paragraph3);

                Paragraph paragraph4 = new Paragraph(new Chunk("1、产品的名称，数量，型号，单价，总价", new Font(baseFont, 8f, Font.NORMAL)));
                paragraph4.Alignment = Rectangle.ALIGN_LEFT;
                paragraph4.Leading = 10f;
                document.Add(paragraph4);

                Paragraph paragraph5 = new Paragraph(new Chunk(" ", new Font(baseFont, 8f, Font.NORMAL)));
                paragraph5.Alignment = Rectangle.ALIGN_LEFT;
                paragraph5.Leading = 2f;
                document.Add(paragraph5);

                PdfPTable table = new PdfPTable(dt.Columns.Count); //列数
                table.TotalWidth = 574; //绝对宽度
                table.LockedWidth = true;
                //设置行宽
                int[] widths = new int[] { 4, 34, 32, 8, 10, 12 }; //百分比  基于绝对宽度
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
                        if (i == dt.Rows.Count - 1)
                        {
                            if (j == 0)
                            {
                                cell.Colspan = 2;
                                j++;
                            }
                            else if (j == 2)
                            {
                                cell.Colspan = 3;
                                j += 2;
                            }
                        }
                        cell.BorderColor = new BaseColor(0, 139, 139);
                        table.AddCell(cell);
                    }
                }

                //为当前document加入内容
                table.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                document.Add(table);

                string[] strs = new string[]
                {
                    "2、成交方式：CIF深圳",
                    "3、包装方式：22",
                    "4、装运口岸和目的地：HKG000、深圳",
                    "5、质量要求：进口产品必须符合中国国家标准或者行业标准，符合双方商定的技术要求。",
                    "6、产品的交（提）货期限：合同生效后15天内交（提）货。",
                    "7、结算方式：电汇。",
                    "8、对产品提出异议的时间和办法：买方在验收中，发现产品的型号、规格、质量不合格，应一面妥善保管，一面在30天内向卖方提出书面异议；卖方在接到买方书面异议后，需在10天内处理。买方签收货物超过30天未提出异议的，视为产品合格。对处理结果有异议，需要索赔的，索赔期限为90天。",
                    "9、违约责任：卖方未能按期交货的，承担货款110%的违约金；买方未能按时付款的，承担全部货款5%滞纳金。",
                    "10、不可抗力：任何一方由于不可抗力的原因不能履行合同的，应及时告知对方，并免于承担违约责任。",
                    "11、其他:本协议履行过程中如发生争议，双方应友好协商解决。如协商不成的，可通过法律途径解决。",
                };

                foreach (var str in strs)
                {
                    Paragraph paragraph = new Paragraph(new Chunk(str, new Font(baseFont, 8f, Font.NORMAL)));
                    paragraph.Alignment = Rectangle.ALIGN_LEFT;
                    paragraph.Leading = 10f;
                    document.Add(paragraph);
                }
                //签名章

                PdfPTable tableQianZhang = new PdfPTable(4);
                tableQianZhang.TotalWidth = 574; //绝对宽度
                tableQianZhang.LockedWidth = true;
                tableQianZhang.SetWidths(new int[] { 10, 50, 10, 30 });

                var a = new PdfPCell(new Phrase("签名盖章", new Font(baseFont, 8f, Font.NORMAL)))
                {
                    HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                    VerticalAlignment = Rectangle.ALIGN_MIDDLE,
                };
                a.DisableBorderSide(15);
                tableQianZhang.AddCell(a);

                Image image1 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.SealUrl));
                //同比例缩放
                //float resizeWidth = image.Width;
                //float resizeHeight = image.Height;
                image1.ScalePercent(80f);
                var b = new PdfPCell(image1) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f, };
                b.DisableBorderSide(15);
                tableQianZhang.AddCell(b);

                var c = new PdfPCell(new Phrase("签名盖章", new Font(baseFont, 8f, Font.NORMAL)))
                {
                    HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                    VerticalAlignment = Rectangle.ALIGN_MIDDLE,
                };
                c.DisableBorderSide(15);
                tableQianZhang.AddCell(c);

                var szseal = PurchaserContext.Current.SealUrl;
                Image image2 = Image.GetInstance(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, szseal));
                image2.ScalePercent(80f);
                var d = new PdfPCell(image2) { VerticalAlignment = Rectangle.ALIGN_MIDDLE, PaddingLeft = 10f };
                d.DisableBorderSide(15);
                tableQianZhang.AddCell(d);

                document.Add(tableQianZhang);

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
            string fileName = this.ContractNo + "-合同.pdf";
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
                    ContractCY contractCY = new ContractCY(vendor, fileDic.FilePath, this);
                    contractCY.Execute();
                }
                else if (!string.IsNullOrEmpty(vendor.PdfStyle) && vendor.PdfStyle == "style2")
                {
                    ContractWLT contractWLT = new ContractWLT(vendor, fileDic.FilePath, this);
                    contractWLT.Execute();
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
