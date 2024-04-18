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
    /// 付款通知文件
    /// </summary>
    public class PaymentNoticeFile : IUnique, IPersist, IPersistence
    {
        public string ID { get; set; }
        public string PaymentNoticeID { get; set; }
        public string AdminID { get; set; }
        public string FileName { get; set; }
        public FileType FileType { get; set; }
        public string FileFormat { get; set; }
        public string Url { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string Summary { get; set; }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;

        /// <summary>
        /// 当新增或修改成功时发生
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public PaymentNoticeFile()
        {
            //付款凭证
            this.FileType = FileType.PaymentVoucher;
            this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        /// <summary>
        /// 删除
        /// </summary>
        virtual public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>(
                    new
                    {
                        Status = Status.Delete
                    }, item => item.ID == this.ID);
            }
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

        virtual public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNoticeFiles>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PaymentNoticeFile);
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PaymentNoticeFiles
                    {
                        ID = this.ID,
                        PaymentNoticeID = this.PaymentNoticeID,
                        AdminID = this.AdminID,
                        Name = this.FileName,
                        FileType = (int)this.FileType,
                        FileFormat = this.FileFormat,
                        Url = this.Url,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.PaymentNoticeFiles
                    {
                        PaymentNoticeID = this.PaymentNoticeID,
                        AdminID = this.AdminID,
                        Name = this.FileName,
                        FileType = (int)this.FileType,
                        FileFormat = this.FileFormat,
                        Url = this.Url,
                        Status = (int)this.Status,
                        Summary = this.Summary,
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

    public class PaymentNoticeFiles : BaseItems<PaymentNoticeFile>
    {
        internal PaymentNoticeFiles(IEnumerable<PaymentNoticeFile> enums) : base(enums)
        {
        }

        internal PaymentNoticeFiles(IEnumerable<PaymentNoticeFile> enums, Action<PaymentNoticeFile> action) : base(enums, action)
        {
        }

        public override void Add(PaymentNoticeFile item)
        {
            base.Add(item);
        }

        protected override IEnumerable<PaymentNoticeFile> GetEnumerable(IEnumerable<PaymentNoticeFile> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}