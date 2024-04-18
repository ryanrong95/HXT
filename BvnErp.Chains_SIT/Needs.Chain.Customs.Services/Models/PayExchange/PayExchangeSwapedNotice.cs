using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 用户的换汇申请中已被用于换汇的金额
    /// </summary>
    public class PayExchangeSwapedNotice : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 属性

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// DecHeads表的ID
        /// </summary>
        public string DecHeadID { get; set; } = string.Empty;

        /// <summary>
        /// 用户申请的金额中，已被换汇的金额
        /// </summary>
        public decimal UnHandleAmount { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public Enums.SwapedNoticeHandleStatus HandleStatus { get; set; }

        /// <summary>
        /// 数据状态（200、400）
        /// </summary>

        public Enums.Status Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        #endregion

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices>(new Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices
                    {
                        ID = this.ID,
                        DecHeadID = this.DecHeadID,
                        UnHandleAmount = this.UnHandleAmount,
                        HandleStatus = (int)this.HandleStatus,
                        Status = (int)this.Status,
                        CreateTime = this.CreateTime,
                        UpdateTime = this.UpdateTime,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices>(new
                    {
                        DecHeadID = this.DecHeadID,
                        UnHandleAmount = this.UnHandleAmount,
                        HandleStatus = (int)this.HandleStatus,
                        Status = (int)this.Status,
                        CreateTime = this.CreateTime,
                        UpdateTime = this.UpdateTime,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        Status = (int)Enums.Status.Delete
                    }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

    }
}
