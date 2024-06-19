using Kdn.Library;
using Newtonsoft.Json.Linq;
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
using WinApp.Services.Print;
using Yahv.PsWms.PvRoute.Services.Models;
using Yahv.Utils.Kdn;

namespace WinApp
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
            //var statusBar = new Controls.UcStatusBar();
            //SimHelper.Initialize(statusBar);
        }

        /// <summary>
        /// 拍照上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPhoto_Click(object sender, EventArgs e)
        {
            ///单数 上传
            //GeckoHelper.FormPhoto(new PhotoMap
            //{
            //    AdminID = "1234",
            //    SessionID = "asdfasdf",
            //    Data = new PhotoMap.MyData
            //    {
            //        InputID = "001",
            //        NoticeID = "N001",
            //        WaybillID = "W001",
            //        WsOrderID = "Order001",
            //        //LsOrderID = "Ls0001"
            //    }
            //});
            //Services.Controls.PhotoPage.Current.Show();


            ///复数 上传
            //List<PhotoMap> list = new List<PhotoMap>();
            //list.Add(new PhotoMap
            //{
            //    AdminID = "1234",
            //    SessionID = "asdfasdf",
            //    Data = new PhotoMap.MyData
            //    {
            //        InputID = "001",
            //        NoticeID = "N001",
            //        WaybillID = "W001",
            //        WsOrderID = "Order001",
            //        //LsOrderID = "Ls0001"
            //    }
            //});
            //list.Add(new PhotoMap
            //{
            //    AdminID = "1234",
            //    SessionID = "asdfasdf",
            //    Data = new PhotoMap.MyData
            //    {
            //        InputID = "002",
            //        NoticeID = "N002",
            //        WaybillID = "W002",
            //        WsOrderID = "Order002",
            //        //LsOrderID = "Ls0001"
            //    }
            //});

            //GeckoHelper.FormPhotos(new PhotoMaps
            //{
            //    PhotoMapes = list.ToArray()
            //});

            Services.Controls.PhotoPages.Current.Show();
        }

        private void 测试上传_Click(object sender, EventArgs e)
        {
            //new Yahv.Services.Views.MyClass_456789();

            //using (WebClient client = new WebClient())
            //{
            //    client.UploadFile("http://uuws.b1b.com/Api/Upload", "POST", @"D:\Projects_vs2015\Yahv\Yahv.PfWms\WinformApp\Html\QQ图片20191022114443.jpg");
            //}
        }


        /// <summary>
        /// 文件选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelector_Click(object sender, EventArgs e)
        {
            ///单数 上传
            //GeckoHelper.SeletUploadFile(new PhotoMap
            //{
            //    AdminID = "1234",
            //    SessionID = "asdfasdf",
            //    Data = new PhotoMap.MyData
            //    {
            //        //InputID = "001",
            //        //NoticeID = "N001",
            //        //WaybillID = "W001",
            //        //WsOrderID = "Order001",
            //        PayID = "Pay0001"
            //    }
            //});

            ///复数 上传
            //List<PhotoMap> list = new List<PhotoMap>();
            //list.Add(new PhotoMap
            //{
            //    AdminID = "1234",
            //    SessionID = "asdfasdf",
            //    Data = new PhotoMap.MyData
            //    {
            //        InputID = "001",
            //        NoticeID = "N001",
            //        WaybillID = "W001",
            //        WsOrderID = "Order001",
            //        //LsOrderID = "Ls0001"
            //    }
            //});
            //list.Add(new PhotoMap
            //{
            //    AdminID = "1234",
            //    SessionID = "asdfasdf",
            //    Data = new PhotoMap.MyData
            //    {
            //        InputID = "002",
            //        NoticeID = "N002",
            //        WaybillID = "W002",
            //        WsOrderID = "Order002",
            //        //LsOrderID = "Ls0001"
            //    }
            //});

            //GeckoHelper.SeletUploadFiles(new PhotoMaps
            //{
            //    PhotoMapes = list.ToArray()
            //});
        }

        /// <summary>
        /// 快递鸟顺丰打印测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnKdn_Click(object sender, EventArgs e)
        {
            //PrinterHelper.FacePrint("SFOrder0001", "SF", SfExpType.顺丰标快, 1, new Yahv.PsWms.PvRoute.Services.Models.Sender()
            //{
            //    Company = "LV",

            //    Contact = "Taylor",
            //    Mobile = "15018442396",
            //    Province = "上海",
            //    City = "上海",
            //    Region = "青浦区",
            //    Address = "上海青浦区明珠路73号4层501",

            //}, new Yahv.PsWms.PvRoute.Services.Models.Receiver()
            //{
            //    Company = "GCCUI",
            //    Contact = "Yann",
            //    Mobile = "15018442396",
            //    Province = "北京",
            //    City = "北京",
            //    Region = "海淀区",
            //    Address = "北京海淀区彩和坊路立方庭大厦5楼502",
            //},
            //1, "小心轻放", 0.01, 0.01, "", false, new Yahv.PsWms.PvRoute.Services.Models.Commodity
            //{
            //    GoodsName = "客户器件"
            //});


            //PrintKdnForm form = new PrintKdnForm();

            ////需要约定打印机配置名称

            //string printerName = Services.KdnHelper.GetPrinterName(ShipperCode.SF);
            //form.PrinterName = printerName;
            //form.Show();
            ////D:\Projects_vs2015\Yahv\Yahv.PfWms\WinformApp\Html\HTMLPage2.html
            ////D:\Vs2015_Projects\Yahv\Yahv.PfWms\WinformApp\Html\HTMLPage2.html 乔霞本地测试地址
            //string html = System.IO.File.ReadAllText(@"D:\Projects_vs2015\Yahv\Yahv.PfWms\WinformApp\Html\HTMLPage2.html", Encoding.UTF8);
            //string correct = Services.KdnHelper.GetCorrect(ShipperCode.SF);
            //form.Html(html, correct);



            //SimHelper.PrintStatus = $"运单号:{code}的面单打印完成";

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


            ////不可行：+%&<>
            ////  可行：!$^ ()_ -.=#\/@*[]{}?':|;,~
            Services.KdnHelper.FacePrint(ShipperCode.SF, SfExpType.顺丰微小件,1, new Kdn.Library.Models.Sender()
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

        /// <summary>
        /// 快递鸟跨越速运打印测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnKdnKysy_Click(object sender, EventArgs e)
        {

            ////调用跨越接口进行打印
            //PrinterHelper.FacePrint("KYOrder0001", ShipperCode.KYSY, KysyExpType.隔日达, 10, new Kdn.Library.Models.Sender()
            //{
            //    Company = "LV",
            //    Name = "Taylor",
            //    Mobile = "15018442396",
            //    ProvinceName = "上海",
            //    CityName = "上海",
            //    ExpAreaName = "青浦区",
            //    Address = "明珠路73号4层501",
            //}, new Kdn.Library.Models.Receiver()
            //{
            //    Company = "GCCUI",
            //    Name = "Yann",
            //    Mobile = "15018442396",
            //    ProvinceName = "江西",
            //    CityName = "吉安",
            //    ExpAreaName = "永新县",
            //    Address = "埠前镇小屋岭鸿兴花园",
            //},
            //1, "小心轻放", 0.01, 1.1, new Kdn.Library.Models.Commodity
            //{
            //    GoodsName = "客户器件",

            //});


            //快递鸟接口进行打印
            //Services.KdnHelper.FacePrint("跨越速运", KysyExpType.隔日达,2, new Kdn.Library.Models.Sender()
            //{
            //    Company = "LV",
            //    Name = "Taylor",
            //    Mobile = "15018442396",
            //    ProvinceName = "上海",
            //    CityName = "上海",
            //    ExpAreaName = "青浦区",
            //    Address = "上海青浦区明珠路73号4层501",
            //}, new Kdn.Library.Models.Receiver()
            //{
            //    Company = "GCCUI",
            //    Name = "Yann",
            //    Mobile = "15018442396",
            //    ProvinceName = "北京",
            //    CityName = "北京",
            //    ExpAreaName = "朝阳区",
            //    Address = "三里屯街道雅秀大厦",
            //},
            //1, "小心轻放", 0.01, 0.01, new Kdn.Library.Models.Commodity
            //{
            //    GoodsName = "客户器件"
            //});

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
            GeckoHelper.FilesProcess(new PictureUrl
            {
                Url = "http://uuws.b1b.com/2019/12/11/F15760643678506183.jpeg"
            });
        }

        /// <summary>
        /// 打印PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            GeckoHelper.FilePrint(new PrinterConfig
            {
                PrinterName = "QR-668 LABEL",
                Url = @"http://out.bj.51db.com:27770/scm/download/file/45433212730"  // http://out.bj.51db.com:27770/scm/download/file/45433212730   //D:\protocol.pdf
            });
        }

        /// <summary>
        /// 深圳送货单内容打印（放弃）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {


            //string printerName = PrinterConfigs.Current[PrinterConfigs.送货单打印].PrinterName;
            //string html = System.IO.File.ReadAllText(@"D:\Projects_vs2015\Yahv\Yahv.PfWms\WinformApp\Html\大数据送货单打印.html", Encoding.UTF8);
            //PrintSZDeliveryListForm form = new PrintSZDeliveryListForm
            //{
            //    PrinterName = printerName,
            //    Numcopies = 1
            //};
            //form.Show();
            //form.Print(html);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //SFWaybillPrinter.WayBillPrinterTools(null);
            JObject obj = new JObject();
            //obj["code"] = "80013137949";
            //obj["code"] = "SF7444420841017"; //以前的顺丰单子，抛异常
            obj["code"] = "SF7444468381774"; 
            //obj["code"] = "SF1604488760697"; //线上的顺丰单子
            GeckoHelper.ReprintFaceSheet(obj);
        }

        /// <summary>
        /// SF/跨越/EMS测试打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            PrinterHelper.FacePrint("20230719order0002","SF",1,1, new Yahv.PsWms.PvRoute.Services.Models.Sender()
            {
                Company = "上海(LV)旗舰店",
                Contact = "CCCCCC",
                Mobile = "15018442396",
                Province = "上海",
                City = "上海",
                Region = "青浦区",
                Address = "上海青浦区明珠路73号4层501",
            }, new Yahv.PsWms.PvRoute.Services.Models.Receiver()
            {
                Company = "北京(GCCUI)品牌店",
                Contact = "DDDDDD",
                Mobile = "15018442396",
                Province = "北京",
                City = "北京",
                Region = "朝阳区",
                Address = "三里屯街道雅秀大厦",
            },
            1, "小心轻放", 0.01, 0.01,"",true,new Yahv.PsWms.PvRoute.Services.Models.Commodity
            {
                GoodsName = "客户器件"
            });


            var ss = new EmsApiHelper().EmsXmlGenerate();

            MessageBox.Show(ss);
        }

       
    }
}
