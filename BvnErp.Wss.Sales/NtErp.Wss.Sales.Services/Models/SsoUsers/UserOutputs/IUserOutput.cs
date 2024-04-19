
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
    /// 用户账户支出
    /// </summary>
    public interface IUserOutput : IUnique, IPersistence
    {
        /// <summary>
        /// 收入 ID
        /// </summary>
        string UserInputID { get; set; }
        /// <summary>
        /// 订单 ID
        /// </summary>
        string OrderID { get; set; }
        /// <summary>
        /// 账户类型
        /// </summary>
        UserAccountType Type { get; set; }
        /// <summary>
        /// UserID
        /// </summary>
        string UserID { get; set; }

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
