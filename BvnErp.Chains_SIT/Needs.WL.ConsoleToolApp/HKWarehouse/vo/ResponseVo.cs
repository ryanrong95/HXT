using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.WL.ConsoleToolApp
{
    public class ResponseVo
    {
        public int Total { get; set; }
        public int Size { get; set; }
        public int Index { get; set; }
        public List<Object> Data { get; set; }
    }
}
