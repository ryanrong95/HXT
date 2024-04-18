//using Needs.Linq;
//using Needs.Underly;
//using NtErp.Services.Models.Orders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NtErp.Services.Models
//{
//    /// <summary>
//    /// 订单
//    /// </summary>
//    public class Order : IUnique, IFulSuccess, IFulError, IPersistence
//    {
//        #region 属性
//        public string ID { get; set; }
//        /// <summary>
//        /// 用户ID
//        /// </summary>
//        public string UserID { get; set; }
//        /// <summary>
//        /// 用户名
//        /// </summary>
//        public string SiteUserName { set; get; }
//        public DateTime CreateDate { get; set; }
//        public DateTime UpdateDate { get; set; }
//        /// <summary>
//        /// 备注
//        /// </summary>
//        public string Summary { get; set; }
//        /// <summary>
//        /// 状态
//        /// </summary>
//        public OrderStatus Status { get; set; }
//        /// <summary>
//        /// 承运方式
//        /// </summary>
//        public TransportTerm Transport { get; set; }
//        /// <summary>
//        /// 交货区
//        /// </summary>
//        public District District { get; set; }
//        /// <summary>
//        /// 币种
//        /// </summary>
//        public Currency Currency { get; set; }
//        /// <summary>
//        /// 订单来源
//        /// </summary>
//        public OrderType Type { get; set; }

//        #endregion 

//        public event ErrorHanlder AbandonError;
//        public event SuccessHanlder AbandonSuccess;
//        public event ErrorHanlder EnterError;
//        public event SuccessHanlder EnterSuccess;

//        #region 持久化
//        public void Abandon()
//        {
//            throw new NotImplementedException();
//        }

//        public void Enter()
//        {
//            throw new NotImplementedException();
//        }
//        #endregion

//    }
//}
