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
    /// 付汇申请文件
    /// </summary>
    public class PayExchangeApplyFile : IUnique, IPersist, IPersistence
    {
        public string ID { get; set; }
        public string PayExchangeApplyID { get; set; }
        public string AdminID { get; set; }
        public string UserID { get; set; }
        public string FileName { get; set; }
        public FileType FileType { get; set; }
        public string FileFormat { get; set; }
        public string Url { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string Summary { get; set; }

        public string ErmAdminID { get; set; }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;

        /// <summary>
        /// 当新增或修改成功时发生
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public PayExchangeApplyFile()
        {
            this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        /// <summary>
        /// 根据付汇申请构造付汇委托书
        /// </summary>
        /// <param name="apply">付汇申请</param>
        public PayExchangeApplyFile(PayExchangeApply apply) : this()
        {
            this.PayExchangeApplyID = apply.ID;

            using (var view = new Views.PayExchangeApplyFileView())
            {
                var file = view.Where(item => item.PayExchangeApplyID == this.PayExchangeApplyID && item.FileType == FileType.PayExchange && item.Status == Status.Normal).FirstOrDefault();
                if (file != null)
                {
                    this.ID = file.ID;
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        virtual public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>(new { Status = Status.Delete }, item => item.ID == this.ID);
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
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyFile);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
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


    public class PayExchangeApplyFiles : BaseItems<PayExchangeApplyFile>
    {
        public PayExchangeApplyFiles(IEnumerable<PayExchangeApplyFile> enums) : base(enums)
        {
        }

        public PayExchangeApplyFiles(IEnumerable<PayExchangeApplyFile> enums, Action<PayExchangeApplyFile> action) : base(enums, action)
        {
        }
        public override void Add(PayExchangeApplyFile item)
        {
            base.Add(item);
        }

        protected override IEnumerable<PayExchangeApplyFile> GetEnumerable(IEnumerable<PayExchangeApplyFile> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}