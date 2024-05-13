using Kdn.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Services;
using WinApp.Services.Controls;
using Yahv.Utils.Kdn;

namespace WinApp
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void btnPhoto_Click(object sender, EventArgs e)
        {
            GeckoHelper.FormPhoto(new PhotoMap
            {
                AdminID = "1234",
                SessionID = "asdfasdf",
                Data=new
                {
                    InputID="001",
                    NoticeID="N001",
                    WaybillID="W001",
                    WsOrderID="Order001",
                    LsOrderID = "Ls0001"
                }
            });
            Services.Controls.PhotoPage.Current.Show();
        }

        private void 测试上传_Click(object sender, EventArgs e)
        {
            new Yahv.Services.Views.MyClass_456789();

            //using (WebClient client = new WebClient())
            //{
            //    client.UploadFile("http://uuws.b1b.com/Api/Upload", "POST", @"D:\Projects_vs2015\Yahv\Yahv.PfWms\WinformApp\Html\QQ图片20191022114443.jpg");
            //}
        }

        private void btnSelector_Click(object sender, EventArgs e)
        {
            GeckoHelper.SeletUploadFile(new PhotoMap {
                AdminID = "1234",
                SessionID = "asdfasdf",
                Data = new
                {
                    InputID = "001",
                    NoticeID = "N001",
                    WaybillID = "W001",
                    WsOrderID = "Order001",
                    LsOrderID="Ls0001"
                }
            });
        }

        private void btnKdn_Click(object sender, EventArgs e)
        {
            //KdnAddress senderAddress;
            //KdnAddress receiverAddress;

            //if (!"senderAddress".TryAddress(out senderAddress))
            //{
            //    MessageBox.Show("地址错误！");
            //}

            //if (!"receiverAddress".TryAddress(out receiverAddress))
            //{
            //    MessageBox.Show("地址错误！");
            //}


            //PrintKdnForm form = new PrintKdnForm();
            ////form.PrinterName = "ZDesigner GK888t (EPL)";
            //form.PrinterName = "QR-668 LABEL";
            //form.Show();
            //form.Html(WinApp.Services.Properties.Resource.TestKdn);
 
            //不可行：+%&<>
            //  可行：!$^ ()_ -.=#\/@*[]{}?':|;,~
            Services.Kdn.KdnHelper.FacePrint(ShipperCode.SF, SfExpType.顺丰微小件, new Kdn.Library.Models.Sender()
            {
                Company = "LV",
                Name = "Taylor",
                Mobile = "15018442396",
                ProvinceName = "上海",
                CityName = "上海",
                ExpAreaName = "青浦区",
                Address = "上海青浦区明珠路73号4层501",

            }, new Kdn.Library.Models.Receiver()
            {
                Company = "GCCUI",
                Name = "Yann",
                Mobile = "15018442396",
                ProvinceName = "北京",
                CityName = "北京",
                ExpAreaName = "海淀区",
                Address = "北京海淀区三环到四环之间 +%&<>海淀区●彩和坊路1+1大厦5楼502",
            },
            1, "小心轻放", 0.01, 0.01, new Kdn.Library.Models.Commodity
            {
                GoodsName = "客户器件"
            });
        }

        private void btnOuputNotice_Click(object sender, EventArgs e)
        {
            string printerName = "ZDesigner GK888t (副本 1)";

            PrintOutputForm form = new PrintOutputForm
            {
                PrinterName = printerName
            };

            form.Height = 100;

            if (!PrinterConfigs.Connected(printerName))
            {
                MessageBox.Show($"请配置快递鸟打印机!");
                return;
            }

            form.Show();

            form.Print("");

        }

        private void btnKdnKysy_Click(object sender, EventArgs e)
        {
            Services.Kdn.KdnHelper.FacePrint(ShipperCode.KYSY, KysyExpType.隔日达, new Kdn.Library.Models.Sender()
            {
                Company = "LV",
                Name = "Taylor",
                Mobile = "15018442396",
                ProvinceName = "上海",
                CityName = "上海",
                ExpAreaName = "青浦区",
                Address = "上海青浦区明珠路73号4层501",
            }, new Kdn.Library.Models.Receiver()
            {
                Company = "GCCUI",
                Name = "Yann",
                Mobile = "15018442396",
                ProvinceName = "北京",
                CityName = "北京",
                ExpAreaName = "朝阳区",
                Address = "三里屯街道雅秀大厦",
            },
            1, "小心轻放", 0.01, 0.01, new Kdn.Library.Models.Commodity
            {
                GoodsName = "客户器件"
            });

            //PrintKdnForm form = new PrintKdnForm();
            //form.PrinterName = "QR-668 LABEL (快递鸟跨越速递)";
            //form.Show();
            //string html = System.IO.File.ReadAllText(@"D:\Projects_vs2015\Yahv\Yahv.PfWms\WinApp.Services\Html\Test\TestKdn.kysy.html", Encoding.UTF8);
            //form.Html(html, WinApp.Services.Properties.Resource.correctKdnKysy);
        }

        private void btnDeliveryList_Click(object sender, EventArgs e)
        {
            GeckoHelper.PrintDeliveryList(null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GeckoHelper.PictureShow(new PictureUrl
            {
               Url= "http://uuws.b1b.com/2019/12/11/F15760643678506183.jpeg"
            });
        }
    }
}
