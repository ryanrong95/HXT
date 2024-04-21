using System;
using System.Diagnostics;
using System.Reflection;
using Layers.Data;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;

namespace Yahv.Finance.Services
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
        /// <param name="adminID">当前登录人id</param>
        /// <param name="modular">操作模块</param>
        /// <param name="type">操作类型</param>
        /// <param name="operation">操作内容</param>
        /// <param name="remark">备注</param>
        static public void Oplog(string adminID, LogModular modular, string type, string operation, string remark, string url = "")
        {
            try
            {
                using (var reponsitory = new PvFinanceReponsitory())
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.Logs_Operation()
                    {
                        ID = PKeySigner.Pick(PKeyType.LogsOprate),
                        Modular = modular.ToString(),
                        Type = type,
                        Operation = operation,
                        CreatorID = adminID,
                        CreateDate = DateTime.Now,
                        Remark = remark,
                        Url = url
                    });
                }
            }
            catch
            {

            }

        }

        /// <summary>
        /// 获取方法名
        /// </summary>
        /// <returns></returns>
        public static string GetMethodInfo()
        {
            StackTrace ss = new StackTrace(true);
            MethodBase mb = ss.GetFrame(1).GetMethod();
            return mb.DeclaringType.FullName + "." + mb.Name;
        }
    }
}