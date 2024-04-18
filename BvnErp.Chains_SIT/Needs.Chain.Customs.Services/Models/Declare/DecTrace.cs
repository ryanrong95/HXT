using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单轨迹/回执信息
    /// </summary>
    [Serializable]
    public class DecTrace : IUnique, IPersist
    {
        /// <summary>
        /// 主键ID DECT+8位年月日+6位流水号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 报关单ID
        /// </summary>
        public string DeclarationID { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string ChannelName {
            get
            {
                return MultiEnumUtils.ToText<Enums.CusDecStatus>(this.Channel);
            }
        }

        /// <summary>
        /// 海关备注
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 回执时间
        /// </summary>
        public DateTime NoticeDate { get; set; }

        /// <summary>
        /// 回执报文文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 读取回执，做备份时使用
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 备份报文路径
        /// </summary>
        public string BackupUrl { get; set; }

        public DecTrace()
        {


        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.ID = ChainsGuid.NewGuidUp();
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecTraces
                {
                    ID = this.ID,
                    DeclarationID = this.DeclarationID,
                    Channel = this.Channel,
                    Message = this.Message,
                    NoticeDate = this.NoticeDate,
                    FileName = this.FileName,
                    BackupUrl = this.BackupUrl
                });
            }
            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
