using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.DappForm.Services.FilePrints
{
    public class PdfApp : FilePrintBase
    {

        string filename;

        internal PdfApp(string printer) : base(printer)
        {
        }

        public override void Print(string fileName)
        {
            //还未测试
            //throw new Exception("5");

            this.filename = new System.IO.FileInfo(fileName).Name;

            using (PdfDocument doc = new PdfDocument())
            using (var settings = doc.PrintSettings)
            {
                if (!string.IsNullOrWhiteSpace(this.Printer))
                {
                    settings.PrinterName = this.Printer;//设置打印机
                }

                settings.DocumentName = fileName;
                ////打印事件
                settings.PaperSettings += Settings_PaperSettings;

                //FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //doc.LoadFromStream(fs);

                doc.LoadFromFile(fileName);


                ////静默打印PDF文档
                //doc.PrintSettings.PrintController = new StandardPrintController();
                doc.Print();
                //doc.Dispose();
                //doc.Close();
            }
        }

        private void Settings_PaperSettings(object sender, Spire.Pdf.Print.PdfPaperSettingsEventArgs e)
        {
            var doc = ((PdfDocumentBase)sender);

            SimHelper.PrintStatus = $"打印文件:{this.filename} , 正在打印第{e.PaperSources.Count()}页，共{doc.Pages.Count}页";
        }

        //private void Settings_EndPrint(object sender, PrintEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
