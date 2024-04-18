using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 接口消息推送日志
    /// </summary>
    public class ApiNoticeLog : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 接口推送ID
        /// </summary>
        public string ApiNoticeID { get; set; }

        /// <summary>
        /// 推送报文
        /// </summary>
        public string PushMsg { get; set; }

        /// <summary>
        /// 响应报文
        /// </summary>
        public string ResponseMsg { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public ApiNoticeLog()
        {
            this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.ID = ChainsGuid.NewGuidUp();
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNoticeLogs
                {
                    ID = this.ID,
                    ApiNoticeID = this.ApiNoticeID,
                    PushMsg = this.PushMsg,
                    ResponseMsg = this.ResponseMsg,
                    CreateDate = this.CreateDate,
                    Summary = this.Summary
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
