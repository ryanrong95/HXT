using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;

namespace Needs.Wl.Logs.Services.Model
{
    /// <summary>
    /// 对象的操作日志
    /// </summary>
    public class Log : ModelBase<Layer.Data.Sqls.ScCustoms.Logs, ScCustomsReponsitory>, IUnique, IPersistence
    {
        public string Name { get; set; }

        /// <summary>
        ///对象ID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// 操作人ID
        /// UserID
        /// AdminID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 操作数据
        /// </summary>
        public string Json { get; set; }

        public override void Enter()
        {
            this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs()
            {
                ID = Guid.NewGuid().ToString("N"),
                Name = this.Name,
                AdminID = this.AdminID,
                MainID = this.MainID,
                Json = this.Json,
                Summary = this.Summary,
                CreateDate = DateTime.Now
            });
        }
    }
}