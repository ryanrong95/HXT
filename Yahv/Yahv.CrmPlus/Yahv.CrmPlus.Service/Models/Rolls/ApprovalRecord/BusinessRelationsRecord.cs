using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.CrmPlus.Service.Models.Rolls
{
    public class BusinessRelationsRecord : BaseApplyRecord
    {
        /// <summary>
        /// 主企业
        /// </summary>
        public string MainEnterpriseName { set; get; }
        /// <summary>
        /// 辅企业-关系企业
        /// </summary> 
        public string SubEnterpriseName { set; get; }
        /// <summary>
        /// 关系类型
        /// </summary>
        public Yahv.Underly.BusinessRelationType RelationType { get; set; }

    }
}
