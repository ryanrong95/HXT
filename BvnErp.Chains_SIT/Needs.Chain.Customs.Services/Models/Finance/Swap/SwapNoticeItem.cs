using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 发票运单
    /// </summary>
    public class SwapNoticeItem : IUnique, IFulError, IFulSuccess
    {
        #region 数据库属性

        public string ID { get; set; }

        /// <summary>
        /// 换汇通知
        /// </summary>
        public string SwapNoticeID { get; set; }

        /// <summary>
        /// 报关单合同编号
        /// </summary>
        public SwapDecHead SwapDecHead { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 本次购汇金额
        /// </summary>
        public decimal Amount { get; set; }

        public Enums.Status Status { get; set; }

        #endregion

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public SwapNoticeItem()
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
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.SwapNoticeItem);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>(new
                        {
                            ID = this.ID,
                            SwapNoticeID = this.SwapNoticeID,
                            DecHeadID = this.SwapDecHead.ID,
                            CreateDate = this.CreateDate,
                            Amount = this.Amount,
                            Status = (int)this.Status,
                        }, item => item.ID == this.ID);
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
