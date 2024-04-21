using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class nBrand_Chenhan : Yahv.Linq.IUnique
    {
        public nBrand_Chenhan()
        {
            this.Status = DataStatus.Normal;
        }
        #region 属性
        public string ID { set; get; }

        public string BrandID { set; get; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { internal set; get; }
        /// <summary>
        /// 品牌简称
        /// </summary>
        public string ShortName { internal set; get; }
        /// <summary>
        /// 品牌中文名称
        /// </summary>
        public string ChineseName { internal set; get; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { set; get; }

        /// <summary>
        /// 相关企业名称
        /// </summary>
        public string EnterpriseName { get; set; }

        public DataStatus Status { set; get; }
        /// <summary>
        /// 是否代理
        /// </summary>
        public bool IsAgent { set; get; }

        #endregion
    }
}
