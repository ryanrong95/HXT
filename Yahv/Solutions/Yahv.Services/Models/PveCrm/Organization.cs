using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.Services.Models.PveCrm
{
    public class Organization : IUnique
    {
        public string ID { get; set; }

        public string Name { get; set; }

        #region 纯粹为了兼容原来的PvbCrm，让其他使用公共视图的系统少一点代码改动

        public string Corporation { get; set; }

        public string RegAddress { get; set; }

        public string Uscc { get; set; }

        #endregion
    }
}
