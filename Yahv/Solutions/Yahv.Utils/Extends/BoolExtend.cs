using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Utils.Extends
{
    public static class BoolExtend
    {
        public static string Text(this bool mybool)
        {
            return mybool == true ? "是" : "否";
        }
    }
}
