using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsPortal.Services.Models
{
    public class Xdt2Response<T> where T : class
    {
        public int count { get; set; }

        public string code { get; set; }

        public string message { get; set; }

        public T data { get; set; }

        public bool success { get; set; }

    }
}
