
using NtErp.Wss.Sales.Services.Models.SsoUsers;
using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Model
{
    namespace China
    {
        /// <summary>
        /// 开票信息(国内)
        /// </summary>
        public class Invoice : InvoiceBase, IInvoice
        {

            public Invoice()
            {
                this.CreateDate = this.UpdateDate = DateTime.Now;
                this.Status = SelfStatus.Normal;
            }
            public Invoice(string id)
            {
                this.ID = id;
                this.UpdateDate = DateTime.Now;
            }

            #region 属性
            public string ID { get; set; }
            /// <summary>
            /// UserID
            /// </summary>
            string userid;
            public string UserID
            {
                get
                {
                    if (father != null)
                    {
                        this.userid = father.UserID;
                    }
                    return this.userid;
                }
                set
                {
                    this.userid = value;
                }
            }
            /// <summary>
            /// 公司名称
            /// </summary>
            public string CompanyName { get; set; }
            /// <summary>
            /// 纳税人识别号
            /// </summary>
            public string SCC { get; set; }
            /// <summary>
            /// 注册地址
            /// </summary>
            public string RegAddress { get; set; }
            /// <summary>
            /// 联系电话
            /// </summary>
            public string Tel { get; set; }
            /// <summary>
            /// 开户银行
            /// </summary>
            public string BankName { get; set; }
            /// <summary>
            /// 银行账号
            /// </summary>
            public string BankAccount { get; set; }


            /// <summary>
            /// 发票类型  1.普票   2.增票
            /// </summary>
            public InvoiceType Type { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            public SelfStatus Status { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateDate { get; set; }
            /// <summary>
            /// 修改时间
            /// </summary>
            public DateTime UpdateDate { get; set; }

            /// <summary>
            /// 是否默认
            /// </summary>
            public bool IsDefault { get; set; }

            #endregion

            public event ErrorHanlder Error;
            public event EnterSuccessHanlder EnterSuccess;
            public event AbandonSuccessHanlder AbandonSuccess;

            #region 持久化
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
}
