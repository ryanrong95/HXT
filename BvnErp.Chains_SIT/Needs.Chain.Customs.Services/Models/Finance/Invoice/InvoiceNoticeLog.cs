using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceNoticeLog : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public Admin Admin { get; set; }

        public string InvoiceNoticeID { get; set; }
        /// <summary>
        /// 操作状态
        /// </summary>
        public Enums.InvoiceNoticeStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public InvoiceNoticeLog()
        {
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable < Layer.Data.Sqls.ScCustoms.InvoiceNoticeLogs>().Count(item => item.ID == this.ID);

                    if (count == 0)
                    {
                        this.ID = ChainsGuid.NewGuidUp();
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

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
