using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;

namespace Yahv.PsWms.DappApi.Services.Models
{
    /// <summary>
    /// 库位
    /// </summary>
    public class Shelve : IUnique
    {
        #region 库位
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 原则上Code一致 库位编码：[sc:]位前缀
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 字典LocationSize
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        #endregion

        public Shelve()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
        }
    }
}
