using Needs.Underly.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;
using NtErp.Wss.Sales.Services.Underly.Collections;

namespace NtErp.Wss.Sales.Services.Model
{
    sealed public class Consignees : Alert<Consignee>, IConsignee
    {
        public Consignees()
        {
            //this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #region 属性
        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get
            {
                return this.Current.Address;
            }
            set
            {
                this.Modifier.Address = value;
            }
        }
        /// <summary>
        /// 公司
        /// </summary>
        public string Company
        {
            get
            {
                return this.Current.Company;
            }
            set
            {
                this.Modifier.Company = value;
            }
        }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact
        {
            get
            {
                return this.Current.Contact;
            }
            set
            {
                this.Modifier.Contact = value;
            }
        }
        /// <summary>
        /// 国家区域
        /// </summary>
        public string Country
        {
            get
            {
                return this.Current.Country;
            }
            set
            {
                this.Modifier.Country = value;
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get
            {
                return this.Current.CreateDate;
            }
            set
            {
                this.Modifier.CreateDate = value;
            }
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get
            {
                return this.Current.Email;
            }
            set
            {
                this.Modifier.Email = value;
            }
        }
        /// <summary>
        /// 名
        /// </summary>
        public string FirstName
        {
            get
            {
                return this.Current.FirstName;
            }
            set
            {
                this.Modifier.FirstName = value;
            }
        }

        public string ID
        {
            get
            {
                return this.Current.ID;
            }
            set
            {
                this.Modifier.ID = value;
            }
        }
        /// <summary>
        /// 姓
        /// </summary>
        public string LastName
        {
            get
            {
                return this.Current.LastName;
            }
            set
            {
                this.Modifier.LastName = value;
            }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.Current.Tel;
            }
            set
            {
                this.Modifier.Tel = value;
            }
        }
        /// <summary>
        /// 更改时间
        /// </summary>
        public DateTime UpdateDate
        {
            get
            {
                return this.Current.UpdateDate;
            }
            set
            {
                this.Modifier.UpdateDate = value;
            }
        }
        /// <summary>
        /// UserID
        /// </summary>
        public string UserID
        {
            get
            {
                return this.Current.UserID;
            }
            set
            {
                this.Modifier.UserID = value;
            }
        }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Zipcode
        {
            get
            {
                return this.Current.Zipcode;
            }
            set
            {
                this.Modifier.Zipcode = value;
            }
        }

        #endregion

        #region  持久化
        public void Enter()
        {
            this.Current.Enter();
        }
        #endregion 
    }
}
