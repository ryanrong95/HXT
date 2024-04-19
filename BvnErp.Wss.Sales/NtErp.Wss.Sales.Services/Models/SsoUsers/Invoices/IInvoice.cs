
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
    /// 用户发票
    /// </summary>
    public interface IInvoice : IUnique, IPersistence
    {
        /// <summary>
        /// UserID
        /// </summary>
        string UserID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        string CompanyName { get; set; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        string SCC { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        string RegAddress { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        string Tel { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        string BankName { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        string BankAccount { get; set; }
        /// <summary>
        /// 提货人
        /// </summary>
        Consignee Consignee { get; set; }
        /// <summary>
        /// 发票类型  1.普票   2.增票
        /// </summary>
        InvoiceType Type { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        SelfStatus Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateDate { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        DateTime UpdateDate { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        bool IsDefault { get; set; }
    }
}
