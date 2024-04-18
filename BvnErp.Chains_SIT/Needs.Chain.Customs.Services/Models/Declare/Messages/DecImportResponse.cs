using Needs.Ccs.Services.Hanlders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单导入系统响应报文
    /// </summary>
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.chinaport.gov.cn/dec")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.chinaport.gov.cn/dec", IsNullable = false)]
    public class DecImportResponse
    {
        public string ResponseCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ClientSeqNo { get; set; }
        public string SeqNo { get; set; }
        public string TrnPreId { get; set; }

        /// <summary>
        /// 拓展-真实回执时间
        /// </summary>
        public DateTime? ResponseTime { get; set; }

        /// <summary>
        /// 拓展-回执文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 拓展-回执文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 拓展-回执文件备份路径
        /// </summary>
        public string BackupUrl { get; set; }

        [XmlIgnore]
        public DecHead DecHead { get; set; }


        public event DecResponseSucceedHanlder DecResponseSucceed;

        public event DecResponseFailedHanlder DecResponseFailed;

        public event DecResponseFailedHanlder DecResponseFailedDraft;

        public DecImportResponse()
        {
            //报文响应成功时
            this.DecResponseSucceed += DecImportResponse_DecResponseSucceed;

            this.DecResponseFailed += DecImportResponse_DecResponseFailed;

            this.DecResponseFailedDraft += DecImportResponse_DecResponseFailedDraft;
        }

        public void SetHead()
        {
            this.DecHead = new Views.DecHeadsView().First(t => t.ID == this.ClientSeqNo);
        }

        public void SaveAs()
        {
            switch (ResponseCode)
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

        /// <summary>
        /// 报文响应成功
        /// </summary>
        public void ResponseSucceed()
        {
            this.DecHead.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.S0);
            this.DecHead.SeqNo = this.SeqNo;
            #region 试运行
            ////TODO:试运行期间模拟生成海关号码，换汇用,正式运行需要改
            //this.DecHead.EntryId = "5303" + DateTime.Now.ToString("yyyyMMddfffff");
            ////TODO:试运行期间模拟
            //this.DecHead.DDate = DateTime.Now;
            
            #endregion

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.DecHead.CusDecStatus, this.DecHead.SeqNo,this.DecHead.EntryId,this.DecHead.DDate }, item => item.ID == this.DecHead.ID);
            }
            //this.DecHead.EnterSuccess(this.DecHead, new Linq.SuccessEventArgs(this.DecHead.ID));
            ////TODO:暂存成功作为报关成功，触发报关成功事件，待删除
            //this.DecHead.DeclareSucceess();

            this.OnResponseSucceed(new DecResponseSucceedEventArgs(this));
        }
        public virtual void OnResponseSucceed(DecResponseSucceedEventArgs args)
        {
            this.DecResponseSucceed?.Invoke(this, args);
        }
        private void DecImportResponse_DecResponseSucceed(object sender, DecResponseSucceedEventArgs e)
        {
            //报关单轨迹
            e.DecImportResponse.DecHead.Trace(e.DecImportResponse.ErrorMessage, e.DecImportResponse.ResponseTime.Value, e.DecImportResponse.FileName, e.DecImportResponse.FilePath, e.DecImportResponse.BackupUrl);
        }

        /// <summary>
        /// 报文响应失败
        /// </summary>
        public void ResponseFailed()
        {
            //暂存失败，状态改为草稿，这样可以再编辑
            this.DecHead.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.F1);

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.DecHead.CusDecStatus, }, item => item.ID == this.DecHead.ID);
            }

            this.OnResponseFailed(new DecResponseFailedEventArgs(this));
        }
        public virtual void OnResponseFailed(DecResponseFailedEventArgs args)
        {
            this.DecResponseFailed?.Invoke(this, args);
        }
        private void DecImportResponse_DecResponseFailed(object sender, DecResponseFailedEventArgs e)
        {
            //报关单轨迹
            e.DecImportResponse.DecHead.Trace(e.DecImportResponse.ErrorMessage, e.DecImportResponse.ResponseTime.Value, e.DecImportResponse.FileName, e.DecImportResponse.FilePath, e.DecImportResponse.BackupUrl);           
        }

        public void ResponseFailedDraft()
        {
            //暂存失败，状态改为草稿，这样可以再编辑
            this.DecHead.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft);

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.DecHead.CusDecStatus, }, item => item.ID == this.DecHead.ID);
            }

            this.OnResponseFailed(new DecResponseFailedEventArgs(this));
        }

        public virtual void OnResponseFailedDraft(DecResponseFailedEventArgs args)
        {
            this.DecResponseFailedDraft?.Invoke(this, args);
        }

        private void DecImportResponse_DecResponseFailedDraft(object sender, DecResponseFailedEventArgs e)
        {
            //报关单轨迹
            e.DecImportResponse.DecHead.Trace("暂存失败，状态变更为草稿");
        }
    }



    /// <summary>
    /// 报关单导入系统报文()
    /// </summary>
    public class Root
    {
        public string resultFlag { get; set; }
        public string failCode { get; set; }
        public string failInfo { get; set; }
        public string retData { get; set; }

        public string ClientSeqNo { get; set; }
        /// <summary>
        /// 拓展-真实回执时间
        /// </summary>
        public DateTime? ResponseTime { get; set; }

        /// <summary>
        /// 拓展-回执文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 拓展-回执文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 拓展-回执文件备份路径
        /// </summary>
        public string BackupUrl { get; set; }

        [XmlIgnore]
        public DecHead DecHead { get; set;}

        public void SetHead()
        {
            this.DecHead = new Views.DecHeadsView().First(t => t.ID == this.ClientSeqNo);
        }

        public void SaveAs()
        {
            //暂存失败，状态改为草稿，这样可以再编辑
            this.DecHead.CusDecStatus = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.Draft);

            //更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.DecHead.CusDecStatus, }, item => item.ID == this.DecHead.ID);
            }

            //报关单轨迹
            this.DecHead.Trace(this.failInfo, this.ResponseTime, this.FileName,this.FilePath, this.BackupUrl);
        }
    }
}
