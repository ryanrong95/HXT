using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    public class DecSub
    {
        /// <summary>
        /// 用于转换
        /// </summary>
        private DecMessage decMessage { get; set; }

        /// <summary>
        /// 海关订阅回执转换为系统可操作对象
        /// </summary>
        private DecHead DecHead { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        private string FilePath { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// 备份路径
        /// </summary>
        private string BackupUrl { get; set; }

        public DecSub(string path)
        {
            this.decMessage = XmlSerializerExtend.XmlTo<DecMessage>(XmlService.LoadXmlFile(path));
            this.DecHead = Tool.Current.Customs.DecHeads.Where(item => item.SeqNo == decMessage.DecHead.SeqNo).FirstOrDefault();
            this.FilePath = path;
        }

        //日志记录
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 处理订阅报文
        /// </summary>
        public void HandleDecSub() 
        {
            try
            {
                //业务回执
                Logger.Trace("-------------------");
                Logger.Trace("获取报关单订阅报文:" + this.decMessage.DecHead.SeqNo);

                this.FileName = Path.GetFileName(this.FilePath);
                this.BackupUrl = Tool.Current.Folder.OtherMainFolder + @"\" + ConstConfig.InBox_BK + @"\" + ConstConfig.DecSub + @"\" + this.FileName;
                this.SaveDecSub();
                Logger.Trace("-------------------");
            }
            catch (Exception ex)
            {
                Logger.Error("获取报关单订阅报文：" + ex.Message);
                Logger.Error("-------------------");
                Logger.Error("StackTrace:" + ex.StackTrace);
                Logger.Error("InnerException:" + ex.InnerException);
                Logger.Error("========================================================");
            }

        }

        /// <summary>
        /// 订阅报文同步进系统数据
        /// </summary>
        public void SaveDecSub() 
        {
            this.DecHead.IEDate = this.decMessage.DecHead.IEDate;
            this.DecHead.DDate = DateTime.ParseExact(this.decMessage.DecHead.DDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            this.DecHead.NoteS = this.decMessage.DecHead.NoteS;
            this.DecHead.EntryId = this.decMessage.DecHead.EntryId;
            this.DecHead.PreEntryId = this.decMessage.DecHead.PreEntryId;

            this.DecHead.EnterSuccess += DecHead_EnterSuccess;
            this.DecHead.Enter();
        }

        private void DecHead_EnterSuccess(object sender, Linq.SuccessEventArgs e)
        {
            this.DecHead.Trace("报关单数据订阅：" + this.decMessage.DecHead.NoteS + " 申报日期："+ this.decMessage.DecHead.DDate + " 进口日期：" + this.decMessage.DecHead.IEDate,
                DateTime.Now, this.FileName, this.FilePath, this.BackupUrl);
        }
    }
}
