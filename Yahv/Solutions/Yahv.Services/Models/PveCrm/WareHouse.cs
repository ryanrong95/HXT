using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models.PveCrm
{
    public class WareHouse : Organization
    {
        public string Code { get; set; }

        public WarehouseGrade Grade { get; set; }

        public string District { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        #region 纯粹为了兼容原来的PvbCrm的命名，让其他使用公共视图的系统少一点代码改动

        /// <summary>
        /// 库房编码
        /// </summary>
        public string WsCode { get; set; }

        /// <summary>
        /// 所属地区
        /// </summary>
        public string Region { get; set; }
        public string RegionDes { get; set; }
        /// <summary>
        /// 具体地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { get; set; }

        #endregion
    }
}
