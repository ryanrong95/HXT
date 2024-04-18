using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Needs.Ccs.Services.Models
{
    public class WayParter : IUnique
    {
        #region 属性

        private string id;
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.id))
                {
                    this.id = string.Concat(this.Company, this.Place, this.Address, this.Contact, this.Phone, this.Zipcode, this.Email).MD5();
                }
                return this.id;
            }
            internal set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 公司名称,没有公司名称的时候和Contact等同
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 收/发 地区
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string Zipcode { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        public IDType? IDType { get; set; }

        public string IDNumber { get; set; }

        #endregion

        public WayParter()
        {
            this.CreateDate = DateTime.Now;
            this.IDType = Needs.Ccs.Services.Enums.IDType.IDCard;
        }
    }


   
}
