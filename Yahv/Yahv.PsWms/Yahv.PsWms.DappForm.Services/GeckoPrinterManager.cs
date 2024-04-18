using Gecko;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.DappForm.Services
{
    /// <summary>
    /// 打印管理者
    /// </summary>
    public class GeckoPrinterManager
    {

        #region 第一种写法

        GeckoWebBrowser firefox;

        public GeckoPrinterManager(GeckoWebBrowser firefox)
        {
            this.firefox = firefox;
        }

        static PrintDocument fPrintDocument = new PrintDocument();

        static public string DefaultPrinter
        {
            get { return fPrintDocument.PrinterSettings.PrinterName; }
        }

        static public string[] GetPrinters()
        {
            List<string> fPrinters = new List<string>();
            fPrinters.Add(DefaultPrinter); // 默认打印机始终出现在列表的第一项  
            foreach (string fPrinterName in PrinterSettings.InstalledPrinters)
            {
                if (!fPrinters.Contains(fPrinterName))
                    fPrinters.Add(fPrinterName);
            }
            return fPrinters.ToArray();

        }

        /// <summary>
        /// 高保真打印
        /// </summary>
        /// <param name="printer">打印机</param>
        /// <param name="numcopies">打印份数</param>
        /// <returns></returns>
        public nsIPrintSettings GetSilent(string printer, int numcopies = 1)
        {

            var domWindow = firefox.Window.DomWindow;
            var proxy = (mozIDOMWindowProxy)domWindow;
            nsIWebBrowserPrint print = Xpcom.QueryInterface<nsIWebBrowserPrint>(proxy);
            var settings = print.GetGlobalPrintSettingsAttribute();

            var nothing = new nsAString();
            nothing.SetData("");

            settings.SetDocURLAttribute(nothing);
            //setting.SetDuplexAttribute("");

            //边距
            settings.SetEdgeBottomAttribute(0);
            settings.SetEdgeLeftAttribute(0);
            settings.SetEdgeRightAttribute(0);
            settings.SetEdgeTopAttribute(0);


            //setting.SetEndPageRangeAttribute(0);

            //页脚


            settings.SetFooterStrCenterAttribute(nothing);
            settings.SetFooterStrLeftAttribute(nothing);
            settings.SetFooterStrRightAttribute(nothing);

            //页眉
            settings.SetHeaderStrCenterAttribute(nothing);
            settings.SetHeaderStrLeftAttribute(nothing);
            settings.SetHeaderStrRightAttribute(nothing);


            //setting.SetHowToEnableFrameUIAttribute(0);
            //setting.SetIsCancelledAttribute(false);
            settings.SetIsInitializedFromPrefsAttribute(false);
            settings.SetIsInitializedFromPrinterAttribute(false);


            //setting.SetMarginInTwips();

            //外补
            settings.SetMarginBottomAttribute(0);
            settings.SetMarginLeftAttribute(0);
            settings.SetMarginTopAttribute(0);
            settings.SetMarginTopAttribute(0);

            //打印份数
            settings.SetNumCopiesAttribute(numcopies);

            //setting.SetOrientationAttribute()

            //setting.SetOutputFormatAttribute();
            //setting.SetPaperDataAttribute();
            //setting.SetPaperHeightAttribute();
            //setting.SetPaperNameAttribute();

            //setting.SetPaperSizeTypeAttribute();
            //setting.SetPaperWidthAttribute();

            //setting.SetPaperSizeUnitAttribute();

            //保留页边距框
            //setting.SetPersistMarginBoxSettingsAttribute(false);

            //setting.SetPlexNameAttribute();
            settings.SetPrintBGColorsAttribute(false);
            settings.SetPrintBGImagesAttribute(false);
            //setting.SetPrintCommandAttribute();

            //设置打印机
            if (!string.IsNullOrEmpty(printer))
            {
                var printerName = new nsAString();
                printerName.SetData(printer);
                settings.SetPrinterNameAttribute(printerName);
            }
            //var printerName = new nsAString();
            //printerName.SetData("asdfasdf");

            //settings.SetPrinterNameAttribute(printerName);


            //setting.SetPrintSilentAttribute(true);

            settings.SetPrintSilentAttribute(true);
            settings.SetPrintToFileAttribute(false);
            settings.SetShowPrintProgressAttribute(false);
            settings.SetOutputFormatAttribute(1); //2 == PDF,1代表实际打印机打印
            //settings.SetToFileNameAttribute(new nsAString($"c:\\{Guid.NewGuid().ToString()}.pdf"));

            //var path = new nsAString();
            //path.SetData(@"D:\temp" + DateTime.Now.Ticks + ".pdf");
            //settings.SetToFileNameAttribute(path);

            return settings;
        }

        #endregion

        #region 第二种写法
        ///// <summary>
        ///// 高保真打印
        ///// </summary>
        ///// <param name="browser">浏览器</param>
        ///// <param name="printer">打印机</param>
        ///// <param name="numcopies">打印份数</param>
        ///// <param name="pdf">是否生成不进行实际打印的pdf格式的文件</param>
        //public static void Print(this Gecko.GeckoWebBrowser browser, string printer, int numcopies = 1, bool pdf = false)
        //{
        //    var domWindow = browser.Window.DomWindow;

        //    var proxy = (mozIDOMWindowProxy)domWindow;

        //    proxy.Print(printer, numcopies,pdf);

        //}



        ///// <summary>
        ///// 高保真打印
        ///// </summary>
        ///// <param name="browser">浏览器</param>
        ///// <param name="printer">打印机</param>
        ///// <param name="numcopies">打印份数</param>
        //public static void Print(this Gecko.mozIDOMWindowProxy proxy, string printer, int numcopies = 1, bool pdf = false)
        //{

        //    nsIWebBrowserPrint print = Xpcom.QueryInterface<nsIWebBrowserPrint>(proxy);
        //    var settings = print.GetGlobalPrintSettingsAttribute();

        //    var nothing = new nsAString();
        //    nothing.SetData("");
        //    settings.SetDocURLAttribute(nothing);

        //    //边距
        //    settings.SetEdgeBottomAttribute(0);
        //    settings.SetEdgeLeftAttribute(0);
        //    settings.SetEdgeRightAttribute(0);
        //    settings.SetEdgeTopAttribute(0);

        //    //setting.SetEndPageRangeAttribute(0);

        //    //页脚

        //    settings.SetFooterStrCenterAttribute(nothing);
        //    settings.SetFooterStrLeftAttribute(nothing);
        //    settings.SetFooterStrRightAttribute(nothing);

        //    //页眉
        //    settings.SetHeaderStrCenterAttribute(nothing);
        //    settings.SetHeaderStrLeftAttribute(nothing);
        //    settings.SetHeaderStrRightAttribute(nothing);


        //    //setting.SetHowToEnableFrameUIAttribute(0);
        //    //setting.SetIsCancelledAttribute(false);
        //    settings.SetIsInitializedFromPrefsAttribute(false);
        //    settings.SetIsInitializedFromPrinterAttribute(false);

        //    //setting.SetMarginInTwips();

        //    //外补
        //    settings.SetMarginBottomAttribute(0);
        //    settings.SetMarginLeftAttribute(0);
        //    settings.SetMarginTopAttribute(0);
        //    settings.SetMarginRightAttribute(0);

        //    //打印份数
        //    settings.SetNumCopiesAttribute(numcopies);

        //    //setting.SetOrientationAttribute()

        //    //setting.SetOutputFormatAttribute();
        //    //settings.SetPaperDataAttribute();
        //    //settings.SetPaperHeightAttribute(500);
        //    //settings.SetPaperWidthAttribute(300);

        //    //settings.SetPaperSizeUnitAttribute(1);

        //    //保留页边距框
        //    //setting.SetPersistMarginBoxSettingsAttribute(false);

        //    //setting.SetPlexNameAttribute();
        //    settings.SetPrintBGColorsAttribute(false);
        //    settings.SetPrintBGImagesAttribute(false);
        //    //setting.SetPrintCommandAttribute();


        //    //设置打印机
        //    if (!string.IsNullOrEmpty(printer))
        //    {
        //        var printerName = new nsAString();
        //        printerName.SetData(printer);
        //        settings.SetPrinterNameAttribute(printerName);
        //    }
        //    settings.SetPrintBGColorsAttribute(true);

        //    settings.SetPrintSilentAttribute(true);
        //    settings.SetPrintToFileAttribute(false);
        //    settings.SetShowPrintProgressAttribute(false);
        //    if (pdf)
        //    {
        //        settings.SetOutputFormatAttribute(2); //2 == PDF,1代表实际打印机打印
        //        settings.SetToFileNameAttribute(new nsAString($"c:\\{Guid.NewGuid().ToString()}.pdf"));
        //    }
        //    else
        //    {
        //        settings.SetOutputFormatAttribute(1);
        //    }

        //    //return settings;

        //    print.Print(settings, null);


        //}


        #endregion

    }
}
