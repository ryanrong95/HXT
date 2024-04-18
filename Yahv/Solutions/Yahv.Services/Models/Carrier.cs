using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    public class Carrier : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 承运商代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 快递的图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 统一社会信用编码
        /// </summary>
        public string Uscc { get; set; }

        /// <summary>
        /// 公司法人
        /// </summary>
        public string Corporation { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        #endregion
    }
}
