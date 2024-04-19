
using NtErp.Wss.Sales.Services.Models.SsoUsers;
using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Model
{
    /// <summary>
    /// 用户账户收入
    /// </summary>
    public interface IUserInput : IUnique, IPersistence
    {
        /// <summary>
        /// 账户类型
        /// </summary>
        UserAccountType Type { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        InputSource Source { get; set; }
        /// <summary>
        /// UserID
        /// </summary>
        string UserID { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        string Code { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        Currency Currency { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        decimal Amount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateDate { get; set; }
    }
}
