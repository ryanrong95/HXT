using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 仓库费用日志
    /// </summary>
    public class OrderWhesPremiumLog : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }

        public string OrderID { get; set; }

        public string OrderWhesPremiumID { get; set; }

        public Enums.WarehousePremiumType Type { get; set; }

        public Admin Admin { get; set; } 

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public OrderWhesPremiumLog()
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
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderWhesPremiumLog);
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderWhesPremiumLogs
                    {
                        ID = this.ID,
                        OrderID = this.OrderID,
                        OrderWhesPremiumID = this.OrderWhesPremiumID,
                        Type = (int)this.Type,
                        AdminID = this.Admin.ID,                  
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    });
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

    public class OrderWhesPremiumLogs : BaseItems<OrderWhesPremiumLog>
    {
        internal OrderWhesPremiumLogs(IEnumerable<OrderWhesPremiumLog> enums) : base(enums)
        {
        }

        internal OrderWhesPremiumLogs(IEnumerable<OrderWhesPremiumLog> enums, Action<OrderWhesPremiumLog> action) : base(enums, action)
        {
        }

        public override void Add(OrderWhesPremiumLog item)
        {
            base.Add(item);
        }
    }
}