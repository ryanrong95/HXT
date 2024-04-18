using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yahv.PsWms.DappForm.Services;
using Yahv.PsWms.PvRoute.Services;

namespace Yahv.PsWms.DappForm
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPhoto_Click(object sender, EventArgs e)
        {
            try
            {

                //GeckoHelper.FormPhoto(new PhotoMap
                //{
                //    AdminID = "1234",
                //    Data = new PhotoMap.MyData
                //    {
                //        MainID = "Order001",
                //        Type = 1
                //        //LsOrderID = "Ls0001"
                //    }
                //});

                ////自己插入tablelayoutPanel第三行测试
                //GeckoHelper.FormPhoto1(new PhotoMap
                //{
                //    AdminID = "1234",
                //    Data = new PhotoMap.MyData
                //    {
                //        MainID = "Order001",
                //        Type = 1
                //        //LsOrderID = "Ls0001"
                //    }
                //});

                //tablelayoutPanel第三行显示和隐藏测试
                GeckoHelper.FormPhoto2(new PhotoMap
                {
                    AdminID = "1234",
                    Data = new PhotoMap.MyData
                    {
                        MainID = "Order001",
                        Type = 1
                        //LsOrderID = "Ls0001"
                    }
                });

            }
            catch (Exception ex)
            {

                MessageBox.Show((ex.InnerException ?? ex).Message);
            }


            //Services.Controls.PhotoPages.Current.Show();
        }

        /// <summary>
        /// 传照
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpload_Click(object sender, EventArgs e)
        {
            GeckoHelper.SeletUploadFiles(new PhotoMap
            {
                AdminID = "1234",
                Data = new PhotoMap.MyData
                {
                    MainID = "Order001",
                    Type = 1
                    //LsOrderID = "Ls0001"
                }
            });
        }

        /// <summary>
        /// 顺丰/跨越打印测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //对接顺丰接口测试
            PrinterHelper.FacePrint("KYOrder0027", ShipperCode.KY, KysyExpType.同城即日, 1, new PvRoute.Services.Models.Sender()
            {
                Company = "LV",
                Contact = "Taylor",
                Mobile = "15018442396",
                Province = "上海",
                City = "上海",
                Region = "青浦区",
                Address = "上海青浦区明珠路73号4层501",

            }, new PvRoute.Services.Models.Receiver()
            {
                Company = "GCCUI",
                Contact = "Yann",
                Mobile = "15018442396",
                Province = "广东",
                City = "深圳",
                Region = "宝安区",
                Address = "广东省深圳市宝安区西乡大道288号",
            },
            1, "小心轻放", 0.01, 0.01, "",true,new PvRoute.Services.Models.Commodity
            {
                GoodsName = "客户器件"
            });
        }
    }
}
