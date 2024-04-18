using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yahv.Usually;

namespace Yahv.PsWms.DappForm.Services.FilePrints
{
    public class WordApp : FilePrintBase
    {
        //事件定义
        public event SuccessHanlder PrintCompleted;

        internal WordApp(string printer) : base(printer)
        {
            //增加事件
            this.PrintCompleted += WordApp_PrintCompleted;
        }


        public override void Print(string fileName)
        {
            //应用程序域抛异常
            //throw new Exception("5");
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = false;


            if (!string.IsNullOrWhiteSpace(this.Printer))
            {
                wordApp.ActivePrinter = this.Printer;//设置打印机
            }
            wordApp.DocumentBeforePrint += WordApp_DocumentBeforePrint;


            object missing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Document document = null;
            try
            {
                document = wordApp.Documents.Open(fileName);
                document.PrintOut();
            }
            finally
            {
                document.Close();
                wordApp.Quit();
                Marshal.ReleaseComObject(wordApp);//释放
            }

            //打印完成调用事件
            WordApp_PrintCompleted(this, new SuccessEventArgs(this));
        }

        private void WordApp_DocumentBeforePrint(Microsoft.Office.Interop.Word.Document Doc, ref bool Cancel)
        {
            //总页数
            int pageCount = Doc.ComputeStatistics(Microsoft.Office.Interop.Word.WdStatistic.wdStatisticPages);
            SimHelper.PrintStatus = $"打印文件:{Doc.Name}.共{ pageCount}页";
            //SimHelper.TransferStatus = "";
        }

        private void WordApp_PrintCompleted(object sender, SuccessEventArgs e)
        {
            SimHelper.PrintStatus = "打印完成";
        }

    }
}
