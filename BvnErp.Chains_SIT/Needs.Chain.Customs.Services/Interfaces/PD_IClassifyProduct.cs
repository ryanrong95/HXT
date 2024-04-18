using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Underly;
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
    public interface PD_IClassifyProduct : IProduct
    {
        #region 事件

        /// <summary>
        /// 当产品归类完成时发生
        /// </summary>
        event ProductClassifiedHanlder Classified;

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
        /// 锁定
        /// </summary>
        /// <returns></returns>
        JMessage Lock();

        /// <summary>
        /// 解锁
        /// </summary>
        /// <returns></returns>
        JMessage UnLock();

        /// <summary>
        /// 退回
        /// </summary>
        void Return();

        /// <summary>
        /// 删除
        /// </summary>
        void Delete();
    }
}
