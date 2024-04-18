using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models
{
    public static class DecTraceExtends
    {
        /// <summary>
        /// 写入报关单日志
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void Trace(this DecHead head, string message, DateTime? noticeDate, string fileName, string filePath, string backupUrl)
        {
            DecTrace trace = new DecTrace();
            trace.DeclarationID = head.ID;
            trace.Channel = head.CusDecStatus;
            trace.Message = message;
            trace.NoticeDate = noticeDate.HasValue ? noticeDate.Value : DateTime.Now;
            trace.FilePath = filePath;
            trace.FileName = fileName;
            trace.BackupUrl = backupUrl;
            trace.EnterSuccess += Trace_EnterSuccess;
            trace.Enter();
        }

        private static void Trace_EnterSuccess(object sender, Linq.SuccessEventArgs e)
        {
            
            var decTrace = sender as DecTrace;
            File.Move(decTrace.FilePath, decTrace.BackupUrl);
        }

        /// <summary>
        /// 写入报关单日志
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void Trace(this DecHead head, string message)
        {
            DecTrace trace = new DecTrace();
            trace.DeclarationID = head.ID;
            trace.Channel = head.CusDecStatus;
            trace.Message = message;
            trace.NoticeDate = DateTime.Now;
            trace.Enter();
        }
    }
}
