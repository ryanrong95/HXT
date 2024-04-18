using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 申报日志
    /// </summary>
    public class CgLog_Declare
    {
        #region 属性

        public string ID { get; set; }
        public string TinyOrderID { get; set; }
        public TinyOrderDeclareStatus Status { get; set; }
        public string Summary { get; set; }
        public string Specs { get; set; }
        public string GrossWeight { get; set; }
        public string BoxCode { get; set; }
        public string AdminID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        #endregion
    }
}
