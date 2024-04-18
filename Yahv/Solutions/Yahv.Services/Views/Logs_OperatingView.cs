using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 操作日志视图
    /// </summary>
    /// <remarks>
    /// 这样写也是为方便各个系统有自己的日志
    /// </remarks>
    public class Logs_OperatingTopView<TReponsitory> : UniqueView<Log_Operating, TReponsitory>
           where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Logs_OperatingTopView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Logs_OperatingTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 结果查询
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Log_Operating> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_OperatingTopView>()
                   select new Log_Operating
                   {
                       ID = entity.ID,
                       Type = (Enums.LogType)entity.Type,
                       MainID = entity.MainID,
                       Operation = entity.Operation,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       Creator = entity.Creator
                   };
        }

        /// <summary>
        /// 持久化
        /// </summary>
        ///<remarks>
        /// 就是Enter ，这里用Add or  Insert 等就是表示只能增加日志
        /// </remarks>
        virtual public void Add(ILog_Operatring log)
        {
            //操作日志插入数据库
            this.Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_Operating
            {
                ID = log.ID ?? Guid.NewGuid().ToString(),
                Type = (int)log.Type,
                MainID = log.MainID,
                Operation = log.Operation,
                Summary = log.Summary,
                CreateDate = log.CreateDate,
                Creator = log.Creator
            });
        }
    }


    /// <summary>
    /// 操作日志视图
    /// </summary>
    /// <remarks>
    /// 这样写也是为方便各个系统有自己的日志
    /// </remarks>
    public class Logs_OperatingTopViewTest : UniqueView2<Log_Operating, Layers.Linq.IReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Logs_OperatingTopViewTest() : this(new PvCenterReponsitory())
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Logs_OperatingTopViewTest(Layers.Linq.IReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 结果查询
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Log_Operating> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_OperatingTopView>()
                   select new Log_Operating
                   {
                       ID = entity.ID,
                       Type = (Enums.LogType)entity.Type,
                       MainID = entity.MainID,
                       Operation = entity.Operation,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       Creator = entity.Creator
                   };
        }

        /// <summary>
        /// 持久化
        /// </summary>
        ///<remarks>
        /// 就是Enter ，这里用Add or  Insert 等就是表示只能增加日志
        /// </remarks>
        virtual public void Add(ILog_Operatring log)
        {
            //操作日志插入数据库
            this.Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_Operating
            {
                ID = log.ID,
                Type = (int)log.Type,
                MainID = log.MainID,
                Operation = log.Operation,
                Summary = log.Summary,
                CreateDate = log.CreateDate,
                Creator = log.Creator
            });
        }
    }
}
