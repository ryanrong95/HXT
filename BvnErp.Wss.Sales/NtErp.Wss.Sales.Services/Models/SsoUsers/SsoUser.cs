using NtErp.Wss.Sales.Services.Model.SsoUsers;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Serializers;
using NtErp.Wss.Sales.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Models.SsoUsers
{
    public class SsoUser
    {
        internal const string CookieName = "ydxcyht_new_big_sso";

        public SsoUser()
        {
            this.Properties = new Document();
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = UserStatus.Normal;

        }

        public SsoUser(string id) : this()
        {

            this.ID = id;
        }

        public SsoUser(System.Xml.Linq.XElement xml)
        {
            var entity = xml.XmlEleTo<SsoUser>();
            this.Properties = entity.Properties;
        }

        Document properties;
        public Document Properties
        {
            get
            {
                return this.properties;
            }
            set
            {
                this.properties = value;
            }
        }

        public Elements this[string index]
        {
            get { return this.properties[index]; }
            set { this.properties[index] = value; }
        }


        #region 属性
        [XmlIgnore]
        public string ID { get; set; }

        public string UserID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [XmlIgnore]
        public string UserName
        {
            get
            {
                return this[nameof(this.UserName)];
            }
            set
            {
                this[nameof(this.UserName)] = value;
            }
        }
        /// <summary>
        /// 登录密码
        /// </summary>
        [XmlIgnore]
        public string Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [XmlIgnore]
        public string Email
        {
            get
            {
                return this[nameof(this.Email)];
            }
            set
            {
                this[nameof(this.Email)] = value;
            }
        }
        /// <summary>
        /// 手机号
        /// </summary>
        [XmlIgnore]
        public string Mobile
        {
            get
            {
                return this[nameof(this.Mobile)];
            }
            set
            {
                this[nameof(this.Mobile)] = value;
            }
        }
        /// <summary>
        /// 用户类型
        /// </summary>
        [XmlIgnore]
        public UserRegisterType Type { set; get; }
        /// <summary>
        /// 用户信息
        /// </summary>
        [XmlIgnore]
        public string Context { set; get; }


        /// <summary>
        /// 用户状态
        /// </summary>
        [XmlIgnore]
        public UserStatus Status { get; set; }
        [XmlIgnore]
        public DateTime CreateDate { get; set; }
        [XmlIgnore]
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [XmlIgnore]
        public string CompanyName
        {
            get
            {
                return this[nameof(this.CompanyName)];
            }
            set
            {
                this[nameof(this.CompanyName)] = value;
            }
        }

        #endregion

        #region 资产
        /// <summary>
        /// 现金
        /// </summary>
        [XmlIgnore]
        public Assets Cash
        {
            get
            {
                return new Assets(this.ID, UserAccountType.Cash);
            }
        }

        /// <summary>
        /// 信用
        /// </summary>
        [XmlIgnore]
        public Assets Credit
        {
            get
            {
                return new Assets(this.ID, UserAccountType.Credit);
            }
        }
        /// <summary>
        /// 是否是信用用户
        /// </summary>
        [XmlIgnore]
        public bool IsCredit
        {
            get
            {
                return new UserInputsView(this.ID, UserAccountType.Credit).Sum(item => item.Amount as decimal?).GetValueOrDefault() > 0;
            }
        }
        #endregion



    }
}
