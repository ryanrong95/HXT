using Needs.Wl.Logs.Services.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace Needs.Wl.Services
{
    public static partial class LogManager
    {
        public static Logs Logs
        {
            get { return new Logs(); }
        }
    }

    public sealed class Logs
    {
        internal Logs()
        {

        }

        public LogsView this[string name]
        {
            get { return new LogsView(name); }
        }
    }
}