using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 报关单附件
    /// </summary>
    public class DecHeadFile : ModelBase<Layer.Data.Sqls.ScCustoms.DecHeadFiles, ScCustomsReponsitory>, IUnique, IPersist
    {
        /// <summary>
        /// 报关单ID
        /// </summary>
        public string DecHeadID { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public Enums.FileType FileType { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { get; set; }

        /// <summary>
        /// URL地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 管理员
        /// 上传人
        /// </summary>
        public string AdminID { get; set; }

        public override void Enter()
        {
            base.Enter();
        }
    }
}
