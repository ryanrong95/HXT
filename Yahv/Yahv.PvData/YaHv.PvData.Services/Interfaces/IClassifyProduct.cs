using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using YaHv.PvData.Services.Handlers;

namespace YaHv.PvData.Services.Interfaces
{
    /// <summary>
    /// 归类产品
    /// </summary>
    public interface IClassifyProduct : IUnique
    {
        #region 事件

        /// <summary>
        /// 当产品归类锁定时发生
        /// </summary>
        event ProductLockedHandler Locked;

        /// <summary>
        /// 当解除产品归类锁定时发生
        /// </summary>
        event ProductLockedHandler UnLocked;

        /// <summary>
        /// 当产品归类完成时发生
        /// </summary>
        event ProductClassifiedHandler Classified;

        /// <summary>
        /// 当产品归类退回时发生
        /// </summary>
        event ProductReturnedHandler Returned;

        #endregion

        #region 属性

        /// <summary>
        /// 操作人/报关员
        /// </summary>
        //Admin Creator { get; set; }

        /// <summary>
        /// 操作人/报关员ID
        /// </summary>
        String CreatorID { get; set; }

        /// <summary>
        /// 操作人/报关员真实姓名
        /// </summary>
        string CreatorName { get; set; }

        /// <summary>
        /// 归类阶段
        /// </summary>
        ClassifyStep Step { get; set; }

        /// <summary>
        /// 报关员角色
        /// </summary>
        DeclarantRole Role { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        string Summary { get; set; }

        #endregion

        /// <summary>
        /// 归类
        /// </summary>
        void DoClassify();

        /// <summary>
        /// 归类锁定
        /// </summary>
        void Lock();

        /// <summary>
        /// 归类解锁
        /// </summary>
        void UnLock();

        /// <summary>
        /// 退回
        /// </summary>
        void Return();
    }
}
