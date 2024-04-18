using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    /// <summary>
    /// 客户地址
    /// </summary>
    public class Address : IUnique
    {
        public string ID { get; set; }

        public Enums.AddressType Type { get; set; }

        public string ClientID { get; set; }

        /// <summary>
        /// 单位（公司）
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        public string ClientAddress { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public Underly.GeneralStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public string TypeDec
        {
            get
            {
                return this.Type.GetDescription();
            }
        }
    }
}
