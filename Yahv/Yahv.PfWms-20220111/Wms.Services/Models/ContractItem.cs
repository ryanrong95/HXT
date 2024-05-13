using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Linq.Persistence;

namespace Wms.Services.Models
{
    public class ContractItem : IUnique
    {

        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContractID { get; set; }

        /// <summary>
        /// 库位规格
        /// </summary>
        public string SpecID { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 分配的库位编号,后期回填,以","分隔
        /// </summary>
        public string ShelveIDs { get; set; }

        #endregion

    }
}
