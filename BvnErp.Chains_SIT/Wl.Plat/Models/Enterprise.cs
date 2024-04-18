using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Models
{
    /// <summary>
    /// 基本信息
    /// </summary>
    public class Enterprise : IUnique
    {
        public Enterprise()
        {
            this.Status = ApprovalStatus.Normal;
        }
        #region 属性
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID
        {
            get
            {
                return this.id ?? this.Name.MD5();

            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 管理编码
        /// </summary>
        /// <chenhan>保障局部唯一</chenhan>
        public string AdminCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 地域、地区（废弃）
        ///// </summary>
        public string District { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 企业法人
        /// </summary>

        public string Corporation { set; get; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { set; get; }
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string Uscc { set; get; }
        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
        #endregion


    }
}
