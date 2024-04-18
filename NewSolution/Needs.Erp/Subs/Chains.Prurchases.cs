//using Needs.Erp.Generic;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Needs.Erp
//{
//    /// <summary>
//    /// 供应链 Purchase
//    /// </summary>
//    public class Purchases
//    {
//        IGenericAdmin Admin;

//        public Purchases(IGenericAdmin admin)
//        {
//            this.Admin = admin;
//        }

//        /// <summary>
//        /// 采购订单
//        /// </summary>
//        public Needs.Cps.Services.Views.OrdersView Orders
//        {
//            get
//            {
//                return new Needs.Cps.Services.Views.OrdersView(this.Admin);
//            }
//        }

//        /// <summary>
//        /// 采购订单Item
//        /// </summary>
//        public Needs.Cps.Services.Views.OrderItemsView OrderItems
//        {
//            get
//            {
//                return new Needs.Cps.Services.Views.OrderItemsView();
//            }
//        }

//        /// <summary>
//        /// 采购水单
//        /// </summary>
//        public Needs.Cps.Services.Views.FormsView Forms
//        {
//            get
//            {
//                return new Needs.Cps.Services.Views.FormsView();
//            }
//        }

//        public Needs.Cps.Services.Views.PremiumsView Premiums
//        {
//            get
//            {
//                return new Needs.Cps.Services.Views.PremiumsView();
//            }
//        }
//    }
//}
