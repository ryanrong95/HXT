using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 付汇申请文件
    /// </summary>
    public class PayExchangeApplyFile : ModelBase<Layer.Data.Sqls.ScCustoms.PayExchangeApplyLogs, ScCustomsReponsitory>, IUnique, IPersist, IPersistence
    {
        public string PayExchangeApplyID { get; set; }

        public string Name { get; set; }

        public Needs.Wl.Models.Enums.FileType FileType { get; set; }

        public string FileFormat { get; set; }

        public string Url { get; set; }

        public string AdminID { get; set; }

        public string UserID { get; set; }

        public event SuccessHanlder AbandonSuccess;

        /// <summary>
        /// 当新增或修改成功时发生
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        public PayExchangeApplyFile()
        {
            this.CreateDate = DateTime.Now;
            this.Status = (int)Enums.Status.Normal;
        }

        /// <summary>
        /// 删除
        /// </summary>
        override public void Abandon()
        {
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            this.OnAbandon();
        }

        virtual protected void OnAbandon()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        override public void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyFile);
                this.Reponsitory.Insert(this.ToLinq());
            }
            else
            {
                this.Reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
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