using System;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Underly.Erps;

namespace Yahv.Payments
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
                using (var reponsitory = new PvbErmReponsitory())
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

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="adminID">当前登录人id</param>
        /// <param name="url">操作页面</param>
        /// <param name="remark">自定义错误</param>
        /// <param name="ex">异常</param>
        /// <example>
        /// </example>
        static public void Logs_Error(string adminID, string url, Exception ex, string remark = "")
        {
            try
            {
                using (var reponsitory = new PvbErmReponsitory())
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Logs_Errors()
                    {
                        Codes = "",
                        AdminID = adminID,
                        CreateDate = DateTime.Now,
                        Message = ex.Message,
                        Page = url,
                        Source = ex.Source,
                        Stack = remark == "" ? ex.StackTrace : remark,
                    });
                }
            }
            catch
            {

            }

        }
    }
}