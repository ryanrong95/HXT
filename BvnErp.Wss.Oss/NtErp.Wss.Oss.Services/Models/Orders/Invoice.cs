using Needs.Linq;
using Needs.Utils.Descriptions;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 发票
    /// </summary>
    public class Invoice : IUnique, IPersistence, IFulSuccess, IFulError
    {
        public Invoice()
        {
            this.Type = InvoiceType.None;

        }

        #region 属性

        public string ID { get; set; }
        /// <summary>
        /// 是否需要
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// 发票类型
        /// </summary>
        public InvoiceType Type { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        internal string CompanyID { get; set; }
        /// <summary>
        /// 联系人
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
        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
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
        /// 公司
        /// </summary>
        public Company Company { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public Contact Contact { get; set; }

        #endregion 

        #region 持久化

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            try
            {
                this.Company?.Enter();
                this.CompanyID = this.Company?.ID;
                this.Contact?.Enter();
                this.ContactID = this.Contact?.ID;

                using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
                {
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Invoice);

                        reponsitory.Insert(this.ToLinq());
                    }
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
            catch (Exception ex)
            {
                if (this != null && this.EnterError != null)
                {
                    this.EnterError(this, new ErrorEventArgs(ex.Message, ErrorType.System));
                }
            }
        }

        public void Abandon()
        {
            throw new Exception("This method does not support!");
        }

        #endregion
    }

    /// <summary>
    /// 发票类型
    /// </summary>
    public enum InvoiceType
    {

        None = 0,

        /// <summary>
        /// 普通
        /// </summary>
        [Description("普通发票")]
        Plain = 1,
        /// <summary>
        /// 增值税
        /// </summary>
        [Description("增值税发票")]
        VAT = 2,
    }
}
