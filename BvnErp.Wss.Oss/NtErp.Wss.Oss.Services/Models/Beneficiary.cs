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
    /// 受益人
    /// </summary>
    public class Beneficiary : IUnique, IPersist
    {
        public Beneficiary()
        {

        }

        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Bank, this.Methord, this.Currency, this.Address, this.Account, this.SwiftCode, this.Contact?.ID).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 汇款方式
        /// </summary>
        public Methord Methord { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Needs.Underly.Currency Currency { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 银行编码
        /// </summary>
        public string SwiftCode { get; set; }


        #endregion

        #region 扩展属性

        /// <summary>
        /// 联系人
        /// </summary>
        public Contact Contact { get; set; }


        /// <summary>
        /// 联系人
        /// </summary>
        public Company Company { get; set; }

        #endregion

        #region 持久化

        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            this.Contact?.Enter();

            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                if (!reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Beneficiaries>().Any(item => item.ID == this.ID))
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

    /// <summary>
    /// 汇款方式
    /// </summary>
    public enum Methord
    {
        /// <summary>
        /// 汇款
        /// </summary>
        Remittance = 1,
    }
}
