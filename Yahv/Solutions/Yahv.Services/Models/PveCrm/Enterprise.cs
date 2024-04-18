using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models.PveCrm
{
    public class Enterprise : IUnique
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Place { get; set; }

        public string District { get; set; }

        public string DyjCode { get; set; }

        #region 纯粹为了兼容原来的PvbCrm，让其他使用公共视图的系统少一点代码改动

        public string Corporation { get; set; }

        public string RegAddress { get; set; }

        public string Uscc { get; set; }

        #endregion
    }
}
