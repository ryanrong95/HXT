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
    ///  订单管控审批流程
    /// </summary>
    public class OrderControlStep : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性

        string id;
        public string ID
        {
            get
            {
                //主键ID（OrderControlID+Step的MD5值）
                return this.id ?? string.Concat(this.OrderControlID, this.Step).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 订单管控ID
        /// </summary>
        public string OrderControlID { get; set; }

        /// <summary>
        /// 审核步骤/审核层级：北京总部、跟单员
        /// </summary>
        public Enums.OrderControlStep Step { get; set; }

        /// <summary>
        /// 管控状态
        /// </summary>
        public Enums.OrderControlStatus ControlStatus { get; set; }

        /// <summary>
        /// 审核人ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        public OrderControlStep()
        {
            this.ControlStatus = Enums.OrderControlStatus.Auditing;
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
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
