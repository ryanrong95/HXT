using Needs.Ccs.Services.Hanlders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Needs.Ccs.Services.Models.ManifestMessage.Messages
{
    /// <summary>
    /// 舱单回执拓展
    /// </summary>
    public partial class Manifest
    {
        /// <summary>
        /// 拓展-回执文件路径
        /// </summary>
        [XmlIgnore]
        public string FilePath { get; set; }

        /// <summary>
        /// 拓展-回执文件名称
        /// </summary>
        [XmlIgnore]
        public string FileName { get; set; }

        /// <summary>
        /// 拓展-回执文件备份路径
        /// </summary>
        [XmlIgnore]
        public string BackupUrl { get; set; }

        /// <summary>
        /// 舱单（运单）
        /// </summary>
        [XmlIgnore]
        public ManifestConsignment ManifestConsignment { get; set; }


        public event MftResponseNormalHanlder MftResponseNormal;
        public event MftResponseRefusedHanlder MftResponseRefused;
        public event MftResponseSucceedHanlder MftResponseSucceed;
        public event MftResponseTransHanlder MftResponseTrans;
        public event MftResponseErrorHanlder MftResponseError;

        public Manifest()
        {
            this.MftResponseNormal += Manifest_MftResponseNormal;
            this.MftResponseRefused += Manifest_MftResponseRefused;
            this.MftResponseSucceed += Manifest_MftResponseSucceed;
            this.MftResponseTrans += Manifest_MftResponseTrans;
            this.MftResponseError += Manifest_MftResponseError;
        }



        public void SetManifestConsignment()
        {
            this.ManifestConsignment = new Views.ManifestConsignmentsView().First(t => t.ID == this.Head.MessageID);
        }

        /// <summary>
        /// 保存业务回执
        /// </summary>
        public void SaveBusinessAs()
        {
            switch (this.Response.AdditionalInformation.StatementCode.Value)
            {
                case CC020.Item0:

                    break;
                case CC020.Item01:
                    ResponseSuccess();
                    break;
                case CC020.Item02:
                    ResponseNornmal();
                    break;
                case CC020.Item03:
                case CC020.Item12:
                case CC020.Item13:
                    ResponseRefuse();
                    break;
                case CC020.Item11:
                    ResponseNornmal();
                    break;
                default:
                    ResponseError();
                    break;
            }

        }

        #region 海关传输

        /// <summary>
        /// 保存传输回执
        /// </summary>
        public void SaveTransAs()
        {
            this.ManifestConsignment.CusMftStatus = this.Response.ResponseType.Code;

            //更新舱单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.ManifestConsignment.CusMftStatus }, item => item.ID == this.ManifestConsignment.ID);
            }

            this.OnResponseTrans(new MftResponseTransEventArgs(this));
        }
        public virtual void OnResponseTrans(MftResponseTransEventArgs args)
        {
            this.MftResponseTrans?.Invoke(this, args);
        }
        private void Manifest_MftResponseTrans(object sender, MftResponseTransEventArgs e)
        {
            e.Manifest.ManifestConsignment.Trace(e.Manifest.Response.ResponseType.Text,
                DateTime.ParseExact(e.Manifest.Head.SendTime.Value, "yyyyMMddHHmmssfff", System.Globalization.CultureInfo.CurrentCulture), e.Manifest.FileName, e.Manifest.FilePath, e.Manifest.BackupUrl);
        }

        #endregion

        #region 海关回执失败（报文或客户端有错）

        public void ResponseError()
        {
            this.ManifestConsignment.CusMftStatus = MultiEnumUtils.ToCode<Enums.CusMftStatus>(Enums.CusMftStatus.Error);

            //更新舱单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.ManifestConsignment.CusMftStatus }, item => item.ID == this.ManifestConsignment.ID);
            }

            this.OnResponseError(new MftResponseErrorEventArgs(this));
        }
        public virtual void OnResponseError(MftResponseErrorEventArgs args)
        {
            this.MftResponseError?.Invoke(this, args);
        }

        private void Manifest_MftResponseError(object sender, MftResponseErrorEventArgs e)
        {
            e.Manifest.ManifestConsignment.Trace(e.Manifest.Response.AdditionalInformation.StatementCode.Value + " " + e.Manifest.Response.AdditionalInformation.StatementDescription,
                DateTime.ParseExact(e.Manifest.Head.SendTime.Value, "yyyyMMddHHmmssfff", System.Globalization.CultureInfo.CurrentCulture), e.Manifest.FileName, e.Manifest.FilePath, e.Manifest.BackupUrl);
        }

        #endregion

        #region 海关回执正常

        public void ResponseNornmal()
        {
            this.ManifestConsignment.CusMftStatus = this.Response.AdditionalInformation.StatementCode.Value.GetXmlEnumAttributeValueFromEnum();

            //更新舱单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.ManifestConsignment.CusMftStatus }, item => item.ID == this.ManifestConsignment.ID);
            }

            this.OnResponseNornmal(new MftResponseNormalEventArgs(this));
        }
        public virtual void OnResponseNornmal(MftResponseNormalEventArgs args)
        {
            this.MftResponseNormal?.Invoke(this, args);
        }
        private void Manifest_MftResponseNormal(object sender, MftResponseNormalEventArgs e)
        {
            e.Manifest.ManifestConsignment.Trace(e.Manifest.Response.AdditionalInformation.StatementDescription, 
                DateTime.ParseExact(e.Manifest.Head.SendTime.Value, "yyyyMMddHHmmssfff", System.Globalization.CultureInfo.CurrentCulture), e.Manifest.FileName, e.Manifest.FilePath, e.Manifest.BackupUrl);
        }

        #endregion

        #region 海关回执拒绝
        public void ResponseRefuse()
        {
            this.ManifestConsignment.CusMftStatus = this.Response.Consignment[0].AdditionalInformation.StatementCode.Value.GetXmlEnumAttributeValueFromEnum();

            //更新舱单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.ManifestConsignment.CusMftStatus }, item => item.ID == this.ManifestConsignment.ID);
            }

            this.OnResponseRefused(new MftResponseRefusedEventArgs(this));
        }
        public virtual void OnResponseRefused(MftResponseRefusedEventArgs args)
        {
            this.MftResponseRefused?.Invoke(this, args);
        }
        private void Manifest_MftResponseRefused(object sender, MftResponseRefusedEventArgs e)
        {
            e.Manifest.ManifestConsignment.Trace(e.Manifest.Response.AdditionalInformation.StatementDescription,
                DateTime.ParseExact(e.Manifest.Head.SendTime.Value, "yyyyMMddHHmmssfff", System.Globalization.CultureInfo.CurrentCulture), e.Manifest.FileName, e.Manifest.FilePath, e.Manifest.BackupUrl);
        }

        #endregion

        #region 海关回执成功

        public void ResponseSuccess()
        {
            this.ManifestConsignment.CusMftStatus = this.Response.Consignment[0].AdditionalInformation.StatementCode.Value.GetXmlEnumAttributeValueFromEnum();

            //更新舱单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.ManifestConsignment.CusMftStatus }, item => item.ID == this.ManifestConsignment.ID);
            }

            this.OnResponseSucceed(new MftResponseSucceedEventArgs(this));
        }
        public virtual void OnResponseSucceed(MftResponseSucceedEventArgs args)
        {
            this.MftResponseSucceed?.Invoke(this, args);
        }
        private void Manifest_MftResponseSucceed(object sender, MftResponseSucceedEventArgs e)
        {
            e.Manifest.ManifestConsignment.Trace(e.Manifest.Response.Consignment[0].AdditionalInformation.StatementDescription,
                DateTime.ParseExact(e.Manifest.Head.SendTime.Value, "yyyyMMddHHmmssfff", System.Globalization.CultureInfo.CurrentCulture), e.Manifest.FileName, e.Manifest.FilePath, e.Manifest.BackupUrl);
        }

        #endregion




    }


    
    public partial class Response
    {
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public ResponseType ResponseType { get; set; }
    }

    public partial class ResponseType
    {

        public string Code;

        public string Text;
    }
}
