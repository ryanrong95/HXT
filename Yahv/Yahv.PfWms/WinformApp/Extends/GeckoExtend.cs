using Gecko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp
{
    public static class GeckoExtend
    {
        /// <summary>
        /// 高保真打印
        /// </summary>
        /// <param name="browser">浏览器</param>
        /// <param name="printer">打印机</param>
        /// <param name="numcopies">打印分数</param>
        public static void Print(this Gecko.GeckoWebBrowser browser,string printer,int numcopies=1)
        {
            var domWindow = browser.Window.DomWindow;
            
            var proxy = (mozIDOMWindowProxy)domWindow;
            nsIWebBrowserPrint print = Xpcom.QueryInterface<nsIWebBrowserPrint>(proxy);
            var settings = print.GetGlobalPrintSettingsAttribute();
            
            var nothing = new nsAString();
            nothing.SetData("");
            settings.SetDocURLAttribute(nothing);

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
            //settings.SetPaperDataAttribute();
            //settings.SetPaperHeightAttribute(500);
            //settings.SetPaperWidthAttribute(300);

            settings.SetPaperSizeUnitAttribute(1);

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
            settings.SetPrintBGColorsAttribute(true);           
     
            settings.SetPrintSilentAttribute(true);
            settings.SetPrintToFileAttribute(false);
            settings.SetShowPrintProgressAttribute(false);
            settings.SetOutputFormatAttribute(1); //2 == PDF,1代表实际打印机打印
            //settings.SetToFileNameAttribute(new nsAString($"c:\\{Guid.NewGuid().ToString()}.pdf"));
            
            //return settings;
            
            print.Print(settings, null);
        }
    }
}
