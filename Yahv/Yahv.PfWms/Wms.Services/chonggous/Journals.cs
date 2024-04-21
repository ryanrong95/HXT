//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Linq;

//namespace Wms.Services.chonggous
//{
//    /// <summary>
//    /// 日志视图
//    /// </summary>
//    /// <typeparam name="T">视图类型</typeparam>
//    public class JournalsView<T> : QueryView<T, PvWmsRepository> where T : class, IJournal
//    {
//        protected override IQueryable<T> GetIQueryable()
//        {
//            if (typeof(T) == typeof(Operating))
//            {
//                return from log in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Logs_Operating>()
//                       select new Operating
//                       {
//                           ID = log.ID,
//                           WaybillID = log.WaybillID,
//                           OrderID = log.OrderID,
//                           TinyOrderID = log.TinyOrderID,
//                           OrderItemID = log.OrderItemID,
//                           SortingID = log.SortingID,
//                           PickingID = log.PickingID,
//                           StorageID = log.StorageID,
//                           Context = log.Context,
//                           CreateDate = log.CreateDate,
//                           AdminID = log.AdminID,
//                       } as T;
//            }

//            return default(IQueryable<T>);
//        }
//    }

//    /// <summary>
//    /// 日志管理
//    /// </summary>
//    public class Journals
//    {
//        Journals()
//        {

//        }

//        static Journals current;
//        static object locker = new object();

//        /// <summary>
//        /// 当前引用
//        /// </summary>
//        static public Journals Current
//        {
//            get
//            {
//                if (current == null)
//                {
//                    lock (locker)
//                    {
//                        if (current == null)
//                        {
//                            current = new Journals();
//                        }
//                    }
//                }
//                return current;
//            }
//        }

//        /// <summary>
//        /// 日志视图
//        /// </summary>
//        /// <typeparam name="T">视图类型</typeparam>
//        /// <returns>视图</returns>
//        /// <remarks>需要释放</remarks>
//        static public JournalsView<T> View<T>() where T : class, IJournal
//        {
//            return new JournalsView<T>();
//        }

//        /// <summary>
//        /// 保存操作日志
//        /// </summary>
//        /// <param name="log">操作日志对象</param>
//        public void Write(EnterOperating log)
//        {
//            using (var reponsitory = new PvWmsRepository())
//            {
//                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Operating
//                {
//                    ID = Guid.NewGuid().ToString(),
//                    WaybillID = log.WaybillID,
//                    OrderID = log.OrderID,
//                    TinyOrderID = log.TinyOrderID,
//                    OrderItemID = log.OrderItemID,
//                    SortingID = log.SortingID,
//                    PickingID = log.PickingID,
//                    StorageID = log.StorageID,
//                    Context = log.Context,
//                    AdminID = log.AdminID,
//                    CreateDate = DateTime.Now,
//                });
//            }
//        }

//        /// <summary>
//        /// 操作哦日志
//        /// </summary>
//        public class EnterOperating
//        {
//            public string WaybillID { get; set; }
//            public string OrderID { get; set; }
//            public string TinyOrderID { get; set; }
//            public string OrderItemID { get; set; }
//            public string SortingID { get; set; }
//            public string PickingID { get; set; }
//            public string StorageID { get; set; }
//            public string Context { get; set; }
//            public string AdminID { get; set; }
//        }
//    }


//    /// <summary>
//    /// 库房接口
//    /// </summary>
//    public interface IJournal
//    {

//    }

//    /// <summary>
//    /// 操作日志
//    /// </summary>
//    public class Operating : IJournal
//    {
//        public string ID { get; internal set; }
//        public string WaybillID { get; internal set; }
//        public string OrderID { get; internal set; }
//        public string TinyOrderID { get; internal set; }
//        public string OrderItemID { get; internal set; }
//        public string SortingID { get; internal set; }
//        public string PickingID { get; internal set; }
//        public string StorageID { get; internal set; }
//        public string Context { get; internal set; }
//        public DateTime CreateDate { get; internal set; }
//        public string AdminID { get; internal set; }
//    }
//}
