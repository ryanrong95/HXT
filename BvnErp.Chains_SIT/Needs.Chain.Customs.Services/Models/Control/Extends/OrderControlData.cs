using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OrderControlData : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性

        string id;
        public string ID
        {
            get
            {
                if (!string.IsNullOrEmpty(this.id))
                {
                    return this.id;
                }
                else
                {
                    //主键ID（OrderID+OrderItemID+ControlType的MD5值）
                    return this.id ?? string.Concat(this.OrderID, this.OrderItemID, this.ControlType).MD5();
                }               
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// 管控类型
        /// </summary>
        public Enums.OrderControlType ControlType { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        public OrderControlData()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>().Count(item => item.ID == this.ID);

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
