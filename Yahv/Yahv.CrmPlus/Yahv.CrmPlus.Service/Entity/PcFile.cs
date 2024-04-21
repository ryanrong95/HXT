using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.CrmPlus.Service.Entity
{
    public class PcFile : Linq.IEntity
    {
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
    }
}
