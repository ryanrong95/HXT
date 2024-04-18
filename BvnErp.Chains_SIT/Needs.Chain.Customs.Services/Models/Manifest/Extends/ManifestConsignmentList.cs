using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ManifestConsignmentList : IUnique
    {
        public string ID { get; set; }
        public string VoyageNo { get; set; }
        public string BillNo { get; set; }
        public string ContrNO { get; set; }
        public string Port { get; set; }
        public int? PackNo { get; set; }
        public string ConsigneeName { get; set; }
        public string AdminName { get; set; }
        public DateTime CreateTime { get; set; }
        public string CusMftStatus { get; set; }
        public string DoubleCheckerName { get; set; }
        public string DoubleCheckerAdminID { get; set; }

        /// <summary>
        /// 单据状态名字
        /// </summary>
        public string StatusName
        {
            get
            {
                return MultiEnumUtils.ToText<Enums.CusMftStatus>(this.CusMftStatus);
            }
        }
    }
}
