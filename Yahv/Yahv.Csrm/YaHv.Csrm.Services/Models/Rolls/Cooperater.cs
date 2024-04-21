using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.Csrm.Services.Models.Rolls
{
    public class Cooperater
    {
        public Origins.Enterprise Enterprise { get; set; }
        /// <summary>
        /// 合作类型
        /// </summary>
        public CooperType CooperType { set; get; }
        /// <summary>
        /// 合作公司信息
        /// </summary>
        public Origins.Company Company { set; get; }

    }
}
