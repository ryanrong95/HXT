using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;
using Needs.Utils.Converters;
using System.Xml.Serialization;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Collections;

namespace NtErp.Wss.Sales.Services.Model
{
    /// <summary>
    /// 提货人 （收货人）
    /// </summary>
    sealed public class Consignee : IConsignee, IAlter, IPersistence
    {
        public Consignee()
        {
            this.CreateDate = this.UpdateDate = this.AlterDate = DateTime.Now;
            this.Status = AlterStatus.Normal;
        }
        public Consignee(string id)
        {
            this.ID = id;
        }

        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// UserID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 国家区域
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        public string FirstName { get; set; }


        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Zipcode { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        public AlterStatus Status { get; set; }

        public DateTime AlterDate { get; set; }

        /// <summary>
        /// 更改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }


        /// <summary>
        /// 是否是默认地址
        /// </summary>
        public bool IsDefault { get; set; }

        #endregion 

        #region 持久化

        public event ErrorHanlder Error;
        public event EnterSuccessHanlder EnterSuccess;
        public event AbandonSuccessHanlder AbandonSuccess;

        public void Enter()
        {
           
        }

        public void Abandon()
        {
            
        }

        public void SetDefault()
        {
            
        }
        #endregion
    }
}
