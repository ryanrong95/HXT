using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp.Services.Controls
{
    public partial class TNTForm : Form
    {
        public TNTForm()
        {
            InitializeComponent();
        }

        private void TNTForm_Load(object sender, EventArgs e)
        {
            string strDefaultPrinter = PrinterName;//获取打印机名

        }
        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrinterName { get; set; }
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font font;
            string str;
            float xPos;       //x点坐标
            float yPos;       //y点的坐标
            float topMargin = 10;
            float leftMargin = -10;

            font = new Font("宋体", 10);
            xPos = leftMargin + 250;
            yPos = topMargin + 3;
            str = tb1.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 55;
            yPos = yPos + font.GetHeight(e.Graphics) + 15;
            str = tb75.Text ;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 250;
            yPos = yPos + font.GetHeight(e.Graphics) - 15;
            str = tb2.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 24;
            yPos = yPos + font.GetHeight(e.Graphics) + 23;
            str = tb3.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            #region 发件地址
            font = new Font("宋体", 6);
            xPos = leftMargin + 140;
            yPos = yPos + font.GetHeight(e.Graphics) + 23;
            str = tb42.Text = "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO., LIMITED";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 90;
            yPos = yPos + font.GetHeight(e.Graphics) + 8;
            font = new Font("宋体", 6);
            str = tb43.Text = "INIT B 2/F HOUTEX INDUSTRIAL BUILDING 16 HUNG TO ROAD KWUN TONG KL";// = "Kelly";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 30;
            yPos = yPos + font.GetHeight(e.Graphics) + 8;
            font = new Font("宋体", 6);
            str = tb44.Text = "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 350;
            yPos = topMargin + 152;
            font = new Font("宋体", 6);
            str = tb45.Text ;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 300;
            yPos = yPos + font.GetHeight(e.Graphics) + 25;
            font = new Font("宋体", 6);
            str = tb49.Text = "HK";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 140;
            yPos = yPos + font.GetHeight(e.Graphics) + 8;
            font = new Font("宋体", 6);
            str = tb50.Text = "林團欲";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 300;
            yPos = yPos + font.GetHeight(e.Graphics) - 11;
            font = new Font("宋体", 6);
            str = tb51.Text = "0852-31019258";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());
            #endregion
            #region 收件地址

            xPos = leftMargin + 140;
            yPos = yPos + font.GetHeight(e.Graphics) + 25;
            font = new Font("宋体", 6);
            str = tb52.Text ;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 90;
            yPos = yPos + font.GetHeight(e.Graphics) + 8;
            font = new Font("宋体", 6);
            str = tb53.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 30;
            yPos = yPos + font.GetHeight(e.Graphics) + 8;
            font = new Font("宋体", 6);
            str = tb54.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 350;
            yPos = topMargin + 287;
            font = new Font("宋体", 6);
            str = tb55.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 300;
            yPos = yPos + font.GetHeight(e.Graphics) + 27;
            font = new Font("宋体", 6);
            str = tb59.Text ;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 140;
            yPos = yPos + font.GetHeight(e.Graphics) + 8;
            font = new Font("宋体", 6);
            str = tb60.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 300;
            yPos = yPos + font.GetHeight(e.Graphics) - 11;
            font = new Font("宋体", 6);
            str = tb61.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            #endregion
            #region 递送地址

            xPos = leftMargin + 140;
            yPos = yPos + font.GetHeight(e.Graphics) + 20;
            font = new Font("宋体", 6);
            str = tb62.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 90;
            yPos = yPos + font.GetHeight(e.Graphics) + 8;
            font = new Font("宋体", 6);
            str = tb63.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 30;
            yPos = yPos + font.GetHeight(e.Graphics) + 8;
            font = new Font("宋体", 6);
            str = tb64.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 350;
            yPos = topMargin + 415;
            font = new Font("宋体", 6);
            str = tb65.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 300;
            yPos = yPos + font.GetHeight(e.Graphics) + 27;
            font = new Font("宋体", 6);
            str = tb69.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 140;
            yPos = yPos + font.GetHeight(e.Graphics) + 8;
            font = new Font("宋体", 6);
            str = tb70.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 300;
            yPos = yPos + font.GetHeight(e.Graphics) - 11;
            font = new Font("宋体", 6);
            str = tb71.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            #endregion
            #region 危险物品

            xPos = leftMargin + 355;
            yPos = yPos + font.GetHeight(e.Graphics) + 15;
            font = new Font("宋体", 10);
            str = tb72.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 400;
            yPos = yPos + font.GetHeight(e.Graphics) - 15;
            font = new Font("宋体", 10);
            str = tb73.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            #endregion
            #region 发件人签名

            xPos = leftMargin + 140;
            yPos = yPos + font.GetHeight(e.Graphics) + 20;
            font = new Font("宋体", 10);
            str = tb74.Text = "林團欲";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 52;
            yPos = yPos + font.GetHeight(e.Graphics) + 8;
            font = new Font("宋体", 10);
            str = tb4.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 90;
            yPos = yPos + font.GetHeight(e.Graphics) - 15;
            font = new Font("宋体", 10);
            str = tb5.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 120;
            yPos = yPos + font.GetHeight(e.Graphics) - 16;
            font = new Font("宋体", 10);
            str = tb6.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            #endregion
            #region 服务
            xPos = leftMargin + 587;
            yPos = topMargin + 142;
            font = new Font("宋体", 10);
            str = tb7.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 645;
            yPos = yPos + font.GetHeight(e.Graphics) - 16;
            font = new Font("宋体", 10);
            str = tb8.Text ;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 587;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb9.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 645;
            yPos = yPos + font.GetHeight(e.Graphics) - 16;
            font = new Font("宋体", 10);
            str = tb10.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 587;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb11.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 645;
            yPos = yPos + font.GetHeight(e.Graphics) - 16;
            font = new Font("宋体", 10);
            str = tb12.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 587;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb13.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 645;
            yPos = yPos + font.GetHeight(e.Graphics) - 16;
            font = new Font("宋体", 10);
            str = tb14.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 587;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb15.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 645;
            yPos = yPos + font.GetHeight(e.Graphics) - 16;
            font = new Font("宋体", 10);
            str = tb16.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 645;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb17.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());
            #endregion
            //#region 特殊递送

            //xPos = leftMargin + 3;
            //yPos = yPos + font.GetHeight(e.Graphics) + 1;
            //font = new Font("宋体", 10);
            //str = tb76.Text = "特殊递送";
            //e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            //xPos = leftMargin + 3;
            //yPos = yPos + font.GetHeight(e.Graphics) + 1;
            //font = new Font("宋体", 10);
            //str = tb77.Text = "货物分类编码";
            //e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            //#endregion
            #region 货物的详细描述
            /*********************************1********************************************************************/
            xPos = leftMargin + 450;
            yPos = topMargin + 384;
            font = new Font("宋体", 8);
            str = tb78.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 450;
            yPos = yPos + font.GetHeight(e.Graphics) + 10;
            font = new Font("宋体", 8);
            str = tb79.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 450;
            yPos = yPos + font.GetHeight(e.Graphics) + 10;
            font = new Font("宋体", 8);
            str = tb80.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 450;
            yPos = yPos + font.GetHeight(e.Graphics) + 10;
            font = new Font("宋体", 8);
            str = tb81.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 585;
            yPos = topMargin + 384;
            font = new Font("宋体", 8);
            str = tb82.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 618;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb87.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 660;
            yPos = yPos + font.GetHeight(e.Graphics) - 12;
            font = new Font("宋体", 8);
            str = tb92.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 690;
            yPos = yPos + font.GetHeight(e.Graphics) - 11;
            font = new Font("宋体", 8);
            str = tb97.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 740;
            yPos = yPos + font.GetHeight(e.Graphics) - 10;
            font = new Font("宋体", 8);
            str = tb101.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 780;
            yPos = yPos + font.GetHeight(e.Graphics) - 10;
            font = new Font("宋体", 8);
            str = tb105.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            /*********************************************************************************************************/
            /*********************************2********************************************************************/
            xPos = leftMargin + 585;
            yPos = yPos + font.GetHeight(e.Graphics) + 11;
            font = new Font("宋体", 8);
            str = tb83.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 618;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb88.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 660;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb93.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 690;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb98.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 740;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb102.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 780;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb106.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            /*********************************************************************************************************/
            /*********************************3********************************************************************/
            xPos = leftMargin + 585;
            yPos = yPos + font.GetHeight(e.Graphics) + 11;
            font = new Font("宋体", 8);
            str = tb84.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 618;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb89.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 660;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb94.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 690;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb99.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 740;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb103.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 780;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb107.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            /*********************************************************************************************************/
            /*********************************4********************************************************************/
            xPos = leftMargin + 585;
            yPos = yPos + font.GetHeight(e.Graphics) + 11;
            font = new Font("宋体", 8);
            str = tb85.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 618;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb90.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 660;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb95.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 690;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb100.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 740;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb104.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 780;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb108.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            /*********************************************************************************************************/
            /*********************************合计********************************************************************/
            xPos = leftMargin + 585;
            yPos = yPos + font.GetHeight(e.Graphics) + 11;
            font = new Font("宋体", 8);
            str = tb86.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 618;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb91.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 660;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 8);
            str = tb96.Text;
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            /*********************************************************************************************************/
            #endregion
        }
        /// <summary>
        /// 打印信息
        /// </summary>
        /// <param name="?">打印机名称</param>
        /// <param name="sho_printNum">打印数量</param>
        public void print(string str_Printer, short sho_printNum)
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = str_Printer;
                pd.PrinterSettings.Copies = sho_printNum;
                pd.PrintController = new StandardPrintController();
                pd.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
                if (pd.PrinterSettings.IsValid)
                {
                    pd.Print();
                }
                else
                {
                    MessageBox.Show("打印机连接错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void DataPrint(InternationalAirWaybillPrint Data)
        {
            tb50.Text = "林團欲";
            tb51.Text = "0852-31019258";
            tb49.Text = "HK";
            tb43.Text = "INIT B 2/F HOUTEX INDUSTRIAL BUILDING 16 HUNG TO ROAD KWUN TONG KL";
            tb42.Text = "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO., LIMITED";
            
            tb52.Text = Data.ReceiverCompany;
            tb53.Text = Data.ReceiverAddress;
            tb55.Text = Data.ReceiverPostalCode;
            tb59.Text = Data.ReceiverCountry;
            tb60.Text = Data.NameOfContactPerson;
            tb61.Text = Data.TelOfContactPerson;

            tb74.Text = "林團欲";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strDefaultPrinter = PrinterName;//获取打印机名
            print(strDefaultPrinter, 1);
        }
    }
}
