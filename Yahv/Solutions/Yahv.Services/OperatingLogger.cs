using Layers.Data.Sqls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Services
{
    /// <summary>
    /// 操作日志
    /// </summary>
    /// <remarks>
    /// 只提供中心日志的调用
    /// </remarks>
    public class OperatingLogger
    {
        LogType logType;

        protected OperatingLogger(LogType logType)
        {
            this.logType = logType;
        }

        string adminID;

        /// <summary>
        /// 管理端操作日志访问构造函数
        /// </summary>
        /// <param name="admin"></param>
        protected internal OperatingLogger(string AdminID) : this(LogType.Admin)
        {
            this.adminID = AdminID;
        }

        public OperatingLogger this[LogType index]
        {
            get
            {
                var logger = new OperatingLogger(adminID);
                logger.logType = index;
                return logger;
            }
        }

        /// <summary>
        /// 各业务操作日志调用方法
        /// </summary>
        /// <param name="logs"></param>
        public void Log(params OperatingLog[] logs)
        {
            //基于这类型  ILog_Operatring ， 反射 类似CenterLog_OperatingView 类
            //基于这类型  ILog_Operatring ， 反射 应该使用的 TReponsitory

            //也可以把这些东西开发为特性  例如 Log_Operatring
            //例如：我们只考虑 中心的 ，就直接实例化
            //静态调用，一般在释放上写显示调用。

            //var first = logs.First();
            //using (var view = first.GetView())


            using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
            using (var view = new Logs_OperatingTopView<PvCenterReponsitory>(reponsitory))
            {
                foreach (var log in logs.Select(item => new Log_Operating
                {
                    Type = this.logType,
                    MainID = item.MainID,
                    Operation = item.Operation,
                    Creator = this.adminID,
                    Summary = item.Summary,
                }))
                {
                    view.Add(log);
                }
                //这样写的目的是为了防止，日志重复
                //就好像订单一样，先判断OrderID是否存在
                //如果没有以上要求也可以直接开发为 reponsitory.insert
                //但是这样不利于  ILog_Operatring 的在各个 类似派生 CenterLog_OperatingView 多态 
            }
        }

        /// <summary>
        /// 系统日志,没有关联ID
        /// </summary>
        /// <param name="logs"></param>
        public void Log(params string[] logs)
        {
            //基于这类型  ILog_Operatring ， 反射 类似CenterLog_OperatingView 类
            //基于这类型  ILog_Operatring ， 反射 应该使用的 TReponsitory

            //也可以把这些东西开发为特性  例如 Log_Operatring
            //例如：我们只考虑 中心的 ，就直接实例化
            //静态调用，一般在释放上写显示调用。

            using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
            using (var view = new Logs_OperatingTopView<PvCenterReponsitory>(reponsitory))
            {
                foreach (var log in logs.Select(item => new Log_Operating
                {
                    Type = this.logType,
                    MainID = string.Empty,
                    Operation = item,
                    Creator = this.adminID,
                    Summary = string.Empty,
                }))
                {
                    view.Add(log);
                }
                //这样写的目的是为了防止，日志重复
                //就好像订单一样，先判断OrderID是否存在
                //如果没有以上要求也可以直接开发为 reponsitory.insert
                //但是这样不利于  ILog_Operatring 的在各个 类似派生 CenterLog_OperatingView 多态 
            }
        }
    }
}
