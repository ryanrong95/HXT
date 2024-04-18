using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    [Serializable]
    public class ExitNotice : IUnique, IPersist
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 库房 (100 - 香港, 200 - 深证)
        /// </summary>
        public Enums.WarehouseType WarehouseType { get; set; }

        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// DecHeadID
        /// </summary>
        public string DecHeadID { get; set; } = string.Empty;

        /// <summary>
        /// 出库类型
        /// </summary>
        public Enums.ExitType ExitType { get; set; }

        /// <summary>
        /// 出库通知状态
        /// </summary>
        public Enums.ExitNoticeStatus ExitNoticeStatus { get; set; }

        /// <summary>
        /// AdminID
        /// </summary>
        public string AdminID { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// UpdateDate
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        ///打印状态
        /// </summary>
        public int? IsPrint { get; set; }

        public event SuccessHanlder AbandonSuccess;
        //public event ErrorHanlder AbandonError;
        public event SuccessHanlder EnterSuccess;
        //public event ErrorHanlder EnterError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.ExitNotices>(new Layer.Data.Sqls.ScCustoms.ExitNotices()
                    {
                        ID = this.ID,
                        WarehouseType = (int)this.WarehouseType,
                        OrderID = this.OrderID,
                        DecHeadID = this.DecHeadID,
                        ExitType = (int)this.ExitType,
                        ExitNoticeStatus = (int)this.ExitNoticeStatus,
                        AdminID = this.AdminID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        IsPrint = this.IsPrint,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(new
                    {
                        WarehouseType = (int)this.WarehouseType,
                        OrderID = this.OrderID,
                        DecHeadID = this.DecHeadID,
                        ExitType = (int)this.ExitType,
                        ExitNoticeStatus = (int)this.ExitNoticeStatus,
                        AdminID = this.AdminID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        IsPrint = this.IsPrint,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandon();
        }

        virtual protected void OnAbandon()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

    }
}
