using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;
using Needs.Wl.CustomsTool.WinForm.App_Utils;
using Needs.Wl.CustomsTool.WinForm.Models;
using Needs.Wl.CustomsTool.WinForm.Models.Hanlders;
using Needs.Wl.CustomsTool.WinForm.Models.Messages;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    /// <summary>
    /// 报关暂存回执
    /// </summary>
    public class DecSuccess
    {
        private DecHead DecHead { get; set; }

        private DecImportResponse DecImportResponse { get; set; }

        public DecSuccess(string path)
        {
            this.DecResponseSucceed += DecImportResponse_DecResponseSucceed;
            this.DecImportResponse = XmlSerializerExtend.XmlTo<DecImportResponse>(XmlService.LoadXmlFile(path));
            this.DecHead = Tool.Current.Customs.DecHeads[DecImportResponse.ClientSeqNo];
            this.FilePath = path;
            this.FileName = Path.GetFileName(path);
        }

        /// <summary>
        /// 响应时间
        /// </summary>
        private DateTime? ResponseTime { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 备份地址
        /// </summary>
        public string BackupUrl { get; set; }

        //日志记录
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 处理回执
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FullPath"></param>
        public void HandleDecHeadResponse()
        {
            try
            {
                //暂存回执
                Logger.Trace("-------------------");
                Logger.Trace("获取报关单导入响应回执:" + this.DecImportResponse.ClientSeqNo);
                //content
                Logger.Trace("报关单响应回执内容:" + this.DecImportResponse.ClientSeqNo + " Code: " + this.DecImportResponse.ResponseCode + " Message: " + this.DecImportResponse.ErrorMessage);
                var filenName = Path.GetFileName(this.FilePath);
                var responsTime = filenName.Substring(filenName.LastIndexOf("_") + 1, 14);
                this.ResponseTime = DateTime.ParseExact(responsTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                this.FileName = FileName;
                this.BackupUrl = Tool.Current.Folder.DecMainFolder + @"\" + ConstConfig.InBox_BK + @"\" + this.FileName;
                this.SaveAs();
            }
            catch (Exception ex)
            {
                Logger.Error("获取报关单导入回执失败：" + ex.Message);
            }
        }

        private void SaveAs()
        {
            switch (this.DecImportResponse.ResponseCode)
            {
                case "0":
                    ResponseSucceed();
                    break;
                case "1":
                    ResponseFailed();
                    ResponseFailedDraft();
                    break;
                default:
                    break;
            }
        }

        private event DecResponseSucceedHanlder DecResponseSucceed;

        private event DecResponseFailedHanlder DecResponseFailed;

        private event DecResponseFailedHanlder DecResponseFailedDraft;

        /// <summary>
        /// 报文响应成功
        /// </summary>
        private void ResponseSucceed()
        {
            this.DecHead.CusDecStatus = MultiEnumUtils.ToCode<CusDecStatus>(CusDecStatus.S0);
            this.DecHead.SeqNo = this.DecImportResponse.SeqNo;

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.DecHead.CusDecStatus, this.DecHead.SeqNo, this.DecHead.EntryId, this.DecHead.DDate }, item => item.ID == this.DecHead.ID);
            }
            this.OnResponseSucceed(new DecResponseSucceedEventArgs(this.DecImportResponse));
        }

        public virtual void OnResponseSucceed(DecResponseSucceedEventArgs args)
        {
            this.DecResponseSucceed?.Invoke(this, args);
        }

        private void DecImportResponse_DecResponseSucceed(object sender, DecResponseSucceedEventArgs e)
        {
            //报关单轨迹
            this.DecHead.Trace(e.DecImportResponse.ErrorMessage, ResponseTime.Value, this.FileName, this.FilePath, this.BackupUrl);
        }

        /// <summary>
        /// 报文响应失败
        /// </summary>
        private void ResponseFailed()
        {
            //暂存失败，状态改为草稿，这样可以再编辑
            this.DecHead.CusDecStatus = MultiEnumUtils.ToCode<CusDecStatus>(CusDecStatus.F1);

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.DecHead.CusDecStatus, }, item => item.ID == this.DecHead.ID);
            }

            this.OnResponseFailed(new DecResponseFailedEventArgs(this.DecImportResponse));
        }

        public virtual void OnResponseFailed(DecResponseFailedEventArgs args)
        {
            DecResponseFailed?.Invoke(this, args);
        }

        private void DecImportResponse_DecResponseFailed(object sender, DecResponseFailedEventArgs e)
        {
            //报关单轨迹
            this.DecHead.Trace(e.DecImportResponse.ErrorMessage,this.ResponseTime.Value, this.FileName, this.FilePath, this.BackupUrl);
        }

        public void ResponseFailedDraft()
        {
            //暂存失败，状态改为草稿，这样可以再编辑
            this.DecHead.CusDecStatus = MultiEnumUtils.ToCode<CusDecStatus>(CusDecStatus.Draft);

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.DecHead.CusDecStatus, }, item => item.ID == this.DecHead.ID);
            }

            this.OnResponseFailed(new DecResponseFailedEventArgs(this.DecImportResponse));
        }

        public virtual void OnResponseFailedDraft(DecResponseFailedEventArgs args)
        {
            this.DecResponseFailedDraft?.Invoke(this, args);
        }

        private void DecImportResponse_DecResponseFailedDraft(object sender, DecResponseFailedEventArgs e)
        {
            //报关单轨迹
            this.DecHead.Trace("暂存失败，状态变更为草稿");
        }
    }
}
