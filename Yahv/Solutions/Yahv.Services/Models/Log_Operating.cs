using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Enums;

namespace Yahv.Services.Models
{
    public interface ILog_Operatring
    {
        string ID { get; }

        LogType Type { get; }

        string MainID { get; }

        string Operation { get; }

        string Summary { get; }

        string Creator { get; }

        DateTime CreateDate { get; }
    }

    public class OperatingLog
    {
        /// <summary>
        /// 操作内容
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// 关联ID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 返回固定的数据库实现接口
        /// </summary>
        /// <returns>链接对象</returns>
        //abstract internal Views.Logs_OperatingTopViewTest GetView();
    }


    /// <summary>
    /// 日志
    /// </summary>
    /// <remarks>
    /// 利用统一接口(ILog_Operatring)或是基础类(OperatingLog)
    /// </remarks>
    public class Log_Operating : OperatingLog, IUnique, ILog_Operatring
    {
        public Log_Operating()
        {
            this.CreateDate = DateTime.Now;
        }

        #region 基础属性
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 可以设置自己的类型
        /// </summary>
        /// <remarks>
        /// 日志类型如果用枚举最好使用indexer,这是固定的开发模式
        /// </remarks>
        public LogType Type { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion

        #region 方法
        //override internal Views.Logs_OperatingTopViewTest GetView()
        //{
        //    return new Views.Logs_OperatingTopViewTest(new Layers.Data.Sqls.PvCenterReponsitory());
        //}
        #endregion
    }
}
