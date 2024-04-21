using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 标准品牌
    /// </summary>
    public class StandardBrand : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID { set; get; }
        public string Name { set; get; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { set; get; }
        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 是否代理
        /// </summary>
        public bool IsAgent { set; get; }

        #endregion

    }
}
