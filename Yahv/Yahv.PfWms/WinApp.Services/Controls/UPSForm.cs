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
    public partial class UPSForm : Form
    {
        public UPSForm()
        {
            InitializeComponent();
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
            float topMargin = 25;
            float leftMargin = 30;

            #region 左边1

            font = new Font("宋体", 10);
            xPos = leftMargin + 3;
            yPos = topMargin;
            str = tb1.Text;// = "30AE60";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 150;
            yPos = topMargin;
            str = tb2.Text;// = "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 15;
            font = new Font("宋体", 10);
            str = tb3.Text;// = "Kelly";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 150;
            yPos = yPos + font.GetHeight(e.Graphics) - 16;
            font = new Font("宋体", 10);
            str = tb4.Text;//= "85234264941";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 12;
            font = new Font("宋体", 10);
            str = tb5.Text;//= "Anda International Trade Group Limited";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb6.Text;//= "Unit B1,2/F.,Hputex Ind. Bldg.,16 Hung ";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb7.Text;//= "To Rd., Kwun Tong ,KowIoon";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb8.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb9.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 50;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb10.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 240;
            yPos = yPos + font.GetHeight(e.Graphics) - 15;
            font = new Font("宋体", 10);
            str = tb11.Text;//= "H0NGKONG";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            #endregion
            #region 左边2

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 13;
            font = new Font("宋体", 10);
            str = tb12.Text;//= "UPS SAVER # 226342";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 150;
            yPos = yPos + font.GetHeight(e.Graphics) - 14;
            font = new Font("宋体", 10);
            str = tb13.Text;// = "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 16;
            font = new Font("宋体", 10);
            str = tb14.Text;//= "Remco van der Lugt";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 150;
            yPos = yPos + font.GetHeight(e.Graphics) - 14;
            font = new Font("宋体", 10);
            str = tb15.Text;//= "31 10 4352181";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 15;
            font = new Font("宋体", 10);
            str = tb16.Text;//= "Epicor Components B.V.";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb17.Text;// = "Kethelweg 46 3135 GL Vlaardingen ";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb18.Text;// = "The N etherlands";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb19.Text;// = "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 3;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb20.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 50;
            yPos = yPos + font.GetHeight(e.Graphics) + 1;
            font = new Font("宋体", 10);
            str = tb21.Text;//= "3135";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 240;
            yPos = yPos + font.GetHeight(e.Graphics) - 16;
            font = new Font("宋体", 10);
            str = tb22.Text;//= "The NetherLands";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            #endregion
            #region 左边3

            xPos = leftMargin - 10;
            yPos = yPos + font.GetHeight(e.Graphics) + 28;
            font = new Font("宋体", 10);
            str = cb1.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 95;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 10);
            str = cb2.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 205;
            yPos = yPos + font.GetHeight(e.Graphics) - 14;
            font = new Font("宋体", 10);
            str = cb3.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin - 20;
            yPos = yPos + font.GetHeight(e.Graphics) + 2;
            font = new Font("宋体", 10);
            str = cb4.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 40;
            yPos = yPos + font.GetHeight(e.Graphics) - 18;
            font = new Font("宋体", 10);
            str = cb5.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 180;
            yPos = yPos + font.GetHeight(e.Graphics) - 18;
            font = new Font("宋体", 10);
            str = tb23.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            #region 表格
            xPos = leftMargin - 10;
            yPos = yPos + font.GetHeight(e.Graphics) + 50;
            font = new Font("宋体", 10);
            str = cb6.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 95;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 10);
            str = cb7.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 205;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            font = new Font("宋体", 10);
            str = cb8.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 85;
            yPos = yPos + font.GetHeight(e.Graphics) + 3;
            font = new Font("宋体", 6);
            str = tb24.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 85;
            yPos = yPos + font.GetHeight(e.Graphics) + 5;
            font = new Font("宋体", 6);
            str = tb25.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 275;
            yPos = yPos + font.GetHeight(e.Graphics) - 9;
            font = new Font("宋体", 6);
            str = tb26.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());
            #endregion

            #endregion
            #region 右边4
            font = new Font("宋体", 8);
            xPos = leftMargin + 530;
            yPos = topMargin + 105;
            str = tb28.Text; //"";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 8);
            xPos = leftMargin + 530;
            yPos = yPos + font.GetHeight(e.Graphics) + 2;
            str = tb29.Text; //"";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 530;
            yPos = yPos + font.GetHeight(e.Graphics) + 2;
            font = new Font("宋体", 8);
            str = tb30.Text; //"";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 530;
            yPos = yPos + font.GetHeight(e.Graphics) + 2;
            font = new Font("宋体", 8);
            str = tb31.Text; //"";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 530;
            yPos = yPos + font.GetHeight(e.Graphics) + 2;
            font = new Font("宋体", 8);
            str = tb32.Text; //"X";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 530;
            yPos = yPos + font.GetHeight(e.Graphics) + 2;
            font = new Font("宋体", 8);
            str = tb33.Text; //"";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());
            #endregion
            #region 右边5
            font = new Font("宋体", 8);
            xPos = leftMargin + 350;
            yPos = yPos + font.GetHeight(e.Graphics) + 35;
            str = tb34.Text;//= "1";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 8);
            xPos = leftMargin + 425;
            yPos = yPos + font.GetHeight(e.Graphics) - 10;
            str = tb35.Text;//= "2.22Kg";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 486;
            yPos = yPos + font.GetHeight(e.Graphics) - 12;
            font = new Font("宋体", 8);
            str = tb36.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 551;
            yPos = yPos + font.GetHeight(e.Graphics) - 15;
            font = new Font("宋体", 8);
            str = tb37.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());


            font = new Font("宋体", 8);
            xPos = leftMargin + 350;
            yPos = yPos + font.GetHeight(e.Graphics) + 20;
            str = tb38.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 8);
            xPos = leftMargin + 430;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            str = tb39.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 505;
            yPos = yPos + font.GetHeight(e.Graphics) - 15;
            font = new Font("宋体", 8);
            str = tb40.Text;// = "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            ////other
            font = new Font("宋体", 10);
            xPos = leftMargin + 350;
            yPos = yPos + font.GetHeight(e.Graphics) + 5;
            str = cb9.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 400;
            yPos = yPos + font.GetHeight(e.Graphics) - 20;
            str = cb10.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            xPos = leftMargin + 455;
            yPos = yPos + font.GetHeight(e.Graphics) - 18;
            font = new Font("宋体", 10);
            str = cb11.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 515;
            yPos = yPos + font.GetHeight(e.Graphics) - 15;
            str = cb12.Checked ? "√" : "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 530;
            yPos = yPos + font.GetHeight(e.Graphics) - 15;
            str = tb41.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 350;
            yPos = yPos + font.GetHeight(e.Graphics) + 20;
            str = tb42.Text;// = "IC";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 545;
            yPos = yPos + font.GetHeight(e.Graphics) - 20;
            str = cb13.Checked ? "X" : ""; //"";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 350;
            yPos = yPos + font.GetHeight(e.Graphics) + 45;
            str = tb43.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 465;
            yPos = yPos + font.GetHeight(e.Graphics) - 14;
            str = tb44.Text;//= "9249.3";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 420;
            yPos = yPos + font.GetHeight(e.Graphics) + 10;
            str = tb45.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 420;
            yPos = yPos + font.GetHeight(e.Graphics) + 5;
            str = tb46.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 420;
            yPos = yPos + font.GetHeight(e.Graphics) + 5;
            str = tb47.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 350;
            yPos = yPos + font.GetHeight(e.Graphics) + 20;
            str = tb48.Text;//= "";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());
            #endregion
            #region 最右边7
            font = new Font("宋体", 10);
            xPos = leftMargin + 590;
            yPos = topMargin + 457;
            str = tb49.Text;//= "19";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 620;
            yPos = yPos + font.GetHeight(e.Graphics) - 13;
            str = tb50.Text; //= "6";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 663;
            yPos = yPos + font.GetHeight(e.Graphics) - 15;
            str = tb51.Text;//= "2021";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            font = new Font("宋体", 10);
            xPos = leftMargin + 695;
            yPos = yPos + font.GetHeight(e.Graphics) - 18;
            str = tb52.Text;//= "xxxx";
            e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            //font = new Font("宋体", 8);
            //xPos = leftMargin + 590;
            //yPos = yPos + font.GetHeight(e.Graphics) + 20;
            //str = tb53.Text = "45";
            //e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            //font = new Font("宋体", 8);
            //xPos = leftMargin + 715;
            //yPos = yPos + font.GetHeight(e.Graphics) - 15;
            //str = tb54.Text = "222";
            //e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());

            //font = new Font("宋体", 8);
            //xPos = leftMargin + 740;
            //yPos = yPos + font.GetHeight(e.Graphics) - 14;
            //str = tb55.Text = "13:01";
            //e.Graphics.DrawString(str, font, Brushes.Black, xPos, yPos, new StringFormat());


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
            tb1.Text = Data.ShipperAccountNo;
            tb2.Text = Data.ShipperCustomNo;
            tb3.Text = Data.SenderName = "林團欲";
            tb4.Text = Data.SenderTel = "0852-31019258";
            //if (!string.IsNullOrEmpty(Data.SenderCompany))
            //{
            // string senderCompanyAndAddress = Data.SenderCompanyAndAddress = "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO.,LIMITED INIT B 2/F HOUTEX INDUSTRIAL BUILDING 16 HUNG TO ROAD KWUN TONG KL";//一行有45个字符
            string senderCompany = "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO.,LIMITED";//一行有45个字符
            SubstringCompany(senderCompany);
            //}
            //if (!string.IsNullOrEmpty(Data.SenderAddress))
            //{
            string senderAddress = "INIT B 2/F HOUTEX INDUSTRIAL BUILDING 16 HUNG TO ROAD KWUN TONG KL";
            SubstringAddress(senderAddress);
            //}
            tb10.Text = Data.SenderPostalCode;
            tb11.Text = Data.SenderCountry = "HONGKONG";

            tb12.Text = Data.ReceiverAccountNo;
            tb13.Text = Data.ReceiverCustomNo;
            tb14.Text = Data.NameOfContactPerson;
            tb15.Text = Data.TelOfContactPerson;
            if (!string.IsNullOrEmpty(Data.ReceiverCompany))
            {
                string receiverCompany = Data.ReceiverCompany;
                SubstringReceiverCompany(receiverCompany);
            }
            if (!string.IsNullOrEmpty(Data.ReceiverAddress))
            {
                string receiverAddress = Data.ReceiverAddress;
                SubstringReceiverAddress(receiverAddress);

            }
            tb21.Text = Data.ReceiverPostalCode;
            tb22.Text = Data.ReceiverCountry;
        }
        private void UPSForm_Load(object sender, EventArgs e)
        {

        }
        void SubstringCompany(string senderCompany)
        {
            string company = "";
            int companyLength = 0;
            string[] spaceCompany = senderCompany.Split(' ');
            company = spaceCompany[0];
            for (int i = 1; i < spaceCompany.Length; i++)
            {
                company = company + " " + spaceCompany[i];
                if (company.Length <= 45)
                {
                    tb5.Text = company;
                }
                else
                {
                    company = spaceCompany[i];
                    companyLength = i;
                    break;
                }

            }
            if (companyLength + 1 < spaceCompany.Length)
            {
                for (int j = companyLength + 1; j < spaceCompany.Length; j++)
                {
                    company = company + " " + spaceCompany[j];
                    if (company.Length <= 45)
                    {
                        tb6.Text = company;
                    }
                    else
                    {
                        company = spaceCompany[j - 1];
                        companyLength = j - 1;
                        break;
                    }
                    companyLength = spaceCompany.Length + 1;
                }
                tb6.Text = company;
            }
            else
            {
                tb6.Text = company;
                company = "";
                companyLength = spaceCompany.Length;
            }
            if (companyLength + 1 < spaceCompany.Length)
            {
                for (int k = companyLength + 1; k < spaceCompany.Length; k++)
                {
                    company = company + " " + spaceCompany[k];
                    if (company.Length <= 45)
                    {
                        tb7.Text = company;
                    }
                    else
                    {
                        company = spaceCompany[k - 1];
                        companyLength = k - 1;
                        break;
                    }
                    companyLength = spaceCompany.Length + 1;
                }

            }
            else
            {
                tb7.Text = company;
                company = "";
                companyLength = spaceCompany.Length;
            }
            if (companyLength + 1 < spaceCompany.Length)
            {
                for (int m = companyLength + 1; m < spaceCompany.Length; m++)
                {
                    company = company + " " + spaceCompany[m];
                    if (company.Length <= 45)
                    {
                        tb8.Text = company;
                    }
                    else
                    {
                        company = spaceCompany[m - 1];
                        companyLength = m - 1;
                        break;
                    }
                    companyLength = spaceCompany.Length + 1;
                }
                tb8.Text = company;
            }
            else
            {
                tb8.Text = company;
                company = "";
                companyLength = spaceCompany.Length;
            }
        }
        void SubstringAddress(string senderAddress)
        {
            string address = "";
            int addressLength = 0;
            string[] spaceAddress = senderAddress.Split(' ');
            address = spaceAddress[0];
            string addressText = "";

            for (int i = 1; i < spaceAddress.Length; i++)
            {
                address = address + " " + spaceAddress[i];
                int count = i;
                if (address.Length <= 45)
                {
                    addressText = address;
                }
                else
                {
                    address = spaceAddress[i];
                    addressLength = i;
                    break;
                }
            }
            if (string.IsNullOrEmpty(tb5.Text))
            {
                tb5.Text = addressText;
            }
            else if (string.IsNullOrEmpty(tb6.Text))
            {
                tb6.Text = addressText;
            }
            else if (string.IsNullOrEmpty(tb7.Text))
            {
                tb7.Text = addressText;
            }
            else if (string.IsNullOrEmpty(tb8.Text))
            {
                tb8.Text = addressText;
            }
            else if (string.IsNullOrEmpty(tb9.Text))
            {
                tb9.Text = addressText;
            }

            if (addressLength + 1 < spaceAddress.Length)
            {
                for (int j = addressLength + 1; j < spaceAddress.Length; j++)
                {
                    address = address + " " + spaceAddress[j];
                    if (address.Length <= 45)
                    {
                        addressText = address;
                    }
                    else
                    {
                        address = spaceAddress[j - 1];
                        addressLength = j - 1;
                        break;
                    }
                    addressLength = spaceAddress.Length + 1;
                }
                if (string.IsNullOrEmpty(tb5.Text))
                {
                    tb5.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb6.Text))
                {
                    tb6.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb7.Text))
                {
                    tb7.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb8.Text))
                {
                    tb8.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb9.Text))
                {
                    tb9.Text = addressText;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(tb5.Text))
                {
                    tb5.Text = address;
                }
                else if (string.IsNullOrEmpty(tb6.Text))
                {
                    tb6.Text = address;
                }
                else if (string.IsNullOrEmpty(tb7.Text))
                {
                    tb7.Text = address;
                }
                else if (string.IsNullOrEmpty(tb8.Text))
                {
                    tb8.Text = address;
                }
                else if (string.IsNullOrEmpty(tb9.Text))
                {
                    tb9.Text = address;
                }
                address = "";
                addressLength = spaceAddress.Length;
            }

            if (addressLength + 1 < spaceAddress.Length)
            {
                for (int k = addressLength + 1; k < spaceAddress.Length; k++)
                {
                    address = address + " " + spaceAddress[k];
                    if (address.Length <= 45)
                    {
                        addressText = address;
                    }
                    else
                    {
                        address = spaceAddress[k - 1];
                        addressLength = k - 1;
                        break;
                    }
                    addressLength = spaceAddress.Length + 1;
                }
                if (string.IsNullOrEmpty(tb5.Text))
                {
                    tb5.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb6.Text))
                {
                    tb6.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb7.Text))
                {
                    tb7.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb8.Text))
                {
                    tb8.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb9.Text))
                {
                    tb9.Text = addressText;
                }
            }
            else
            {
                address = "";
                addressLength = spaceAddress.Length;
            }

            if (addressLength + 1 < spaceAddress.Length)
            {
                for (int n = addressLength + 1; n < spaceAddress.Length; n++)
                {
                    address = address + " " + spaceAddress[n];
                    if (address.Length <= 45)
                    {
                        addressText = address;
                    }
                    else
                    {
                        address = spaceAddress[n - 1];
                        addressLength = n - 1;
                        break;
                    }
                    addressLength = spaceAddress.Length + 1;
                }
                if (string.IsNullOrEmpty(tb5.Text))
                {
                    tb5.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb6.Text))
                {
                    tb6.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb7.Text))
                {
                    tb7.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb8.Text))
                {
                    tb8.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb9.Text))
                {
                    tb9.Text = addressText;
                }
            }
            else
            {
                address = "";
                addressLength = spaceAddress.Length;
            }

            if (addressLength + 1 < spaceAddress.Length)
            {
                for (int m = addressLength + 1; m < spaceAddress.Length; m++)
                {
                    address = address + " " + spaceAddress[m];
                    if (address.Length <= 45)
                    {
                        addressText = address;
                    }
                    else
                    {
                        address = spaceAddress[m - 1];
                        addressLength = m - 1;
                        break;
                    }
                    addressLength = spaceAddress.Length + 1;
                }
                if (string.IsNullOrEmpty(tb5.Text))
                {
                    tb5.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb6.Text))
                {
                    tb6.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb7.Text))
                {
                    tb7.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb8.Text))
                {
                    tb8.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb9.Text))
                {
                    tb9.Text = addressText;
                }
            }
            else
            {

                address = "";
                addressLength = spaceAddress.Length;
            }
        }
        void SubstringReceiverCompany(string receiverCompany)
        {
            string company = "";
            int companyLength = 0;
            if (!string.IsNullOrEmpty(receiverCompany))
            {

                string[] spaceCompany = receiverCompany.Split(' ');
                company = spaceCompany[0];
                for (int i = 1; i < spaceCompany.Length; i++)
                {
                    company = company + " " + spaceCompany[i];
                    if (company.Length <= 45)
                    {
                        tb16.Text = company;
                        companyLength = i;
                    }
                    else
                    {
                        company = spaceCompany[i];
                        companyLength = i;
                        break;
                    }

                }
                if (companyLength + 1 < spaceCompany.Length)
                {
                    for (int j = companyLength + 1; j < spaceCompany.Length; j++)
                    {
                        company = company + " " + spaceCompany[j];
                        if (company.Length <= 45)
                        {
                            tb17.Text = company;
                        }
                        else
                        {
                            company = spaceCompany[j - 1];
                            companyLength = j - 1;
                            break;
                        }
                        companyLength = spaceCompany.Length + 1;
                    }
                }

                if (companyLength + 1 < spaceCompany.Length)
                {
                    for (int k = companyLength + 1; k < spaceCompany.Length; k++)
                    {
                        company = company + " " + spaceCompany[k];
                        if (company.Length <= 45)
                        {
                            tb18.Text = company;
                        }
                        else
                        {
                            company = spaceCompany[k - 1];
                            companyLength = k - 1;
                            break;
                        }
                        companyLength = spaceCompany.Length + 1;
                    }

                }

                if (companyLength + 1 < spaceCompany.Length)
                {
                    for (int m = companyLength + 1; m < spaceCompany.Length; m++)
                    {
                        company = company + " " + spaceCompany[m];
                        if (company.Length <= 45)
                        {
                            tb19.Text = company;
                        }
                        else
                        {
                            company = spaceCompany[m - 1];
                            companyLength = m - 1;
                            break;
                        }
                        companyLength = spaceCompany.Length + 1;
                    }
                    tb19.Text = company;
                }

            }
        }
        void SubstringReceiverAddress(string receiverAddress)
        {
            string address = "";
            int addressLength = 0;
            if (!string.IsNullOrEmpty(receiverAddress))
            {
                string[] spaceAddress = receiverAddress.Split(' ');
                address = spaceAddress[0];
                string addressText = "";

                for (int i = 1; i < spaceAddress.Length; i++)
                {
                    address = address + " " + spaceAddress[i];
                    int count = i;
                    if (address.Length <= 45)
                    {
                        addressText = address;
                        addressLength = i;
                    }
                    else
                    {
                        address = spaceAddress[i];
                        addressLength = i;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(tb16.Text))
                {
                    tb16.Text = addressText;
                }
                if (string.IsNullOrEmpty(tb17.Text))
                {
                    tb17.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb18.Text))
                {
                    tb18.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb19.Text))
                {
                    tb19.Text = addressText;
                }
                else if (string.IsNullOrEmpty(tb20.Text))
                {
                    tb20.Text = addressText;
                }
                if (addressLength + 1 < spaceAddress.Length)
                {
                    for (int j = addressLength + 1; j < spaceAddress.Length; j++)
                    {
                        address = address + " " + spaceAddress[j];
                        if (address.Length <= 45)
                        {
                            addressText = address;
                        }
                        else
                        {
                            address = spaceAddress[j - 1];
                            addressLength = j - 1;
                            break;
                        }
                        addressLength = spaceAddress.Length + 1;
                    }
                    if (string.IsNullOrEmpty(tb16.Text))
                    {
                        tb16.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb17.Text))
                    {
                        tb17.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb18.Text))
                    {
                        tb18.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb19.Text))
                    {
                        tb19.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb20.Text))
                    {
                        tb20.Text = addressText;
                    }
                }

                if (addressLength + 1 < spaceAddress.Length)
                {
                    for (int k = addressLength + 1; k < spaceAddress.Length; k++)
                    {
                        address = address + " " + spaceAddress[k];
                        if (address.Length <= 45)
                        {
                            addressText = address;
                        }
                        else
                        {
                            address = spaceAddress[k - 1];
                            addressLength = k - 1;
                            break;
                        }
                        addressLength = spaceAddress.Length + 1;
                    }
                    if (string.IsNullOrEmpty(tb16.Text))
                    {
                        tb16.Text = addressText;
                    }
                    if (string.IsNullOrEmpty(tb17.Text))
                    {
                        tb17.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb18.Text))
                    {
                        tb18.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb19.Text))
                    {
                        tb19.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb20.Text))
                    {
                        tb20.Text = addressText;
                    }
                }

                if (addressLength + 1 < spaceAddress.Length)
                {
                    for (int n = addressLength + 1; n < spaceAddress.Length; n++)
                    {
                        address = address + " " + spaceAddress[n];
                        if (address.Length <= 45)
                        {
                            addressText = address;
                        }
                        else
                        {
                            address = spaceAddress[n - 1];
                            addressLength = n - 1;
                            break;
                        }
                        addressLength = spaceAddress.Length + 1;
                    }
                    if (string.IsNullOrEmpty(tb16.Text))
                    {
                        tb16.Text = addressText;
                    }
                    if (string.IsNullOrEmpty(tb17.Text))
                    {
                        tb17.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb18.Text))
                    {
                        tb18.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb19.Text))
                    {
                        tb19.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb20.Text))
                    {
                        tb20.Text = addressText;
                    }
                }

                if (addressLength + 1 < spaceAddress.Length)
                {
                    for (int m = addressLength + 1; m < spaceAddress.Length; m++)
                    {
                        address = address + " " + spaceAddress[m];
                        if (address.Length <= 45)
                        {
                            addressText = address;
                        }
                        else
                        {
                            address = spaceAddress[m - 1];
                            addressLength = m - 1;
                            break;
                        }
                        addressLength = spaceAddress.Length + 1;
                    }
                    if (string.IsNullOrEmpty(tb16.Text))
                    {
                        tb16.Text = addressText;
                    }
                    if (string.IsNullOrEmpty(tb17.Text))
                    {
                        tb17.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb18.Text))
                    {
                        tb18.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb19.Text))
                    {
                        tb19.Text = addressText;
                    }
                    else if (string.IsNullOrEmpty(tb20.Text))
                    {
                        tb20.Text = addressText;
                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string strDefaultPrinter = PrinterName;//获取打印机名
            print(strDefaultPrinter, 1);
        }
    }
}
