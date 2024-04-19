using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 记账方向
    /// </summary>
    [Obsolete("废弃")]
    public enum AccountDirection
    {

        /// <summary>
        /// 借
        /// </summary>
        [Naming("借")]
        Borrow = 1,
        /// <summary>
        /// 贷
        /// </summary>
        [Naming("代")]
        Loan = 2

    }
}
