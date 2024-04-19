using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 联系人
    /// </summary>
    public class Contact : IUnique, IPersist, IEnterSuccess
    {
        public Contact()
        {

        }

        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.Company?.ID, this.Email, this.Mobile, this.Tel).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 公司
        /// </summary>
        public Company Company { get; set; }

        #endregion 

        #region 持久化

        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            if (this.Company != null)
            {
                this.Company.Enter();
            }

            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                if (reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Contacts>().Any(item => item.ID == this.ID))
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    reponsitory.Insert(this.ToLinq());
                }
            }
            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
