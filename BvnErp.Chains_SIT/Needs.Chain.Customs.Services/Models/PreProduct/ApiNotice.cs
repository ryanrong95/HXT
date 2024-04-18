using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 接口推送通知
    /// </summary>
    public class ApiNotice : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 推送类型：归类结果、完税价格
        /// </summary>
        public PushType PushType { get; set; }

        public string ClientID { get; set; }
        public Client Client { get; set; }

        public string ItemID { get; set; }

        /// <summary>
        /// 推送状态：未推送、已推送
        /// </summary>
        public PushStatus PushStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        #endregion

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public ApiNotice()
        {
            this.PushStatus = Enums.PushStatus.Unpush;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiNotices>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    this.ID = ChainsGuid.NewGuidUp();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNotices
                    {
                        ID = this.ID,
                        PushType = (int)this.PushType,
                        ClientID = this.ClientID,
                        ItemID = this.ItemID,
                        PushStatus = (int)this.PushStatus,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(new
                    {
                        PushType = (int)this.PushType,
                        ClientID = this.ClientID,
                        ItemID = this.ItemID,
                        PushStatus = (int)this.PushStatus,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
                }
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
