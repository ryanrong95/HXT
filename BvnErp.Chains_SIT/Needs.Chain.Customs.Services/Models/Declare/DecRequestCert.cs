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
    /// 报关单-申请单证信息
    /// </summary>
    [Serializable]
    public class DecRequestCert : IUnique,IPersist
    {
        /// <summary>
        /// 主键ID（DeclarationID+AppCertCode）.MD5
        /// </summary>
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.DeclarationID, this.AppCertCode).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 报关单Id
        /// </summary>
        public string DeclarationID { get; set; }

        /// <summary>
        /// 申请单证代码
        /// </summary>
        public string AppCertCode { get; set; }

        /// <summary>
        /// 申请单证正本数
        /// </summary>
        public int ApplOri { get; set; }

        /// <summary>
        /// 申请单证副本数
        /// </summary>
        public int ApplCopyQuan { get; set; }


        public DecRequestCert()
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
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecRequestCerts>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecRequestCerts
                    {
                        ID = this.ID,
                        DeclarationID = this.DeclarationID,
                        AppCertCode = this.AppCertCode,
                        ApplOri = this.ApplOri,
                        ApplCopyQuan = this.ApplCopyQuan
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecRequestCerts
                    {
                        ID = this.ID,
                        DeclarationID = this.DeclarationID,
                        AppCertCode = this.AppCertCode,
                        ApplOri = this.ApplOri,
                        ApplCopyQuan = this.ApplCopyQuan
                    }, item => item.ID == this.ID);
                }
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
