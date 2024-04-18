using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 提货通知- 来源表（DeliveryNotices）
    /// </summary>
    public class DeliveryNotice : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public Interfaces.IOrder Order { get; set; }

        /// <summary>
        /// 提货状态
        /// </summary>
        public Enums.DeliveryNoticeStatus DeliveryNoticeStatus { get; set; }

        public Enums.Status Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 交货信息
        /// </summary>
        public DeliveryConsignee DeliveryConsignees { get; set; }

        /// <summary>
        /// 当前操作人
        /// </summary>
        public Admin CurrentAdmin { get; set; }

        /// <summary>
        /// 提货通知类初始化
        /// </summary>
        public DeliveryNotice()
        {
            this.DeliveryNoticeStatus = Enums.DeliveryNoticeStatus.UnDelivery;
            this.Status = Enums.Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Confirmed += Delivery_Confirmed;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event StatusChangedEventHanlder Confirmed;
        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveryNotices>().Count(item => item.ID == this.ID);

                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.DeliveryNotice);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 去持久化
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeliveryNotices>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        private void Delivery_Confirmed(object sender, StatusChangedEventArgs e)
        {
            //写入日志
            var deliverynotice = (DeliveryNotice)e.Object;
            deliverynotice.Log("仓库人员[" + deliverynotice.CurrentAdmin?.RealName + "]完成了提货。");
        }

        /// <summary>
        /// 确认提货
        /// </summary>
        /// <param name="admin">提货人</param>
        public void Confirm()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeliveryNotices>(new { DeliverNoticeStatus = Enums.DeliveryNoticeStatus.Delivered }, item => item.ID == this.ID);
            }

            this.OnEnterSuccess();
            this.OnConfirmed();
        }

        void OnConfirmed()
        {
            if (this != null && this.Confirmed != null)
            {
                this.Confirmed(this, new StatusChangedEventArgs(this));
            }
        }
    }
}