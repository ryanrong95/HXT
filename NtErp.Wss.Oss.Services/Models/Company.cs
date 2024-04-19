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
    /// 公司
    /// </summary>
    public class Company : IUnique, IPersist, IEnterSuccess
    {
        /// <summary>
        /// 类型
        /// </summary>
        public CompanyType Type { get; set; }

        public Company()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.Type).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 公司地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 统一机构代码
        /// </summary>
        public string Code { get; set; }

        public string Summary { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        #endregion

        #region 持久化

        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                if (reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Companies>().Any(item => item.ID == this.ID))
                {
                    this.UpdateDate = DateTime.Now;
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
