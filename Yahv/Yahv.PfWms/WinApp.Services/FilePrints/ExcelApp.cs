using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
//using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace WinApp.Services.FilePrints
{
    public class ExcelApp : FilePrintBase
    {
        internal ExcelApp(string printer) : base(printer)
        {
        }

        public override void Print(string fileName)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = true;

            if (!string.IsNullOrWhiteSpace(this.Printer))
            {
                excelApp.ActivePrinter = this.Printer;//设置打印机
            }

            object missing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            try
            {
                workbook = excelApp.Workbooks.Open(fileName);
                workbook.PrintOutEx();
            }
            finally
            {
                workbook.Close();
                excelApp.Quit();
                Marshal.ReleaseComObject(excelApp);//释放
            }
        }
    }
}
