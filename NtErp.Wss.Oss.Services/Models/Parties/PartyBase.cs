using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 当事人 [提货人、交货人等]
    /// </summary>
    abstract public class PartyBase : IUnique
    {
        public PartyBase()
        {

        }

        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Company?.Name, this.Contact?.Name, this.Address, this.Postzip).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 公司ID
        /// </summary>
        internal string CompanyID { get; set; }

        /// <summary>
        /// 联系人ID
        /// </summary>
        internal string ContactID { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { get; set; }


        #endregion

        #region 扩展属性

        /// <summary>
        /// 公司
        /// </summary>
        public Company Company { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public Contact Contact { get; set; }

        #endregion

        #region 持久化

        abstract public void Enter();

        #endregion

    }
}
