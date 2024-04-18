using Layers.Data.Sqls;
using Layers.Linq;
using System;
using Yahv.Underly.Erps;

namespace Yahv
{
    /// <summary>
    /// 操作日志开发
    /// </summary>
    /// <remarks>自行建立数据表</remarks>
    static public class Oplogs
    {
        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="admin">当前登录人</param>
        /// <param name="url">操作页面</param>
        /// <param name="sys">操作子系统</param>
        /// <param name="type">操作类型</param>
        /// <param name="operation">操作内容</param>
        /// <param name="remark">备注</param>
        /// <example>
        /// 调用示例：
        /// Yahv.Erp.Current.Oplog(Request.Url, nameof(Yahv.Systematic.RFQ), Yahv.RFQ.Services.OplogType.CheckReject.ToString(), "", "不通过");
        /// </example>
        static public void Oplog(this IErpAdmin admin, string url, string sys, string type, string operation, string remark)
        {
            try
            {
                using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Oplogs
                    {
                        Url = url,
                        Sys = sys,
                        Type = type,
                        Operation = operation,
                        AdminID = admin.ID,
                        CreateDate = DateTime.Now,
                        Remark = remark
                    });
                }
            }
            catch
            {

            }
            
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="admin">当前登录人id</param>
        /// <param name="url">操作页面</param>
        /// <param name="sys">操作子系统</param>
        /// <param name="type">操作类型</param>
        /// <param name="operation">操作内容</param>
        /// <param name="remark">备注</param>
        /// <example>
        /// 调用示例：
        /// Yahv.Erp.Current.Oplog(AdminID, Request.Url, nameof(Yahv.Systematic.RFQ), Yahv.RFQ.Services.OplogType.CheckReject.ToString(), "", "不通过");
        /// </example>
        static public void Oplog(string adminID, string url, string sys, string type, string operation, string remark)
        {
            try
            {
                using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Oplogs
                    {
                        Url = url,
                        Sys = sys,
                        Type = type,
                        Operation = operation,
                        AdminID = adminID,
                        CreateDate = DateTime.Now,
                        Remark = remark
                    });
                }
            }
            catch
            {

            }

        }
    }


    /*
    操作类型type说明：
    数据类型：enum;
    目录：子系统Services命名空间下;
    参数：Yahv.RFQ.Services.OplogType.CheckReject.ToString();

    // 枚举示例：
    namespace Yahv.RFQ.Services
    {
        public enum OplogType
        {
            /// <summary>
            /// 询价提交
            /// </summary>
            InquirySumbit,
            /// <summary>
            /// 预判通过
            /// </summary>
            CheckSubmit,
            /// <summary>
            /// 预判打回
            /// </summary>
            CheckReject,
            /// <summary>
            /// 报价
            /// </summary>
            Quote,
             /// <summary>
            /// 审批不通过
            /// </summary>
            Reject,

            // ......
        }
    }
    */
}
