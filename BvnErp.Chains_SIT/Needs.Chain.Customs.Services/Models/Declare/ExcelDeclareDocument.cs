using Needs.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ExcelDeclareDocument 
    {
        public string EntyPortCode { get; set; }
        public string IEPort { get; set; }
        public string CIQPort { get; set; }
        public string ContrNo { get; set; }
        public string OwnerName { get; set; }
        public string OwnerCusCode { get; set; }
        public string OwnerScc { get; set; }
        public string VoyNo { get; set; }
        public string BillNo { get; set; }
        public int PackNo { get; set; }
        public decimal GrossWt { get; set; }
        public decimal NetWt { get; set; }
        public bool IsInspection { get; set; }
        public bool IsQuarantine { get; set; }
        public decimal SumQty { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; }
        private DecLists lists;
        public string TempletePath { get; set; }
        public string DeclarationID { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public event SuccessHanlder ExcelDeclareSuccess;

        public string CustomSubmiterAdminID { get; set; }

        public ExcelDeclareDocument(DecHead head)
        {
            this.EntyPortCode = head.EntyPortCode;
            this.IEPort = head.IEPort;
            this.CIQPort = "";
            this.ContrNo = head.ContrNo;
            this.OwnerName = head.OwnerName;
            this.OwnerCusCode = head.OwnerCusCode;
            this.OwnerScc = head.OwnerScc;
            this.VoyNo = head.VoyNo;
            this.BillNo = head.BillNo;
            this.PackNo = head.PackNo;
            this.GrossWt = head.GrossWt;
            this.NetWt = head.NetWt;
            this.IsInspection = head.IsInspection;
            this.IsQuarantine = head.IsQuarantine.Value;
            this.lists = head.Lists;
            this.Currency = lists[0].TradeCurr;

            this.DeclarationID = head.ID;
            ExcelDeclareSuccess += ExcelDeclared;

            this.TempletePath =  System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SysConfig.ExcelDeclareFileName);
        }

        public IWorkbook toExcel(Vendor vendor)
        {
            IWorkbook workBook = null;        

            var info = new Dictionary<object, int[]>();
            var InPort = "";//入境口岸
            var CusPort = "";//关区代码
            var CIQPort = "";//检验检疫机关代码
            var CusPortName = "";//关区名称
            switch (this.EntyPortCode)
            {
                //文锦渡
                case "470401":
                    InPort = "470401";
                    CusPort = "5320";
                    CIQPort = "475400";
                    CusPortName = "文锦渡关";
                    break;
                //皇岗
                case "470201":
                    InPort = "470201";
                    CusPort = "5301";
                    CIQPort = "475200";
                    CusPortName = "皇岗海关";
                    break;
                //沙头角
                case "470501":
                    InPort = "470501";
                    CusPort = "5303";
                    CIQPort = "475500";
                    CusPortName = "沙头角关";
                    break;
                    //默认皇岗
                default:
                    InPort = "470201";
                    CusPort = "5301";
                    CIQPort = "475200";
                    CusPortName = "皇岗海关";
                    break;
            }

            //标题
            info.Add(PurchaserContext.Current.ShortName + "进口报关单0=0", new int[] { 0, 0 });

            info.Add(CusPort + "2=1", new int[] { 2, 1 });
            info.Add(CusPort + "3=4", new int[] { 3, 4 });
            info.Add(InPort + "17=4", new int[] { 17, 4 });
            info.Add(CIQPort + "81=2", new int[] { 81, 2 });
            info.Add(CIQPort + "91=2", new int[] { 91, 2 });
            info.Add(CIQPort + "92=2", new int[] { 92, 2 });
            info.Add(CIQPort + "91=6", new int[] { 91, 6 });

            //合同号
            info.Add(this.ContrNo + "3=1", new int[] { 3, 1 });
            info.Add(this.OwnerName + "7=7", new int[] { 7, 7 });//消费使用单位-客户名称
            info.Add(this.OwnerCusCode + "7=1", new int[] { 7, 1 });//消费使用单位代码-10位
            info.Add(this.OwnerScc + "7=3", new int[] { 7, 3 });//消费单位 同意社会信用代码

            //境内收发货人
            info.Add(PurchaserContext.Current.CompanyName + "5=7", new int[] { 5, 7 });//消费使用单位-客户名称
            info.Add(PurchaserContext.Current.CustomsCode + "5=1", new int[] { 5, 1 });//消费使用单位代码-10位
            info.Add(PurchaserContext.Current.Code + "5=3", new int[] { 5, 3 });//消费单位 统一社会信用代码
            info.Add(PurchaserContext.Current.CiqCode + "5=6", new int[] { 5, 6 });//消费单位 检验检疫代码

            //境外收发货人
            info.Add(vendor.CompanyName + "6=6", new int[] { 6, 6 });//名称

            //申报单位
            info.Add(PurchaserContext.Current.CompanyName + "8=7", new int[] { 8, 7 });//申报单位-客户名称
            info.Add(PurchaserContext.Current.CustomsCode + "8=1", new int[] { 8, 1 });//申报单位-10位
            info.Add(PurchaserContext.Current.Code + "8=3", new int[] { 8, 3 });//申报单位 统一社会信用代码
            info.Add(PurchaserContext.Current.CiqCode + "8=6", new int[] { 8, 6 });//申报单位 检验检疫代码

            info.Add(this.VoyNo + "9=7", new int[] { 9, 7 });//航次号
            info.Add(this.BillNo + "10=1", new int[] { 10, 1 });//提运单号

            info.Add(this.PackNo + "16=1", new int[] { 16, 1 });//件数
            info.Add(this.PackNo + "79=22", new int[] { 79, 22 });//汇总件数

            info.Add(DateTime.Now.ToString("yyyy-MM-dd") + "18=15", new int[] { 18, 15 });           
            
            info.Add(this.GrossWt + "16=7", new int[] { 16, 7 });//毛重
            info.Add(this.GrossWt + "79=24", new int[] { 79, 24 });//汇总毛重          
          
            info.Add(this.NetWt + "16=10", new int[] { 16, 10 });//净重
            info.Add(this.NetWt + "79=23", new int[] { 79, 23 });//汇总净重

            if (this.IsInspection||this.IsQuarantine)
            {
                info.Add("是" + "25=1", new int[] { 25, 1 });                
                info.Add("企业承诺" + "80=3", new int[] { 80, 3 });
                info.Add("是" + "80=5", new int[] { 80, 5 });
                info.Add("(企业承诺法检必填)" + "80=6", new int[] { 80, 6 });
            }

            info.Add(PurchaserContext.Current.UseOrgPersonCode + "84=6", new int[] { 84, 6 });
            info.Add(PurchaserContext.Current.UseOrgPersonTel + "84=8", new int[] { 84, 8 });
            info.Add(DateTime.Now.ToString("yyyy-MM-dd") + "93=2", new int[] { 93, 2 });//启运日期

            info.Add(DateTime.Now.AddDays(3).ToString("yyyy-MM-dd") + "100=2", new int[] { 100, 2 });//卸毕日期

            //发货人
            info.Add(PurchaserContext.Current.DomesticConsigneeEname + "97=2", new int[] { 97, 2 });
            info.Add(vendor.OverseasConsignorCname + "98=2", new int[] { 98, 2 });
            info.Add(vendor.OverseasConsignorAddr + "99=2", new int[] { 99, 2 });

            var data = new string[this.lists.Count, 36];
            var currency = this.Currency;
            var i = 0;
            var sumQty = 0.0000M;
            var totalPrice = new decimal();

            var itemOrdered = this.lists.OrderBy(item => item.GNo).ToList();
            var CountryView = new Needs.Ccs.Services.Views.BaseCountriesView();
            var orderItemCategory = new Needs.Ccs.Services.Views.OrderItemCategoriesView();
            var orderItemTax = new Needs.Ccs.Services.Views.OrderItemTaxesView();
            itemOrdered.ForEach(t =>
            {
                data[i, 0] = i.ToString();
                //data[i, 1] = t.CustomsCode;
                data[i, 2] = t.CodeTS;
                //data[i, 3] = t.CIQCode;
                data[i, 3] = "";//20180810 取消登革热检疫
                data[i, 4] = t.GName;
                data[i, 5] = t.GModel;
                data[i, 6] = t.GQty.ToString();//成交数量
                data[i, 7] = "007";//成交单位 默认007
                data[i, 8] = t.FirstQty.HasValue ? t.FirstQty.Value.ToString() : string.Empty;//法一数量
                data[i, 9] = t.FirstUnit;//法一单位
                data[i, 10] = t.SecondQty.HasValue ? t.SecondQty.Value.ToString() : string.Empty;//法二数量
                data[i, 11] = t.SecondUnit;//法二单位
                data[i, 12] = t.DeclPrice.ToString();//单价
                data[i, 13] = t.DeclTotal.ToString();//总价
                data[i, 14] = currency;
                data[i, 15] = CountryView.Where(a => a.Code == t.OriginCountry).FirstOrDefault()?.Name;
                data[i, 16] = "中国";
                //17
                data[i, 18] = "深圳特区";
                data[i, 19] = "深圳市龙岗区";
                data[i, 20] = "照章征税";
                //21
                data[i, 21] = this.ContrNo;
                data[i, 22] = t.CaseNo;
                data[i, 23] = t.NetWt.ToString();
                data[i, 24] = t.GrossWt.ToString();
                //25-27
                data[i, 28] = t.GoodsModel;
                data[i, 29] = t.GQty.ToString();
                data[i, 30] = orderItemCategory.Where(item => item.OrderItemID == t.OrderItemID).FirstOrDefault()?.TaxName;
                data[i, 31] = t.GoodsBrand;
                var tariff = orderItemTax.Where(item => item.OrderItemID == t.OrderItemID&&item.Type== Enums.CustomsRateType.ImportTax).FirstOrDefault()?.Rate;
                //data[i, 32] = tariff == 0 ? "" : tariff.ToString();
                data[i, 32] = tariff == 0M ? "0" : tariff.ToString();
                data[i, 33] = this.OwnerName;
                data[i, 34] = i.ToString();
                data[i, 35] = t.DeclTotal.ToString(); //总价

                //2018-08-06 海关登革热检疫
                if (IsInspection || this.IsQuarantine)
                {
                    data[i, 3] = t.CiqCode;
                    //2018-12-1 检验检疫货物规格加公式LK
                    data[i, 25] = "需要公式";
                    data[i, 26] = "3C目录外,正常";
                    data[i, 27] = "其他";
                }

                i++;
                sumQty += t.GQty;
                totalPrice += t.DeclTotal;
            });

         
            info.Add(this.ContrNo + "79=4", new int[] { 79, 4 });
            info.Add(sumQty + "79=6", new int[] { 79, 6 });
            info.Add(totalPrice + "79=13", new int[] { 79, 13 });
            info.Add(currency + "79=14", new int[] { 79, 14 });
            info.Add(this.OwnerName + "79=15", new int[] { 79, 15 });//消费使用单位

            workBook = GenerateExcel(info, i, data);

            return workBook;
        }

        private IWorkbook GenerateExcel(Dictionary<object, int[]> info, int rows, string[,] data = null)
        {
            IWorkbook xssfworkbook = null;           

            FileStream file = new FileStream(this.TempletePath, FileMode.Open, FileAccess.Read);
            xssfworkbook = new XSSFWorkbook(file);
            ISheet sheet1 = xssfworkbook.GetSheet("报关单");


            XSSFCellStyle stylewrap = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            stylewrap.WrapText = true;
            //字体
            IFont font1 = xssfworkbook.CreateFont();//字体
            font1.FontHeightInPoints = 9;
            XSSFCellStyle stylewrapandfont = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            stylewrapandfont.WrapText = true;
            stylewrapandfont.SetFont(font1);

            XSSFCellStyle stylefour = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formatfour = xssfworkbook.CreateDataFormat();
            stylefour.SetDataFormat(formatfour.GetFormat("0.0000"));

            XSSFCellStyle styletwo = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formattwo = xssfworkbook.CreateDataFormat();
            styletwo.SetDataFormat(formattwo.GetFormat("0.00"));

            XSSFCellStyle stylethree = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formatthree = xssfworkbook.CreateDataFormat();
            stylethree.SetDataFormat(formatthree.GetFormat("0.000"));
            foreach (var dic in info)
            {
                if (sheet1.GetRow(dic.Value[0]) == null)
                    sheet1.CreateRow(dic.Value[0]);

                if (sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]) == null)
                    sheet1.GetRow(dic.Value[0]).CreateCell(dic.Value[1]);

                sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), ""));

                //口岸代码
                if ((dic.Value[0] == 2 && (dic.Value[1] == 1))
                    || (dic.Value[0] == 3 && (dic.Value[1] == 4))
                    || (dic.Value[0] == 17 && (dic.Value[1] == 4))
                    || (dic.Value[0] == 81 && (dic.Value[1] == 2))
                    || (dic.Value[0] == 91 && (dic.Value[1] == 2))
                    || (dic.Value[0] == 92 && (dic.Value[1] == 2))
                    || (dic.Value[0] == 91 && (dic.Value[1] == 6)))
                {
                    sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(int.Parse(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }

                //件数汇总
                if ((dic.Value[0] == 79 && dic.Value[1] == 22) || (dic.Value[0] == 16 && dic.Value[1] == 1))
                {
                    sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(int.Parse(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }
                //金额汇总
                if (dic.Value[0] == 79 && (dic.Value[1] == 13 || dic.Value[1] == 23 || dic.Value[1] == 24))
                {
                    sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(Convert.ToDouble(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }

                //数量汇总(数量有小数) 2018.06.20 LK
                if ((dic.Value[0] == 79 && dic.Value[1] == 6) || (dic.Value[0] == 16 && dic.Value[1] == 7))
                {
                    sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(Double.Parse(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }

                //日期格式
                if (dic.Value[0] == 18 && (dic.Value[1] == 15))
                {
                    //XSSFCellStyle styletime = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
                    //IDataFormat formattime = xssfworkbook.CreateDataFormat();
                    //styletime.SetDataFormat(formattime.GetFormat("yyyy-MM-dd"));
                    //styletime.VerticalAlignment = VerticalAlignment.Center;
                    //styletime.Alignment = HorizontalAlignment.Center;

                    sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(DateTime.Parse(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                    //sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).CellStyle = styletime;
                }
            }

            if (data != null)
            {
                for (int i = 29; i < data.GetLength(0) + 29; i++)
                {
                    if (sheet1.GetRow(i) == null)
                        sheet1.CreateRow(i);
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (sheet1.GetRow(i).GetCell(j) == null)
                            sheet1.GetRow(i).CreateCell(j);
                        if (j == 0)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(int.Parse(data[i - 29, j]) + 1);//序号
                        }
                        //设置单元格值，并修改数字格式(成交数量，第二数量，金额)
                        else if (j == 5)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(data[i - 29, j]);
                            sheet1.GetRow(i).GetCell(j).CellStyle = stylewrapandfont;
                        }
                        else if (j == 34)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(int.Parse(data[i - 29, j]) + 1);//后面的序号
                        }

                        else if (j == 13 || j == 23 || j == 24)
                        {
                            if (!string.IsNullOrEmpty(data[i - 29, j]))
                            {
                                sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(data[i - 29, j]));//金额，净重，毛重，两位小数
                                sheet1.GetRow(i).GetCell(j).CellStyle = styletwo;
                            }

                        }
                        else if (j == 10)
                        {
                            if (!string.IsNullOrEmpty(data[i - 29, j]))
                            {
                                sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(data[i - 29, j]));//第二数量三位小数
                                sheet1.GetRow(i).GetCell(j).CellStyle = stylethree;
                            }
                        }
                        else if (j == 12)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(data[i - 29, j]));//单价保留4位小数
                            sheet1.GetRow(i).GetCell(j).CellStyle = stylefour;
                        }
                        else if (j == 6 || j == 8 || j == 29) //成交数量,数量，法定数量，有小数就保留小数，没小数就整数
                        {
                            if (data[i - 29, j] != "")
                            {
                                sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(Convert.ToDouble(data[i - 29, j]).ToString("0.####")));
                                string[] arryNumber = Convert.ToDouble(data[i - 29, j]).ToString("0.####").Split('.');
                                if (arryNumber.Length > 1)
                                {
                                    if (arryNumber[1].Length > 3)
                                    {
                                        sheet1.GetRow(i).GetCell(j).CellStyle = stylefour;
                                    }
                                }
                            }
                        }
                        else if (j == 25) // 商检，设置公式
                        {
                            if (data[i - 29, j] != null)
                            {
                                sheet1.GetRow(i).GetCell(j).CellFormula = "\";;;;;\" & AC" + (i + 1).ToString() + " & \";\" & AF" + (i + 1).ToString() + " & \";;***\"";
                            }
                        }
                        else
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(data[i - 29, j]);//其它，文本类型
                        }
                    }
                }
            }
            //合并单元格
            int start = 0;//记录同组开始行号
            int end = 0;//记录同组结束行号
            string temp = "";
            for (int i = 0; i <= rows; i++)
            {
                int j = 22;
                string cellText = i != rows ? data[i, j].ToString() : "";

                if (cellText == temp)//上下行相等，记录要合并的最后一行
                {
                    end = i;
                }
                else//上下行不等，记录
                {
                    if (start != end)
                    {
                        CellRangeAddress region = new CellRangeAddress(start + 29, end + 29, 22, 22);
                        sheet1.AddMergedRegion(region);
                        CellRangeAddress region2 = new CellRangeAddress(start + 29, end + 29, 24, 24);
                        sheet1.AddMergedRegion(region2);
                    }
                    start = i;
                    end = i;
                    temp = cellText;
                }
            }

            //Force excel to recalculate all the formula while open
            sheet1.ForceFormulaRecalculation = true;

            return xssfworkbook;
        }

        public void setFilePath(string filePath)
        {
            this.FilePath = filePath;
        }
        public string[] SaveAs(string fileName, Vendor vendor)
        {
            var currentDraftCount = new Needs.Ccs.Services.Views.CurrentDraftCountView().GetCurrentDraftCount();

            this.CustomSubmiterAdminID = GetCustomSubmiterAdminID(currentDraftCount);


            this.FileName = fileName;
            var result = new string[3];
            string ExportUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,this.FilePath+this.FileName);
            try
            {
                IWorkbook doc = this.DirectToExcel(vendor);
               using (FileStream file = new FileStream(ExportUrl, FileMode.OpenOrCreate))
                {
                    result[0] = this.FileName;
                    result[1] = "";
                    result[2] = file.Length.ToString();

                    doc.Write(file);
                    file.Close();
                }

                //this.CustomSubmiterAdminID = customSubmiterAdminID;

                this.OnExceled();               
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        /// <summary>
        /// 获取发单人ID
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        private string GetCustomSubmiterAdminID(List<Views.CurrentDraftCountViewModel> listModel)
        {
            if (listModel == null || !listModel.Any())
            {
                return null;
            }

            var minCount = listModel.OrderBy(t => t.DraftCount).FirstOrDefault().DraftCount;
            int[] serialNos = listModel.Where(t => t.DraftCount == minCount).Select(t => t.SerialNo).ToArray();

            Random rand = new Random();
            int arrNum = rand.Next(0, serialNos.Count() - 1);

            var theSelectedModel = listModel.Where(t => t.SerialNo == serialNos[arrNum]).FirstOrDefault();

            for (int i = 0; i < listModel.Count; i++)
            {
                if (listModel[i].SerialNo == serialNos[arrNum])
                {
                    listModel[i].DraftCount = listModel[i].DraftCount + 1;
                    break;
                }
            }

            return theSelectedModel.AdminID;
        }

        virtual protected void OnExceled()
        {
            if (this != null && this.ExcelDeclareSuccess != null)
            {
                this.ExcelDeclareSuccess(this, new SuccessEventArgs(this.DeclarationID));
            }
        }

        private void ExcelDeclared(object sender, SuccessEventArgs e)
        {
            Needs.Ccs.Services.Models.DecHead decHead = new Ccs.Services.Views.DecHeadsView()[e.Object];
            decHead.ExcelMake();

            decHead.SetCustomSubmiter(this.CustomSubmiterAdminID);
        }

        private IWorkbook DirectToExcel(Vendor vendor)
        {
            IWorkbook xssfworkbook = null;

            #region 格式
            FileStream file = new FileStream(this.TempletePath, FileMode.Open, FileAccess.Read);
            xssfworkbook = new XSSFWorkbook(file);
            ISheet sheet1 = xssfworkbook.GetSheet("报关单");


            XSSFCellStyle stylewrap = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            stylewrap.WrapText = true;
            //字体
            IFont font1 = xssfworkbook.CreateFont();//字体
            font1.FontHeightInPoints = 9;
            XSSFCellStyle stylewrapandfont = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            stylewrapandfont.WrapText = true;
            stylewrapandfont.SetFont(font1);

            XSSFCellStyle stylefour = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formatfour = xssfworkbook.CreateDataFormat();
            stylefour.SetDataFormat(formatfour.GetFormat("0.0000"));

            XSSFCellStyle styletwo = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formattwo = xssfworkbook.CreateDataFormat();
            styletwo.SetDataFormat(formattwo.GetFormat("0.00"));

            XSSFCellStyle stylethree = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formatthree = xssfworkbook.CreateDataFormat();
            stylethree.SetDataFormat(formatthree.GetFormat("0.000"));
            #endregion


            #region 表头
            var InPort = "";//入境口岸
            var InPortName = "";
            var CusPort = "";//关区代码
            var CusPortName = "";
            var CIQPort = "";//检验检疫机关代码
            var CIQPortName = "";
            switch (this.EntyPortCode)
            {
                //文锦渡
                case "470401":
                    InPort = "470401";
                    InPortName = "文锦渡";
                    CusPort = "5320";
                    CusPortName = "文锦渡关";
                    CIQPort = "475400";
                    CIQPortName = "文锦渡局本部";
                    break;
                //皇岗
                case "470201":
                    InPort = "470201";
                    InPortName = "皇岗";
                    CusPort = "5301";
                    CusPortName = "皇岗海关";
                    CIQPort = "475200";
                    CIQPortName = "皇岗局本部";
                    break;
                //沙头角
                case "470501":
                    InPort = "470501";
                    InPortName = "沙头角";
                    CusPort = "5303";
                    CusPortName = "沙头角关";
                    CIQPort = "475500";
                    CIQPortName = "沙头角局本部";
                    break;
                //默认皇岗
                default:
                    InPort = "470201";
                    InPortName = "皇岗";
                    CusPort = "5301";
                    CusPortName = "皇岗海关";
                    CIQPort = "475200";
                    CIQPortName = "皇岗局本部";
                    break;
            }

            sheet1.GetRow(0).GetCell(0).SetCellValue(PurchaserContext.Current.ShortName + "进口报关单");

            sheet1.GetRow(2).GetCell(1).SetCellValue(int.Parse(CusPort));
            sheet1.GetRow(2).GetCell(2).SetCellValue(CusPortName);
            sheet1.GetRow(3).GetCell(4).SetCellValue(int.Parse(CusPort));
            sheet1.GetRow(3).GetCell(5).SetCellValue(CusPortName);
            sheet1.GetRow(17).GetCell(4).SetCellValue(int.Parse(InPort));
            sheet1.GetRow(17).GetCell(5).SetCellValue(InPortName);
            sheet1.GetRow(81).GetCell(2).SetCellValue(int.Parse(CIQPort));
            sheet1.GetRow(81).GetCell(3).SetCellValue(CIQPortName);
            sheet1.GetRow(91).GetCell(2).SetCellValue(int.Parse(CIQPort));
            sheet1.GetRow(91).GetCell(3).SetCellValue(CIQPortName);
            sheet1.GetRow(92).GetCell(2).SetCellValue(int.Parse(CIQPort));
            sheet1.GetRow(92).GetCell(3).SetCellValue(CIQPortName);
            sheet1.GetRow(91).GetCell(6).SetCellValue(int.Parse(CIQPort));
            sheet1.GetRow(91).GetCell(7).SetCellValue(CIQPortName);

            sheet1.GetRow(3).GetCell(1).SetCellValue(this.ContrNo);//合同号
            sheet1.GetRow(7).GetCell(7).SetCellValue(this.OwnerName);//消费使用单位-客户名称
            sheet1.GetRow(7).GetCell(1).SetCellValue(this.OwnerCusCode);//消费使用单位代码-10位
            sheet1.GetRow(7).GetCell(3).SetCellValue(this.OwnerScc);//消费单位 同意社会信用代码

            //境内收发货人          
            sheet1.GetRow(5).GetCell(7).SetCellValue(PurchaserContext.Current.CompanyName);//消费使用单位-客户名称
            sheet1.GetRow(5).GetCell(1).SetCellValue(PurchaserContext.Current.CustomsCode);//消费使用单位代码-10位
            sheet1.GetRow(5).GetCell(3).SetCellValue(PurchaserContext.Current.Code);//消费单位 统一社会信用代码
            sheet1.GetRow(5).GetCell(6).SetCellValue(PurchaserContext.Current.CiqCode);//消费单位 检验检疫代码

            //境外收发货人
            sheet1.GetRow(6).GetCell(6).SetCellValue(vendor.CompanyName);//名称

            //申报单位           
            sheet1.GetRow(8).GetCell(7).SetCellValue(PurchaserContext.Current.CompanyName);//申报单位-客户名称
            sheet1.GetRow(8).GetCell(1).SetCellValue(PurchaserContext.Current.CustomsCode);//申报单位-10位
            sheet1.GetRow(8).GetCell(3).SetCellValue(PurchaserContext.Current.Code);//申报单位 统一社会信用代码
            sheet1.GetRow(8).GetCell(6).SetCellValue(PurchaserContext.Current.CiqCode);//申报单位 检验检疫代码

            sheet1.GetRow(9).GetCell(2).SetCellValue("公路运输");//运输方式
            sheet1.GetRow(9).GetCell(7).SetCellValue(this.VoyNo);//航次号
            sheet1.GetRow(10).GetCell(1).SetCellValue(this.BillNo);//提运单号

            sheet1.GetRow(10).GetCell(5).SetCellValue("一般贸易");//监管方式
            sheet1.GetRow(10).GetCell(8).SetCellValue("一般征税");//征免性质
            sheet1.GetRow(11).GetCell(5).SetCellValue("香港");//启运国
            sheet1.GetRow(11).GetCell(8).SetCellValue("中国香港");//经停港
            sheet1.GetRow(12).GetCell(2).SetCellValue("CIF");//成交方式
            sheet1.GetRow(12).GetCell(8).SetCellValue("中国香港");//经停港

            sheet1.GetRow(16).GetCell(1).SetCellValue(this.PackNo);//件数
            sheet1.GetRow(16).GetCell(5).SetCellValue("纸制或纤维板制盒/箱");//包装
            sheet1.GetRow(79).GetCell(22).SetCellValue(this.PackNo);//汇总件数

            sheet1.GetRow(17).GetCell(2).SetCellValue(DateTime.Now.ToString("香港"));
            sheet1.GetRow(18).GetCell(15).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd"));

            sheet1.GetRow(16).GetCell(7).SetCellValue(Convert.ToDouble(this.GrossWt));//毛重
            sheet1.GetRow(79).GetCell(24).SetCellValue(Convert.ToDouble(this.GrossWt));//汇总毛重 

            sheet1.GetRow(16).GetCell(10).SetCellValue(Convert.ToDouble(this.NetWt));//净重
            sheet1.GetRow(79).GetCell(23).SetCellValue(Convert.ToDouble(this.NetWt));//汇总净重

            sheet1.GetRow(79).GetCell(14).SetCellValue(this.Currency);
            sheet1.GetRow(79).GetCell(15).SetCellValue(this.OwnerName);

            if (this.IsInspection || this.IsQuarantine)
            {
                sheet1.GetRow(25).GetCell(1).SetCellValue("是");
                sheet1.GetRow(80).GetCell(3).SetCellValue("企业承诺");
                sheet1.GetRow(80).GetCell(5).SetCellValue("是");
                sheet1.GetRow(80).GetCell(6).SetCellValue("(企业承诺法检必填)");
            }

            sheet1.GetRow(84).GetCell(6).SetCellValue(PurchaserContext.Current.UseOrgPersonCode);
            sheet1.GetRow(84).GetCell(8).SetCellValue(PurchaserContext.Current.UseOrgPersonTel);
            sheet1.GetRow(93).GetCell(2).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd"));//启运日期

            sheet1.GetRow(100).GetCell(2).SetCellValue(DateTime.Now.AddDays(3).ToString("yyyy-MM-dd"));//卸毕日

            //发货人          
            sheet1.GetRow(97).GetCell(2).SetCellValue(PurchaserContext.Current.DomesticConsigneeEname);
            sheet1.GetRow(98).GetCell(2).SetCellValue(vendor.OverseasConsignorCname);
            sheet1.GetRow(99).GetCell(2).SetCellValue(vendor.OverseasConsignorAddr);

            #endregion

            #region 表体
            var currency = this.Currency;
            var sumQty = 0.0000M;
            var totalPrice = new decimal();

            var itemOrdered = this.lists.OrderBy(item => item.GNo).ToList();
            var CountryView = new Needs.Ccs.Services.Views.BaseCountriesView();
            var orderItemCategory = new Needs.Ccs.Services.Views.OrderItemCategoriesView();
            var orderItemTax = new Needs.Ccs.Services.Views.OrderItemTaxesView();
            int irow = 29;
            int ino = 1;
            itemOrdered.ForEach(t =>
            {
                sheet1.GetRow(irow).GetCell(0).SetCellValue(ino.ToString());
                sheet1.GetRow(irow).GetCell(2).SetCellValue(t.CodeTS);
                sheet1.GetRow(irow).GetCell(4).SetCellValue(t.GName);
                sheet1.GetRow(irow).GetCell(5).SetCellValue(t.GModel);
                sheet1.GetRow(irow).GetCell(5).CellStyle = stylewrapandfont;               
                sheet1.GetRow(irow).GetCell(6).SetCellValue(Convert.ToDouble(t.GQty.ToString("0.####")));//成交数量 
                string[] arryNumber = t.GQty.ToString("0.####").Split('.');
                if (arryNumber.Length > 1)
                {
                    if (arryNumber[1].Length > 3)
                    {
                        sheet1.GetRow(irow).GetCell(6).CellStyle = stylefour;
                    }
                }

                sheet1.GetRow(irow).GetCell(7).SetCellValue("007");//成交单位 默认007
               
                if (t.FirstQty.HasValue)
                {
                    sheet1.GetRow(irow).GetCell(8).SetCellValue(Convert.ToDouble(t.FirstQty.Value.ToString("0.####")));//法一数量
                    string[] arryFirst = t.FirstQty.Value.ToString("0.####").Split('.');
                    if (arryFirst.Length > 1)
                    {
                        if (arryFirst[1].Length > 3)
                        {
                            sheet1.GetRow(irow).GetCell(8).CellStyle = stylefour;
                        }
                    }
                }

                sheet1.GetRow(irow).GetCell(9).SetCellValue(t.FirstUnit);//法一单位               
                if (t.SecondQty.HasValue)
                {
                    sheet1.GetRow(irow).GetCell(10).SetCellValue(Convert.ToDouble(t.SecondQty.Value));//法二数量  
                    sheet1.GetRow(irow).GetCell(10).CellStyle = stylethree;
                }
                
                sheet1.GetRow(irow).GetCell(11).SetCellValue(t.SecondUnit);//法二单位           
                sheet1.GetRow(irow).GetCell(12).SetCellValue(Convert.ToDouble(t.DeclPrice));//单价
                sheet1.GetRow(irow).GetCell(12).CellStyle = stylefour;
                sheet1.GetRow(irow).GetCell(13).SetCellValue(Convert.ToDouble(t.DeclTotal));//总价 
                sheet1.GetRow(irow).GetCell(13).CellStyle = styletwo;
                sheet1.GetRow(irow).GetCell(14).SetCellValue(currency);
                string v15 = CountryView.Where(a => a.Code == t.OriginCountry).FirstOrDefault()?.Name;
                sheet1.GetRow(irow).GetCell(15).SetCellValue(v15);
                sheet1.GetRow(irow).GetCell(16).SetCellValue("中国");
                //17          
                sheet1.GetRow(irow).GetCell(18).SetCellValue("深圳特区");
                sheet1.GetRow(irow).GetCell(19).SetCellValue("深圳市龙岗区");
                sheet1.GetRow(irow).GetCell(20).SetCellValue("照章征税");
                //21           
                sheet1.GetRow(irow).GetCell(21).SetCellValue(this.ContrNo);
                sheet1.GetRow(irow).GetCell(22).SetCellValue(t.CaseNo);
                sheet1.GetRow(irow).GetCell(23).SetCellValue(Convert.ToDouble(t.NetWt));
                sheet1.GetRow(irow).GetCell(23).CellStyle = styletwo;
                sheet1.GetRow(irow).GetCell(24).SetCellValue(Convert.ToDouble(t.GrossWt));
                sheet1.GetRow(irow).GetCell(24).CellStyle = styletwo;
                //25 - 27
                sheet1.GetRow(irow).CreateCell(28);
                sheet1.GetRow(irow).CreateCell(29);
                sheet1.GetRow(irow).CreateCell(30);
                sheet1.GetRow(irow).CreateCell(31);
                sheet1.GetRow(irow).CreateCell(32);
                sheet1.GetRow(irow).CreateCell(33);
                sheet1.GetRow(irow).CreateCell(34);
                sheet1.GetRow(irow).CreateCell(35);

                sheet1.GetRow(irow).GetCell(28).SetCellValue(t.GoodsModel);                

                sheet1.GetRow(irow).GetCell(29).SetCellValue(Convert.ToDouble(t.GQty.ToString("0.####")));//成交数量 
                string[] arryGQty = t.GQty.ToString("0.####").Split('.');
                if (arryGQty.Length > 1)
                {
                    if (arryGQty[1].Length > 3)
                    {
                        sheet1.GetRow(irow).GetCell(29).CellStyle = stylefour;
                    }
                }

                string v30 = orderItemCategory.Where(item => item.OrderItemID == t.OrderItemID).FirstOrDefault()?.TaxName;
                sheet1.GetRow(irow).GetCell(30).SetCellValue(v30);
                sheet1.GetRow(irow).GetCell(31).SetCellValue(t.GoodsBrand);
                var tariff = orderItemTax.Where(item => item.OrderItemID == t.OrderItemID && item.Type == Enums.CustomsRateType.ImportTax).FirstOrDefault()?.Rate;
                string v32 = tariff == 0M ? "0" : tariff.ToString();
                sheet1.GetRow(irow).GetCell(32).SetCellValue(v32);

                sheet1.GetRow(irow).GetCell(33).SetCellValue(this.OwnerName);
                sheet1.GetRow(irow).GetCell(34).SetCellValue(ino);
                sheet1.GetRow(irow).GetCell(35).SetCellValue(t.DeclTotal.ToString());

                //2018-08-06 海关登革热检疫
                if (IsInspection || this.IsQuarantine)
                {
                    sheet1.GetRow(irow).GetCell(3).SetCellValue(t.CiqCode);
                    sheet1.GetRow(irow).GetCell(25).SetCellValue(";;;;;"  + t.GoodsModel +";"+t.GoodsBrand+ ";;***");
                    sheet1.GetRow(irow).GetCell(26).SetCellValue("3C目录外,正常");
                    sheet1.GetRow(irow).GetCell(27).SetCellValue("其他");
                }

                ino++;
                irow++;
                sumQty += t.GQty;
                totalPrice += t.DeclTotal;
            });
            #endregion

            #region
            //合并单元格
            int start = 0;//记录同组开始行号
            int end = 0;//记录同组结束行号
            string temp = "";
            int rows = itemOrdered.Count();
            for (int i = 0; i <= rows; i++)
            {
                //int j = 22;
                string cellText = i != rows ? itemOrdered[i].CaseNo : "";

                if (cellText == temp)//上下行相等，记录要合并的最后一行
                {
                    end = i;
                }
                else//上下行不等，记录
                {
                    if (start != end)
                    {
                        CellRangeAddress region = new CellRangeAddress(start + 29, end + 29, 22, 22);
                        sheet1.AddMergedRegion(region);
                        CellRangeAddress region2 = new CellRangeAddress(start + 29, end + 29, 24, 24);
                        sheet1.AddMergedRegion(region2);
                    }
                    start = i;
                    end = i;
                    temp = cellText;
                }
            }
            #endregion

            sheet1.ForceFormulaRecalculation = true;

            return xssfworkbook;
        }

    }
}
