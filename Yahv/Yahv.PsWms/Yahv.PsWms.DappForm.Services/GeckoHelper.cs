using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yahv.PsWms.DappForm.Services.Controls;
using Yahv.PsWms.DappForm.Services.Print;
using Yahv.PsWms.DappForm.Services.Printers;
using Yahv.PsWms.PvRoute.Services;
using Yahv.Utils.Http;

namespace Yahv.PsWms.DappForm.Services
{
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
        /// 拍照（复数）
        /// </summary>
        /// <param name="session"></param>
        [GeckoFuntion]
        static public void FormPhoto(PhotoMap session)
        {
            Controls.PhotoPage.Current.Show();
            Controls.PhotoPage.Current.SetUploadParams(session);
        }

        static public void FormPhoto1(PhotoMap session)
        {
            Controls.PhotoPage1.Current.Show();
            Controls.PhotoPage1.Current.SetUploadParams(session);
        }

        static public void FormPhoto2(PhotoMap session)
        {
            Controls.PhotoPage2.Current.Show();
            Controls.PhotoPage2.Current.SetUploadParams(session);
        }


        /// <summary>
        /// 选择文件并上传（复数）
        /// </summary>
        /// <remarks>
        /// 文件上传（复数）
        /// </remarks>
        [GeckoFuntion]
        static public void SeletUploadFiles(PhotoMap map)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = Extensions.Current.GetOpenFileDialogFilter();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var fileNames = fileDialog.FileNames;
                var uploader = new GeckoUploader(map, fileNames);
                uploader.Upload();

                //string file = fileDialog.FileName;
                //var uploader = new GeckoUploader(maps, file);
                //uploader.Uploads();

                //string[] files = fileDialog.FileNames;
                //foreach (var file in files)
                //{
                //    var uploader = new GeckoUploader(maps, file);
                //    uploader.Uploads();
                //}
            }
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
            FileProcess.Current.Process(data.Url);
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
        /// 打印顺丰/跨越，对接顺丰/跨越接口
        /// </summary>
        /// <param name="data"></param>
        [GeckoFuntion]
        static public void PrintFaceSheet(FacePrint print)
        {
            //1.先请求顺丰/跨越下单接口
            //2.请求打印接口
            SimHelper.PrintStatus = $"正在打印:{print.ShipperCode}的面单";

            PrinterHelper.FacePrint(print.OrderID, print.ShipperCode, print.ExpType, print.ExPayType,
                print.Sender, print.Receiver, print.Quantity, print.Remark, print.Volume, print.Weight,print.MonthlyCard, print.IsSignBack,/*false,*/
                print.Commodity);

            SimHelper.PrintStatus = $"{print.ShipperCode}的面单打印完成";
        }

        /// <summary>
        /// 第二次以上打印顺丰/跨越，对接顺丰/跨越接口
        /// </summary>
        /// <param name="data"></param>
        [GeckoFuntion]
        static public void ReprintFaceSheet(JObject data)
        {
            //1.先请求顺丰/跨越下单接口
            //2.请求打印接口
            var code = data["Code"].Value<string>();

            if (string.IsNullOrWhiteSpace(code))
            {
                return;
            }

            //SimHelper.PrintStatus = $"正在重新打印运单号:{code}的面单";

            string url = $"{Config.ApiUrlPrex}/Printer/GetFaceSheet";
            FaceOrderResult result = ApiHelper.Current.Get<FaceOrderResult>(url, new
            {
                code = code
            });


            if (result == null)
            {
                MessageBox.Show($"运单号{code}不是系统生成，因此无法完成面单的重新打印！");
            }

            //判断是顺丰还是跨越打印

            //顺丰处理
            if (result.Source == (int)PvRoute.Services.PrintSource.SF)
            {
                string printerName = PrinterHelper.GetPrinterName(nameof(PvRoute.Services.PrintSource.SF));
                SFPrint form = new SFPrint
                {
                    PrinterName = printerName
                };
                form.Print(null, result.Html);
            }
            //跨越处理
            else if (result.Source == (int)PvRoute.Services.PrintSource.KY)
            {
                string printerName = PrinterHelper.GetPrinterName(nameof(PvRoute.Services.PrintSource.KY));
                KYWaybillPrinter printer = new KYWaybillPrinter();

                //打印
                printer.WayBillPrinterTools(result.SendJson, printerName,result.MainID);
            }
            //
            else
            {
                MessageBox.Show($"打印接口目前支持：{string.Join(",", new ShipperCode())}，请重试!");
                return;
            }

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

            //var language = (Languages)Enum.Parse(typeof(Languages), data["Language"].Value<string>(), true);

            form.Show();
            form.Print(data);
        }

        /// <summary>
        /// 打印预出库单
        /// </summary>
        /// <param name="data"></param>
        [GeckoFuntion]
        static public void PrintPreDeliveryLabel(JObject data)
        {
            string printerName = PrinterConfigs.Current[PrinterConfigs.预出库单打印].PrinterName;

            if (!PrinterConfigs.Connected(printerName))
            {
                MessageBox.Show($"请配置{nameof(PrinterConfigs.预出库单打印)}机!");
                return;
            }

            PrintPreDelivery form = new PrintPreDelivery
            {
                PrinterName = printerName,
            };

            //var language = (Languages)Enum.Parse(typeof(Languages), data["Language"].Value<string>(), true);

            form.Show();
            form.Print(data);
        }

        ///// <summary>
        ///// 出库标签打印
        ///// </summary>
        ///// <param name="data">数据</param>
        //[GeckoFuntion]
        //static public void PrintOuptNotice(JObject data)
        //{
        //    string printerName = PrinterConfigs.Current[PrinterConfigs.出库标签打印].PrinterName;

        //    if (!PrinterConfigs.Connected(printerName))
        //    {
        //        MessageBox.Show($"请配置{nameof(PrinterConfigs.出库标签打印)}机!");
        //        return;
        //    }

        //    PrintOutputFormLabel form = new PrintOutputFormLabel
        //    {
        //        PrinterName = printerName
        //    };
        //    form.Print(data);
        //}
    }
}
