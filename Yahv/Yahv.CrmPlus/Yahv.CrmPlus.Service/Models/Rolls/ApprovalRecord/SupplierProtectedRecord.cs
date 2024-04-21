using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.CrmPlus.Service.Models.Rolls
{
    /// <summary>
    /// 保护申请记录：客户和供应商
    /// </summary>
    public class ProtectedRecord : BaseApplyRecord
    {
        public string EnterpriseName { set; get; }
    }
}
