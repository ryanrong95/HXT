using Needs.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PayApproverList : IUnique
    {

        public string ID { get; set; }
        /// <summary>
        /// CostApplyID
        /// </summary>
        public string CostApplyID { get; set; } = string.Empty;

        /// <summary>
        /// 收款人姓名
        /// </summary>
        public string PayeeName { get; set; } = string.Empty;

        /// <summary>
        /// 费用类型
        /// </summary>
        public Enums.CostTypeEnum CostType { get; set; }

        /// <summary>
        /// 费用名称
        /// </summary>
        public Enums.FeeTypeEnum FeeType { get; set; }

        /// <summary>
        /// 其它
        /// </summary>
        public string FeeDesc { get; set; } = string.Empty;

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 申请费用状态
        /// </summary>
        public Enums.CostStatusEnum CostStatus { get; set; }

        /// <summary>
        /// 申请费用状态(int)
        /// </summary>
        public int CostStatusInt { get; set; }

        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string AdminID { get; set; } = string.Empty;

        /// <summary>
        /// 纸质票据状态
        /// </summary>
        public Enums.CheckPaperNotesEnum PaperNotesStatus { get; set; }

        /// <summary>
        /// 纸质票据状态(Int)
        /// </summary>
        public int PaperNotesStatusInt { get; set; }


        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplies>().Count(item => item.ID == this.ID);
                if (count != 0)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplies>(new
                    {
                        UpdateDate = this.CreateDate,
                        //CreateDate = this.CreateDate,
                        PaperNotesStatus = this.PaperNotesStatusInt
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
    }
}
