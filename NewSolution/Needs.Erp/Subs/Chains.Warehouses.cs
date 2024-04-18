//using Needs.Erp.Generic;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Needs.Erp
//{
//    /// <summary>
//    /// 库房
//    /// </summary>
//    public class Warehouses
//    {
//        IGenericAdmin Admin;

//        public Warehouses(IGenericAdmin admin)
//        {
//            this.Admin = admin;
//        }

//        /// <summary>
//        /// 入库
//        /// </summary>
//        public InWarehouse InWarehouses
//        {
//            get { return new InWarehouse(this.Admin); }
//        }

//        /// <summary>
//        /// 分拣
//        /// </summary>
//        public Sorting Sorting
//        {
//            get { return new Sorting(this.Admin); }
//        }

//        /// <summary>
//        /// 拣货
//        /// </summary>
//        public Picking Picking
//        {
//            get { return new Picking(this.Admin); }
//        }

//        /// <summary>
//        /// 上架
//        /// </summary>
//        public InShelve InShelve
//        {
//            get { return new InShelve(this.Admin); }
//        }
//    }

//    /// <summary>
//    ///  战位 入库
//    /// </summary>
//    public class InWarehouse
//    {
//        IGenericAdmin Admin;

//        public InWarehouse(IGenericAdmin admin)
//        {
//            this.Admin = admin;
//        }

//        /// <summary>
//        /// 入库通知
//        /// </summary>
//        public Needs.Whs.Services.Views.MyEntryNoticesView MyEntryNotices
//        {
//            get
//            {
//                return new Needs.Whs.Services.Views.MyEntryNoticesView(this.Admin);
//            }
//        }
//    }

//    /// <summary>
//    ///  战位 出库
//    /// </summary>
//    public class OutWarehouse
//    {
//        IGenericAdmin Admin;

//        public OutWarehouse(IGenericAdmin admin)
//        {
//            this.Admin = admin;
//        }

//        /// <summary>
//        /// 出库通知
//        /// </summary>
//        public Needs.Whs.Services.Views.MyExitNoticesView MyEntryNotices
//        {
//            get
//            {
//                return new Needs.Whs.Services.Views.MyExitNoticesView(this.Admin);
//            }
//        }
//    }

//    /// <summary>
//    /// 战位  分拣人的战位
//    /// </summary>
//    public class Sorting
//    {
//        IGenericAdmin Admin;

//        public Sorting(IGenericAdmin admin)
//        {
//            this.Admin = admin;
//        }

//        /// <summary>
//        /// 分拣结果
//        /// </summary>
//        public Needs.Whs.Services.Views.SortingsView MySortings
//        {
//            get
//            {
//                return new Needs.Whs.Services.Views.MySortingsView(this.Admin);
//            }
//        }
//    }

//    /// <summary>
//    /// 战位  拣货人的战位
//    /// </summary>
//    public class Picking
//    {
//        IGenericAdmin Admin;

//        public Picking(IGenericAdmin admin)
//        {
//            this.Admin = admin;
//        }

//        /// <summary>
//        /// 拣货结果
//        /// </summary>
//        public Needs.Whs.Services.Views.MyPickingsView MyPickings
//        {
//            get
//            {
//                return new Needs.Whs.Services.Views.MyPickingsView(this.Admin);
//            }
//        }
//    }

//    /// <summary>
//    /// 战位  库内战位-上架
//    /// </summary>
//    public class InShelve
//    {
//        IGenericAdmin Admin;

//        public InShelve(IGenericAdmin admin)
//        {
//            this.Admin = admin;
//        }

//        /// <summary>
//        /// 分拣上架
//        /// </summary>
//        public Needs.Whs.Services.Views.SortingsView Sortings
//        {
//            get
//            {
//                return new Needs.Whs.Services.Views.SortingsView();
//            }
//        }
        
//        /// <summary>
//        /// 拣货上架
//        /// </summary>
//        public Needs.Whs.Services.Views.PickingsView Pickings
//        {
//            get
//            {
//                return new Needs.Whs.Services.Views.PickingsView();
//            }
//        }
//    }
//}