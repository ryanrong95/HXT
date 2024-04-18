using Needs.Ccs.Services;
using Needs.Utils.Serializers;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Wl.CustomsTool.WinForm.Models.Messages;
using Needs.Wl.CustomsTool.WinForm.Models;
using Needs.Wl.CustomsTool.WinForm.Models.Hanlders;
using System.IO;

namespace Needs.Wl.CustomsTool.WinForm.Business
{
    public class ManiReceipt
    {
        private Models.Messages.Manifest Manifest { get; set; }

        /// <summary>
        /// 舱单（运单）
        /// </summary>
        private ManifestConsignment ManifestConsignment { get; set; }

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

        public ManiReceipt(string path)
        {
            this.MftResponseNormal += Manifest_MftResponseNormal;
            this.MftResponseSucceed += Manifest_MftResponseSucceed;
            this.MftResponseTrans += Manifest_MftResponseTrans;
            var xml = System.Text.RegularExpressions.Regex.Replace(XmlService.LoadXmlFile(path), @"(xmlns:?[^=]*=[""][^""]*[""])|(ns2:)", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            this.Manifest = XmlSerializerExtend.XmlTo<Models.Messages.Manifest>(xml);
            this.ManifestConsignment = Tool.Current.Customs.Manifests.Where(item=>item.ID== this.Manifest.Head.MessageID).FirstOrDefault();
            this.FilePath = path;
        }

        //日志记录
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void HandleManifestResponse()
        {
            try
            {
                Logger.Trace("-------------------");

                //导入失败回执，只可通过文件名取MessageID
                if (string.IsNullOrEmpty(this.Manifest.Head.MessageID))
                {
                    this.Manifest.Head.MessageID = FileName.Split('_')[2];
                }
                //舱单删除报文ID调整，以便正确读取
                this.Manifest.Head.MessageID = this.Manifest.Head.MessageID.Replace("DEL", string.Empty);

                Logger.Trace("获取舱单回执:" + this.Manifest.Head.MessageID);
                //content

                this.FileName = Path.GetFileName(this.FilePath);
                this.BackupUrl = Tool.Current.Folder.RmftMainFolder + @"\" + ConstConfig.InBox_BK + @"\" + this.FileName;

                //addition为空：传输回执
                if (this.Manifest.Response.AdditionalInformation == null && this.Manifest.Response.Consignment == null)
                {
                    //发往海关成功
                    Logger.Trace("舱单回执内容:" + this.Manifest.Head.MessageID + " FunctionCode: " + this.Manifest.Head.FunctionCode
                        + " ResponseCode: " + this.Manifest.Response.ResponseType.Code
                        + " Description: " + this.Manifest.Response.ResponseType.Text);

                    SaveTransAs();
                }
                else if (this.Manifest.Response.Consignment != null)
                {
                    //发往海关成功
                    Logger.Trace("舱单回执内容:" + this.Manifest.Head.MessageID + " FunctionCode: " + this.Manifest.Head.FunctionCode
                        + " ResponseCode: " + this.Manifest.Response.Consignment[0].AdditionalInformation.StatementCode.Value.GetXmlEnumAttributeValueFromEnum()
                        + " Description: " + this.Manifest.Response.Consignment[0].AdditionalInformation.StatementDescription);

                    ResponseSuccess();
                }
                else
                {
                    //调用成功
                    Logger.Trace("舱单回执内容:" + this.Manifest.Head.MessageID + " FunctionCode: " + this.Manifest.Head.FunctionCode
                        + " StatementCode: " + this.Manifest.Response.AdditionalInformation.StatementCode.Value.GetXmlEnumAttributeValueFromEnum()
                        + " Description: " + this.Manifest.Response.AdditionalInformation.StatementDescription);

                    ResponseNornmal();
                }

                Logger.Trace("-------------------");

            }
            catch (Exception ex)
            {
                Logger.Error("获取舱单导入回执失败：" + ex.Message);
            }
        }
        private event MftResponseNormalHanlder MftResponseNormal;
        private event MftResponseRefusedHanlder MftResponseRefused;
        private event MftResponseSucceedHanlder MftResponseSucceed;
        private event MftResponseTransHanlder MftResponseTrans;
        private event MftResponseErrorHanlder MftResponseError;

        #region 海关传输

        /// <summary>
        /// 保存传输回执
        /// </summary>
        private void SaveTransAs()
        {
            if (this.ManifestConsignment.CusMftStatus != MultiEnumUtils.ToCode<Ccs.Services.Enums.CusMftStatus>(Ccs.Services.Enums.CusMftStatus.Deleted))
            {
                this.ManifestConsignment.CusMftStatus = this.Manifest.Response.ResponseType.Code;

                //更新舱单状态
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.ManifestConsignment.CusMftStatus }, item => item.ID == this.ManifestConsignment.ID);
                }
            }

            this.OnResponseTrans(new MftResponseTransEventArgs(this.Manifest));
        }
        public virtual void OnResponseTrans(MftResponseTransEventArgs args)
        {
            this.MftResponseTrans?.Invoke(this, args);
        }
        private void Manifest_MftResponseTrans(object sender, MftResponseTransEventArgs e)
        {
            this.ManifestConsignment.Trace(e.Manifest.Response.ResponseType.Text,
                DateTime.ParseExact(e.Manifest.Head.SendTime.Value, "yyyyMMddHHmmssfff", System.Globalization.CultureInfo.CurrentCulture), this.FileName, this.FilePath, this.BackupUrl);
        }

        #endregion

        #region 海关回执成功

        private void ResponseSuccess()
        {
            if (this.ManifestConsignment.CusMftStatus != MultiEnumUtils.ToCode<Ccs.Services.Enums.CusMftStatus>(Ccs.Services.Enums.CusMftStatus.Deleted))
            {
                this.ManifestConsignment.CusMftStatus = this.Manifest.Response.Consignment[0].AdditionalInformation.StatementCode.Value.GetXmlEnumAttributeValueFromEnum();

                //更新舱单状态
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.ManifestConsignment.CusMftStatus }, item => item.ID == this.ManifestConsignment.ID);
                }
            }

            this.OnResponseSucceed(new MftResponseSucceedEventArgs(this.Manifest));
        }
        public virtual void OnResponseSucceed(MftResponseSucceedEventArgs args)
        {
            this.MftResponseSucceed?.Invoke(this, args);
        }
        private void Manifest_MftResponseSucceed(object sender, MftResponseSucceedEventArgs e)
        {
            this.ManifestConsignment.Trace(e.Manifest.Response.Consignment[0].AdditionalInformation.StatementDescription,
                DateTime.ParseExact(e.Manifest.Head.SendTime.Value, "yyyyMMddHHmmssfff", System.Globalization.CultureInfo.CurrentCulture), this.FileName, this.FilePath, this.BackupUrl);
        }

        #endregion

        #region 海关回执正常

        private void ResponseNornmal()
        {
            if (this.ManifestConsignment.CusMftStatus != MultiEnumUtils.ToCode<Ccs.Services.Enums.CusMftStatus>(Ccs.Services.Enums.CusMftStatus.Deleted))
            {
                this.ManifestConsignment.CusMftStatus = this.Manifest.Response.AdditionalInformation.StatementCode.Value.GetXmlEnumAttributeValueFromEnum();

                //更新舱单状态
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.ManifestConsignment.CusMftStatus }, item => item.ID == this.ManifestConsignment.ID);
                }
            }

            this.OnResponseNornmal(new MftResponseNormalEventArgs(this.Manifest));
        }
        public virtual void OnResponseNornmal(MftResponseNormalEventArgs args)
        {
            this.MftResponseNormal?.Invoke(this, args);
        }
        private void Manifest_MftResponseNormal(object sender, MftResponseNormalEventArgs e)
        {
            this.ManifestConsignment.Trace(e.Manifest.Response.AdditionalInformation.StatementDescription,
                DateTime.ParseExact(e.Manifest.Head.SendTime.Value, "yyyyMMddHHmmssfff", System.Globalization.CultureInfo.CurrentCulture), this.FileName, this.FilePath, this.BackupUrl);
        }

        #endregion

        private static void ManifestBill_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Logger.Trace("舱单回执更新报关单成功");
        }

        private static void ManifestBill_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Logger.Trace("舱单回执更新报关单失败");
        }
    }
}
