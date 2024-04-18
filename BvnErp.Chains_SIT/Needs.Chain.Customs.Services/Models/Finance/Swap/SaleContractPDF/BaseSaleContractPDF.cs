using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Drawing;
using System.Linq;


namespace Needs.Ccs.Services.Models
{
    public abstract class BaseSaleContractPDF
    {
        public Purchaser purchaser = PurchaserContext.Current;
        public SwapNotice SwapNotice { get; set; }
   
        public abstract string ClearingType();
        public PdfDocument ToSaleContractPDf(string clearingType)
        {
            var vendor = new VendorContext(VendorContextInitParam.SwapNoticeID, SwapNotice.ID, "CaiWu").Current1;

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
            PdfTrueTypeFont font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            PdfTrueTypeFont font4 = new PdfTrueTypeFont(new Font("SimSun", 16f, FontStyle.Bold), true);

            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头

            float y = 5;
            float width = page.Canvas.ClientSize.Width;
            var contractNo = SwapNotice.DocumentNo;//"PS" + DateTime.Now.ToString("yyyyMMdd");
            var dateTime = SwapNotice.ContrDate; //DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
            string message = vendor.CompanyName;

            //page.Canvas.DrawString(message, font4, brush, 0, y, formatLeft);
            page.Canvas.DrawString(message, font4, brush, page.Canvas.ClientSize.Width / 2, y, formatCenter);
            y += font4.MeasureString(message, formatCenter).Height + 8;

            message = vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            //message = "Unit B2,2/F.,Houtex Ind.Bldg.,16Hung To Rd.,Kwun Tong,Kowloon,HK 电话:(852)31019258";
            //message = "Unit B2, 2 / F.,Houtex Ind. Bldg., 16 Hung To Rd., Kwun Tong, Kowloon，HK 电话：（852）31019258";
            message = vendor.AddressEN + ".电话：" + vendor.Tel;

            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            message = "销售合同";
            page.Canvas.DrawString(message, font4, brush, width / 2, y, formatCenter);
            y += font4.MeasureString(message, formatCenter).Height + 8;

            message = "SALES CONTRACT";
            page.Canvas.DrawString(message, font3, brush, width / 2, y, formatCenter);
            y += font3.MeasureString(message, formatCenter).Height + 8;

            message = "买方:" + purchaser.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            y += font3.MeasureString(message, formatLeft).Height + 15;

            message = "卖方:" + vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "日期:" + dateTime;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 15;

            message = "经买卖双方同意,按以下条款成交: ";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "合同编号:" + contractNo;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 15;
            #endregion

            #region 

            message = "1、";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            y += font3.MeasureString(message, formatLeft).Height + 5;

            ////创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 8f), true);

            //设置列宽
            grid.Columns.Add(6);
            grid.Columns[0].Width = width * 0.2f;
            grid.Columns[1].Width = width * 0.2f;
            grid.Columns[2].Width = width * 0.2f;
            grid.Columns[3].Width = width * 0.1f;
            grid.Columns[4].Width = width * 0.2f;
            grid.Columns[5].Width = width * 0.1f; ;

            //产品信息
            PdfGridRow row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = "单号";
            row.Cells[1].Value = "货物名称";
            row.Cells[2].Value = "净重(KGS)";
            row.Cells[3].Value = "数量";
            row.Cells[4].Value = "总价(USD)";
            row.Cells[5].Value = "备注";

            foreach (PdfGridCell cell in row.Cells)
            {
                cell.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }
            decimal totalQty = 0;
            decimal? netWt = 0;
            var DespPortCode = "";
            row = grid.Rows.Add();
            row.Height = 20;
            row.Cells[0].Value = contractNo;
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "电子元器件";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            foreach (var item in SwapNotice.Items)
            {
                netWt += item.SwapDecHead.Lists.Sum(x => x.NetWt)?.ToRound(2);
                totalQty += item.SwapDecHead.Lists.Sum(x => x.GQty).ToRound(0);
                DespPortCode = item.SwapDecHead.Contract.DespPortCode;
            }
            row.Cells[2].Value = netWt.ToString();
            row.Cells[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[3].Value = totalQty.ToString();
            row.Cells[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = SwapNotice.TotalAmount.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            row.Cells[5].Value = SwapNotice.Summary;
            row.Cells[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            for (int i = 0; i < 8; i++)
            {
                row = grid.Rows.Add();
                row.Height = 20;
            }
            //合计行
            row = grid.Rows.Add();
            row.Height = 16;
            row.Cells[0].ColumnSpan = 3;
            row.Cells[0].Value = "总计:" + ConvertZh(SwapNotice.TotalAmount.ToString("0.00"));
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[4].Value = SwapNotice.TotalAmount.ToString("0.00");
            row.Cells[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);

            //设置边框
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }

            PdfLayoutResult result = grid.Draw(page, new PointF(0, y));
            y += result.Bounds.Height + 20;

            #endregion

            //取币制的中文
            string CurrencyName = "";
            CurrencyName = new Needs.Ccs.Services.Views.BaseCurrenciesView().Where(item => item.Code == SwapNotice.Currency).FirstOrDefault()?.Name;

            #region 尾
            font3 = new PdfTrueTypeFont(new Font("SimSun", 10f, FontStyle.Regular), true);
            message =
                      "2、合同金额：USD" + SwapNotice.TotalAmount.ToString("0.00") + "     " + ConvertZh(SwapNotice.TotalAmount.ToString("0.00")) + "(" + CurrencyName + "）" + "   \r\n" +
                      "\r\n" +
                      "3、成交方式：CIF深圳  \r\n" +
                        "\r\n" +
                      "4、包装方式：纸箱   \r\n" +
                        "\r\n" +
                      "5、装运口岸和目的地：香港、深圳  \r\n" +
                        "\r\n" +
                      "6、结算方式："+ clearingType;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);
            y += font3.MeasureString(message, formatLeft).Height + 20;


            message = "卖方:" + vendor.CompanyName;
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            message = "买方:" + purchaser.CompanyName;
            page.Canvas.DrawString(message, font3, brush, width, y, formatRight);
            y += font3.MeasureString(message, formatLeft).Height + 20;

            message = "签名盖章:";
            page.Canvas.DrawString(message, font3, brush, 0, y, formatLeft);

            PdfImage HTimage = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, vendor.SignSealUrl));
            page.Canvas.DrawImage(HTimage, 50, y - 35);

            message = "签名盖章:";
            page.Canvas.DrawString(message, font3, brush, 375, y, formatRight);
            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.ContactStamp));
            page.Canvas.DrawImage(image, 330, y - 70);
            y += font3.MeasureString(message, formatLeft).Height + 5;
            #endregion

            return pdf;



        }

        private string ConvertZh(string money)
        {
            //ryan 20201204 修改正确的转换大写方法
            decimal m = decimal.Parse(money);
            return CmycurD(m);
        }

        /// <summary>
        /// 数字转大写
        /// </summary>
        /// <param name="Num">数字</param>/// <returns></returns>
        private string CmycurD(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字 
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字 
            string str3 = "";    //从原num值中取出的值 
            string str4 = "";    //数字的字符串形式 
            string str5 = "";  //人民币大写金额形式 
            int i;    //循环变量 
            int j;    //num的值乘以100的字符串长度 
            string ch1 = "";    //数字的汉语读法 
            string ch2 = "";    //数字位的汉字读法 
            int nzero = 0;  //用来计算连续的零值是几个 
            int temp;            //从原num值中取出的值 

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数 
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式 
            j = str4.Length;      //找出最高位 
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分 

            //循环取出每一位需要转换的值 
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值 
                temp = Convert.ToInt32(str3);      //转换为数字 
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时 
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位 
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位，则必须写上 
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整” 
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }

        public PdfDocument SaleContractPdf()
        {
            string clearingType = ClearingType();
            return ToSaleContractPDf(clearingType);
        }
    }
}
