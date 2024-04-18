using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Needs.Wl.Environment.Models
{
    /// <summary>
    /// 深圳进口报关公司
    /// 代理报关公司
    /// </summary>
    public class Company
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 公司简称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 开户行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 开户名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 开户账户
        /// </summary>
        public string AccountNo { get; set; }
    }
}