using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单-随附单证
    /// </summary>
    [Serializable]
    public class DecLicenseDocu : IUnique, IPersist
    {
        #region 属性
        /// <summary>
        /// 主键ID（DeclarationID+DocuCode+CerCode）.MD5
        /// </summary>
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.DeclarationID, this.DocuCode,this.CertCode).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 报关单ID
        /// </summary>
        public string DeclarationID { get; set; }

        /// <summary>
        /// 单证代码
        /// </summary>
        public string DocuCode { get; set; }

        /// <summary>
        /// 随附单证
        /// </summary>
        public BaseDocuCode DocuCodeCertify { get; set; }

        /// <summary>
        /// 单证编号
        /// </summary>
        public string CertCode { get; set; }

        /// <summary>
        /// 附件地址
        /// </summary>
        public string FileUrl { get; set; }

        #endregion

        public DecLicenseDocu()
        {
            //TODO：设置默认值
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLicenseDocus>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecLicenseDocus
                    {
                        ID = this.ID,
                        DeclarationID = this.DeclarationID,
                        DocuCode = this.DocuCode,
                        CertCode = this.CertCode,
                        FileURL = this.FileUrl
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecLicenseDocus
                    {
                        ID = this.ID,
                        DeclarationID = this.DeclarationID,
                        DocuCode = this.DocuCode,
                        CertCode = this.CertCode,
                        FileURL = this.FileUrl
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        public void PhysicalDelete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecLicenseDocus>(item => item.ID == this.ID);
            }
            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
