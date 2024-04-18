using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 代理订单轨迹
    /// </summary>
    [Serializable]
    public class OrderTrace : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性
        string id;
        public string ID
        {
            get
            {
                //主键ID
               return Needs.Overall.PKeySigner.Pick(PKeyType.OrderTrace);
            }
            set
            {
                this.id = value;
            }
        }

        public string OrderID { get; set; }

        /// <summary>
        /// 后台管理员
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 平台用户
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// 订单轨迹：提货、香港入库、香港装箱、香港出库、订单报关、深圳入库、深圳出库、客户收货
        /// </summary>
        public Enums.OrderTraceStep Step { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        public OrderTrace()
        {
            this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderTraces
                {
                    ID = this.ID,
                    OrderID = this.OrderID,
                    AdminID = this.Admin?.ID,
                    UserID = this.User?.ID,
                    Step = (int)this.Step,
                    CreateDate = this.CreateDate,
                    Summary = this.Summary
                });
            }

            this.OnEnterSuccess();
        }

        virtual protected void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
