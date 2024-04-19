//using NtErp.Wss.Oss.Services.Models;
//using NtErp.Wss.Oss.Services.Views;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NtErp.Wss.Oss.Services.Extends
//{
//    static public class OrderItemsExtends
//    {
        
//        /// <summary>
//        /// 订单项-修改
//        /// </summary>
//        /// <param name="entity">新订单项 (id 不能变)</param>
//        /// <param name="adminID">操作人</param>
//        /// <param name="summary">备注</param>
//        static public void Change(this OrderItem entity, string adminID, string summary)
//        {
//            var id = entity.ID;
//            using (var alls = new OrderAlls())
//            {
//                var order = alls.Single(t => t.ID == entity.OrderID);
//                var orderitem = order.Items.Single(t => t.ID == id);
//                orderitem.Status = OrderItemStatus.Altered;
//                orderitem.Enter();
//            }

//            entity.ID = "";
//            entity.Enter();

         
//        }
//        /// <summary>
//        /// 订单项-增加附加价值
//        /// </summary>
//        /// <param name="entity">订单项</param>
//        /// <param name="adminID">操作人</param>
//        /// <param name="summary">备注</param>
//        static public void Add(this OrderItem entity, string adminID, string summary)
//        {
//            entity.Enter();

         
//        }

//        /// <summary>
//        /// 订单项-增加附加价值
//        /// </summary>
//        /// <param name="entity">订单项</param>
//        /// <param name="premium">附加价值</param>
//        /// <param name="adminID">操作人</param>
//        /// <param name="summary">备注</param>
//        static public void AddPremium(this OrderItem entity, Premium premium, string adminID, string summary)
//        {
//            premium.Enter();

           
//        }

//        /// <summary>
//        /// 订单-增加附加价值
//        /// </summary>
//        /// <param name="entity">订单</param>
//        /// <param name="premium">附加价值</param>
//        /// <param name="adminID">操作人</param>
//        /// <param name="summary">备注</param>
//        static public void AddPremium(this Order entity, Premium premium, string adminID, string summary)
//        {
//            premium.Enter();

           
           
//        }

//        /// <summary>
//        /// 订单项-发货
//        /// </summary>
//        /// <param name="entity">订单项</param>
//        /// <param name="adminID">操作人</param>
//        /// <param name="summary">备注</param>
//        static public void Delivery(this Order entity, Premium premium, string adminID, string summary)
//        {
//            //premium.Enter();
 
//        }

//    }
//}
