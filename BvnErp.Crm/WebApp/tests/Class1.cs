using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.tests
{

    /// <summary>
    /// 审核状态
    /// </summary>
    public enum AuditStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        Waiting = 1,
        /// <summary>
        /// 审核中
        /// </summary>
        Doing = 100,
        /// <summary>
        /// 通过
        /// </summary>
        Passed = 200,
        /// <summary>
        /// 否决
        /// </summary>
        Vetoed = 500
    }

    /// <summary>
    /// 审核接口
    /// </summary>
    public interface IAudited
    {
        /// <summary>
        /// 审核状态
        /// </summary>
        AuditStatus Audited { get; }

        /// <summary>
        /// 通过
        /// </summary>
        /// <remarks>
        /// 当完成全部审批流程并审核通过后调用本方法
        /// </remarks>
        void Allow();

        /// <summary>
        /// 否决
        /// </summary>
        /// <remarks>
        /// 当审核中有任意一步出现否决的，调用本方法
        /// </remarks>
        void Veto();
    }

    public class Class1
    {
    }
}