using Gecko;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Services;
using WinApp.Services.FilePrints;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace WinApp.Printers
{
    public abstract class PrinterBase : IDisposable
    {
        public PrinterBase()
        {

        }

        virtual public void Dispose()
        {
          
        }
    }
}
