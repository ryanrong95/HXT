using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Underly
{
    public interface IUser
    {
        /// <summary>
        /// 用户 只读 ID
        /// </summary>
        string ID { get; }
        /// <summary>
        /// 用户 只读 登录名
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 用户 只读 真实姓名
        /// </summary>
        string RealName { get; }

        /// <summary>
        /// 客户ID
        /// </summary>
        string EnterpriseID { get; }

        /// <summary>
        /// 华芯通客户ID
        /// </summary>
        string XDTClientID { get; }

        /// <summary>
        /// 是否为主账号
        /// </summary>
        bool IsMain { get; }
    }
}
