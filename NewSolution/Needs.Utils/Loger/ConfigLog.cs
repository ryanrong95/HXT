using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Utils
{
    public class ConfigLog
    {
        public ConfigLog()
        {

        }

        public void LogText(string text)
        {
            Loger.Instance().AddConfig($"{text}\r\n");
        }
    }
}
