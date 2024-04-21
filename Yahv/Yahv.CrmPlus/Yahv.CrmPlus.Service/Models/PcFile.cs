using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Models
{
    public class PcFile : Linq.IDataEntity, IUnique
    {
        #region 属性
        public string ID { set; get; }
        public string MainID { set; get; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public Underly.CrmFileType Type { set; get; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string CustomName { set; get; }
        public string Url { set; get; }
        public string CreatorID { set; get; }
        public string SiteUserID { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        #endregion

    }
    public class FileDescription : Linq.IDataEntity, IUnique
    {
        #region 属性
        public string ID { set; get; }
        public string EnterpriseID { set; get; }
        public string SubID { set; get; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public Underly.CrmFileType Type { set; get; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string CustomName { set; get; }
        public string Url { set; get; }
        public string CreatorID { set; get; }
        public string SiteUserID { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        public DataStatus Status { set; get; }
        public string Summary { set; get; }
        #endregion

    }
}
