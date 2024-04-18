using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 产品归类操作日志
    /// </summary>
    public class Log_ClassifyOperating : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// OrderItemID/预归类产品ID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public LogType LogType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Summary { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                this.ID = Layers.Data.PKeySigner.Pick(PKeyType.ClassifyOperatingLog);
                reponsitory.Insert(new Layers.Data.Sqls.PvData.Logs_ClassifyOperating()
                {
                    ID = this.ID,
                    MainID = this.MainID,
                    CreatorID = this.CreatorID,
                    LogType = (int)this.LogType,
                    CreateDate = this.CreateDate,
                    Summary = this.Summary
                });
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
