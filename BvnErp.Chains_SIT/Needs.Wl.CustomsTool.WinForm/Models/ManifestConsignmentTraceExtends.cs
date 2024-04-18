using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models
{
    public static class ManifestConsignmentTraceExtends
    {
        /// <summary>
        /// 写入舱单日志
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void Trace(this ManifestConsignment manifest, string message, DateTime? noticeDate, string fileName, string filePath, string backupUrl)
        {
            ManifestConsignmentTrace trace = new ManifestConsignmentTrace();
            trace.ManifestConsignmentID = manifest.ID;
            trace.StatementCode = manifest.CusMftStatus;
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
            var decTrace = sender as ManifestConsignmentTrace;
            File.Move(decTrace.FilePath, decTrace.BackupUrl);
        }
    }
}
