using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Model.Origins
{
    public class CustomMasterDefault : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        public string IEPortCode { get; set; }

        public string EntyPortCode { get; set; }

        public string OrgCode { get; set; }

        public string VsaOrgCode { get; set; }

        public string InspOrgCode { get; set; }

        public string PurpOrgCode { get; set; }
        #endregion
    }
}
