using Kdn.Library;
using Kdn.Library.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Printers;
using WinApp.Services.Controls;
using WinApp.Services;
using Yahv.Utils.Extends;
using Yahv.Utils.Http;
using Yahv.Utils.Kdn;
using Yahv.Utils.Serializers;
using System.Drawing;

namespace WinApp.Services
{
    //[GeckoClass]
    public class GeckoHelper
    {
        /// <summary>
        /// 模版打印
        /// </summary>
        /// <param name="parameter">模版打印参数</param>
        [GeckoFuntion]
        static public void TemplatePrint(TemplatePrintParameter parameter)
        {
            for (int index = 0; index < parameter.Data.Length; index++)
            {
                PrintHelper.Current.Template.Print(parameter.Data[index], parameter.Setting);
            }
        }

        /// <summary>
        /// 文件打印
        /// </summary>
        /// <param name="setting">配置参数</param>
        [GeckoFuntion]
        static public void FilePrint(PrinterConfig setting)
        {
            PrintHelper.Current.File.Print(setting);
        }

        /// <summary>
        /// 登录后存cookie数据
        /// </summary>
        /// <param name="data"></param>
        [GeckoFuntion]
        static public void Logon(string data)
        {
            Cookies.Cookie(data);
        }

        /// <summary>
        /// 是否需要配置
        /// </summary>
        /// <returns></returns>
        [GeckoFuntion]
        static public object NeedPrinterConfig()
        {
            return PrinterConfigs.Current.Any(item => !PrinterConnected(item.PrinterName));
        }

        /// <summary>
        /// 判断是否连接打印机
        /// </summary>
        /// <param name="printerName">打印机名称</param>
        static public bool PrinterConnected(string printerName)
        {
            return PrinterConfigs.Connected(printerName);
        }


        /// <summary>
        /// 返回当前打印配置
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 专用于配置列表
        /// </remarks>
        [GeckoFuntion]
        static public object GetPrinterConfig()
        {
            return PrinterConfigs.Current.ToArray();
        }

        /// <summary>
        /// 返回当前打印配置
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 专用于实际功能使用
        /// </remarks>
        [GeckoFuntion]
        static public object GetPrinterDictionary()
        {
            return PrinterConfigs.Current.ToDictionary(item => item.Name);
        }

        /// <summary>
        /// 保存当前打印配置
        /// </summary>
        /// <returns></returns>
        [GeckoFuntion]
        static public void SetPrinterConfig(PrinterConfig[] arry)
        {
            PrinterConfigs.Current.Save(arry);
        }

        /// <summary>
        /// 获取全部打印机名称
        /// </summary>
        /// <returns></returns>
        [GeckoFuntion]
        static public string[] GetAllPrinterNames()
        {
            //未实现
            // throw new Exception("6");
            string[] notIn = "发送至 OneNote 2010,Microsoft XPS Document Writer,Microsoft Print to PDF,Fax".Split(',');
            var view = PrinterSettings.InstalledPrinters.Cast<string>();
            return view.Where(item => !notIn.Contains(item)).ToArray();
        }

        /// <summary>
        /// 拍照
        /// </summary>
        /// <returns></returns>
        [GeckoFuntion]
        static public void FormPhoto(PhotoMap session)
        {
            Controls.PhotoPage.Current.Show();
            Controls.PhotoPage.Current.SetUploadParams(session);
        }

        /// <summary>
        /// 拍照（复数）
        /// </summary>
        /// <param name="session"></param>
        //[GeckoFuntion]
        static public void FormPhotos(PhotoMaps session)
        {
            Controls.PhotoPages.Current.Show();
            Controls.PhotoPages.Current.SetUploadParams(session);
        }

        /// <summary>
        /// 文件处理（图片显示，doc文档/pdf文档下载）
        /// </summary>
        /// <param name="data"></param>
        [GeckoFuntion]
        static public void FilesProcess(PictureUrl data)
        {
            //Controls.PictureShow.url = data.Url;
            //Controls.PictureShow.Current.Show();
            FileProcess.Current.Process(data);
        }

        /// <summary>
        /// 选择文件并上传
        /// </summary>
        /// <remarks>
        /// 文件上传
        /// </remarks>
        [GeckoFuntion]
        static public void SeletUploadFile(PhotoMap map)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = Extensions.Current.GetOpenFileDialogFilter();

            //var data = map.Data;
            //Console.WriteLine(data.GetType());


            //fileDialog.Filter = "(*.txt)|*.txt|所有文件(*.*)|*.*";

            //fileDialog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            //fileDialog.Filter = "Image Files|*.BMP;*.JPG;*.GIF";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //string[] files = fileDialog.FileNames;
                string file = fileDialog.FileName;
                var uploader = new GeckoUploader(map, file);
                uploader.Upload();
            }
        }

        /// <summary>
        /// 选择文件并上传（复数）
        /// </summary>
        /// <remarks>
        /// 文件上传（复数）
        /// </remarks>
        //[GeckoFuntion]
        static public void SeletUploadFiles(PhotoMaps maps)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = Extensions.Current.GetOpenFileDialogFilter();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                var uploader = new GeckoUploader(maps, file);
                uploader.Uploads();

                //string[] files = fileDialog.FileNames;
                //foreach (var file in files)
                //{
                //    var uploader = new GeckoUploader(maps, file);
                //    uploader.Uploads();
                //}
            }
        }


        /// <summary>
        /// 快递鸟运单的面单打印
        /// </summary>
        /// <param name="kp">快递鸟专有参数:KdnPrint</param>
        [GeckoFuntion]
        static public void PrintKdn(KdnPrint kp)
        {
            #region 测试
            //  Services.Kdn.KdnHelper.FacePrint(ShipperCode.SF, SfExpType.Default, kp.Sender = new Sender()
            //  {
            //      Company = "LV",
            //      Name = "Taylor",
            //      Mobile = "15018442396",
            //      ProvinceName = "上海",
            //      CityName = "上海",
            //      ExpAreaName = "青浦区",
            //      Address = "上海青浦区明珠路73号4层501",

            //  }, kp.Receiver = new Receiver()
            //  {
            //      Company = "GCCUI",
            //      Name = "Yann",
            //      Mobile = "15018442396",
            //      ProvinceName = "北京",
            //      CityName = "北京",
            //      ExpAreaName = "朝阳区",
            //      Address = "三里屯街道雅秀大厦",
            //  },
            //1, "小心轻放", 0.01, 0.01, kp.Commodity);
            #endregion

            SimHelper.PrintStatus = $"正在打印:{kp.ShipperCode}的面单";

            KdnHelper.FacePrint(kp.ShipperCode, kp.ExpType, kp.ExPayType,
                kp.Sender, kp.Receiver, kp.Quantity, kp.Remark, kp.Volume, kp.Weight,
                kp.Commodity);

            SimHelper.PrintStatus = $"{kp.ShipperCode}的面单打印完成";
        }

        /// <summary>
        /// 重新打印指定运单号的面单
        /// </summary>
        /// <param name="code">运单号</param>
        [GeckoFuntion]
        static public void ReprintKdnFaceSheet(JObject data)
        {
            var code = data["code"].Value<string>();
            SimHelper.PrintStatus = $"正在重新打印运单号:{code}的面单";

            string url = $"{Config.ApiUrlPrex}/Kdn/GetFaceSheetHtml";
            FaceSheetResult result = ApiHelper.Current.Get<FaceSheetResult>(url, new
            {
                logisticCode = code
            });

            if (result == null)
            {
                MessageBox.Show($"运单号{code}不是系统生成，因此无法完成面单的重新打印！");
            }

            PrintKdnForm form = new PrintKdnForm();

            //需要约定打印机配置名称

            string printerName = KdnHelper.GetPrinterName(result.ShipperCode);
            form.PrinterName = printerName;
            form.Show();

            string correct = KdnHelper.GetCorrect(result.ShipperCode);
            form.Html(result.Html, correct);

            SimHelper.PrintStatus = $"运单号:{code}的面单打印完成";

        }

        /// <summary>
        /// 打印国际快递  20210629 by yeshuangshuang
        /// </summary>
        /// <param name="kp">组织一个国际快递打印类</param>
        [GeckoFuntion]
        static public void PrintNationalWaybill(InternationalAirWaybillPrint IAW)
        {
            string printerName = PrinterConfigs.Current[PrinterConfigs.国际快递针式].PrinterName;
            if (!PrinterConfigs.Connected(printerName))
            {
                MessageBox.Show($"请配置{nameof(PrinterConfigs.国际快递针式)}机!");
                return;
            }
            if (IAW.expType == "UPS")
            {
                UPSForm upsform = new UPSForm
                {
                    PrinterName = printerName
                };
                upsform.DataPrint(IAW);
                upsform.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
                upsform.Location = (Point)new Size(900, 450); //窗体的起始位置为0,0 
                upsform.ShowDialog();
            }
            else if (IAW.expType == "TNT")
            {
                TNTForm tntform = new TNTForm
                {
                    PrinterName = printerName
                };
                tntform.DataPrint(IAW);
                tntform.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
                tntform.Location = (Point)new Size(770, 335); //窗体的起始位置为0,0 
                tntform.ShowDialog();
            }
        }


        /// <summary>
        /// 出库通知打印（香港/深圳） 
        /// </summary>
        /// <param name="data">数据</param>
        [GeckoFuntion]
        static public void PrintOuptNotice(JObject data)
        {
            string printerName = PrinterConfigs.Current[PrinterConfigs.出库通知打印].PrinterName;

            if (!PrinterConfigs.Connected(printerName))
            {
                MessageBox.Show($"请配置{nameof(PrinterConfigs.出库通知打印)}机!");
                return;
            }

            PrintOutputForm form = new PrintOutputForm
            {
                PrinterName = printerName
            };
            form.Print(data);
        }

        /// <summary>
        /// 深圳出库通知打印（未使用）
        /// </summary>
        /// <param name="data"></param>
        //[GeckoFuntion]
        static public void PrintSZOuptNotice(JObject data)
        {
            string printerName = PrinterConfigs.Current[PrinterConfigs.出库通知打印].PrinterName;

            if (!PrinterConfigs.Connected(printerName))
            {
                MessageBox.Show($"请配置{nameof(PrinterConfigs.出库通知打印)}机!");
                return;
            }

            PrintSZOutputForm form = new PrintSZOutputForm
            {
                PrinterName = printerName
            };
            form.Print(data);
        }

        /// <summary>
        /// 送货单首页统计打印(深圳内单)
        /// </summary>
        /// <param name="data">数据</param>
        [GeckoFuntion]
        static public void PrintDeliveryHomeList(JObject data)
        {
            string printerName = PrinterConfigs.Current[PrinterConfigs.送货单打印].PrinterName;

            if (!PrinterConfigs.Connected(printerName))
            {
                MessageBox.Show($"请配置{nameof(PrinterConfigs.送货单打印)}机!");
                return;
            }

            PrintDeliveryHomeListForm form = new PrintDeliveryHomeListForm
            {
                PrinterName = printerName,
                Numcopies = data["Numcopies"].Value<int?>() ?? 1
            };

            form.Show();
            form.Print(data);
        }


        /// <summary>
        /// 送货单内容打印（香港/深圳除内单外的打印）
        /// </summary>
        /// <param name="data">数据</param>
        [GeckoFuntion]
        static public void PrintDeliveryList(JObject data)
        {
            string printerName = PrinterConfigs.Current[PrinterConfigs.送货单打印].PrinterName;

            if (!PrinterConfigs.Connected(printerName))
            {
                MessageBox.Show($"请配置{nameof(PrinterConfigs.送货单打印)}机!");
                return;
            }

            PrintDeliveryListForm form = new PrintDeliveryListForm
            {
                PrinterName = printerName,
                Numcopies = data["Numcopies"].Value<int?>() ?? 1
            };

            var language = (Languages)Enum.Parse(typeof(Languages), data["Language"].Value<string>(), true);

            form.Show();
            form.Print(data, language);
        }

        /// <summary>
        /// 深圳内单送货单内容打印
        /// </summary>
        /// <param name="data">数据</param>
        [GeckoFuntion]
        static public void PrintSZDeliveryList(JObject data)
        {
            string printerName = PrinterConfigs.Current[PrinterConfigs.送货单打印].PrinterName;

            if (!PrinterConfigs.Connected(printerName))
            {
                MessageBox.Show($"请配置{nameof(PrinterConfigs.送货单打印)}机!");
                return;
            }

            PrintSZDeliveryListForm form = new PrintSZDeliveryListForm
            {
                PrinterName = printerName,
                Numcopies = data["Numcopies"].Value<int?>() ?? 1
            };

            form.Show();
            form.Print(data);
        }

        /// <summary>
        /// 入库单打印（目前只有深圳有）
        /// </summary>
        /// <param name="data">数据</param>
        [GeckoFuntion]
        static public void PrintInputList(JObject data)
        {
            string printerName = PrinterConfigs.Current[PrinterConfigs.入库单打印].PrinterName; ;

            if (!PrinterConfigs.Connected(printerName))
            {
                MessageBox.Show($"请配置{nameof(PrinterConfigs.入库单打印)}机!");
                return;
            }

            PrintInputForm form = new PrintInputForm
            {
                PrinterName = printerName,
                //Numcopies = data["Numcopies"].Value<int?>()??1//打印份数
            };

            form.Show();
            form.Print(data);
        }

        /// <summary>
        /// 顺丰打印（测试）
        /// </summary>
        /// <param name="data"></param>
        [GeckoFuntion]
        static public void PrintSF(JObject data)
        {
            // PrinterHelper.FacePrint("Order0051", ShipperCode.SF, SfExpType.顺丰标快, 1, new Kdn.Library.Models.Sender()
            // {
            //     Company = "LV",
            //     Name = "Taylor",
            //     Mobile = "15018442396",
            //     ProvinceName = "上海",
            //     CityName = "上海",
            //     ExpAreaName = "青浦区",
            //     Address = "上海青浦区明珠路73号4层501",

            // }, new Kdn.Library.Models.Receiver()
            // {
            //     Company = "GCCUI",
            //     Name = "Yann",
            //     Mobile = "15018442396",
            //     ProvinceName = "北京",
            //     CityName = "北京",
            //     ExpAreaName = "海淀区",
            //     Address = "北京海淀区彩和坊路立方庭大厦5楼502",
            // },
            //1, "小心轻放", 0.01, 0.01, new Kdn.Library.Models.Commodity
            //{
            //    GoodsName = "客户器件",
            //    GoodsWeight = 1.1//(单位是千克)
            //});


            //SFWaybillPrinter.WayBillPrinterTools(data);
        }

        /// <summary>
        /// 打印顺丰/跨越/EMS，对接顺丰/跨越/EMS接口
        /// </summary>
        /// <param name="data"></param>
        //[GeckoFuntion]
        //static public void PrintFaceSheet(NewPrint print)
        //{
        //    //1.先请求顺丰/跨越下单接口
        //    //2.请求打印接口
        //    SimHelper.PrintStatus = $"正在打印:{print.ShipperCode}的面单";

        //    PrinterHelper.FacePrint(print.OrderID, print.ShipperCode, print.ExpType, print.ExPayType,
        //        print.Sender, print.Receiver, print.Quantity, print.Remark, print.Volume, print.Weight,
        //        print.Commodity);

        //    SimHelper.PrintStatus = $"{print.ShipperCode}的面单打印完成";
        //}


        [GeckoFuntion]
        static public void PrintFaceSheet(FacePrint print)
        {
            //1.先请求顺丰/跨越下单接口
            //2.请求打印接口
            SimHelper.PrintStatus = $"正在打印:{print.ShipperCode}的面单";

            PrinterHelper.FacePrint(print.OrderID, print.ShipperCode, print.ExpType, print.ExPayType,
                print.Sender, print.Receiver, print.Quantity, print.Remark, print.Volume, print.Weight, print.MonthlyCard, print.IsSignBack,/*false,*/
                print.Commodity);

            SimHelper.PrintStatus = $"{print.ShipperCode}的面单打印完成";
        }

        /// <summary>
        /// 第二次以上打印顺丰/跨越/EMS，对接顺丰/跨越/EMS接口
        /// </summary>
        /// <param name="data"></param>
        [GeckoFuntion]
        static public void ReprintFaceSheet(JObject data)
        {

            //MessageBox.Show("测试弹出1," + data.ToString());

            //1.先请求顺丰/跨越下单接口
            //2.请求打印接口
            var code = data["code"].Value<string>();
            //SimHelper.PrintStatus = $"正在重新打印运单号:{code}的面单";

            if (string.IsNullOrWhiteSpace(code))
            {
                return;
            }


            string url = $"{Config.ApiUrlPrex}/Printer/GetFaceSheet";

            //MessageBox.Show($"{url}?code={code}");

            FaceOrderResult result = ApiHelper.Current.Get<FaceOrderResult>(url, new
            {
                code = code
            });


            //MessageBox.Show("测试弹出3," + data.ToString());

            //MessageBox.Show(result.Json());

            if (result == null)
            {
                MessageBox.Show($"运单号{code}不是系统生成，因此无法完成面单的重新打印！");
                return;
            }

            //判断是顺丰还是跨越打印

            //顺丰处理
            if (result.Source == (int)Yahv.PsWms.PvRoute.Services.PrintSource.SF)
            {
                string printerName = PrinterHelper.GetPrinterName(nameof(Yahv.PsWms.PvRoute.Services.PrintSource.SF));
                SFPrint form = new SFPrint
                {
                    PrinterName = printerName
                };
                form.Print(null, result.Html);
            }
            //跨越处理
            else if (result.Source == (int)Yahv.PsWms.PvRoute.Services.PrintSource.KY)
            {
                string printerName = PrinterHelper.GetPrinterName(nameof(Yahv.PsWms.PvRoute.Services.PrintSource.KY));
                KYWaybillPrinter printer = new KYWaybillPrinter();

                //打印
                printer.WayBillPrinterTools(result.SendJson, printerName, result.MainID);
            }
            //EMS处理
            else if (result.Source == (int)Yahv.PsWms.PvRoute.Services.PrintSource.EMS)
            {
                //string printerName = PrinterHelper.GetPrinterName(nameof(Yahv.PsWms.PvRoute.Services.PrintSource.EMS));
                EMSWaybillPrinter printer = new EMSWaybillPrinter();
                var setting = PrinterConfigs.Current["EMS打印"];

                var settingUrl = Config.SchemeName + "://" + Config.DomainName + setting.Url;
                //打印
                printer.Print(result.SendJson.JsonTo(), new PrinterConfig { 
                    Url = settingUrl,
                    Name = setting.Name,
                    Summary = setting.Summary,
                    PrinterName = setting.PrinterName
                });
            }
            //
            else
            {
                MessageBox.Show($"打印接口目前支持：{string.Join(",", new ShipperCode())}，请重试!");
                return;
            }
        }
    }
}


