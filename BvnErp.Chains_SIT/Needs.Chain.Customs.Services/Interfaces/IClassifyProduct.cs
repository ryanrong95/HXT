using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Interfaces
{
    /// <summary>
    /// 归类产品接口
    /// </summary>
    public interface IClassifyProduct : IProduct
    {
        #region 事件

        /// <summary>
        /// 当产品归类锁定时发生
        /// </summary>
        event ProductLockedHanlder Locked;

        /// <summary>
        /// 当解除产品归类锁定时发生
        /// </summary>
        event ProductLockedHanlder UnLocked;

        /// <summary>
        /// 当产品归类完成时发生
        /// </summary>
        event ProductClassifiedHanlder Classified;

        /// <summary>
        /// 当产品归类异常时发生
        /// </summary>
        event ProductClassifiedHanlder Anomalied;

        #endregion

        #region 属性

        /// <summary>
        /// 操作人/报关员
        /// </summary>
        Admin Admin { get; set; }

        /// <summary>
        /// 归类状态
        /// </summary>
        Enums.ClassifyStatus ClassifyStatus { get; set; }

        /// <summary>
        /// 归类阶段
        /// </summary>
        Enums.ClassifyStep ClassifyStep { get; set; }

        #endregion

        /// <summary>
        /// 归类
        /// </summary>
        void DoClassify();

        /// <summary>
        /// 一键归类
        /// </summary>
        void QuickClassify();

        /// <summary>
        /// 归类锁定
        /// </summary>
        void Lock();

        /// <summary>
        /// 归类解锁
        /// </summary>
        void UnLock();

        /// <summary>
        /// 归类异常
        /// </summary>
        void Anomaly();
    }
}
